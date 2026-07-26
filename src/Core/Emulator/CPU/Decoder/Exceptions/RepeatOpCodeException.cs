using System;
using System.Collections.Generic;
using System.Text;

namespace RealNES.Core.Emulator.CPU.Decoder.Exceptions;

internal class RepeatOpCodeException(byte opCode,string instr1Name,string instr2Name) : Exception
{
    public byte OpCode => opCode;
    public string Instr1Name => instr1Name;
    public string Instr2Name => instr2Name;
}