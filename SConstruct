
#!/usr/bin/env python
import os
import sys

env = SConscript("godot-cpp/SConstruct")

def getSubdirs(path) :  
    lst = [ os.path.join(path, name) 
        for name in os.listdir(path) 
            if os.path.isdir(os.path.join(path, name)) and 
                name != ".idea" and 
                name != "x64" and 
                name != ".temp" and 
                name != "RegAutomation" and
                name != "ProjectLauncher"]
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

# Add project includes
env.Append(INCLUDE="source");

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
