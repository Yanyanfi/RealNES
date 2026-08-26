using RealNES.Core.Emulator.Memory;

namespace RealNES.Core.Emulator.Bus;

internal sealed class CpuBus(InternalRam cpuRam)
{
    private readonly InternalRam _cpuRam = cpuRam;
    public byte LastData { get; private set; }
    public ushort LastAddress { get; private set; }
    #region DebugProperty
    public bool IsWrite { get; private set; }
    public bool IsRead { get; private set; }
    #endregion
    public bool IsReadOrWriteLastCycle { get; private set; } = false;
    public void Write(byte value, ushort address)
    {
        //switch (address)
        //{
        //    case < 0x2000:
        //        _cpuRam[(ushort)(address % 0x800)] = value;
        //        break;
        //}
        _cpuRam[address] = value;
#if DEBUG
        IsWrite = true;
        LastData = value;
        LastAddress = address;
#endif
    }
    public byte Read(ushort address)
    {
        //var value = address switch
        //{
        //    < 0x2000 => _cpuRam[(ushort)(address % 0x800)],
        //    _ => throw new Exception()
        //};
        var value = _cpuRam[address];
#if DEBUG
        IsRead = true;
        LastAddress = address;
        LastData = value;
#endif
        return value;
    }
    public void ClearIOState()
    {
        IsRead = false;
        IsWrite = false;
    }
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
