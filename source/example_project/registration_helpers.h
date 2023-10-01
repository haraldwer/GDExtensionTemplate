#pragma once

#ifndef REG_IN_IDE

// Macro for adding quotes
#define REG_STRINGIFY(X) REG_STRINGIFY2(X)    
#define REG_STRINGIFY2(X) #X

// Macros for concatenating tokens
#define REG_CAT(X,Y) REG_CAT2(X,Y)
#define REG_CAT2(X,Y) X##Y
#define REG_CAT_2 REG_CAT

#endif