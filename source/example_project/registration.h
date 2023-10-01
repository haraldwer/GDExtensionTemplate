
// Delaying include guard intentionally
#include "registration_internal.h"
#pragma once

// Use these macros to automate registration

/**
 * Replace GDCLASS(name, parent) with #include REG_CLASS(name)
 */
#define REG_CLASS(name) \
REG_INJECT(name) \

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
