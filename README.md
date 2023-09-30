# GodotProject

This template project provides some useful functionality for working with GDExtension in Visual Studio / Rider in Windows. 

Features: 
 - A .sln solution with an example .vcxproj project
 - Recursive SConstruct file (allowing for cpp files in subfolders)
 - One-click IDE compilation + debugging
 - A pre-compile step that automatically registers classes, members and variables (similar to the unreal UCLASS/UPROPERTY functionality) (WIP)
 - Some useful bat scripts 

Prerequsites:
 - Godot 4 executable
 - C++ compiler
 - SCons
 - .NETFramework v4.8

See: [Compiling Godot](https://docs.godotengine.org/en/stable/contributing/development/compiling/compiling_for_windows.html#requirements)

Setup: 
 - Clone the repository
 - Copy godot executable to repository folder and rename to "godot.exe"

Notes: 
 - Intellisense / Intellij will only work after first compile
 - NMake .sln requires an exe path without parameters. The GodotRun project is a workaround for this. 
