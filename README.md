# GodotCpp

This template project provides some useful functionality for working with GDExtension in Visual Studio / Rider in Windows. 

## Features
 * A ``.sln`` solution with an example ``.vcxproj`` project
 * Recursive ``SConstruct`` file (allowing for cpp files in subfolders)
 * One-click IDE compilation
 * A pre-compile step that automatically registers classes, members and variables (similar to the unreal UCLASS/UPROPERTY functionality) (WIP)
 * Some useful .bat scripts 

## Prerequsites
 * Godot 4 executable
 * C++ compiler
 * SCons
 * .NETFramework v4.8 (older versions might also work)

See [Compiling Godot](https://docs.godotengine.org/en/stable/contributing/development/compiling/compiling_for_windows.html#requirements)

## Setup 
 * Clone the repository
 * Copy godot executable to repository folder and rename to ``godot.exe``

## Usage
Some macros exist in reg.h that provide automated registration for classes, functions and properties. 
Here is how to use them:
 * ``REG_CLASS()`` - Use this macro instead of GDCLASS()
 * ``REG_FUNCTION()`` - Put this in front of your function
 * ``REG_PROPERTY()`` - Put this in front of your property

Registration-code will be injected into ``extension.cpp``. Class bindings will be generated based on ``reg_class.template``. 

## Known issues
 * The debugger does not attach automatically to the godot process. You can still attach manually.

## Notes
 * The ``.sln`` and ``.vcxproj`` is based on the generated result of running ``scons platform=windows vsproj=yes``
 * Intellisense / Intellij will only work after first compile
 * The ``RegAutomation`` project will compile and run when you compile your c++ code. ``RegAutomation.exe`` is responsible for generating the registration code and you can find the resulting files in ``/source/{project name}/.generated/``. 
 * NMake .sln requires an exe path without parameters. The ``ProjectLauncher`` project is a workaround for this.
 * This has only been tested using Rider 2023.2
