// Copyright (c) 2023 NicoIer and Contributors.
// Licensed under the MIT License (MIT). See LICENSE in the repository root for more information.

using System;
using System.IO;

namespace zstd
{
    public class ZstdException : Exception
    {
        public ZstdException(string message) : base(message) { }
    }

    /// <summary>
    /// Safe managed wrapper for libzstd. Provides byte[] and Stream-based APIs without exposing unsafe pointers.
    /// </summary>
    public static class ZStandardAPI
    {
        /// <summary>
        /// Zstandard library version number. Loaded at first access.
        /// </summary>
        public static readonly uint versionNumber = Methods.ZSTD_versionNumber();

        private static string _versionString;

        /// <summary>
        /// Zstandard library version string. Cached on first call.
        /// </summary>
        public static string versionString
        {
            get
            {
                if (_versionString == null)
                {
                    unsafe
                    {
                        var s = Methods.ZSTD_versionString();
                        _versionString = PtrToAnsiString(s);
                    }
                }
                return _versionString;
            }
        }

        /// <summary>
        /// Compress input data to a new byte[] using the given compression level (default: ZSTD_defaultCLevel()).
        /// </summary>
        public static byte[] Compress(byte[] src, int compressionLevel = 0)
        {
            if (src == null) throw new ArgumentNullException(nameof(src));

            if (compressionLevel == 0)
            {
                compressionLevel = Methods.ZSTD_defaultCLevel();
            }

            // Determine max compressed size and rent destination buffer
            nuint max = Methods.ZSTD_compressBound((nuint)src.Length);
            byte[] dst = new byte[(int)max];

            unsafe
            {
                fixed (byte* pSrc = src)
                fixed (byte* pDst = dst)
                {
                    nuint written = Methods.ZSTD_compress(pDst, (nuint)dst.Length, pSrc, (nuint)src.Length, compressionLevel);
                    ValidateResult(written);

                    // If written smaller than dst length, resize to exact
                    if (written != (nuint)dst.Length)
                    {
                        byte[] result = new byte[(int)written];
                        Buffer.BlockCopy(dst, 0, result, 0, (int)written);
                        return result;
                    }

                    return dst;
                }
            }
        }

        /// <summary>
        /// Decompress input data to a new byte[]. Automatically detects decompressed size from frame header.
        /// </summary>
        public static byte[] Decompress(byte[] src)
        {
            if (src == null) throw new ArgumentNullException(nameof(src));

            unsafe
            {
                fixed (byte* pSrc = src)
                {
                    ulong contentSize = Methods.ZSTD_getFrameContentSize(pSrc, (nuint)src.Length);
                    if (contentSize == ulong.MaxValue)
                    {
                        throw new ZstdException("Invalid frame: content size unknown or invalid.");
                    }
                    if (contentSize == ulong.MaxValue - 1)
                    {
                        throw new ZstdException("Frame content size error.");
                    }

                    if (contentSize > int.MaxValue)
                    {
                        throw new ZstdException("Decompressed size exceeds supported managed buffer size.");
                    }

                    byte[] dst = new byte[(int)contentSize];
                    fixed (byte* pDst = dst)
                    {
                        nuint written = Methods.ZSTD_decompress(pDst, (nuint)dst.Length, pSrc, (nuint)src.Length);
                        ValidateResult(written);

                        if (written != (nuint)dst.Length)
                        {
                            // shrink if library wrote fewer bytes
                            byte[] result = new byte[(int)written];
                            Buffer.BlockCopy(dst, 0, result, 0, (int)written);
                            return result;
                        }

                        return dst;
                    }
                }
            }
        }

        /// <summary>
        /// Compress from stream to stream. Reads all data from input, writes compressed output to destination.
        /// Buffering is internal and avoids unsafe usage. Optional compressionLevel (default ZSTD_defaultCLevel).
        /// </summary>
        public static void Compress(Stream input, Stream output, int compressionLevel = 0, int bufferSize = 1 << 20)
        {
            if (input == null) throw new ArgumentNullException(nameof(input));
            if (output == null) throw new ArgumentNullException(nameof(output));
            if (!input.CanRead) throw new ArgumentException("Input stream must be readable.", nameof(input));
            if (!output.CanWrite) throw new ArgumentException("Output stream must be writable.", nameof(output));
            if (bufferSize <= 0) bufferSize = 1 << 20;

            if (compressionLevel == 0)
            {
                compressionLevel = Methods.ZSTD_defaultCLevel();
            }

            // Create streaming compression context
            unsafe
            {
                ZSTD_CCtx_s* cctx = Methods.ZSTD_createCStream();
                try
                {
                    nuint r = Methods.ZSTD_initCStream(cctx, compressionLevel);
                    ValidateResult(r);

                    byte[] inBuf = new byte[bufferSize];
                    byte[] outBuf = new byte[(int)Methods.ZSTD_CStreamOutSize()];

                    while (true)
                    {
                        int read = input.Read(inBuf, 0, inBuf.Length);
                        bool lastChunk = read == 0;

                        fixed (byte* pIn = inBuf)
                        fixed (byte* pOut = outBuf)
                        {
                            ZSTD_inBuffer_s inBuffer = default;
                            ZSTD_outBuffer_s outBuffer = default;

                            inBuffer.src = pIn;
                            inBuffer.size = (nuint)read;
                            inBuffer.pos = 0;

                            ZSTD_EndDirective directive = lastChunk ? ZSTD_EndDirective.ZSTD_e_end : ZSTD_EndDirective.ZSTD_e_continue;

                            do
                            {
                                outBuffer.dst = pOut;
                                outBuffer.size = (nuint)outBuf.Length;
                                outBuffer.pos = 0;

                                nuint res = Methods.ZSTD_compressStream2(cctx, &outBuffer, &inBuffer, directive);
                                ValidateResult(res);

                                if (outBuffer.pos > 0)
                                {
                                    output.Write(outBuf, 0, (int)outBuffer.pos);
                                }
                            }
                            while ((lastChunk && outBuffer.pos > 0) || (!lastChunk && inBuffer.pos < inBuffer.size));
                        }

                        if (lastChunk) break;
                    }
                }
                finally
                {
                    Methods.ZSTD_freeCStream(cctx);
                }
            }
        }

        /// <summary>
        /// Decompress from stream to stream. Reads compressed data from input, writes decompressed output to destination.
        /// </summary>
        public static void Decompress(Stream input, Stream output, int bufferSize = 1 << 20)
        {
            if (input == null) throw new ArgumentNullException(nameof(input));
            if (output == null) throw new ArgumentNullException(nameof(output));
            if (!input.CanRead) throw new ArgumentException("Input stream must be readable.", nameof(input));
            if (!output.CanWrite) throw new ArgumentException("Output stream must be writable.", nameof(output));
            if (bufferSize <= 0) bufferSize = 1 << 20;

            unsafe
            {
                ZSTD_DCtx_s* dctx = Methods.ZSTD_createDStream();
                try
                {
                    nuint r = Methods.ZSTD_initDStream(dctx);
                    ValidateResult(r);

                    byte[] inBuf = new byte[(int)Methods.ZSTD_DStreamInSize()];
                    byte[] outBuf = new byte[(int)Methods.ZSTD_DStreamOutSize()];

                    while (true)
                    {
                        int read = input.Read(inBuf, 0, inBuf.Length);
                        bool lastChunk = read == 0;

                        fixed (byte* pIn = inBuf)
                        fixed (byte* pOut = outBuf)
                        {
                            ZSTD_inBuffer_s inBuffer = default;
                            ZSTD_outBuffer_s outBuffer = default;

                            inBuffer.src = pIn;
                            inBuffer.size = (nuint)read;
                            inBuffer.pos = 0;

                            while (inBuffer.pos < inBuffer.size)
                            {
                                outBuffer.dst = pOut;
                                outBuffer.size = (nuint)outBuf.Length;
                                outBuffer.pos = 0;

                                nuint res = Methods.ZSTD_decompressStream(dctx, &outBuffer, &inBuffer);
                                ValidateResult(res);

                                if (outBuffer.pos > 0)
                                {
                                    output.Write(outBuf, 0, (int)outBuffer.pos);
                                }
                            }
                        }

                        if (lastChunk) break;
                    }
                }
                finally
                {
                    Methods.ZSTD_freeDStream(dctx);
                }
            }
        }

        /// <summary>
        /// Compress using dictionary (bytes) to a new byte[].
        /// </summary>
        public static byte[] CompressUsingDict(byte[] src, byte[] dict, int compressionLevel = 0)
        {
            if (src == null) throw new ArgumentNullException(nameof(src));
            if (dict == null) throw new ArgumentNullException(nameof(dict));

            if (compressionLevel == 0)
            {
                compressionLevel = Methods.ZSTD_defaultCLevel();
            }

            nuint max = Methods.ZSTD_compressBound((nuint)src.Length);
            byte[] dst = new byte[(int)max];

            unsafe
            {
                fixed (byte* pSrc = src)
                fixed (byte* pDst = dst)
                fixed (byte* pDict = dict)
                {
                    nuint written = Methods.ZSTD_compress_usingDict(null, pDst, (nuint)dst.Length, pSrc, (nuint)src.Length, pDict, (nuint)dict.Length, compressionLevel);
                    ValidateResult(written);

                    if (written != (nuint)dst.Length)
                    {
                        byte[] result = new byte[(int)written];
                        Buffer.BlockCopy(dst, 0, result, 0, (int)written);
                        return result;
                    }

                    return dst;
                }
            }
        }

        /// <summary>
        /// Decompress using dictionary (bytes) to a new byte[]. Decompressed size is guessed from frame header.
        /// </summary>
        public static byte[] DecompressUsingDict(byte[] src, byte[] dict)
        {
            if (src == null) throw new ArgumentNullException(nameof(src));
            if (dict == null) throw new ArgumentNullException(nameof(dict));

            unsafe
            {
                fixed (byte* pSrc = src)
                fixed (byte* pDict = dict)
                {
                    ulong contentSize = Methods.ZSTD_getFrameContentSize(pSrc, (nuint)src.Length);
                    if (contentSize == ulong.MaxValue || contentSize == ulong.MaxValue - 1)
                        throw new ZstdException("Invalid or unknown decompressed content size.");

                    if (contentSize > int.MaxValue)
                        throw new ZstdException("Decompressed size exceeds supported managed buffer size.");

                    byte[] dst = new byte[(int)contentSize];
                    fixed (byte* pDst = dst)
                    {
                        nuint written = Methods.ZSTD_decompress_usingDict(null, pDst, (nuint)dst.Length, pSrc, (nuint)src.Length, pDict, (nuint)dict.Length);
                        ValidateResult(written);

                        if (written != (nuint)dst.Length)
                        {
                            byte[] result = new byte[(int)written];
                            Buffer.BlockCopy(dst, 0, result, 0, (int)written);
                            return result;
                        }

                        return dst;
                    }
                }
            }
        }

        /// <summary>
        /// Validate a zstd size_t result. Throws ZstdException with error details on failure.
        /// </summary>
        private static void ValidateResult(nuint result)
        {
            uint isError = Methods.ZSTD_isError(result);
            if (isError != 0)
            {
                ZSTD_ErrorCode code = Methods.ZSTD_getErrorCode(result);
                unsafe
                {
                    sbyte* name = Methods.ZSTD_getErrorName(result);
                    string message = $"ZSTD error ({code}): {PtrToAnsiString(name)}";
                    throw new ZstdException(message);
                }
            }
        }

        /// <summary>
        /// Convert a C-style const char* (ASCII/UTF-8 without multi-byte decoding) to managed string.
        /// </summary>
        private static unsafe string PtrToAnsiString(sbyte* ptr)
        {
            if (ptr == null) return string.Empty;
            // Find length until null terminator
            int len = 0;
            for (sbyte* p = ptr; *p != 0; p++) len++;
            return new string(ptr, 0, len);
        }
    }
}