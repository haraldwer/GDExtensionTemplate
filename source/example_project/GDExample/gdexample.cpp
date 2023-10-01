#include "gdexample.h"

using namespace godot;

void GDExample::update(double delta)
{
    time_passed += delta;
    set_position(Vector3(Math::sin(time_passed) * property, 0.0f, 0.0f));
}

void GDExample::set_var(int var)
{
    printf("Var: %i\n", var);
}

