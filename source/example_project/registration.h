#pragma once

// Use these macros to automate registration

/**
 * Replace GDCLASS(name, parent) with #include REG_CLASS()
 */
#define REG_CLASS(...) 

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

/*
 * TODO:
 */
#define REG_SIGNAL()
