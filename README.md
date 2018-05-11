# VidyoConnector-WPF

## Clone Repository
git clone https://github.com/Vidyo/VidyoConnector-WPF.git

## Overview
VidyoConnector-WPF is a Windows desktop application written in C# using WPF technology and MVVM approach. It contains single project within solution. 

## Acquire Framework
1. Download the latest Vidyo.io Windows SDK package for VisualStudio 2013 (https://static.vidyo.io/4.1.22.9/package/VidyoClient-WindowsSDK.zip) or for VisualStudio 2017 (https://static.vidyo.io/4.1.22.9/package/VidyoClient-WinVS2017SDK.zip).
2. Extract contents and locate '~\VidyoClient-WindowsSDK\samples\VidyoConnector' folder.

## Build and Run Application
1. Put VidyoConnector-WPF sources into folder located above, parallel to the 'win' folder.
2. Open 'VidyoConnector15.sln' file in VisualStudio.
3. Build solution.

> If you are unable to initially build the solution, please take a look at the post-build events. It contains a small script which copies the libVidyoClient.dll from Windows SDK to the VidyoConnector-WPF project output.

4. Run solution in debug or release mode.

## Notes
1. All files/classes in the 'sdk' solution folder are added as links to actual files in the SDK. So pay attention to its relative paths.
2. In the SDK package, '~\VidyoClient-WindowsSDK\lib\windows\' folder you will find 'Win32' and 'x64' folder, which both contain libVidyoClient.dll of appropriate architecture. The post-build events mentioned in note above should copy relevant version of this library to the sample's output in order to run and dedug the solution. So, if you are building sample application targeting x86 architecture, you should have libVidyoClient.dll in its output taken from 'Win32' SDK folder. And if you are building sample application targeting x64 architecture, you should have libVidyoClient.dll in its output taken from 'x64' SDK folder.
