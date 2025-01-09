#!/bin/bash

# Modify the RPATH of all binaries to make sure all locally deployed dependencies can be found.

# Ensure a directory is provided
if [ $# -ne 1 ]; then
    echo "Usage: $0 <directory>"
    exit 1
fi

TARGET_DIR=$1

echo "Adding RPATH '\$ORIGIN' to all binaries in $TARGET_DIR..."

find "$TARGET_DIR" -maxdepth 1 -type f -perm -111 | while read -r exe_file; do
    echo "Processing: $exe_file"
    patchelf --set-rpath '$ORIGIN' "$exe_file" && echo "RPATH '\$ORIGIN' added to $exe_file" || echo "Failed to patch $exe_file"
done

echo "Adding RPATH '\$ORIGIN' and '\$ORIGIN/../' to all files in $TARGET_DIR/graphviz..."

find "$TARGET_DIR/graphviz" -type f \( -perm -111 \) | while read -r graphviz_file; do
    echo "Processing: $graphviz_file"
    
    # Set RPATH to include both $ORIGIN and $ORIGIN/../
    patchelf --set-rpath '$ORIGIN:$ORIGIN/../' "$graphviz_file" && \
        echo "RPATH '\$ORIGIN:\$ORIGIN/../' added to $graphviz_file" || \
        echo "Failed to patch $graphviz_file"
done


echo "RPATH update complete."

