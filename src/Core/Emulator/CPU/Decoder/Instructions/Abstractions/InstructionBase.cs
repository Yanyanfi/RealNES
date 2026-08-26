using RealNES.Core.Emulator.Bus;
using RealNES.Core.Emulator.CPU.Decoder.Instructions.Enums;
using RealNES.Core.Emulator.CPU.Decoder.Instructions.Services;

namespace RealNES.Core.Emulator.CPU.Decoder.Instructions.Abstractions;

internal abstract class InstructionBase(InstructionServices services) : IInstruction
{
    protected AddressingType _addressingType;
    protected readonly CpuBus _bus = services.Bus;
    protected readonly FlagSetter _flagSetter = services.FlagSetter;
    protected readonly AddressingService _ads = services.AddressingService;
    protected readonly StackService _stack = services.StackService;
    protected readonly FlagReader _flagReader = services.FlagReader;

    public abstract IReadOnlyList<byte> OpCodes { get; }
    /// <summary>
    /// Cycle start from 2
    /// </summary>
    /// <param name="state"></param>
    /// <param name="code"></param>
    public abstract void Process(ref readonly CpuState state);
    public void Step(ref readonly CpuState state)
    {
        if (state.Cycle == 1)
        {
            var opCode = _bus[state.Pc];
            _addressingType = GetAddressingType(opCode);
            state.Pc++;
            return;
        }
        Process(in state);
    }
    protected abstract AddressingType GetAddressingType(byte opCode);
    protected void EndInstr(ref readonly CpuState state)
    {
        OnEndInstr(in state);
        state.Cycle = 0;
    }

    protected virtual void OnEndInstr(ref readonly CpuState state) { }
}
internal record InstructionServices(CpuBus Bus, FlagSetter FlagSetter, AddressingService AddressingService, StackService StackService, FlagReader FlagReader);