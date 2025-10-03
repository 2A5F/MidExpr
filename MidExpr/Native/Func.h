#pragma once

#include <vector>

#include "Defines.h"
#include "OpCode.h"

namespace Coplt
{
    struct MidFunc;

    struct MidFuncInitInfo
    {
        uint16_t* p_code;
        MidFunc const** p_func;
        uint32_t code_len;
        uint32_t func_len;
        uint32_t all_local_size;
    };

    struct MidFunc
    {
        std::vector<uint16_t> m_code;
        std::vector<const MidFunc*> m_fns;
        uint32_t m_all_local_size{};

        MidFunc() noexcept = default;
        ~MidFunc() noexcept = default;

        void Init(const MidFuncInitInfo& info) noexcept;
    };

    extern "C" COPLT_EXPORT void Coplt_MidFunc_Release(const MidFunc* self) noexcept;
    extern "C" COPLT_EXPORT MidFunc* Coplt_MidFunc_Alloc() noexcept;
    extern "C" COPLT_EXPORT void Coplt_MidFunc_Init(MidFunc* self, const MidFuncInitInfo* info) noexcept;
}
