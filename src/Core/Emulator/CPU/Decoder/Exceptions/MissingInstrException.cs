using System;
using System.Collections.Generic;
using System.Text;

namespace RealNES.Core.Emulator.CPU.Decoder.Exceptions;

internal class MissingInstrException(byte opCode):Exception
{
    public byte OpCode => opCode;
}
