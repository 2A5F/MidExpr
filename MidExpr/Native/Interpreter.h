#pragma once

#include "OpCode.h"
#include "Defines.h"
#include "Func.h"

namespace Coplt
{
    union alignas(16) MidReg
    {
        uint8_t u8;
        uint16_t u16;
        uint32_t u32;
        uint64_t u64;
        size_t usize;
        int8_t i8;
        int16_t i16;
        int32_t i32;
        int64_t i64;
        ptrdiff_t isize;
        float f32;
        double f64;

        MidReg() = default;

        // ReSharper disable once CppNonExplicitConvertingConstructor
        MidReg(const uint8_t v) : MidReg() { u8 = v; } // NOLINT(*-explicit-constructor)
        // ReSharper disable once CppNonExplicitConvertingConstructor
        MidReg(const uint16_t v) : MidReg() { u16 = v; } // NOLINT(*-explicit-constructor)
        // ReSharper disable once CppNonExplicitConvertingConstructor
        MidReg(const uint32_t v) : MidReg() { u32 = v; } // NOLINT(*-explicit-constructor)
        // ReSharper disable once CppNonExplicitConvertingConstructor
        MidReg(const uint64_t v) : MidReg() { u64 = v; } // NOLINT(*-explicit-constructor)
        // ReSharper disable once CppNonExplicitConvertingConstructor
        MidReg(const int8_t v) : MidReg() { i8 = v; } // NOLINT(*-explicit-constructor)
        // ReSharper disable once CppNonExplicitConvertingConstructor
        MidReg(const int16_t v) : MidReg() { i16 = v; } // NOLINT(*-explicit-constructor)
        // ReSharper disable once CppNonExplicitConvertingConstructor
        MidReg(const int32_t v) : MidReg() { i32 = v; } // NOLINT(*-explicit-constructor)
        // ReSharper disable once CppNonExplicitConvertingConstructor
        MidReg(const int64_t v) : MidReg() { i64 = v; } // NOLINT(*-explicit-constructor)
        // ReSharper disable once CppNonExplicitConvertingConstructor
        MidReg(const float v) : MidReg() { f32 = v; } // NOLINT(*-explicit-constructor)
        // ReSharper disable once CppNonExplicitConvertingConstructor
        MidReg(const double v) : MidReg() { f64 = v; } // NOLINT(*-explicit-constructor)
    };

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
