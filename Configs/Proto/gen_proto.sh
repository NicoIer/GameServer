#!/bin/bash

set -euo pipefail

SCRIPT_DIR=$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)
WORKSPACE=$SCRIPT_DIR/..
REPO_ROOT=$WORKSPACE/..
PROTO_ROOT=$SCRIPT_DIR
PROTOC=$WORKSPACE/Tools/protoc/bin/protoc
PROTOC_INCLUDE=$WORKSPACE/Tools/protoc/include
CORE_OUTPUT_DIR=$REPO_ROOT/GameServer.Core/UnityPackage/Runtime/Generated

mkdir -p "$CORE_OUTPUT_DIR"
find "$CORE_OUTPUT_DIR" -maxdepth 1 -name '*.cs' -type f -delete

find_grpc_csharp_plugin() {
    local protoc_bin_dir
    protoc_bin_dir=$(dirname "$PROTOC")

    if [ -f "$protoc_bin_dir/grpc_csharp_plugin" ]; then
        echo "$protoc_bin_dir/grpc_csharp_plugin"
        return
    fi

    local rids=()
    case "$(uname -s):$(uname -m)" in
        Darwin:arm64) rids=("macosx_arm64" "macosx_x64") ;;
        Darwin:*) rids=("macosx_x64") ;;
        Linux:aarch64|Linux:arm64) rids=("linux_arm64") ;;
        Linux:x86_64|Linux:amd64) rids=("linux_x64") ;;
        Linux:*) rids=("linux_x86") ;;
        *) rids=("macosx_x64" "linux_x64" "windows_x64") ;;
    esac

    local rid
    for rid in "${rids[@]}"; do
        local plugin
        plugin=$(find "$HOME/.nuget/packages/grpc.tools" -path "*/tools/$rid/grpc_csharp_plugin" -type f 2>/dev/null | sort -V | tail -n 1 || true)
        if [ -n "$plugin" ]; then
            echo "$plugin"
            return
        fi
    done
}

if ! find "$PROTO_ROOT" -name '*.proto' -type f | grep -q .; then
    exit 0
fi

if [ ! -x "$PROTOC" ]; then
    chmod +x "$PROTOC"
fi

GRPC_CSHARP_PLUGIN=$(find_grpc_csharp_plugin)
if [ -z "$GRPC_CSHARP_PLUGIN" ]; then
    echo "grpc_csharp_plugin not found. Run 'dotnet restore GameServer.Core/GameServer.Core.csproj' or place it next to protoc." >&2
    exit 1
fi

if [ ! -x "$GRPC_CSHARP_PLUGIN" ]; then
    chmod +x "$GRPC_CSHARP_PLUGIN"
fi

PROTO_FILES=()
while IFS= read -r proto_file; do
    PROTO_FILES+=("$proto_file")
done < <(find "$PROTO_ROOT" -name '*.proto' -type f | sort)

"$PROTOC" \
    -I "$PROTO_ROOT" \
    -I "$PROTOC_INCLUDE" \
    --csharp_out="$CORE_OUTPUT_DIR" \
    --grpc_out="$CORE_OUTPUT_DIR" \
    --plugin=protoc-gen-grpc="$GRPC_CSHARP_PLUGIN" \
    "${PROTO_FILES[@]}"
