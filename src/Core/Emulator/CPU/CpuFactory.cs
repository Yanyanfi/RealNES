using RealNES.Core.Emulator.Communication;
using RealNES.Core.Emulator.CPU.Decoder;
using System;
using System.Collections.Generic;
using System.Text;

namespace RealNES.Core.Emulator.CPU;

internal sealed class CpuFactory(CpuBus bus)
{
    private readonly CpuBus _bus = bus;
    public Cpu Create()
    {
        var decoderFactory = new DecoderFactory(_bus);
        var decoder = decoderFactory.Create();
        return new(decoder);
    }
}
