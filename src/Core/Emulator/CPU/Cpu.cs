using RealNES.Core.Emulator.CPU.Decoder;

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
    private bool _delayI;
    #endregion
    private readonly IDecoder _decoder = decoder;
    public void Step()
    {
        var state = new CpuState(ref _a, ref _x, ref _y, ref _pc, ref _sp, ref _p, ref _cycle, ref _delayI);
        _decoder.Step(in state);
    }
    public void Reset()
    {
        _sp = 0xfd;
        _p = 0x24;
    }
}