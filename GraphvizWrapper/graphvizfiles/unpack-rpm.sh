#!/bin/bash

# Ensure an .rpm file is provided
if [ $# -ne 1 ]; then
    echo "Usage: $0 <package.rpm>"
    exit 1
fi

RPM_FILE=$1
TARGET_DIR="linux"
EXTRACT_DIR="rpm_extracted"

# Ensure rpm2cpio and cpio tools are installed
if ! command -v rpm2cpio &>/dev/null || ! command -v cpio &>/dev/null; then
    echo "Error: rpm2cpio and cpio are required but not installed."
    echo "Install them using: sudo apt-get install rpm2cpio cpio (on Debian or Ubuntu)"
    exit 1
fi

# Create clean target and extraction directories
rm -r "$TARGET_DIR" "$EXTRACT_DIR"
mkdir -p "$TARGET_DIR" "$EXTRACT_DIR"

# Extract the .rpm file into EXTRACT_DIR
echo "Extracting $RPM_FILE..."
rpm2cpio "$RPM_FILE" | cpio -idmv -D "$EXTRACT_DIR" || { echo "Failed to extract $RPM_FILE"; exit 1; }

# Copy the desired folders to the target directory
echo "Copying usr/bin, usr/lib, and usr/lib/graphviz to $TARGET_DIR..."

cp "./$EXTRACT_DIR/usr/bin/"* "$TARGET_DIR"
cp "./$EXTRACT_DIR/usr/lib64/"* "$TARGET_DIR"
mkdir -p "$TARGET_DIR/graphviz/"
cp "./$EXTRACT_DIR/usr/lib64/graphviz/"* "$TARGET_DIR/graphviz/"

# Clean up extraction directory
rm -r "$EXTRACT_DIR"

echo "The specified directories have been copied to: $TARGET_DIR"
echo "Result:"
ls -R "$TARGET_DIR"

