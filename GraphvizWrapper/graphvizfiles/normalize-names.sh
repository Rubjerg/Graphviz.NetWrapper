#!/usr/bin/env bash
if [ $# -ne 1 ]; then
  echo "Usage: $0 <directory>"
  exit 1
fi

TARGET_DIR=$1

echo "Normalizing .so filenames under '$TARGET_DIR'..."

find "$TARGET_DIR" -type l -exec rm -v {} +

# Step 2: Rename each real .so.* file to match its SONAME, in place
find "$TARGET_DIR" -type f -name "lib*.so.*" | while read -r file; do
  soname=$(readelf -d "$file" 2>/dev/null | awk -F'[][]' '/SONAME/ { print $2 }')
  if [[ -z "$soname" ]]; then
    echo "No SONAME found in $file — skipping"
    continue
  fi

  dirname=$(dirname "$file")
  target="$dirname/$soname"

  if [[ "$(basename "$file")" != "$soname" ]]; then
    echo "Renaming $file -> $target"
    mv "$file" "$target"
  else
    echo "Skipping $file (already matches SONAME)"
  fi
done
