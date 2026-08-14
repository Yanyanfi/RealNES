using RealNES.Core.Emulator.CPU.Decoder.Exceptions;
using RealNES.Core.Emulator.CPU.Decoder.Instructions.Abstractions;
using RealNES.Core.Emulator.CPU.Decoder.Instructions.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace RealNES.Core.Emulator.CPU.Decoder.Instructions.Bitwise;

internal sealed class EOR(InstructionServices services) : BitWiseBase(services)
{
    public override IReadOnlyList<byte> OpCodes { get; } = [0x49, 0x45, 0x55, 0x4d, 0x5d, 0x59, 0x41, 0x51];

    protected override AddressingType GetAddressingType(byte opCode) => opCode switch
    {
        0x49 => AddressingType.Immediate,
        0x45 => AddressingType.ZeroPage,
        0x55 => AddressingType.ZeroPageX,
        0x4d => AddressingType.Absolute,
        0x5d => AddressingType.AbsoluteX,
        0x59 => AddressingType.AbsoluteY,
        0x41 => AddressingType.IndexedIndirect,
        0x51 => AddressingType.IndirectIndexed,
        _ => throw new MissingInstrException(opCode)
    };

    protected override void ProcessData(in CpuState state, byte data) => state.A ^= data;
}
