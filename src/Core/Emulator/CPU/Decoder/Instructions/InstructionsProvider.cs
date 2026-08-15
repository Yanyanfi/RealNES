using RealNES.Core.Emulator.Communication;
using RealNES.Core.Emulator.CPU.Decoder.Instructions.Abstractions;
using RealNES.Core.Emulator.CPU.Decoder.Instructions.Access;
using RealNES.Core.Emulator.CPU.Decoder.Instructions.Arithmetic;
using RealNES.Core.Emulator.CPU.Decoder.Instructions.Bitwise;
using RealNES.Core.Emulator.CPU.Decoder.Instructions.Branch;
using RealNES.Core.Emulator.CPU.Decoder.Instructions.Compare;
using RealNES.Core.Emulator.CPU.Decoder.Instructions.Jump;
using RealNES.Core.Emulator.CPU.Decoder.Instructions.Other;
using RealNES.Core.Emulator.CPU.Decoder.Instructions.Services;
using RealNES.Core.Emulator.CPU.Decoder.Instructions.Shift;
using RealNES.Core.Emulator.CPU.Decoder.Instructions.Stack;
using RealNES.Core.Emulator.CPU.Decoder.Instructions.Transfer;

namespace RealNES.Core.Emulator.CPU.Decoder.Instructions;

internal sealed class InstructionsProvider(CpuBus bus)
{
    public IReadOnlyList<IInstruction> GetInstructions()
    {
        var addressingService = new AddressingService();
        var flagSetter = new FlagSetter();
        var stack = new StackService(bus);
        var flagReader = new FlagReader();
        var services = new InstructionServices(bus, flagSetter, addressingService, stack, flagReader);
        return
        [
            #region Access
            new LDA(services),
            new LDX(services),
            new LDY(services),
            new STA(services),
            new STX(services),
            new STY(services),
            #endregion
            #region Transfer
            new TAX(services),
            new TXA(services),
            new TAY(services),
            new TYA(services),
            #endregion
            #region Arithmetic
            new ADC(services),
            new SBC(services),
            new INC(services),
            new DEC(services),
            new INX(services),
            new DEX(services),
            new INY(services),
            new DEY(services),
            #endregion
            #region Shift
            new ASL(services),
            new LSR(services),
            new ROL(services),
            new ROR(services),
            #endregion
            #region Bitwise
            new AND(services),
            new ORA(services),
            new EOR(services),
            new BIT(services),
            #endregion
            #region Compare
            new CMP(services),
            new CPX(services),
            new CPY(services),
            #endregion
            #region Branch
            new BCC(services),
            new BCS(services),
            new BEQ(services),
            new BNE(services),
            new BPL(services),
            new BMI(services),
            new BVC(services),
            new BVS(services),
            #endregion
            #region Jump
            new JMP(services),
            new JSR(services),
            new RTS(services),
            new BRK(services),
            new RTI(services),
            #endregion
            #region Stack
            new PHA(services),
            new PLA(services),
            new PHP(services),
            new PLP(services),
            new TXS(services),
            new TSX(services),
            #endregion
            #region Other
            new NOP(services),
            #endregion
        ];
    }
}
