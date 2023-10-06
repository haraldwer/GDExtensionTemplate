#pragma once

#include "example_project/registration.h"

#include "gdexample.h"

namespace godot
{
    class GDInheritanceExample : public GDExample
    {
        REG_CLASS()

    private:

        REG_PROPERTY()
        float child_property = 1.0f; 
        
    };
}
