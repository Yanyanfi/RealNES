using RealNES.Core.Emulator.Memory;

namespace RealNES.Core.Emulator.Communication;

internal sealed class CpuBus(InternalRam cpuRam)
{
    private readonly InternalRam _cpuRam = cpuRam;
    public byte LastData { get; private set; }
    public ushort LastAddress { get; private set; }
    public bool IsWrite { get; private set; }
    public bool IsReadOrWriteLastCycle { get; private set; } = false;
    public void Write(byte value, ushort address)
    {
        switch (address)
        {
            case < 0x2000:
                _cpuRam[(ushort)(address % 0x800)] = value;
                break;
        }
#if DEBUG
        IsWrite = true;
        LastData = value;
        LastAddress = address;
        IsReadOrWriteLastCycle = true;
#endif
    }
    public byte Read(ushort address)
    {
        var value = address switch
        {
            < 0x2000 => _cpuRam[(ushort)(address % 0x800)],
            _ => throw new Exception()
        };
#if DEBUG
        IsWrite = false;
        LastAddress = address;
        LastData = value;
        IsReadOrWriteLastCycle = true;
#endif
        return value;
    }
    public void ClearIOState() => IsReadOrWriteLastCycle = false;
    public byte this[ushort address]
    {
        get => Read(address);
        set => Write(value, address);
    }
    public byte this[int address]
    {
        get => Read((ushort)address);
        set => Write(value, (ushort)address);
    }
}
