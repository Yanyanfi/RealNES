using RealNES.Core.Emulator.CPU;
using RealNES.Core.Emulator.CPU.Decoder;
using RealNES.Core.Emulator.CPU.Decoder.Instructions.Abstractions;
using RealNES.Core.Emulator.CPU.Decoder.Instructions.Enums;
using RealNES.CoreTests.CpuTest.Exceptions;
using System.Reflection;

namespace RealNES.CoreTests.CpuTest;

internal class InstrInfoProvider(Cpu cpu)
{
    private readonly Decoder _decoder = (Decoder)(typeof(Cpu).GetTypeInfo().GetDeclaredField("_decoder")?.GetValue(cpu) ?? throw new FieldNotFoundException("_decoder"));
    public string? GetInstrName()
    {
        var info = typeof(Decoder).GetTypeInfo().GetDeclaredField("_currentInstr") ?? throw new FieldNotFoundException("_currentInstr");
        var instrName = info.GetValue(_decoder)?.GetType().Name;
        return instrName;
    }
    public string? GetAddressingName()
    {
        var info = typeof(Decoder).GetTypeInfo().GetDeclaredField("_currentInstr") ?? throw new FieldNotFoundException("_currentInstr");
        var instr = info.GetValue(_decoder);
        if (instr is not InstructionBase instrBase)
            return null;
        var addrTypeInfo = instrBase.GetType().GetTypeInfo().GetField("_addressingType", BindingFlags.NonPublic | BindingFlags.Instance) ?? throw new FieldNotFoundException("_addressingType");
        return addrTypeInfo.GetValue(instrBase) switch
        {
            AddressingType.Immediate => "imm",
            AddressingType.Absolute => "abs",
            AddressingType.AbsoluteX => "abs x",
            AddressingType.AbsoluteY => "abs y",
            AddressingType.Accumulator => "acc",
            AddressingType.Implicit => "imp",
            AddressingType.ZeroPage => "zp",
            AddressingType.ZeroPageX => "zpx",
            AddressingType.ZeroPageY => "zpy",
            AddressingType.Relative => "rel",
            AddressingType.Indirect => "ind",
            AddressingType.IndexedIndirect => "ind x",
            AddressingType.IndirectIndexed => "ind y",
            _ => null
        };
    }
}
