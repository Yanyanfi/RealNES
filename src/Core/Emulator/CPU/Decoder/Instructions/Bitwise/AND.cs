using RealNES.Core.Emulator.CPU.Decoder.Exceptions;
using RealNES.Core.Emulator.CPU.Decoder.Instructions.Abstractions;
using RealNES.Core.Emulator.CPU.Decoder.Instructions.Enums;

namespace RealNES.Core.Emulator.CPU.Decoder.Instructions.Bitwise;

internal sealed class AND(InstructionServices services) : BitWiseBase(services)
{
    public override IReadOnlyList<byte> OpCodes { get; } = [0x29, 0x25, 0x35, 0x2d, 0x3d, 0x39, 0x21, 0x31];

    protected override AddressingType GetAddressingType(byte opCode)
    {
        return opCode switch
        {
            0x29 => AddressingType.Immediate,
            0x25 => AddressingType.ZeroPage,
            0x35 => AddressingType.ZeroPageX,
            0x2d => AddressingType.Absolute,
            0x3d => AddressingType.AbsoluteX,
            0x39 => AddressingType.AbsoluteY,
            0x21 => AddressingType.IndexedIndirect,
            0x31 => AddressingType.IndirectIndexed,
            _ => throw new MissingInstrException(opCode)
        };
    }

    protected override void ProcessData(in CpuState state, byte data) => state.A &= data;
}
