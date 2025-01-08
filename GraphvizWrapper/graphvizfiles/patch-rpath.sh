#!/bin/bash

# Modify the RPATH of all binaries to make sure all locally deployed dependencies can be found.

# Ensure a directory is provided
if [ $# -ne 1 ]; then
    echo "Usage: $0 <directory>"
    exit 1
fi

TARGET_DIR=$1

echo "Adding RPATH '\$ORIGIN' to all .so files in $TARGET_DIR..."

find "$TARGET_DIR" -type f -name "*.so*" | while read -r so_file; do
    echo "Processing: $so_file"
    patchelf --set-rpath '$ORIGIN' "$so_file" && echo "RPATH '\$ORIGIN' added to $so_file" || echo "Failed to patch $so_file"
done

echo "Adding RPATH '\$ORIGIN' to all executables in $TARGET_DIR..."

find "$TARGET_DIR" -type f -perm -111 | while read -r exe_file; do
    echo "Processing: $exe_file"
    patchelf --set-rpath '$ORIGIN' "$exe_file" && echo "RPATH '\$ORIGIN' added to $exe_file" || echo "Failed to patch $exe_file"
done

echo "RPATH update complete."

