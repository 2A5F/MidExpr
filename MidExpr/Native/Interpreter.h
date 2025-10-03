#pragma once

#include <utility>

#include "OpCode.h"
#include "Defines.h"
#include "Func.h"

#ifdef __x86_64
#include <immintrin.h>
#endif

namespace Coplt
{
#ifdef __x86_64
    using MidReg = __m128;

    COPLT_FORCE_INLINE inline __m128 reg_ld(const uint8_t v)
    {
        return _mm_castsi128_ps(_mm_set_epi8(
            0, 0, 0, 0, 0, 0, 0, 0,
            0, 0, 0, 0, 0, 0, 0, static_cast<int8_t>(v)
        ));
    }

    COPLT_FORCE_INLINE inline __m128 reg_ld(const int8_t v)
    {
        return _mm_castsi128_ps(_mm_set_epi8(
            0, 0, 0, 0, 0, 0, 0, 0,
            0, 0, 0, 0, 0, 0, 0, v
        ));
    }

    COPLT_FORCE_INLINE inline __m128 reg_ld(const uint16_t v)
    {
        return _mm_castsi128_ps(_mm_set_epi16(
            0, 0, 0, 0, 0, 0, 0, static_cast<int16_t>(v)
        ));
    }

    COPLT_FORCE_INLINE inline __m128 reg_ld(const int16_t v)
    {
        return _mm_castsi128_ps(_mm_set_epi16(
            0, 0, 0, 0, 0, 0, 0, v
        ));
    }

    COPLT_FORCE_INLINE inline __m128 reg_ld(const uint32_t v)
    {
        return _mm_castsi128_ps(_mm_set_epi32(
            0, 0, 0, static_cast<int32_t>(v)
        ));
    }

    COPLT_FORCE_INLINE inline __m128 reg_ld(const int32_t v)
    {
        return _mm_castsi128_ps(_mm_set_epi32(
            0, 0, 0, v
        ));
    }

    COPLT_FORCE_INLINE inline __m128 reg_ld(const uint64_t v)
    {
        return _mm_castsi128_ps(_mm_set_epi64x(
            0, static_cast<int64_t>(v)
        ));
    }

    COPLT_FORCE_INLINE inline __m128 reg_ld(const int64_t v)
    {
        return _mm_castsi128_ps(_mm_set_epi64x(
            0, v
        ));
    }

    COPLT_FORCE_INLINE inline __m128 reg_ld(const float v)
    {
        return _mm_set_ps(0, 0, 0, v);
    }

    COPLT_FORCE_INLINE inline __m128 reg_ld(const double v)
    {
        return _mm_castpd_ps(_mm_set_pd(0, v));
    }

    COPLT_FORCE_INLINE inline uint8_t reg_get_u8(const __m128 v)
    {
        return static_cast<uint8_t>(_mm_extract_epi8(v, 0));
    }

    COPLT_FORCE_INLINE inline int8_t reg_get_i8(const __m128 v)
    {
        return static_cast<int8_t>(_mm_extract_epi8(v, 0));
    }

    COPLT_FORCE_INLINE inline uint16_t reg_get_u16(const __m128 v)
    {
        return static_cast<uint16_t>(_mm_extract_epi16(v, 0));
    }

    COPLT_FORCE_INLINE inline int16_t reg_get_i16(const __m128 v)
    {
        return static_cast<int16_t>(_mm_extract_epi16(v, 0));
    }

    COPLT_FORCE_INLINE inline uint32_t reg_get_u32(const __m128 v)
    {
        return static_cast<uint32_t>(_mm_extract_epi32(v, 0));
    }

    COPLT_FORCE_INLINE inline int32_t reg_get_i32(const __m128 v)
    {
        return _mm_extract_epi32(v, 0);
    }

    COPLT_FORCE_INLINE inline uint64_t reg_get_u64(const __m128 v)
    {
        return static_cast<uint64_t>(_mm_extract_epi64(v, 0));
    }

    COPLT_FORCE_INLINE inline int64_t reg_get_i64(const __m128 v)
    {
        return _mm_extract_epi64(v, 0);
    }

    COPLT_FORCE_INLINE inline float reg_get_f32(const __m128 v)
    {
        return _mm_extract_ps(v, 0);
    }

    COPLT_FORCE_INLINE inline double reg_get_f64(const __m128 v)
    {
        return _mm_cvtsd_f64(_mm_castps_pd(v));
    }

    COPLT_FORCE_INLINE inline size_t reg_get_usize(const __m128 v)
    {
        if constexpr (sizeof(size_t) == 4)
        {
            return static_cast<size_t>(_mm_extract_epi32(v, 0));
        }
        else
        {
            return static_cast<size_t>(_mm_extract_epi64(v, 0));
        }
    }

    COPLT_FORCE_INLINE inline __m128 reg_and(const __m128 a, const __m128 b)
    {
        return _mm_and_ps(a, b);
    }

    COPLT_FORCE_INLINE inline __m128 reg_add_u8(const __m128 a, const __m128 b)
    {
        return _mm_castsi128_ps(_mm_add_epi8(_mm_castps_si128(a), _mm_castps_si128(b)));
    }

    COPLT_FORCE_INLINE inline __m128 reg_add_u16(const __m128 a, const __m128 b)
    {
        return _mm_castsi128_ps(_mm_add_epi16(_mm_castps_si128(a), _mm_castps_si128(b)));
    }

    COPLT_FORCE_INLINE inline __m128 reg_add_u32(const __m128 a, const __m128 b)
    {
        return _mm_castsi128_ps(_mm_add_epi32(_mm_castps_si128(a), _mm_castps_si128(b)));
    }

    COPLT_FORCE_INLINE inline __m128 reg_add_u64(const __m128 a, const __m128 b)
    {
        return _mm_castsi128_ps(_mm_add_epi64(_mm_castps_si128(a), _mm_castps_si128(b)));
    }

    COPLT_FORCE_INLINE inline __m128 reg_add_f32(const __m128 a, const __m128 b)
    {
        return _mm_add_ps(a, b);
    }

    COPLT_FORCE_INLINE inline __m128 reg_add_f64(const __m128 a, const __m128 b)
    {
        return _mm_castpd_ps(_mm_add_pd(_mm_castps_pd(a), _mm_castps_pd(b)));
    }

    COPLT_FORCE_INLINE inline __m128 reg_sub_u8(const __m128 a, const __m128 b)
    {
        return _mm_castsi128_ps(_mm_sub_epi8(_mm_castps_si128(a), _mm_castps_si128(b)));
    }

    COPLT_FORCE_INLINE inline __m128 reg_sub_u16(const __m128 a, const __m128 b)
    {
        return _mm_castsi128_ps(_mm_sub_epi16(_mm_castps_si128(a), _mm_castps_si128(b)));
    }

    COPLT_FORCE_INLINE inline __m128 reg_sub_u32(const __m128 a, const __m128 b)
    {
        return _mm_castsi128_ps(_mm_sub_epi32(_mm_castps_si128(a), _mm_castps_si128(b)));
    }

    COPLT_FORCE_INLINE inline __m128 reg_sub_u64(const __m128 a, const __m128 b)
    {
        return _mm_castsi128_ps(_mm_sub_epi64(_mm_castps_si128(a), _mm_castps_si128(b)));
    }

    COPLT_FORCE_INLINE inline __m128 reg_sub_f32(const __m128 a, const __m128 b)
    {
        return _mm_sub_ps(a, b);
    }

    COPLT_FORCE_INLINE inline __m128 reg_sub_f64(const __m128 a, const __m128 b)
    {
        return _mm_castpd_ps(_mm_sub_pd(_mm_castps_pd(a), _mm_castps_pd(b)));
    }

    COPLT_FORCE_INLINE inline __m128 reg_mul_u8(const __m128 a, const __m128 b)
    {
        return reg_ld(static_cast<uint8_t>(reg_get_u8(a) * reg_get_u8(b)));
    }

    COPLT_FORCE_INLINE inline __m128 reg_mul_u16(const __m128 a, const __m128 b)
    {
        return _mm_castsi128_ps(_mm_mullo_epi16(_mm_castps_si128(a), _mm_castps_si128(b)));
    }

    COPLT_FORCE_INLINE inline __m128 reg_mul_u32(const __m128 a, const __m128 b)
    {
        return _mm_castsi128_ps(_mm_mul_epi32(_mm_castps_si128(a), _mm_castps_si128(b)));
    }

    COPLT_FORCE_INLINE inline __m128 reg_mul_u64(const __m128 a, const __m128 b)
    {
        return reg_ld(reg_get_u64(a) * reg_get_u64(b));
    }

    COPLT_FORCE_INLINE inline __m128 reg_mul_f32(const __m128 a, const __m128 b)
    {
        return _mm_mul_ps(a, b);
    }

    COPLT_FORCE_INLINE inline __m128 reg_mul_f64(const __m128 a, const __m128 b)
    {
        return _mm_castpd_ps(_mm_mul_pd(_mm_castps_pd(a), _mm_castps_pd(b)));
    }

    COPLT_FORCE_INLINE inline __m128 reg_div_u8(const __m128 a, const __m128 b)
    {
        return reg_ld(static_cast<uint8_t>(reg_get_u8(a) / reg_get_u8(b)));
    }

    COPLT_FORCE_INLINE inline __m128 reg_div_u16(const __m128 a, const __m128 b)
    {
        return reg_ld(static_cast<uint16_t>(reg_get_u16(a) / reg_get_u16(b)));
    }

    COPLT_FORCE_INLINE inline __m128 reg_div_u32(const __m128 a, const __m128 b)
    {
        return reg_ld(reg_get_u32(a) / reg_get_u32(b));
    }

    COPLT_FORCE_INLINE inline __m128 reg_div_u64(const __m128 a, const __m128 b)
    {
        return reg_ld(reg_get_u64(a) / reg_get_u64(b));
    }

    COPLT_FORCE_INLINE inline __m128 reg_div_f32(const __m128 a, const __m128 b)
    {
        return _mm_div_ps(a, b);
    }

    COPLT_FORCE_INLINE inline __m128 reg_div_f64(const __m128 a, const __m128 b)
    {
        return _mm_castpd_ps(_mm_div_pd(_mm_castps_pd(a), _mm_castps_pd(b)));
    }

    struct MidRegs
    {
        MidReg r0;
        MidReg r1;
        MidReg r2;
        MidReg r3;
        MidReg r4;
        MidReg r5;
        MidReg r6;
        MidReg r7;
    };
#endif

    struct MidFrame
    {
        const MidFunc* m_func;
        char* m_p_stack;
        char* m_p_calc_stack;
        uint32_t m_code_offset;
    };

    extern "C" COPLT_EXPORT void Coplt_MidInterpreter_Exec(
        const MidFunc* root_func,
        MidRegs* regs,
        char* stack_data,
        MidFrame* stack_frames,
        uint32_t stack_data_len,
        uint32_t stack_frames_len
    );
}
