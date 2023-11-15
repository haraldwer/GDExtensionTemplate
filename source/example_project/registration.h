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
enum REG_PROPERTY_PROPERTIES
{
	// [Type: PropertyHint]
	// Specify the property's PropertyHint (default is PROPERTY_HINT_NONE).
	// Automatically overridden if ExportAsNode/ExportAsResource is used.
	// Can also be used on TypedArrays of variant types.
	REG_P_HintType,
	// [Type: String]
	// Specify the hint string used by the property hint (default is empty string).
	// Automatically overridden if the property is a TypedArray or if ExportAsNode/ExportAsResource is used.
	// Can also be used on TypedArrays of variant types.
	REG_P_HintString,
	// [Type: PropertyUsageFlags]
	// Specify the property's PropertyUsageFlags (default is PROPERTY_USAGE_DEFAULT).
	REG_P_UsageFlags,
	// [Type: None]
	// Make it so you can expose pointers to classes that inherit from Node. Do not use together with ExportAsResource.
	// Use with TypedArrays to export an array of nodes.
	REG_P_ExportAsNode,
	// [Type: None]
	// Make it so you can expose Refs to classes that inherit from Resource. Do not use together with ExportAsNode.
	// Use with TypedArrays to export an array of resources.
	REG_P_ExportAsResource,
};

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
	// [Type: None]
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
