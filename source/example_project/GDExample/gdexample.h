#pragma once

#include "example_project/registration.h"

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

        REG_PROPERTY()
    	bool is_happy;

        REG_PROPERTY()
    	bool angry;
        
    private:
        
        double time_passed = 0;

    };
}
