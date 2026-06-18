using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace zstd
{
    public class UnknownException : Exception
    {
    }

    [Serializable]
    public class Manifest
    {
        public string version { get; set; }
        public Dictionary<string, string> fileName2Md5 { get; set; }
    }

    [Serializable]
    public class PatchManifest
    {
        public Dictionary<string, long> patchFiles { get; set; }
        public Dictionary<string, List<long>> needFullDownloads { get; set; }

        public PatchManifest()
        {
            patchFiles = new Dictionary<string, long>();
            needFullDownloads = new Dictionary<string, List<long>>();
        }
    }

    public static class PatchAPI
    {
        public enum PatchSelectDropStrategy
        {
            /// <summary>
            /// 从不删除补丁
            /// </summary>
            NeverDrop,

            /// <summary>
            /// 如果生成的Patch文件比新文件还大，则删除该Patch文件
            /// </summary>
            DropIfLargerThanNewFile,


            /// <summary>
            /// 聪明的Drop方法，结合文件大小绝对值和文件大小比例来决定是否删除补丁文件
            /// </summary>
            SmartDropIfLarger,

            /// <summary>
            /// 总是删除补丁文件
            /// </summary>
            AlwaysDrop
        }


        public static readonly int DefaultCompressLevel = Methods.ZSTD_defaultCLevel();
        public static readonly int MinCompressLevel = Methods.ZSTD_minCLevel();
        public static readonly int MaxCompressLevel = Methods.ZSTD_maxCLevel();


        public const string ManifestFileName = "manifest.json";
        public const string CompressedManifestFileName = "compressed.bytes";
        public const string PatchManifestFileName = "manifest.json";
        public const string PatchFolderName = "patches";
        public const string PatchFileSuffix = "patch";
        public const string NewestVersionFileName = "version";


        public static int VersionStringCompare(in string l, in string r)
        {
            // v1.x.y.z... 形式的版本号比较
            string[] lParts = l.TrimStart('v').Split('.');
            string[] rParts = r.TrimStart('v').Split('.');

            int minLength = Math.Min(lParts.Length, rParts.Length);

            for (int i = 0; i < minLength; i++)
            {
                if (int.TryParse(lParts[i], out int lNum) && int.TryParse(rParts[i], out int rNum))
                {
                    if (lNum < rNum) return -1;
                    if (lNum > rNum) return 1;
                }
                else
                {
                    // 非法版本号，按字符串比较
                    int strComp = string.Compare(lParts[i], rParts[i], StringComparison.Ordinal);
                    if (strComp != 0) return strComp;
                }
            }

            // 如果前面部分都相等，较长的版本号视为更大
            if (lParts.Length < rParts.Length) return -1;
            if (lParts.Length > rParts.Length) return 1;
            return 0;
        }


        public static Manifest GenerateManifest(string folderPath)
        {
            var manifest = new Manifest();
            string folderName = Path.GetFileName(folderPath);
            manifest.version = folderName;

            var dirInfo = new System.IO.DirectoryInfo(folderPath);
            var bundleFiles = dirInfo.GetFiles($"*");
            manifest.fileName2Md5 = new Dictionary<string, string>();
            foreach (var fileInfo in bundleFiles)
            {
                string filePath = System.IO.Path.Combine(folderPath, fileInfo.Name);
                using FileStream stream = File.OpenRead(filePath);
                MD5 md5 = MD5.Create();
                byte[] hash = md5.ComputeHash(stream);
                StringBuilder hex = new StringBuilder(hash.Length * 2);
                foreach (byte b in hash)
                    hex.AppendFormat("{0:x2}", b);
                manifest.fileName2Md5.Add(fileInfo.Name, hex.ToString());
            }

            return manifest;
        }

        public static bool ContainsPatch(
            in string currentFileName,
            Manifest current,
            Manifest newest,
            PatchManifest patchManifest
        )
        {
            // 莫名其妙的 当前的manifest里面都没有这个文件
            if (!current.fileName2Md5.ContainsKey(currentFileName)) return false;
            // 因为最新的manifest里面没有这个文件 所以一定不存在patch
            if (!newest.fileName2Md5.ContainsKey(currentFileName)) return false;

            string currentMd5 = current.fileName2Md5[currentFileName];
            string newMd5 = newest.fileName2Md5[currentFileName];

            string patchName = $"{currentMd5}_{newMd5}.{PatchFileSuffix}";
            if (patchManifest.patchFiles.ContainsKey(patchName)) return true;
            return false;
        }


        public static string GetFileSize(long byteSize)
        {
            if (byteSize < 1024)
                return $"{byteSize} B";
            if (byteSize < 1024 * 1024)
                return $"{(byteSize / 1024.0f):F2} KB";
            if (byteSize < 1024 * 1024 * 1024)
                return $"{(byteSize / (1024.0f * 1024.0f)):F2} MB";
            return $"{(byteSize / (1024.0f * 1024.0f * 1024.0f)):F2} GB";
        }

        public static void CreatePatch(
            string currentVersionFolder,
            string newestVersionFolder,
            Manifest currentManifest,
            Manifest newestManifest,
            string patchesFolder,
            int compressLevel,
            ref PatchManifest patchManifest,
            PatchSelectDropStrategy mode = PatchSelectDropStrategy.NeverDrop
        )
        {
            string currentVersion = currentManifest.version;
            string newestVersion = newestManifest.version;
            string fullDownloadKey = $"{currentVersion}_{newestVersion}";

            foreach (var (fileName, newMd5) in newestManifest.fileName2Md5)
            {
                // 如果当前版本没有这个文件，需要全量下载
                if (!currentManifest.fileName2Md5.ContainsKey(fileName))
                {
                    string newFilePath = Path.Combine(newestVersionFolder, fileName);
                    long newFileSize = new FileInfo(newFilePath).Length;
                    if (!patchManifest.needFullDownloads.ContainsKey(fullDownloadKey))
                        patchManifest.needFullDownloads[fullDownloadKey] = new List<long>();
                    patchManifest.needFullDownloads[fullDownloadKey].Add(newFileSize);
                    continue;
                }
                // 如果md5一样，跳过
                string currentMd5 = currentManifest.fileName2Md5[fileName];
                if (currentMd5 == newMd5) continue;
                // 生成patch
                string patchName = $"{currentMd5}_{newMd5}.{PatchFileSuffix}";
                string patchPath = Path.Combine(patchesFolder, patchName);
                if (File.Exists(patchPath))
                {
                    Console.WriteLine(
                        $"patch already exists from[{currentVersion}_{fileName}] to[{newestVersion}_{fileName}] Skipped");
                    continue;
                }

                string currentFilePath = Path.Combine(currentVersionFolder, fileName);
                string newestFilePath = Path.Combine(newestVersionFolder, fileName);

                // 使用zstd生成patch
                byte[] newestData = File.ReadAllBytes(newestFilePath);
                byte[] refData = File.ReadAllBytes(currentFilePath);
                var patchBytes = ZStandardDiffAPI.CompressPatch(newestData, refData, compressLevel);

                // 根据策略决定是否丢弃补丁
                bool needDrop = mode switch
                {
                    PatchSelectDropStrategy.NeverDrop => false,
                    PatchSelectDropStrategy.DropIfLargerThanNewFile => patchBytes.Length >= newestData.Length,
                    PatchSelectDropStrategy.SmartDropIfLarger => SmartNeedDrop(newestData, patchBytes),
                    PatchSelectDropStrategy.AlwaysDrop => true,
                    _ => throw new UnknownException()
                };
                if (needDrop)
                {
                    Console.WriteLine(
                        $"Dropped patch from[{currentVersion}_{fileName}] to[{newestVersion}_{fileName}] : \n" +
                        $"{GetFileSize(refData.Length)} -> {GetFileSize(newestData.Length)} (patch size: {GetFileSize(patchBytes.Length)})");
                    if (!patchManifest.needFullDownloads.ContainsKey(fullDownloadKey))
                        patchManifest.needFullDownloads[fullDownloadKey] = new List<long>();
                    patchManifest.needFullDownloads[fullDownloadKey].Add(newestData.Length);
                    continue;
                }

                File.WriteAllBytes(patchPath, patchBytes);


                // 大小信息
                Console.WriteLine(
                    $"Generated patch from[{currentVersion}_{fileName}] to[{newestVersion}_{fileName}] : \n" +
                    $"{GetFileSize(refData.Length)} -> {GetFileSize(newestData.Length)} (patch size: {GetFileSize(patchBytes.Length)})");
                patchManifest.patchFiles.Add(patchName, patchBytes.Length);
            }
        }

        public static bool SmartNeedDrop(byte[] newestData, byte[] patchBytes)
        {
            // 分段线性打分：score = α * 比例 + β * 绝对差值比例
            // score 超过阈值则认为“补丁不值得保留” -> 返回 true 丢弃。
            const int SmallFileThresholdBytes = 100 * 1024; // 100 KB：新文件太小直接不打补丁
            const float Alpha = 0.7f; // 比例项权重
            const float Beta = 0.3f; // 绝对差值比例项权重
            const float ScoreThreshold = 0.60f; // 打分阈值，> 即丢弃

            int newSize = newestData.Length;
            if (newSize <= 0) return true; // 异常/空文件直接丢弃
            if (newSize < SmallFileThresholdBytes) return true;

            int patchSize = patchBytes.Length;
            float ratio = (float)patchSize / newSize; // 补丁占新文件比例
            float diffRatio = Math.Abs(newSize - patchSize) / (float)newSize; // 绝对差值的比例化

            float score = Alpha * ratio + Beta * diffRatio;
            return score > ScoreThreshold;
        }
    }
}