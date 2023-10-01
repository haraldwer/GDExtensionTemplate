#pragma once

#include "../registration.h"

#include <godot_cpp/classes/node3d.hpp>

namespace godot
{
    class GDExample : public Node3D
    {

        REG_CLASS()
    
    public:

        REG_FUNCTION()
        void update(double delta);
        
        REG_FUNCTION()
        void set_var(int var);
        
        REG_PROPERTY(PROPERTY_HINT_RANGE, "0,20,0.01")
        float property = 0;
        
    private:
        double time_passed = 0; 
    };
}
