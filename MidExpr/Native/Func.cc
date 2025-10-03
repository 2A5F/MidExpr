#include "Func.h"

using namespace Coplt;

void MidFunc::Init(const MidFuncInitInfo& info) noexcept
{
    m_code = std::vector(info.p_code, info.p_code + info.code_len);
    m_fns = std::vector(info.p_func, info.p_func + info.func_len);
    m_all_local_size = info.all_local_size;
}

void Coplt::Coplt_MidFunc_Release(const MidFunc* self) noexcept
{
    delete self;
}

MidFunc* Coplt::Coplt_MidFunc_Alloc() noexcept
{
    return new MidFunc();
}

void Coplt::Coplt_MidFunc_Init(MidFunc* self, const MidFuncInitInfo* info) noexcept
{
    self->Init(*info);
}
