#include "extension.h"

#include <gdextension_interface.h>
#include <godot_cpp/core/defs.hpp>
#include <godot_cpp/core/class_db.hpp>
#include <godot_cpp/godot.hpp>

#ifndef REG_IN_IDE
#include ".generated/reg_incl.generated.h"
#endif

using namespace godot;

void initialize_extension(ModuleInitializationLevel p_level)
{
    if (p_level != MODULE_INITIALIZATION_LEVEL_SCENE)
        return;

    #ifndef REG_IN_IDE
    #include ".generated/reg_init.generated.h"
    #endif

    
}

void uninitialize_extension(ModuleInitializationLevel p_level)
{
    if (p_level != MODULE_INITIALIZATION_LEVEL_SCENE)
        return;
    
    #ifndef REG_IN_IDE
    #include ".generated/reg_deinit.generated.h"
    #endif
}

extern "C" {
    
    // Initialization.
    GDExtensionBool GDE_EXPORT example_library_init(GDExtensionInterfaceGetProcAddress p_get_proc_address, const GDExtensionClassLibraryPtr p_library, GDExtensionInitialization *r_initialization)
    {
        GDExtensionBinding::InitObject init_obj(p_get_proc_address, p_library, r_initialization);

        init_obj.register_initializer(initialize_extension);
        init_obj.register_terminator(uninitialize_extension);
        init_obj.set_minimum_library_initialization_level(MODULE_INITIALIZATION_LEVEL_SCENE);

        return init_obj.init();
    }
    
}