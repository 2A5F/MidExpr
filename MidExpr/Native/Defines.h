#pragma once

#ifndef COPLT_EXPORT
#ifdef _MSC_VER
#define COPLT_EXPORT __declspec(dllexport)
#else
#define COPLT_EXPORT
#endif
#endif

#ifndef COPLT_FORCE_INLINE
#ifdef _MSC_VER
#define COPLT_FORCE_INLINE __forceinline
#else
#define COPLT_FORCE_INLINE inline __attribute__((__always_inline__))
#endif
#endif
