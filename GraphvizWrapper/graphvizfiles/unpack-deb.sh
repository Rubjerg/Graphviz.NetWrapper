#!/bin/bash

# Ensure a .deb file is provided
if [ $# -ne 1 ]; then
    echo "Usage: $0 <package.deb>"
    exit 1
fi

DEB_FILE=$1
TARGET_DIR="deb_output"

# Create a clean target directory
rm -rf "$TARGET_DIR"
mkdir -p "$TARGET_DIR"

# Extract the .deb file
echo "Extracting $DEB_FILE..."
ar x "$DEB_FILE" || { echo "Failed to extract $DEB_FILE"; exit 1; }

# Extract the data archive (data.tar.*)
if [ -f "data.tar.xz" ]; then
    tar -xvf data.tar.xz
elif [ -f "data.tar.gz" ]; then
    tar -xvzf data.tar.gz
else
    echo "No data.tar.* file found in $DEB_FILE"
    exit 1
fi

# Copy the desired folders to the target directory
echo "Copying usr/bin, usr/lib, and usr/lib/graphviz to $TARGET_DIR..."

cp ./usr/bin/* "$TARGET_DIR"
cp ./usr/lib/* "$TARGET_DIR"
cp ./usr/lib/graphviz/* "$TARGET_DIR"

# Clean up extracted files
rm -r control.tar.* data.tar.* debian-binary usr

echo "The specified directories have been copied to: $TARGET_DIR"
echo "Result:"
ls -R "$TARGET_DIR"

