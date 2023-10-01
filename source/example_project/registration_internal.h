// Skipping include guard intentionally

#include "registration_helpers.h"

// Code injection
// TODO: Maybe remove? 
#ifdef REG_IN_IDE
#define REG_INCL_GEN(X) // Do nothing
#else
#define REG_INCL_GEN(X)                 \
REG_STRINGIFY(REG_CAT_2(X,.gen))   
#endif

