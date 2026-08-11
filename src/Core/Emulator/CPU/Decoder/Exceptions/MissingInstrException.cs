namespace RealNES.Core.Emulator.CPU.Decoder.Exceptions;

internal class MissingInstrException(byte opCode) : Exception($"missing instruction! OPCode: {opCode}")
{
    public byte OpCode => opCode;
}
