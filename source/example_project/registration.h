#pragma once

// Use these macros to automate registration

/**
 * Replace GDCLASS(name, parent) with this macro
 */
#define REG_CLASS(name, parent)         \
GDCLASS(name, parent)                   \
protected:                              \
static void _bind_methods();            \
private:

/**
 * Usage:
 * REG_FUNCTION()
 * void func(float param);
 */
#define REG_FUNCTION()

/**
 * Usage:
 * REG_PROPERTY()
 * float property = 0.0f;
 */
#define REG_PROPERTY()

/*
 * TODO:
 */
#define REG_SIGNAL()

#include "registration_internal.h"