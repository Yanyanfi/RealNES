using System;
using System.Collections.Generic;
using System.Text;

namespace RealNES.Core.Emulator.CPU.Decoder;

internal interface IDecoder
{
    void Step(ref readonly CpuState state);
}
