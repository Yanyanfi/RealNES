using RealNES.Core.Emulator.CPU;
using RealNES.Core.Emulator.CPU.Decoder;
using RealNES.Core.Emulator.CPU.Decoder.Instructions.Abstractions;
using System.Reflection;

namespace RealNES.CoreTests.CpuTest.InstructionTest;

internal class OpCodesProvider(Cpu cpu)
{
    public IEnumerable<byte> GetOpCodes()
    {
        var decoder = typeof(Cpu).GetTypeInfo().GetDeclaredField("_decoder")!.GetValue(cpu);
        var table = (IReadOnlyList<IInstruction>)typeof(Decoder).GetTypeInfo().GetDeclaredField("_instructionTable")!.GetValue(decoder)!;
        return Enumerable.Range(0, 256).Where(e => table[(byte)e] is not null).Select(e => (byte)e);
    }
}
