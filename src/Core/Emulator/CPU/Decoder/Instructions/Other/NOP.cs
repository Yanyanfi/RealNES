using RealNES.Core.Emulator.Communication;
using RealNES.Core.Emulator.CPU.Decoder.Instructions.Abstractions;
using System.Runtime.CompilerServices;

namespace RealNES.Core.Emulator.CPU.Decoder.Instructions.Other;

internal sealed class NOP(CpuBus bus,FlagSetter flagSetter) : InstructionBase(bus,flagSetter)
{
    public override IReadOnlyList<byte> OpCodes => throw new NotImplementedException();

    public override void Process(ref readonly CpuState state)
    {
        throw new NotImplementedException();
    }

    protected override AddressingType GetAddressingType(byte opCode)
    {
        throw new NotImplementedException();
    }
}
