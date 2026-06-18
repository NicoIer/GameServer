# zstd for unity

Copyright (c) 2025 NicoIer

zstd version: 1.5.7


https://github.com/facebook/zstd

ClangSharpPInvokeGenerator is used to generate the C# bindings for zstd.

https://github.com/dotnet/ClangSharp


除了基础的压缩，解压缩功能外，将zstdcli中的差分压缩和解压缩功能进行了集成。


原始的libzstd没有支持差分功能，zstdcli作为命令行工具支持了差分压缩和解压缩功能。

我想让把cli的差分功能集成到libzstd中，以便在unity中使用。

所以我对zstdcli做了一些修改，把差分压缩和解压缩的功能提取出来，集成到了libzstd中。


待完成的工作：
- [ ] android平台编译支持
- [ ] ios平台编译支持
- [ ] windows平台编译支持
- [ ] linux平台编译支持
- [ ] 差分功能的文档编写
- [ ] 差分压缩功能集成
- [ ] 差分解压缩功能集成
- [ ] 差分功能的C#绑定
- [ ] 差分功能的C#示例代码
- [ ] 差分功能的单元测试
- [ ] ClangSharpPInvokeGenerator生成工具以及环境配置文档
- [ ] libzstd编译文档


## 生成&&应用差分文件

```csharp
using System;
using zstd;

class DemoBytes
{
    static void Main()
    {
        // 假设 refData 是参考文件的内容，srcData 是目标文件的内容
        byte[] refData = System.IO.File.ReadAllBytes("ref.bin");
        byte[] srcData = System.IO.File.ReadAllBytes("target.bin");

        // 生成 patch（差分压缩结果）
        byte[] patch = ZStandardDiffAPI.CompressPatch(
            src: srcData,
            refData: refData,
            compressionLevel: 0,                 // 0 使用默认压缩级别
            enableLongDistanceMatching: true,    // 建议启用 LDM
            windowLog: null,                     // 让 API 自动根据 src 大小估算
            chainLog: null,
            targetLength: null
        );

        System.IO.File.WriteAllBytes("target.patch.zst", patch);
        Console.WriteLine($"Patch written: target.patch.zst, size={patch.Length} bytes");

        // 应用 patch（恢复原始目标数据）
        byte[] restored = ZStandardDiffAPI.DecompressPatch(
            patchBytes: patch,
            refData: refData
        );

        System.IO.File.WriteAllBytes("target.restored.bin", restored);
        Console.WriteLine($"Restored written: target.restored.bin, size={restored.Length} bytes");

        // 校验恢复结果是否与原始一致
        bool same = restored.Length == srcData.Length &&
                    System.Linq.Enumerable.SequenceEqual(restored, srcData);
        Console.WriteLine($"Restored equals original: {same}");
    }
}
```

