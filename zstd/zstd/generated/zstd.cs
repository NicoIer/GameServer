using System;
using System.Runtime.InteropServices;

namespace zstd
{
    internal partial struct ZSTD_CCtx_s
    {
    }

    internal partial struct ZSTD_DCtx_s
    {
    }

    [NativeTypeName("unsigned int")]
    internal enum ZSTD_strategy : uint
    {
        ZSTD_fast = 1,
        ZSTD_dfast = 2,
        ZSTD_greedy = 3,
        ZSTD_lazy = 4,
        ZSTD_lazy2 = 5,
        ZSTD_btlazy2 = 6,
        ZSTD_btopt = 7,
        ZSTD_btultra = 8,
        ZSTD_btultra2 = 9,
    }

    [NativeTypeName("unsigned int")]
    internal enum ZSTD_cParameter : uint
    {
        ZSTD_c_compressionLevel = 100,
        ZSTD_c_windowLog = 101,
        ZSTD_c_hashLog = 102,
        ZSTD_c_chainLog = 103,
        ZSTD_c_searchLog = 104,
        ZSTD_c_minMatch = 105,
        ZSTD_c_targetLength = 106,
        ZSTD_c_strategy = 107,
        ZSTD_c_targetCBlockSize = 130,
        ZSTD_c_enableLongDistanceMatching = 160,
        ZSTD_c_ldmHashLog = 161,
        ZSTD_c_ldmMinMatch = 162,
        ZSTD_c_ldmBucketSizeLog = 163,
        ZSTD_c_ldmHashRateLog = 164,
        ZSTD_c_contentSizeFlag = 200,
        ZSTD_c_checksumFlag = 201,
        ZSTD_c_dictIDFlag = 202,
        ZSTD_c_nbWorkers = 400,
        ZSTD_c_jobSize = 401,
        ZSTD_c_overlapLog = 402,
        ZSTD_c_experimentalParam1 = 500,
        ZSTD_c_experimentalParam2 = 10,
        ZSTD_c_experimentalParam3 = 1000,
        ZSTD_c_experimentalParam4 = 1001,
        ZSTD_c_experimentalParam5 = 1002,
        ZSTD_c_experimentalParam7 = 1004,
        ZSTD_c_experimentalParam8 = 1005,
        ZSTD_c_experimentalParam9 = 1006,
        ZSTD_c_experimentalParam10 = 1007,
        ZSTD_c_experimentalParam11 = 1008,
        ZSTD_c_experimentalParam12 = 1009,
        ZSTD_c_experimentalParam13 = 1010,
        ZSTD_c_experimentalParam14 = 1011,
        ZSTD_c_experimentalParam15 = 1012,
        ZSTD_c_experimentalParam16 = 1013,
        ZSTD_c_experimentalParam17 = 1014,
        ZSTD_c_experimentalParam18 = 1015,
        ZSTD_c_experimentalParam19 = 1016,
        ZSTD_c_experimentalParam20 = 1017,
    }

    internal partial struct ZSTD_bounds
    {
        [NativeTypeName("size_t")]
        public nuint error;

        public int lowerBound;

        public int upperBound;
    }

    [NativeTypeName("unsigned int")]
    internal enum ZSTD_ResetDirective : uint
    {
        ZSTD_reset_session_only = 1,
        ZSTD_reset_parameters = 2,
        ZSTD_reset_session_and_parameters = 3,
    }

    [NativeTypeName("unsigned int")]
    internal enum ZSTD_dParameter : uint
    {
        ZSTD_d_windowLogMax = 100,
        ZSTD_d_experimentalParam1 = 1000,
        ZSTD_d_experimentalParam2 = 1001,
        ZSTD_d_experimentalParam3 = 1002,
        ZSTD_d_experimentalParam4 = 1003,
        ZSTD_d_experimentalParam5 = 1004,
        ZSTD_d_experimentalParam6 = 1005,
    }

    internal unsafe partial struct ZSTD_inBuffer_s
    {
        [NativeTypeName("const void *")]
        public void* src;

        [NativeTypeName("size_t")]
        public nuint size;

        [NativeTypeName("size_t")]
        public nuint pos;
    }

    internal unsafe partial struct ZSTD_outBuffer_s
    {
        public void* dst;

        [NativeTypeName("size_t")]
        public nuint size;

        [NativeTypeName("size_t")]
        public nuint pos;
    }

    [NativeTypeName("unsigned int")]
    internal enum ZSTD_EndDirective : uint
    {
        ZSTD_e_continue = 0,
        ZSTD_e_flush = 1,
        ZSTD_e_end = 2,
    }

    internal partial struct ZSTD_CDict_s
    {
    }

    internal partial struct ZSTD_DDict_s
    {
    }

    internal static unsafe partial class Methods
    {
        [DllImport(zstd_dll.ZSTD_DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        [return: NativeTypeName("unsigned int")]
        public static extern uint ZSTD_versionNumber();

        [DllImport(zstd_dll.ZSTD_DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        [return: NativeTypeName("const char *")]
        public static extern sbyte* ZSTD_versionString();

        [DllImport(zstd_dll.ZSTD_DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        [return: NativeTypeName("size_t")]
        public static extern nuint ZSTD_compress(void* dst, [NativeTypeName("size_t")] nuint dstCapacity, [NativeTypeName("const void *")] void* src, [NativeTypeName("size_t")] nuint srcSize, int compressionLevel);

        [DllImport(zstd_dll.ZSTD_DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        [return: NativeTypeName("size_t")]
        public static extern nuint ZSTD_decompress(void* dst, [NativeTypeName("size_t")] nuint dstCapacity, [NativeTypeName("const void *")] void* src, [NativeTypeName("size_t")] nuint compressedSize);

        [DllImport(zstd_dll.ZSTD_DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        [return: NativeTypeName("unsigned long long")]
        public static extern ulong ZSTD_getFrameContentSize([NativeTypeName("const void *")] void* src, [NativeTypeName("size_t")] nuint srcSize);

        [DllImport(zstd_dll.ZSTD_DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        [return: NativeTypeName("unsigned long long")]
        [Obsolete("Replaced by ZSTD_getFrameContentSize")]
        public static extern ulong ZSTD_getDecompressedSize([NativeTypeName("const void *")] void* src, [NativeTypeName("size_t")] nuint srcSize);

        [DllImport(zstd_dll.ZSTD_DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        [return: NativeTypeName("size_t")]
        public static extern nuint ZSTD_findFrameCompressedSize([NativeTypeName("const void *")] void* src, [NativeTypeName("size_t")] nuint srcSize);

        [DllImport(zstd_dll.ZSTD_DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        [return: NativeTypeName("size_t")]
        public static extern nuint ZSTD_compressBound([NativeTypeName("size_t")] nuint srcSize);

        [DllImport(zstd_dll.ZSTD_DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        [return: NativeTypeName("unsigned int")]
        public static extern uint ZSTD_isError([NativeTypeName("size_t")] nuint result);

        [DllImport(zstd_dll.ZSTD_DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern ZSTD_ErrorCode ZSTD_getErrorCode([NativeTypeName("size_t")] nuint functionResult);

        [DllImport(zstd_dll.ZSTD_DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        [return: NativeTypeName("const char *")]
        public static extern sbyte* ZSTD_getErrorName([NativeTypeName("size_t")] nuint result);

        [DllImport(zstd_dll.ZSTD_DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern int ZSTD_minCLevel();

        [DllImport(zstd_dll.ZSTD_DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern int ZSTD_maxCLevel();

        [DllImport(zstd_dll.ZSTD_DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern int ZSTD_defaultCLevel();

        [DllImport(zstd_dll.ZSTD_DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        [return: NativeTypeName("ZSTD_CCtx *")]
        public static extern ZSTD_CCtx_s* ZSTD_createCCtx();

        [DllImport(zstd_dll.ZSTD_DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        [return: NativeTypeName("size_t")]
        public static extern nuint ZSTD_freeCCtx([NativeTypeName("ZSTD_CCtx *")] ZSTD_CCtx_s* cctx);

        [DllImport(zstd_dll.ZSTD_DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        [return: NativeTypeName("size_t")]
        public static extern nuint ZSTD_compressCCtx([NativeTypeName("ZSTD_CCtx *")] ZSTD_CCtx_s* cctx, void* dst, [NativeTypeName("size_t")] nuint dstCapacity, [NativeTypeName("const void *")] void* src, [NativeTypeName("size_t")] nuint srcSize, int compressionLevel);

        [DllImport(zstd_dll.ZSTD_DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        [return: NativeTypeName("ZSTD_DCtx *")]
        public static extern ZSTD_DCtx_s* ZSTD_createDCtx();

        [DllImport(zstd_dll.ZSTD_DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        [return: NativeTypeName("size_t")]
        public static extern nuint ZSTD_freeDCtx([NativeTypeName("ZSTD_DCtx *")] ZSTD_DCtx_s* dctx);

        [DllImport(zstd_dll.ZSTD_DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        [return: NativeTypeName("size_t")]
        public static extern nuint ZSTD_decompressDCtx([NativeTypeName("ZSTD_DCtx *")] ZSTD_DCtx_s* dctx, void* dst, [NativeTypeName("size_t")] nuint dstCapacity, [NativeTypeName("const void *")] void* src, [NativeTypeName("size_t")] nuint srcSize);

        [DllImport(zstd_dll.ZSTD_DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern ZSTD_bounds ZSTD_cParam_getBounds(ZSTD_cParameter cParam);

        [DllImport(zstd_dll.ZSTD_DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        [return: NativeTypeName("size_t")]
        public static extern nuint ZSTD_CCtx_setParameter([NativeTypeName("ZSTD_CCtx *")] ZSTD_CCtx_s* cctx, ZSTD_cParameter param1, int value);

        [DllImport(zstd_dll.ZSTD_DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        [return: NativeTypeName("size_t")]
        public static extern nuint ZSTD_CCtx_setPledgedSrcSize([NativeTypeName("ZSTD_CCtx *")] ZSTD_CCtx_s* cctx, [NativeTypeName("unsigned long long")] ulong pledgedSrcSize);

        [DllImport(zstd_dll.ZSTD_DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        [return: NativeTypeName("size_t")]
        public static extern nuint ZSTD_CCtx_reset([NativeTypeName("ZSTD_CCtx *")] ZSTD_CCtx_s* cctx, ZSTD_ResetDirective reset);

        [DllImport(zstd_dll.ZSTD_DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        [return: NativeTypeName("size_t")]
        public static extern nuint ZSTD_compress2([NativeTypeName("ZSTD_CCtx *")] ZSTD_CCtx_s* cctx, void* dst, [NativeTypeName("size_t")] nuint dstCapacity, [NativeTypeName("const void *")] void* src, [NativeTypeName("size_t")] nuint srcSize);

        [DllImport(zstd_dll.ZSTD_DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern ZSTD_bounds ZSTD_dParam_getBounds(ZSTD_dParameter dParam);

        [DllImport(zstd_dll.ZSTD_DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        [return: NativeTypeName("size_t")]
        public static extern nuint ZSTD_DCtx_setParameter([NativeTypeName("ZSTD_DCtx *")] ZSTD_DCtx_s* dctx, ZSTD_dParameter param1, int value);

        [DllImport(zstd_dll.ZSTD_DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        [return: NativeTypeName("size_t")]
        public static extern nuint ZSTD_DCtx_reset([NativeTypeName("ZSTD_DCtx *")] ZSTD_DCtx_s* dctx, ZSTD_ResetDirective reset);

        [DllImport(zstd_dll.ZSTD_DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        [return: NativeTypeName("ZSTD_CStream *")]
        public static extern ZSTD_CCtx_s* ZSTD_createCStream();

        [DllImport(zstd_dll.ZSTD_DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        [return: NativeTypeName("size_t")]
        public static extern nuint ZSTD_freeCStream([NativeTypeName("ZSTD_CStream *")] ZSTD_CCtx_s* zcs);

        [DllImport(zstd_dll.ZSTD_DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        [return: NativeTypeName("size_t")]
        public static extern nuint ZSTD_compressStream2([NativeTypeName("ZSTD_CCtx *")] ZSTD_CCtx_s* cctx, [NativeTypeName("ZSTD_outBuffer *")] ZSTD_outBuffer_s* output, [NativeTypeName("ZSTD_inBuffer *")] ZSTD_inBuffer_s* input, ZSTD_EndDirective endOp);

        [DllImport(zstd_dll.ZSTD_DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        [return: NativeTypeName("size_t")]
        public static extern nuint ZSTD_CStreamInSize();

        [DllImport(zstd_dll.ZSTD_DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        [return: NativeTypeName("size_t")]
        public static extern nuint ZSTD_CStreamOutSize();

        [DllImport(zstd_dll.ZSTD_DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        [return: NativeTypeName("size_t")]
        public static extern nuint ZSTD_initCStream([NativeTypeName("ZSTD_CStream *")] ZSTD_CCtx_s* zcs, int compressionLevel);

        [DllImport(zstd_dll.ZSTD_DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        [return: NativeTypeName("size_t")]
        public static extern nuint ZSTD_compressStream([NativeTypeName("ZSTD_CStream *")] ZSTD_CCtx_s* zcs, [NativeTypeName("ZSTD_outBuffer *")] ZSTD_outBuffer_s* output, [NativeTypeName("ZSTD_inBuffer *")] ZSTD_inBuffer_s* input);

        [DllImport(zstd_dll.ZSTD_DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        [return: NativeTypeName("size_t")]
        public static extern nuint ZSTD_flushStream([NativeTypeName("ZSTD_CStream *")] ZSTD_CCtx_s* zcs, [NativeTypeName("ZSTD_outBuffer *")] ZSTD_outBuffer_s* output);

        [DllImport(zstd_dll.ZSTD_DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        [return: NativeTypeName("size_t")]
        public static extern nuint ZSTD_endStream([NativeTypeName("ZSTD_CStream *")] ZSTD_CCtx_s* zcs, [NativeTypeName("ZSTD_outBuffer *")] ZSTD_outBuffer_s* output);

        [DllImport(zstd_dll.ZSTD_DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        [return: NativeTypeName("ZSTD_DStream *")]
        public static extern ZSTD_DCtx_s* ZSTD_createDStream();

        [DllImport(zstd_dll.ZSTD_DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        [return: NativeTypeName("size_t")]
        public static extern nuint ZSTD_freeDStream([NativeTypeName("ZSTD_DStream *")] ZSTD_DCtx_s* zds);

        [DllImport(zstd_dll.ZSTD_DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        [return: NativeTypeName("size_t")]
        public static extern nuint ZSTD_initDStream([NativeTypeName("ZSTD_DStream *")] ZSTD_DCtx_s* zds);

        [DllImport(zstd_dll.ZSTD_DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        [return: NativeTypeName("size_t")]
        public static extern nuint ZSTD_decompressStream([NativeTypeName("ZSTD_DStream *")] ZSTD_DCtx_s* zds, [NativeTypeName("ZSTD_outBuffer *")] ZSTD_outBuffer_s* output, [NativeTypeName("ZSTD_inBuffer *")] ZSTD_inBuffer_s* input);

        [DllImport(zstd_dll.ZSTD_DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        [return: NativeTypeName("size_t")]
        public static extern nuint ZSTD_DStreamInSize();

        [DllImport(zstd_dll.ZSTD_DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        [return: NativeTypeName("size_t")]
        public static extern nuint ZSTD_DStreamOutSize();

        [DllImport(zstd_dll.ZSTD_DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        [return: NativeTypeName("size_t")]
        public static extern nuint ZSTD_compress_usingDict([NativeTypeName("ZSTD_CCtx *")] ZSTD_CCtx_s* ctx, void* dst, [NativeTypeName("size_t")] nuint dstCapacity, [NativeTypeName("const void *")] void* src, [NativeTypeName("size_t")] nuint srcSize, [NativeTypeName("const void *")] void* dict, [NativeTypeName("size_t")] nuint dictSize, int compressionLevel);

        [DllImport(zstd_dll.ZSTD_DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        [return: NativeTypeName("size_t")]
        public static extern nuint ZSTD_decompress_usingDict([NativeTypeName("ZSTD_DCtx *")] ZSTD_DCtx_s* dctx, void* dst, [NativeTypeName("size_t")] nuint dstCapacity, [NativeTypeName("const void *")] void* src, [NativeTypeName("size_t")] nuint srcSize, [NativeTypeName("const void *")] void* dict, [NativeTypeName("size_t")] nuint dictSize);

        [DllImport(zstd_dll.ZSTD_DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        [return: NativeTypeName("ZSTD_CDict *")]
        public static extern ZSTD_CDict_s* ZSTD_createCDict([NativeTypeName("const void *")] void* dictBuffer, [NativeTypeName("size_t")] nuint dictSize, int compressionLevel);

        [DllImport(zstd_dll.ZSTD_DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        [return: NativeTypeName("size_t")]
        public static extern nuint ZSTD_freeCDict([NativeTypeName("ZSTD_CDict *")] ZSTD_CDict_s* CDict);

        [DllImport(zstd_dll.ZSTD_DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        [return: NativeTypeName("size_t")]
        public static extern nuint ZSTD_compress_usingCDict([NativeTypeName("ZSTD_CCtx *")] ZSTD_CCtx_s* cctx, void* dst, [NativeTypeName("size_t")] nuint dstCapacity, [NativeTypeName("const void *")] void* src, [NativeTypeName("size_t")] nuint srcSize, [NativeTypeName("const ZSTD_CDict *")] ZSTD_CDict_s* cdict);

        [DllImport(zstd_dll.ZSTD_DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        [return: NativeTypeName("ZSTD_DDict *")]
        public static extern ZSTD_DDict_s* ZSTD_createDDict([NativeTypeName("const void *")] void* dictBuffer, [NativeTypeName("size_t")] nuint dictSize);

        [DllImport(zstd_dll.ZSTD_DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        [return: NativeTypeName("size_t")]
        public static extern nuint ZSTD_freeDDict([NativeTypeName("ZSTD_DDict *")] ZSTD_DDict_s* ddict);

        [DllImport(zstd_dll.ZSTD_DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        [return: NativeTypeName("size_t")]
        public static extern nuint ZSTD_decompress_usingDDict([NativeTypeName("ZSTD_DCtx *")] ZSTD_DCtx_s* dctx, void* dst, [NativeTypeName("size_t")] nuint dstCapacity, [NativeTypeName("const void *")] void* src, [NativeTypeName("size_t")] nuint srcSize, [NativeTypeName("const ZSTD_DDict *")] ZSTD_DDict_s* ddict);

        [DllImport(zstd_dll.ZSTD_DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        [return: NativeTypeName("unsigned int")]
        public static extern uint ZSTD_getDictID_fromDict([NativeTypeName("const void *")] void* dict, [NativeTypeName("size_t")] nuint dictSize);

        [DllImport(zstd_dll.ZSTD_DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        [return: NativeTypeName("unsigned int")]
        public static extern uint ZSTD_getDictID_fromCDict([NativeTypeName("const ZSTD_CDict *")] ZSTD_CDict_s* cdict);

        [DllImport(zstd_dll.ZSTD_DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        [return: NativeTypeName("unsigned int")]
        public static extern uint ZSTD_getDictID_fromDDict([NativeTypeName("const ZSTD_DDict *")] ZSTD_DDict_s* ddict);

        [DllImport(zstd_dll.ZSTD_DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        [return: NativeTypeName("unsigned int")]
        public static extern uint ZSTD_getDictID_fromFrame([NativeTypeName("const void *")] void* src, [NativeTypeName("size_t")] nuint srcSize);

        [DllImport(zstd_dll.ZSTD_DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        [return: NativeTypeName("size_t")]
        public static extern nuint ZSTD_CCtx_loadDictionary([NativeTypeName("ZSTD_CCtx *")] ZSTD_CCtx_s* cctx, [NativeTypeName("const void *")] void* dict, [NativeTypeName("size_t")] nuint dictSize);

        [DllImport(zstd_dll.ZSTD_DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        [return: NativeTypeName("size_t")]
        public static extern nuint ZSTD_CCtx_refCDict([NativeTypeName("ZSTD_CCtx *")] ZSTD_CCtx_s* cctx, [NativeTypeName("const ZSTD_CDict *")] ZSTD_CDict_s* cdict);

        [DllImport(zstd_dll.ZSTD_DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        [return: NativeTypeName("size_t")]
        public static extern nuint ZSTD_CCtx_refPrefix([NativeTypeName("ZSTD_CCtx *")] ZSTD_CCtx_s* cctx, [NativeTypeName("const void *")] void* prefix, [NativeTypeName("size_t")] nuint prefixSize);

        [DllImport(zstd_dll.ZSTD_DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        [return: NativeTypeName("size_t")]
        public static extern nuint ZSTD_DCtx_loadDictionary([NativeTypeName("ZSTD_DCtx *")] ZSTD_DCtx_s* dctx, [NativeTypeName("const void *")] void* dict, [NativeTypeName("size_t")] nuint dictSize);

        [DllImport(zstd_dll.ZSTD_DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        [return: NativeTypeName("size_t")]
        public static extern nuint ZSTD_DCtx_refDDict([NativeTypeName("ZSTD_DCtx *")] ZSTD_DCtx_s* dctx, [NativeTypeName("const ZSTD_DDict *")] ZSTD_DDict_s* ddict);

        [DllImport(zstd_dll.ZSTD_DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        [return: NativeTypeName("size_t")]
        public static extern nuint ZSTD_DCtx_refPrefix([NativeTypeName("ZSTD_DCtx *")] ZSTD_DCtx_s* dctx, [NativeTypeName("const void *")] void* prefix, [NativeTypeName("size_t")] nuint prefixSize);

        [DllImport(zstd_dll.ZSTD_DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        [return: NativeTypeName("size_t")]
        public static extern nuint ZSTD_sizeof_CCtx([NativeTypeName("const ZSTD_CCtx *")] ZSTD_CCtx_s* cctx);

        [DllImport(zstd_dll.ZSTD_DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        [return: NativeTypeName("size_t")]
        public static extern nuint ZSTD_sizeof_DCtx([NativeTypeName("const ZSTD_DCtx *")] ZSTD_DCtx_s* dctx);

        [DllImport(zstd_dll.ZSTD_DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        [return: NativeTypeName("size_t")]
        public static extern nuint ZSTD_sizeof_CStream([NativeTypeName("const ZSTD_CStream *")] ZSTD_CCtx_s* zcs);

        [DllImport(zstd_dll.ZSTD_DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        [return: NativeTypeName("size_t")]
        public static extern nuint ZSTD_sizeof_DStream([NativeTypeName("const ZSTD_DStream *")] ZSTD_DCtx_s* zds);

        [DllImport(zstd_dll.ZSTD_DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        [return: NativeTypeName("size_t")]
        public static extern nuint ZSTD_sizeof_CDict([NativeTypeName("const ZSTD_CDict *")] ZSTD_CDict_s* cdict);

        [DllImport(zstd_dll.ZSTD_DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        [return: NativeTypeName("size_t")]
        public static extern nuint ZSTD_sizeof_DDict([NativeTypeName("const ZSTD_DDict *")] ZSTD_DDict_s* ddict);
    }
}
