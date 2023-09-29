#include "gdexample.h"

using namespace godot;

void GDExample::_bind_methods() {
}

GDExample::GDExample() {
    time_passed = 0.0;
}

void GDExample::_process(double delta) {
    time_passed += delta;

    set_position(Vector3(Math::sin(time_passed), 0.0f, 0.0f));

    
}