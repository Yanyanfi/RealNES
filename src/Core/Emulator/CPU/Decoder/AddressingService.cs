using RealNES.Core.Emulator.Communication;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;

namespace RealNES.Core.Emulator.CPU.Decoder;

//internal sealed class AddressingService(CpuBus cpuBus)
//{
//    public byte Zpxy(byte xy, byte arg) => (byte)((arg + xy) & 0xff);
//    public byte Abxy(byte xy, byte arg) => (byte)(xy + arg);
//    public ushort Izx(byte x, byte arg) => (ushort)(Peek((arg + x) & 0xff) + Peek((arg+x+1)&0xff)*256);
//    public ushort Izy(byte y, byte arg) => (ushort)(Peek(arg) + Peek((arg + 1) & 0xff) * 256 + y);
//    public byte ReadZpxy(byte xy, byte arg) => Peek(Zpxy(xy, arg));
//    public byte ReadAbxy(byte xy, byte arg) => Peek(Abxy(xy, arg));
//    public byte ReadIzx(byte x, byte arg) => Peek(Izx(x, arg));
//    public byte ReadIzy(byte y, byte arg) => Peek(Izy(y, arg));
//    private byte Peek(int address) => cpuBus[(ushort)address];
//    private void Write(int address, byte value) => cpuBus[(ushort)address] = value;
//}
