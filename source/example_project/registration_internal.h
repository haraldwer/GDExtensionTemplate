// Skipping include guard intentionally

#ifndef REG_IN_IDE

// Macro for adding quotes
#define REG_STRINGIFY(X) REG_STRINGIFY2(X)    
#define REG_STRINGIFY2(X) #X

// Macros for concatenating tokens
#define REG_CAT(X,Y) REG_CAT2(X,Y)
#define REG_CAT2(X,Y) X##Y
#define REG_CAT_2 REG_CAT
#define REG_CAT_3(X,Y,Z) REG_CAT(X,REG_CAT(Y,Z))

#define REG_INJECT(X) \
REG_STRINGIFY(REG_CAT_3(.generated/,X,.injected.h))   

#else

#define REG_INJECT(X) __FILE__ // Inject itself = doing nothing

#endif
