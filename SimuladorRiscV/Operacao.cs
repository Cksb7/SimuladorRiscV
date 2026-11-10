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
        Srai
    }
}


