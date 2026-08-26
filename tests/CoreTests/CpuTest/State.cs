namespace RealNES.CoreTests.CpuTest;

internal record class State(ushort Pc, byte A, byte X, byte Y, byte Sp, byte P, int InstrCycle);
