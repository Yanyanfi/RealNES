using RealNES.Core.Emulator.CPU.Decoder.Exceptions;
using RealNES.Core.Emulator.CPU.Decoder.Instructions.Abstractions;
using RealNES.Core.Emulator.CPU.Decoder.Instructions.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace RealNES.Core.Emulator.CPU.Decoder.Instructions.Bitwise;

internal sealed class ORA(InstructionServices services) : BitWiseBase(services)
{
    public override IReadOnlyList<byte> OpCodes { get; } = [0x09, 0x05, 0x15, 0x0d, 0x1d, 0x19, 0x01, 0x11];

    protected override AddressingType GetAddressingType(byte opCode)
    {
        return opCode switch
        {
            0x09 => AddressingType.Immediate,
            0x05 => AddressingType.ZeroPage,
            0x15 => AddressingType.ZeroPageX,
            0x0d => AddressingType.Absolute,
            0x1d => AddressingType.AbsoluteX,
            0x19 => AddressingType.AbsoluteY,
            0x01 => AddressingType.IndexedIndirect,
            0x11 => AddressingType.IndirectIndexed,
            _ => throw new MissingInstrException(opCode)
        };
    }

    protected override void ProcessData(in CpuState state, byte data) => state.A |= data;
}
