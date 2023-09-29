#pragma once

#define REG_INCLUDE() // Depending on if generated content exists already

#define REG_GENERATED() // Generated content that is injected afterwards

#define REG_CLASS(name, parent) \
GDCLASS(name, parent) \
protected: \
static void _bind_methods(); \
private:

#define REG_FUNCTION()

#define REG_PROPERTY()

#define REG_SIGNAL()



// Auto generated
#define REG_EXT_INITIALIZE() 
#define REG_EXT_DEINITIALIZE()
