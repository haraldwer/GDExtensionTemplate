@echo off
echo Starting RegAutomation
cmd /V /C RegAutomation.exe
echo Starting SCons
cmd /V /C scons --directory=%cd%/..