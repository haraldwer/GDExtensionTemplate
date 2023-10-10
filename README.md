# GDExtension Template

This template project provides some useful functionality for working with GDExtension in Visual Studio / Rider in Windows. 

## Features
 * A ``.sln`` solution with an example ``.vcxproj`` project
 * Recursive ``SConstruct`` file (allowing for cpp files in subfolders)
 * One-click IDE compilation
 * A pre-compile step that automatically registers classes, members and variables (similar to the unreal UCLASS/UPROPERTY functionality) (WIP)
 * Some useful .bat scripts 

## Prerequsites
 * Godot 4 executable
 * .NETFramework v4.8 (older versions might also work)
 * Scoop

See [Compiling Godot](https://docs.godotengine.org/en/stable/contributing/development/compiling/compiling_for_windows.html#requirements)

## Setup 
 * Run command ``scoop install gcc python scons make mingw llvm``
 * Clone the repository. If the ``godot-cpp`` folder is empty, try ``--recursive`` when cloning to also get the submodules.
 * Copy godot executable to repository folder and rename to ``godot.exe``.
 * In your IDE, use ``ProjectLauncher`` configuration.

## Usage
Some macros exist in reg.h that provide automated registration for classes, functions and properties. 
Here is how to use them:
 * ``REG_CLASS()`` - Use this macro instead of GDCLASS().
 * ``REG_FUNCTION()`` - Put this in front of your function.
 * ``REG_PROPERTY()`` - Put this in front of your property. Supports ``PropertyInfo`` meta parameters.
 * ``REG_ENUM()`` - Put this in front of your enum.

There is an example class called GDExample that you can use for reference. 
Registration-code will be injected into ``extension.cpp``. Class bindings will be generated based on ``reg_class.template``. 

## Known issues / Future work
 * The debugger does not attach automatically to the godot process. You can still attach manually.

## Notes
 * Intellisense / Intellij will only work after first compile.
 * The ``RegAutomation`` project will compile and run when you compile your c++ code. ``RegAutomation.exe`` is responsible for generating the registration code and you can find the resulting files in ``/source/{project name}/.generated/``. 
 * The ``.sln`` and ``.vcxproj`` is based on the generated result of running ``scons platform=windows vsproj=yes`` in the godot engine repository.
 * NMake .sln requires an exe path without parameters. The ``ProjectLauncher`` project is a workaround for this.
 * I have not yet tested multiple GDExtension projects in the same solution.
