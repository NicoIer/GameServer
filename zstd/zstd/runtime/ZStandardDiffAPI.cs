// Copyright (c) 2023 NicoIer and Contributors.
// Licensed under the MIT License (MIT). See LICENSE in the repository root for more information.

using System;
using System.IO;

namespace zstd
{
    /// <summary>
    /// Safe managed wrapper for libzstd that replicates zstd CLI's "patch-from"/"patch-apply" behavior.
    /// It compresses a target using a reference file as an external dictionary with long-distance matching,
    /// producing a zstd stream that can be decompressed only when the same reference is provided.
    /// </summary>
    public static class ZStandardDiffAPI
    {
        /// <summary>
        /// Create a "diff" (patch) by compressing src using refData as the reference dictionary.
        /// This mirrors `zstd --patch-from=REF src`.
        /// </summary>
        /// <param name="src">Target bytes to compress.</param>
        /// <param name="refData">Reference bytes used as dictionary.</param>
        /// <param name="compressionLevel">Compression level. If 0, uses ZSTD_defaultCLevel().</param>
        /// <param name="enableLongDistanceMatching">Enable long distance matching (recommended true for large refs/src).</param>
        /// <param name="windowLog">Optional override for windowLog. If null, a heuristic is used.</param>
        /// <param name="chainLog">Optional override for chainLog.</param>
        /// <param name="targetLength">Optional override for targetLength.</param>
        public static byte[] CompressPatch(
            byte[] src,
            byte[] refData,
            int compressionLevel = 0,
            bool enableLongDistanceMatching = true,
            int? windowLog = null,
            int? chainLog = null,
            int? targetLength = null)
        {
            if (src == null) throw new ArgumentNullException(nameof(src));
            if (refData == null) throw new ArgumentNullException(nameof(refData));

            if (compressionLevel == 0)
            {
                compressionLevel = Methods.ZSTD_defaultCLevel();
            }

            // Heuristic: ensure window can cover src (similar to CLI behavior "windowSize > srcSize")
            // windowLog ~ ceil(log2(srcSize)), capped by bounds.
            int wl = windowLog ?? ComputeWindowLogHeuristic(src.Length);

            // Prepare destination buffer
            nuint max = Methods.ZSTD_compressBound((nuint)src.Length);
            byte[] dst = new byte[(int)max];

            unsafe
            {
                ZSTD_CCtx_s* cctx = Methods.ZSTD_createCCtx();
                try
                {
                    ValidateResult(Methods.ZSTD_CCtx_reset(cctx, ZSTD_ResetDirective.ZSTD_reset_session_and_parameters));

                    // Load reference dictionary into CCtx
                    fixed (byte* pDict = refData)
                    {
                        ValidateResult(Methods.ZSTD_CCtx_loadDictionary(cctx, pDict, (nuint)refData.Length));
                    }

                    // Parameters: enable LDM and set windowLog, optionally chainLog/targetLength
                    if (enableLongDistanceMatching)
                    {
                        ValidateResult(Methods.ZSTD_CCtx_setParameter(cctx, ZSTD_cParameter.ZSTD_c_enableLongDistanceMatching, 1));
                    }
                    // windowLog
                    ValidateResult(Methods.ZSTD_CCtx_setParameter(cctx, ZSTD_cParameter.ZSTD_c_windowLog, wl));
                    // Optional tuning
                    if (chainLog.HasValue)
                    {
                        ValidateResult(Methods.ZSTD_CCtx_setParameter(cctx, ZSTD_cParameter.ZSTD_c_chainLog, chainLog.Value));
                    }
                    if (targetLength.HasValue)
                    {
                        ValidateResult(Methods.ZSTD_CCtx_setParameter(cctx, ZSTD_cParameter.ZSTD_c_targetLength, targetLength.Value));
                    }
                    // Strategy and checksum/dictID can be tuned as needed; keep defaults.

                    // Compression level
                    ValidateResult(Methods.ZSTD_CCtx_setParameter(cctx, ZSTD_cParameter.ZSTD_c_compressionLevel, compressionLevel));

                    fixed (byte* pSrc = src)
                    fixed (byte* pDst = dst)
                    {
                        nuint written = Methods.ZSTD_compress2(cctx, pDst, (nuint)dst.Length, pSrc, (nuint)src.Length);
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
                finally
                {
                    Methods.ZSTD_freeCCtx(cctx);
                }
            }
        }

        /// <summary>
        /// Apply a "diff" (patch) by decompressing patchBytes using refData as the reference dictionary.
        /// This mirrors `zstd -d --patch-from=REF patch`.
        /// </summary>
        /// <param name="patchBytes">Patch (compressed) bytes produced by CompressPatch.</param>
        /// <param name="refData">The same reference bytes used during compression.</param>
        public static byte[] DecompressPatch(byte[] patchBytes, byte[] refData)
        {
            if (patchBytes == null) throw new ArgumentNullException(nameof(patchBytes));
            if (refData == null) throw new ArgumentNullException(nameof(refData));

            unsafe
            {
                // Determine decompressed size from frame header (required to allocate buffer)
                fixed (byte* pPatch = patchBytes)
                {
                    ulong contentSize = Methods.ZSTD_getFrameContentSize(pPatch, (nuint)patchBytes.Length);
                    if (contentSize == ulong.MaxValue || contentSize == ulong.MaxValue - 1)
                        throw new ZstdException("Invalid or unknown decompressed content size.");

                    if (contentSize > int.MaxValue)
                        throw new ZstdException("Decompressed size exceeds supported managed buffer size.");

                    byte[] dst = new byte[(int)contentSize];

                    ZSTD_DCtx_s* dctx = Methods.ZSTD_createDCtx();
                    try
                    {
                        ValidateResult(Methods.ZSTD_DCtx_reset(dctx, ZSTD_ResetDirective.ZSTD_reset_session_and_parameters));
                        // Load reference dictionary into DCtx
                        fixed (byte* pDict = refData)
                        {
                            ValidateResult(Methods.ZSTD_DCtx_loadDictionary(dctx, pDict, (nuint)refData.Length));
                        }
                        // Decompress
                        fixed (byte* pDst = dst)
                        {
                            nuint written = Methods.ZSTD_decompressDCtx(dctx, pDst, (nuint)dst.Length, pPatch, (nuint)patchBytes.Length);
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
                    finally
                    {
                        Methods.ZSTD_freeDCtx(dctx);
                    }
                }
            }
        }

        /// <summary>
        /// Stream-based patch compression, equivalent to `zstd --patch-from=REF` taking streaming input.
        /// When input is from a stream with unknown total size, you may want to set pledgedSrcSize to improve behavior.
        /// </summary>
        /// <param name="input">Source stream to compress.</param>
        /// <param name="output">Destination stream to write patch.</param>
        /// <param name="refData">Reference bytes used as dictionary.</param>
        /// <param name="compressionLevel">Compression level, default ZSTD_defaultCLevel().</param>
        /// <param name="bufferSize">Input buffer size for reading source.</param>
        /// <param name="pledgedSrcSize">Optional total source size; if known, set to inform the compressor.</param>
        /// <param name="enableLongDistanceMatching">Enable LDM (recommended true).</param>
        /// <param name="windowLog">Optional windowLog override; if null a heuristic is applied when pledgedSrcSize is known.</param>
        public static void CompressPatch(Stream input, Stream output, byte[] refData,
                                         int compressionLevel = 0,
                                         int bufferSize = 1 << 20,
                                         ulong? pledgedSrcSize = null,
                                         bool enableLongDistanceMatching = true,
                                         int? windowLog = null)
        {
            if (input == null) throw new ArgumentNullException(nameof(input));
            if (output == null) throw new ArgumentNullException(nameof(output));
            if (refData == null) throw new ArgumentNullException(nameof(refData));
            if (!input.CanRead) throw new ArgumentException("Input stream must be readable.", nameof(input));
            if (!output.CanWrite) throw new ArgumentException("Output stream must be writable.", nameof(output));
            if (bufferSize <= 0) bufferSize = 1 << 20;

            if (compressionLevel == 0) compressionLevel = Methods.ZSTD_defaultCLevel();

            unsafe
            {
                ZSTD_CCtx_s* cctx = Methods.ZSTD_createCStream();
                try
                {
                    // Initialize stream with compression level
                    ValidateResult(Methods.ZSTD_initCStream(cctx, compressionLevel));

                    // Load reference dictionary
                    fixed (byte* pDict = refData)
                    {
                        ValidateResult(Methods.ZSTD_CCtx_loadDictionary(cctx, pDict, (nuint)refData.Length));
                    }

                    // LDM and window settings
                    if (enableLongDistanceMatching)
                    {
                        ValidateResult(Methods.ZSTD_CCtx_setParameter(cctx, ZSTD_cParameter.ZSTD_c_enableLongDistanceMatching, 1));
                    }

                    if (pledgedSrcSize.HasValue)
                    {
                        // Inform pledged size; helps cli-like behavior when stdin has known size.
                        ValidateResult(Methods.ZSTD_CCtx_setPledgedSrcSize(cctx, pledgedSrcSize.Value));
                        // Heuristic for windowLog based on pledged size if not provided
                        int wl = windowLog ?? ComputeWindowLogHeuristic((long)Math.Min(pledgedSrcSize.Value, (ulong)int.MaxValue));
                        ValidateResult(Methods.ZSTD_CCtx_setParameter(cctx, ZSTD_cParameter.ZSTD_c_windowLog, wl));
                    }
                    else if (windowLog.HasValue)
                    {
                        ValidateResult(Methods.ZSTD_CCtx_setParameter(cctx, ZSTD_cParameter.ZSTD_c_windowLog, windowLog.Value));
                    }

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
        /// Stream-based patch apply, equivalent to `zstd -d --patch-from=REF`.
        /// </summary>
        /// <param name="input">Patch stream (compressed).</param>
        /// <param name="output">Destination stream to write reconstructed data.</param>
        /// <param name="refData">Reference bytes used during compression.</param>
        /// <param name="bufferInSize">Optional input buffer size; defaults to library recommended size.</param>
        /// <param name="bufferOutSize">Optional output buffer size; defaults to library recommended size.</param>
        public static void DecompressPatch(Stream input, Stream output, byte[] refData,
                                           int? bufferInSize = null,
                                           int? bufferOutSize = null)
        {
            if (input == null) throw new ArgumentNullException(nameof(input));
            if (output == null) throw new ArgumentNullException(nameof(output));
            if (refData == null) throw new ArgumentNullException(nameof(refData));
            if (!input.CanRead) throw new ArgumentException("Input stream must be readable.", nameof(input));
            if (!output.CanWrite) throw new ArgumentException("Output stream must be writable.", nameof(output));

            unsafe
            {
                ZSTD_DCtx_s* dctx = Methods.ZSTD_createDStream();
                try
                {
                    ValidateResult(Methods.ZSTD_initDStream(dctx));

                    // Load reference dictionary
                    fixed (byte* pDict = refData)
                    {
                        ValidateResult(Methods.ZSTD_DCtx_loadDictionary(dctx, pDict, (nuint)refData.Length));
                    }

                    int inSize = bufferInSize ?? (int)Methods.ZSTD_DStreamInSize();
                    int outSize = bufferOutSize ?? (int)Methods.ZSTD_DStreamOutSize();

                    byte[] inBuf = new byte[inSize];
                    byte[] outBuf = new byte[outSize];

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
        /// Compute a heuristic windowLog so that window size can cover the source size.
        /// windowLog = clamp(ceil(log2(srcSize)), bounds.lowerBound .. bounds.upperBound)
        /// </summary>
        private static int ComputeWindowLogHeuristic(long srcSize)
        {
            if (srcSize <= 0) srcSize = 1;
            double lg = Math.Log(srcSize, 2.0);
            int wl = (int)Math.Ceiling(lg);

            var bounds = Methods.ZSTD_cParam_getBounds(ZSTD_cParameter.ZSTD_c_windowLog);
            if (bounds.error != 0) return wl; // fallback
            wl = Math.Max(bounds.lowerBound, Math.Min(bounds.upperBound, wl));
            return wl;
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
            int len = 0;
            for (sbyte* p = ptr; *p != 0; p++) len++;
            return new string(ptr, 0, len);
        }
    }
}