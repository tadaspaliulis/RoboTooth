#pragma once
#pragma once
#ifdef VS_TESTBED
//If this is being built for Visual Studio testing, use the empty function definitions.
#include "WireEmpty.h"
#else
//If this is being built for arduino, use the actual arduino libraries.
#include "Wire.h"
#endif