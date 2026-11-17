
namespace SimuladorRiscV
{
    internal enum Operacao
    {
        NaoReconhecida,

        // Formato R
        Add,
        Sub,
        Sll,
        Slt,
        Sltu,
        Xor,
        Srl,
        Sra,
        Or,
        And,

        // Formato I
        Addi,
        Slti,
        Sltiu,
        Xori,
        Ori,
        Andi,

        // Deslocamentos imediatos
        Slli,
        Srli,
        Srai,

        // Acesso à memória
        Sw,
        Lw,

        // Formato B - Branch
        Beq,
        Bne,
        Blt,
        Bge,
        Bltu,
        Bgeu,

        // Formato U
        Lui,
        Auipc,

        // Formato J
        Jal,

        // Instrução bswap
        Bswap
    }
}



