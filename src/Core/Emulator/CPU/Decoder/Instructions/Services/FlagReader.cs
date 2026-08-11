namespace RealNES.Core.Emulator.CPU.Decoder.Instructions.Services;

internal class FlagReader
{
    public bool ReadCarry(ref readonly CpuState state) => ReadFlag(in state, 0);
    public bool ReadZero(ref readonly CpuState state) => ReadFlag(in state, 1);
    public bool ReadInterruptDisable(ref readonly CpuState state) => ReadFlag(in state, 2);
    public bool ReadDecimal(ref readonly CpuState state) => ReadFlag(in state, 3);
    public bool ReadBreak(ref readonly CpuState state) => ReadFlag(in state, 4);
    public bool ReadOverflow(ref readonly CpuState state) => ReadFlag(in state, 6);
    public bool ReadNegative(ref readonly CpuState state) => ReadFlag(in state, 7);
    private static bool ReadFlag(ref readonly CpuState state, int bitIndex) => ((state.P >> bitIndex) & 1) == 1;
}
