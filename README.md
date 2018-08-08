# VidyoConnector-WPF

## Clone Repository
git clone https://github.com/Vidyo/VidyoConnector-WPF.git

## Overview
VidyoConnector-WPF is a Windows desktop application written in C# using WPF technology and MVVM approach. It contains single project within solution. 

## Acquire Framework
1. Download the latest Vidyo.io Windows SDK package for VisualStudio 2013 (https://static.vidyo.io/latest/package/VidyoClient-WindowsSDK.zip) or for VisualStudio 2017 (https://static.vidyo.io/latest/package/VidyoClient-WinVS2017SDK.zip).
2. Extract contents and locate '~\VidyoClient-WindowsSDK\samples\VidyoConnector' folder.
> Note: VidyoClient SDK version 4.1.23.21 or later is required.

## Build and Run Application
1. Put VidyoConnector-WPF sources into folder located above, parallel to the 'win' folder.
2. Open 'VidyoConnector15.sln' file in VisualStudio.
3. Build solution.
4. Run solution in debug or release mode.

## About the VidyoClient Native DLL
The VidyoClient SDK folder `lib\windows\` has the subfolders `Win32` and `x64`. Each of those subfolders contains the VidyoClient native DLL for the associated target architecture:

* Files in that `Win32` folder are for the `x86` (32-bit) platform.
* Files in that `x64` folder are for the `x64` (64-bit) platform.

The VidyoClient SDK provides __Release__ configurations of the DLL, but not __Debug__ configurations.

This project has links to the DLL, in order to copy it to the same output directory as the generated application.

* The links are based on relative paths.
* The project is configured to use the Release DLL, even when performing Debug builds.

This project implements a special scheme for choosing which native DLL (`Win32` or `x64`) to copy during builds. That choice depends on the current target platform (`x86` or `x64`) and configuration (Debug or Release). Refer to the [StackOverflow article "Are there any better ways to copy a native dll to the bin folder?"](https://stackoverflow.com/questions/3863419/) for information.

For simplicity, this project does not support the platform `Any CPU`.

## Notes
1. All files/classes in the 'sdk' solution folder are added as links to actual files in the SDK. So pay attention to its relative paths.
