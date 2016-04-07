#!/bin/bash

# Implements logic to build https://github.com/AzureAD/azure-activedirectory-library-for-objc and produce required libs
# Usage: place this script to azure-activedirectory-library-for-ios repo root and run

BUILD_PATH="build"
BUILD_CONFIGURATION="Debug"

PROJECTS_TO_BUILD=(ADALiOS)

for i in "${PROJECTS_TO_BUILD[@]}"
do
	proj="${i}"
	echo "Building $proj"
	xcodebuild -workspace ADALiOS.xcworkspace -scheme $proj -configuration $BUILD_CONFIGURATION ARCHS="i386 x86_64" -sdk iphonesimulator VALID_ARCHS="i386 x86_64" ONLY_ACTIVE_ARCH=NO CONFIGURATION_BUILD_DIR="../build/emulator" clean build
	xcodebuild -workspace ADALiOS.xcworkspace -scheme $proj -configuration $BUILD_CONFIGURATION ARCHS="armv7 armv7s arm64" -sdk iphoneos VALID_ARCHS="armv7 armv7s arm64" CONFIGURATION_BUILD_DIR="../build/device" clean build
	echo "Creating universal version of $proj"
	rm -rf "$BUILD_PATH/$proj.framework"
	# Initial framework structure (to be updated later)
	cp -R "$BUILD_PATH/emulator/$proj.framework" "$BUILD_PATH/$proj.framework"

	simulatorLibPath="$BUILD_PATH/emulator/lib$proj.a"
	deviceLibPath="$BUILD_PATH/device/lib$proj.a"
	universalLibPath="$BUILD_PATH/lib$proj.a"

	lipo "$simulatorLibPath" "$deviceLibPath" -create -output "$universalLibPath"
	lipo -info "$universalLibPath"
done

# Build ADALiOS.bundle (temporary disabled as it does not work for some reason, required files are linked via <resource-file>)
#xcodebuild -workspace ADALiOS.xcworkspace -scheme ADALiOSBundle clean build CONFIGURATION_BUILD_DIR="../build/"

# Update generated framework (use universal lib version and add missing resources)
cp -R "$BUILD_PATH/libADALiOS.a" "$BUILD_PATH/$proj.framework/Versions/A/ADALiOS"
#cp -R "$BUILD_PATH/ADALiOS.bundle" "$BUILD_PATH/$proj.framework/Versions/A/Resources/ADALiOS.bundle"

echo "Done. Build artifacts could be found at '$BUILD_PATH' folder"