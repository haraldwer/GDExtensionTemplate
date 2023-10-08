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

double GDExample::multiply(double x, double y)
{
    return x * y;
}

void GDExample2::hello_world()
{
    printf("Hello world from GDExample2\n");
}

void GDExample3::hello_world()
{
    printf("Hello world from GDExample3\n");
}

