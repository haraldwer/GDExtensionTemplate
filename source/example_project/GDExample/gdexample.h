#pragma once

#include "../reg.h"

#include <godot_cpp/classes/node3d.hpp>

namespace godot
{
    class GDExample : public Node3D
    {
        
        REG_CLASS(GDExample, Node3D)

    public: 
        void _process(double delta) override;
        
        REG_FUNCTION()
        void set_var(int var);

        REG_FUNCTION()
        void set_property(float p_property)
        {
            property = p_property;
        }

        REG_FUNCTION()
        float get_property() const { return property; }

        REG_PROPERTY()
        float property = 0; 
        
    private:
        double time_passed = 0; 
    };
}
