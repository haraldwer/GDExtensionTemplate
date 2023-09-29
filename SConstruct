
#!/usr/bin/env python
import os
import sys

env = SConscript("godot-cpp/SConstruct")

# For reference:
# - CCFLAGS are compilation flags shared between C and C++
# - CFLAGS are for C-specific compilation flags
# - CXXFLAGS are for C++-specific compilation flags
# - CPPFLAGS are for pre-processor flags
# - CPPDEFINES are for pre-processor defines
# - LINKFLAGS are for linking flags

def getSubdirs(path) :  
    lst = [ os.path.join(path, name) for name in os.listdir(path) if os.path.isdir(os.path.join(path, name)) and name != ".idea" and name != "x64" ]
    return lst

def getDirsRec(path) :
    lst = getSubdirs(path)
    for subdir in lst :
        lst += getDirsRec(subdir)
    return lst

sources = Glob("source/*.cpp")
env.Append(CPPPATH=["source/"])

subdirs = getDirsRec('source')
for subdir in subdirs :
    sources += Glob(subdir + "/*.cpp")
    env.Append(CPPPATH=[subdir + "/"])


if env["platform"] == "macos":
    library = env.SharedLibrary(
        "project/bin/libgdexample.{}.{}.framework/libgdexample.{}.{}".format(
            env["platform"], env["target"], env["platform"], env["target"]
        ),
        source=sources,
    )
else:
    library = env.SharedLibrary(
        "project/bin/libgdexample{}{}".format(env["suffix"], env["SHLIBSUFFIX"]),
        source=sources,
    )

Default(library)
