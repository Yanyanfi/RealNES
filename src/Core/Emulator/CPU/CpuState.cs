namespace RealNES.Core.Emulator.CPU;

internal readonly ref struct CpuState(ref byte a, ref byte x, ref byte y, ref ushort pc, ref byte sp, ref byte p, ref int cycle)
{
    public readonly ref byte A = ref a;
    public readonly ref byte X = ref x;
    public readonly ref byte Y = ref y;
    public readonly ref ushort Pc = ref pc;
    public readonly ref byte Sp = ref sp;
    public readonly ref byte P = ref p;
    public readonly ref int Cycle = ref cycle;
}
