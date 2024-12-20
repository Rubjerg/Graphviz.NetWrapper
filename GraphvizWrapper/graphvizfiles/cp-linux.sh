#!/bin/bash

# Ensure a directory is provided
if [ $# -ne 1 ]; then
    echo "Usage: $0 <directory>"
    exit 1
fi

SOURCE_DIR=$1
TARGET_DIR="linux"
# 
# Check if patchelf is installed
if ! command -v patchelf &>/dev/null; then
    echo "Error: patchelf is not installed. Install it using your package manager."
    exit 1
fi

rm -r "$TARGET_DIR"
mkdir -p "$TARGET_DIR"
cp -r "$SOURCE_DIR"/* "$TARGET_DIR"

# Find all .so files and add RPATH "."
echo "Adding RPATH '\$ORIGIN' to all .so files in $TARGET_DIR..."

find "$TARGET_DIR" -type f -name "*.so*" | while read -r so_file; do
    echo "Processing: $so_file"
    # Add or update RPATH to "."
    patchelf --set-rpath '$ORIGIN' "$so_file" && echo "RPATH '\$ORIGIN' added to $so_file" || echo "Failed to patch $so_file"
done

echo "RPATH update complete."
