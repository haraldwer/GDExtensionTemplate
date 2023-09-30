#pragma once

#define REG_CLASS(name, parent) \
GDCLASS(name, parent) \
protected: \
static void _bind_methods(); \
private:

// Add a way to inject code!
// Preferably in REG_CLASS
// If not in IDE, use include
// #include ".generated/

#define REG_FUNCTION()
#define REG_PROPERTY()
#define REG_SIGNAL()

// Auto generated
#define REG_EXT_INITIALIZE() 
#define REG_EXT_DEINITIALIZE()
