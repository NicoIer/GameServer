// Copyright (c) 2025 NicoIer and Contributors.
// Licensed under the MIT License (MIT). See LICENSE in the repository root for more information.

#if UNITY_5_3_OR_NEWER

using System;
using System.IO;
using Newtonsoft.Json;
using TMPro;
using Unity.Profiling.Memory;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.Profiling;
using UnityEngine.UI;

namespace zstd.samples
{
    public class zstd_test : MonoBehaviour
    {
        [SerializeField] public TextMeshProUGUI versionText;
        [SerializeField] private Button applyPatchButton;
        private static string readOnlyFolder = Application.streamingAssetsPath;
        private static string fileSourceFolder;
        private const string localFolderName = "local";
        public string remotePatchServerUrl = "http://10.30.15.189:8080/";

        private void Awake()
        {
#if UNITY_EDITOR
            fileSourceFolder = Application.streamingAssetsPath;
#else
            fileSourceFolder = Application.persistentDataPath;
#endif
        }

        void Start()
        {
            versionText.text = "zstd version: " + ZStandardAPI.versionString;
            applyPatchButton.onClick.AddListener(OnApplyPatchClicked);
        }

        public static string GetNewestVersionFromRemote(string url)
        {
            using var webClient = new System.Net.WebClient();
            string newestVersion =
                webClient.DownloadString(new Uri(new Uri(url), PatchAPI.NewestVersionFileName));
            return newestVersion.Trim();
        }

        private static string GetLocalNewestFolder(string folderPath)
        {
            string[] versionFolders = Directory.GetDirectories(folderPath);
            string currentVersionFolder = null;
            foreach (var versionFolder in versionFolders)
            {
                string folderName = Path.GetFileName(versionFolder);
                if (currentVersionFolder == null) currentVersionFolder = versionFolder;

                int compare = PatchAPI.VersionStringCompare(
                    Path.GetFileName(versionFolder),
                    Path.GetFileName(currentVersionFolder)
                );

                if (compare > 0)
                {
                    currentVersionFolder = versionFolder;
                }
            }

            return currentVersionFolder;
        }

        [ContextMenu("Apply Patch")]
        private async void OnApplyPatchClicked()
        {
#if UNITY_EDITOR
            fileSourceFolder = Application.streamingAssetsPath;
#endif
            string writableFolder = Path.Combine(fileSourceFolder, localFolderName);

            // 找到最新的版本号对应的文件夹 找到当前我们正在使用的版本

            if (!Directory.Exists(writableFolder))
            {
                Directory.CreateDirectory(writableFolder);
                Debug.Log("创建本地可写文件夹: " + writableFolder);
            }

            string currentVersionFolder = GetLocalNewestFolder(writableFolder);

            bool isFirstVersion = false;

            if (currentVersionFolder == null) // 说明是第一个版本的包，从未更新过
            {
                isFirstVersion = true;
                var targetReadOnlyFolder = Path.Combine(readOnlyFolder, localFolderName);
                var firstVersionFilePath = Path.Combine(targetReadOnlyFolder, PatchAPI.NewestVersionFileName);
#if (UNITY_ANDROID || UNITY_WEBGL )&& !UNITY_EDITOR
                string uri = new Uri(firstVersionFilePath).AbsoluteUri;
                UnityWebRequest request = UnityWebRequest.Get(uri);
                await request.SendWebRequest();
                string firstVersion = request.downloadHandler.text.Trim();
                Debug.Log($"本地没有版本，尝试从只读目录获取当前版本:{targetReadOnlyFolder} 版本号:{firstVersion}");
                currentVersionFolder = Path.Combine(targetReadOnlyFolder, firstVersion);
#else
                string firstVersion = (await File.ReadAllTextAsync(firstVersionFilePath)).Trim();
                Debug.Log($"本地没有版本，尝试从只读目录获取当前版本:{targetReadOnlyFolder} 版本号:{firstVersion}");
                currentVersionFolder = Path.Combine(targetReadOnlyFolder, firstVersion);
#endif
            }

            string currentVersion = Path.GetFileName(currentVersionFolder);

            // 去远程服务器找到最新的版本
            string remoteNewestVersion = GetNewestVersionFromRemote(remotePatchServerUrl);

            Debug.Log($"Current Version: {currentVersion}, Remote Newest Version: {remoteNewestVersion}");

            if (currentVersion == remoteNewestVersion)
            {
                Debug.Log("Already the newest version.");
                return;
            }


            // 创建本地最新文件夹
            string localNewestVersionFolder = Path.Combine(writableFolder, remoteNewestVersion);
            if (Directory.Exists(localNewestVersionFolder))
            {
                Directory.Delete(localNewestVersionFolder, true);
#if UNITY_EDITOR
                // .meta也要删掉
                string metaPath = localNewestVersionFolder + ".meta";
                if (File.Exists(metaPath))
                {
                    File.Delete(metaPath);
                }
#endif
                Debug.LogWarning($"Deleted existing local newest version folder: {localNewestVersionFolder}");
            }

            Directory.CreateDirectory(localNewestVersionFolder);


            // 拿到本地当前版本的manifest
            string localManifestPath = Path.Combine(currentVersionFolder, PatchAPI.ManifestFileName);
            string content = "";
            if (isFirstVersion)
            {
#if (UNITY_ANDROID || UNITY_WEBGL ) && !UNITY_EDITOR
                string uri = new Uri(localManifestPath).AbsoluteUri;
                UnityWebRequest request = UnityWebRequest.Get(uri);
                await request.SendWebRequest();
                content = request.downloadHandler.text;
#else
                content = await File.ReadAllTextAsync(localManifestPath);
#endif
            }
            else
            {
                content = await File.ReadAllTextAsync(localManifestPath);
            }

            Manifest localManifest = JsonConvert.DeserializeObject<Manifest>(
                content
            );

            // 拿到远程最新版本的manifest
            string remoteNewestManifestUrl = new Uri(new Uri(remotePatchServerUrl),
                $"{remoteNewestVersion}/{PatchAPI.ManifestFileName}").ToString();
            using var webClient = new System.Net.WebClient();
            string remoteManifestContent = webClient.DownloadString(remoteNewestManifestUrl);
            Manifest remoteManifest = JsonConvert.DeserializeObject<Manifest>(remoteManifestContent);

            // 拿到远程最新版本的patch manifest
            string remotePatchManifestUrl = new Uri(new Uri(remotePatchServerUrl),
                $"{PatchAPI.PatchFolderName}/{PatchAPI.PatchManifestFileName}").ToString();
            string remotePatchManifestContent = webClient.DownloadString(remotePatchManifestUrl);
            PatchManifest remotePatchManifest =
                JsonConvert.DeserializeObject<PatchManifest>(remotePatchManifestContent);

            // 遍历远程manifest 对比本地manifest
            // 1.远程有 本地没有 直接下载
            // 2.远程有 本地有 
            // 2.1 有patch 下载patch解压
            // 2.2 没有patch 直接下载覆盖
            // 3.远程没有 本地有 不处理
            foreach (var remoteFileEntry in remoteManifest.fileName2Md5)
            {
                string remoteFileName = remoteFileEntry.Key;
                string remoteFileMd5 = remoteFileEntry.Value;
                if (localManifest.fileName2Md5.ContainsKey(remoteFileName))
                {
                    // 本地有这个文件
                    string patchName =
                        $"{localManifest.fileName2Md5[remoteFileName]}_{remoteFileMd5}.{PatchAPI.PatchFileSuffix}";
                    if (remotePatchManifest.patchFiles.ContainsKey(patchName))
                    {
                        // 有patch 下载patch解压
                        byte[] patchData = webClient.DownloadData(
                            new Uri(new Uri(remotePatchServerUrl),
                                $"{PatchAPI.PatchFolderName}/{patchName}"));


                        string localFilePath = Path.Combine(currentVersionFolder, remoteFileName);
                        byte[] localFileData;
                        if (isFirstVersion)
                        {
#if (UNITY_ANDROID || UNITY_WEBGL ) && !UNITY_EDITOR
                            string uri = new Uri(localFilePath).AbsoluteUri;
                            UnityWebRequest request = UnityWebRequest.Get(uri);
                            await request.SendWebRequest();
                            localFileData = request.downloadHandler.data;
#else
                            localFileData = File.ReadAllBytes(localFilePath);
#endif
                        }
                        else
                        {
                            localFileData = File.ReadAllBytes(localFilePath);
                        }
                        
                        Profiler.BeginSample("Apply Patch");
                        byte[] newFileData = ZStandardDiffAPI.DecompressPatch(patchData, localFileData);
                        Profiler.EndSample();
                        await File.WriteAllBytesAsync(
                            Path.Combine(localNewestVersionFolder, remoteFileName), newFileData);
                        Debug.Log($"Applied patch for file: {remoteFileName}");
                    }
                    else // 没有patch 直接下载覆盖
                    {
                        byte[] remoteFileData = webClient.DownloadData(
                            new Uri(new Uri(remotePatchServerUrl),
                                $"{remoteNewestVersion}/{remoteFileName}"));
                        await File.WriteAllBytesAsync(
                            Path.Combine(localNewestVersionFolder, remoteFileName), remoteFileData);
                        Debug.Log($"Downloaded full file: {remoteFileName}");
                    }
                }
                else
                {
                    //本地没有 直接下载
                    byte[] remoteFileData = webClient.DownloadData(
                        new Uri(new Uri(remotePatchServerUrl),
                            $"{remoteNewestVersion}/{remoteFileName}"));
                    await File.WriteAllBytesAsync(
                        Path.Combine(localNewestVersionFolder, remoteFileName), remoteFileData);
                    Debug.Log($"Downloaded new file: {remoteFileName}");
                }
            }

            // 写入manifest
            string newestManifestPath = Path.Combine(localNewestVersionFolder, PatchAPI.ManifestFileName);
            await File.WriteAllTextAsync(newestManifestPath,
                JsonConvert.SerializeObject(remoteManifest, Formatting.Indented));
            
            // 更新version记录文件
            string newestVersionFilePath = Path.Combine(writableFolder, PatchAPI.NewestVersionFileName);
            await File.WriteAllTextAsync(newestVersionFilePath, remoteNewestVersion);

            // 删除旧版本
            if (!isFirstVersion && Directory.Exists(currentVersionFolder))
            {
                Directory.Delete(currentVersionFolder, true);
#if UNITY_EDITOR
                // .meta也要删掉
                string metaPath = currentVersionFolder + ".meta";
                if (File.Exists(metaPath))
                {
                    File.Delete(metaPath);
                }

#endif

                Debug.LogWarning($"Deleted old version folder: {currentVersionFolder}");
            }
        }
    }
}

#endif