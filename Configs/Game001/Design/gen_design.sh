#!/bin/bash

set -euo pipefail

SCRIPT_DIR=$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)
GAME_CONFIG_DIR=$(cd "$SCRIPT_DIR/.." && pwd)
REPO_ROOT=$(cd "$SCRIPT_DIR/../../.." && pwd)
LUBAN_DLL=$REPO_ROOT/Configs/Tools/Luban/Luban.dll
CONF_ROOT=$SCRIPT_DIR
CODE_OUTPUT_DIR=$REPO_ROOT/Game001.Core/UnityPackage/Runtime/Generated
DATA_OUTPUT_DIR=$GAME_CONFIG_DIR/Generated/Luban
UNITY_PROJECT_DIR=$(cd "$REPO_ROOT/../Game001" && pwd)
UNITY_CONFIG_OUTPUT_DIR=$UNITY_PROJECT_DIR/Assets/Games/Game001/Game001Resource/Configs

mkdir -p "$CODE_OUTPUT_DIR" "$DATA_OUTPUT_DIR" "$UNITY_CONFIG_OUTPUT_DIR"

cd "$CONF_ROOT"

dotnet "$LUBAN_DLL" \
    -t all \
    -c cs-dotnet-json \
    -d json \
    --conf "$CONF_ROOT/luban.conf" \
    -x outputCodeDir="$CODE_OUTPUT_DIR" \
    -x outputDataDir="$DATA_OUTPUT_DIR"

dotnet "$LUBAN_DLL" \
    -t client \
    -d json \
    --conf "$CONF_ROOT/luban.conf" \
    -x outputDataDir="$UNITY_CONFIG_OUTPUT_DIR"
