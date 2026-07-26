using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace RealNES.Core.Emulator.Communication;

internal sealed class CpuBus
{
    public void Write(byte value,ushort address)
    {
        throw new NotImplementedException();
    }
    public byte Read(ushort address)
    {
        throw new NotImplementedException();
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
