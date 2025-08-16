#!/usr/bin/env bash

if [ -d "./build/target/MyDiary.app" ]; then
    rm -r ./build/target/MyDiary.app
fi

dotnet publish MyDiary.UI -c Release -r osx-arm64 --self-contained


mkdir -p build/target/MyDiary.app/Contents/MacOS
mkdir -p build/target/MyDiary.app/Contents/Resources

cp -r  ./MyDiary.UI/bin/Release/net10.0/osx-arm64/publish/* ./build/target/MyDiary.app/Contents/MacOS/

cp ./build/resources/Info.plist ./build/target/MyDiary.app/Contents/
cp ./build/resources/App.icns  ./build/target/MyDiary.app/Contents/Resources

chmod +x ./build/target/MyDiary.app/Contents/MacOS/MyDiary.UI

xattr -cr ./build/target/MyDiary.app