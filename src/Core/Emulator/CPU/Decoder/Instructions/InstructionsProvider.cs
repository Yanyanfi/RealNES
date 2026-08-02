using RealNES.Core.Emulator.Communication;
using RealNES.Core.Emulator.CPU.Decoder.Instructions.Abstractions;
using RealNES.Core.Emulator.CPU.Decoder.Instructions.Access;
using RealNES.Core.Emulator.CPU.Decoder.Instructions.Services;
using System;
using System.Collections.Generic;
using System.Text;

namespace RealNES.Core.Emulator.CPU.Decoder.Instructions;

internal sealed class InstructionsProvider(CpuBus bus)
{
    public IReadOnlyList<IInstruction> GetInstructions()
    {
        var addressingService = new AddressingService();
        var flagSetter = new FlagSetter();
        var services = new InstructionServices(bus, flagSetter, addressingService);
        return
        [
            new LDA(services),
            new LDX(services),
            new LDY(services),
            new STA(services),
            new STX(services),
            new STY(services),
        ];
    }
}
