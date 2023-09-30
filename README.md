# GodotCpp

This template project provides some useful functionality for working with GDExtension in Visual Studio / Rider in Windows. 

## Features
 * A .sln solution with an example .vcxproj project
 * Recursive SConstruct file (allowing for cpp files in subfolders)
 * One-click IDE compilation + debugging
 * A pre-compile step that automatically registers classes, members and variables (similar to the unreal UCLASS/UPROPERTY functionality) (WIP)
 * Some useful bat scripts 

## Prerequsites
 * Godot 4 executable
 * C++ compiler
 * SCons
 * .NETFramework v4.8

See: [Compiling Godot](https://docs.godotengine.org/en/stable/contributing/development/compiling/compiling_for_windows.html#requirements)

## Setup 
 * Clone the repository
 * Copy godot executable to repository folder and rename to "godot.exe"

## Usage
Some macros exist in reg.h that provide automated registration for classes, functions and properties. 
Here is how to use them:
 * REG_CLASS() - Use this macro instead of GDCLASS()
 * REG_FUNCTION() - Put this in front of your function
 * REG_PROPERTY() - Put this in front of your property
The extension.cpp with class and function binding will be automatically generated based on extension.cpp.template. If you wish to add code to extension.cpp, just modify the template and your changes will be applied during next rebuild.  

## Notes
 * Intellisense / Intellij will only work after first compile
 * The RegAutomation project will compile and run when you compile your c++ code. RegAutomation is responsible for generating the registration code and you can find the resulting files in /source/{project name}/.generated/. 
 * NMake .sln requires an exe path without parameters. The GodotRun project is a workaround for this. 
