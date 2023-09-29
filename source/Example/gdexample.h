#pragma once

#include <godot_cpp/classes/node3d.hpp>

namespace godot {

class GDExample : public Node3D {
    GDCLASS(GDExample, Node3D)

private:
    double time_passed;

protected:
    static void _bind_methods();

public:
    GDExample();

    void _process(double delta);
};

}
