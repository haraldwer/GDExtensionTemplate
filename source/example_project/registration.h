#pragma once

// Use these macros to automate registration

/**
 * Replace GDCLASS(name, parent) with #include REG_CLASS()
 */
#define REG_CLASS(...) REG_CLASS_INTERNAL_HIDE(REG_CLASS_INTERNAL(__LINE__))

/**
 * Usage:
 * REG_FUNCTION()
 * void func(float param);
 */
#define REG_FUNCTION(...)

/**
 * Usage:
 * REG_PROPERTY()
 * float property = 0.0f;
 */
#define REG_PROPERTY(...)

/**
* Usage:
* REG_ENUM()
* enum MyEnum
*	{
*		EntryA,
*		EntryB = 5,
*		EntryC,
*		EntryD = 15
*	}
*/
#define REG_ENUM(...)
enum REG_ENUM_PROPERTIES
{
	// If passed into REG_ENUM, interpret the enum as a Godot bitfield.
	REG_P_Bitfield,
};

/*
 * TODO:
 */
#define REG_SIGNAL()



// Internal macros, do not use!
// REG_CLASS() -> REG_CLASS_LINE_<line number>()
#define REG_CLASS_INTERNAL_HIDE(x)
#define REG_CLASS_INTERNAL_CONCAT(x) REG_CLASS_LINE_ ## x()
#define REG_CLASS_INTERNAL(x) REG_CLASS_INTERNAL_CONCAT(x)
