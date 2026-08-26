namespace RealNES.Core.Emulator.CPU.Decoder;

internal interface IDecoder
{
    void Step(ref readonly CpuState state);
}
