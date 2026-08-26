using RealNES.Core.Emulator.CPU.Decoder.Exceptions;

namespace RealNES.Core.Emulator.CPU.Decoder.Instructions.Abstractions;

internal interface IInstruction
{
    IReadOnlyList<byte> OpCodes { get; }
    ///<exception cref="MissingInstrException"/>
    void Step(ref readonly CpuState state);
}
