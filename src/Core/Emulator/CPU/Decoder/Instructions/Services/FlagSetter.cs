using System;
using System.Collections.Generic;
using System.Text;

namespace RealNES.Core.Emulator.CPU.Decoder.Instructions.Services;

internal sealed class FlagSetter
{
    public void SetCarry(ref readonly CpuState state, bool value) => SetFlag(in state, 0, value);
    public void SetZero(ref readonly CpuState state, bool value) => SetFlag(in state, 1, value);
    public void SetInterruptDisable(ref readonly CpuState state, bool value) => SetFlag(in state, 2, value);
    public void SetDecimal(ref readonly CpuState state, bool value) => SetFlag(in state, 3, value);
    public void SetBreak(ref readonly CpuState state, bool value) => SetFlag(in state, 4, value);
    public void SetOverflow(ref readonly CpuState state, bool value) => SetFlag(in state, 6, value);
    public void SetNegative(ref readonly CpuState state, bool value) => SetFlag(in state, 7, value);
    public void SetZeroByNumber(ref readonly CpuState state, byte number) => SetZero(in state, number == 0);
    public void SetNegativeByNumber(ref readonly CpuState state, byte number) => SetZero(in state, number > 127);
    private static void SetFlag(ref readonly CpuState state,int bitIndex,bool value)
    {
        var oddFlag = state.P;
        if (value)
            state.P = (byte)(oddFlag | (1 << bitIndex));
        else
            state.P = (byte)(oddFlag & ~(1 << bitIndex));
    }
}
