#include "gdexample.h"

using namespace godot;


void GDExample::_process(double delta)
{
    time_passed += delta;
    set_position(Vector3(Math::sin(time_passed), 0.0f, 0.0f));
}

void GDExample::set_var(int var)
{
}
