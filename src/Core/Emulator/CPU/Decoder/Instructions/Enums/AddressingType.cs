using System;
using System.Collections.Generic;
using System.Text;

namespace RealNES.Core.Emulator.CPU.Decoder.Instructions.Enums;

internal enum AddressingType
{
    Implicit,
    Accumulator,
    Immediate,
    ZeroPage,
    ZeroPageX,
    ZeroPageY,
    Relative,
    Absolute,
    AbsoluteX,
    AbsoluteY,
    Indirect,
    IndexedIndirect,
    IndirectIndexed
}
