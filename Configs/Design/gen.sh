#!/bin/bash

set -euo pipefail

SCRIPT_DIR=$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)
WORKSPACE=$SCRIPT_DIR/..
REPO_ROOT=$WORKSPACE/..
LUBAN_DLL=$WORKSPACE/Tools/Luban/Luban.dll
CONF_ROOT=$SCRIPT_DIR
CODE_OUTPUT_DIR=$REPO_ROOT/GameServer.Config/Generated
DATA_OUTPUT_DIR=$WORKSPACE/Generated/Luban

mkdir -p "$CODE_OUTPUT_DIR" "$DATA_OUTPUT_DIR"

cd "$CONF_ROOT"

dotnet "$LUBAN_DLL" \
    -t all \
    -c cs-dotnet-json \
    -d json \
    --conf "$CONF_ROOT/luban.conf" \
    -x outputCodeDir="$CODE_OUTPUT_DIR" \
    -x outputDataDir="$DATA_OUTPUT_DIR"
