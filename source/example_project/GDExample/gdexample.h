#pragma once

#include "example_project/registration.h"

#include <godot_cpp/classes/node3d.hpp>
#include <godot_cpp/classes/image.hpp>

namespace godot
{
    class GDExample : public Node3D
    {

        REG_CLASS()

        REG_ENUM()
        enum MyEnum
        {
	        A = 1,
            B = 2,
            C, // implicit 3
            D, // implicit 4
            E = 10,
            F, // implicit 11
        };
    
    public:

        REG_FUNCTION()
        void update(double delta);
        
        REG_FUNCTION()
        void set_var(int var);

        REG_FUNCTION()
        static double multiply(double x, double y);
        
        REG_PROPERTY(
            REG_P_HintType=PROPERTY_HINT_RANGE,
            REG_P_HintString="0,20,0.01")
        float property = 0;

        REG_PROPERTY()
    	bool is_happy; 

        REG_PROPERTY()
        /*
         * (TEST COMMENT)
         */
    	bool angry;

        REG_PROPERTY(
            REG_P_HintType=PROPERTY_HINT_FILE,
            REG_P_HintString="*.png,*.webp,*.svg")
        TypedArray<String> image_filepath_array;

        REG_PROPERTY(
            REG_P_HintType=PROPERTY_HINT_RANGE,
            REG_P_HintString="0,1,0.001")
        TypedArray<float> float_array;

        REG_PROPERTY()
        PackedInt32Array packed_int32_array;
        
        REG_PROPERTY(REG_P_ExportAsResource)
        TypedArray<Image> image_array;

        REG_PROPERTY(REG_P_ExportAsResource)
        Ref<Image> image_ref;

        REG_PROPERTY(REG_P_ExportAsNode)
        TypedArray<Node3D> node_array;

        REG_PROPERTY(REG_P_ExportAsNode)
        Node3D *node_pointer = nullptr;
        
    private:
        
        double time_passed = 0;

    };
    class GDExample2 : public Node
    {

        REG_CLASS()

        REG_ENUM(REG_P_Bitfield)
        enum Animals
        {
            Cat = 1,
            Dog = 1 << 1,
            Monkey = 1 << 2,
            Giraffe = 1 << 3,
        };

        REG_PROPERTY()
    	String string_prop = "hi!!!";

    public:

        REG_FUNCTION()
        void hello_world();

    };
    class GDExample3 : public Node
    {

        REG_CLASS()


        REG_PROPERTY()
        int int_prop = 10;

    public:
        REG_ENUM()
        enum Fruit
        {
            Apple = 17,
            Orange = 10,
            Pear = 0,
            Grape = 78,
        };

        REG_FUNCTION()
        void hello_world();

    };
}
