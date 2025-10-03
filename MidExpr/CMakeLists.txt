project(MidExpr.Native LANGUAGES CXX)

set(CMAKE_CXX_STANDARD 23)

add_library(${PROJECT_NAME} SHARED
        Native/lib.cc
        Native/lib.h
        Native/OpCode.h
        Native/Defines.h
        Native/Interpreter.cc
        Native/Interpreter.h
        Native/Func.cc
        Native/Func.h
)
