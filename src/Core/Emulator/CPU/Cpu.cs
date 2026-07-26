using RealNES.Core.Emulator.CPU.Decoder;
using System;
using System.Collections.Generic;
using System.Text;

namespace RealNES.Core.Emulator.CPU;

internal sealed class Cpu(IDecoder decoder)
{
    #region state
    private byte _a;
    private byte _x;
    private byte _y;
    private ushort _pc;
    private byte _sp;
    private byte _p;
    private int _cycle;
    #endregion
    public void Step()
    {
        var state = new CpuState(ref _a, ref _x, ref _y, ref _pc, ref _sp, ref _p, ref _cycle);
        decoder.Step(in state);
    }

}