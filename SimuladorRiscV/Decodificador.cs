namespace SimuladorRiscV
{
    internal class Decodificador
    {

        public Operacao IdentificarOperacao(uint opcode, uint funct3, uint funct7)
        {
            switch ((opcode, funct3, funct7))
            {
                case (0b0110011u, 0b000u, 0b0000000u):
                    return Operacao.Add;

                case (0b0110011u, 0b000u, 0b0100000u):
                    return Operacao.Sub;

                case (0b0110011u, 0b001u, 0b0000000u):
                    return Operacao.Sll;

                case (0b0110011u, 0b010u, 0b0000000u):
                    return Operacao.Slt;

                case (0b0110011u, 0b011u, 0b0000000u):
                    return Operacao.Sltu;

                case (0b0110011u, 0b100u, 0b0000000u):
                    return Operacao.Xor;

                case (0b0110011u, 0b101u, 0b0000000u):
                    return Operacao.Srl;

                case (0b0110011u, 0b101u, 0b0100000u):
                    return Operacao.Sra;

                case (0b0110011u, 0b110u, 0b0000000u):
                    return Operacao.Or;

                case (0b0110011u, 0b111u, 0b0000000u):
                    return Operacao.And;

                default:
                    return Operacao.NaoReconhecida;
            }
        }



        public Operacao IdentificarOperacaoI(
            uint opcode, uint funct3, uint campoSuperiorImediato)
        {
            switch ((opcode, funct3))
            {
                case (0b0010011u, 0b000u):
                    return Operacao.Addi;

                case (0b0010011u, 0b010u):
                    return Operacao.Slti;

                case (0b0010011u, 0b011u):
                    return Operacao.Sltiu;

                case (0b0010011u, 0b100u):
                    return Operacao.Xori;

                case (0b0010011u, 0b110u):
                    return Operacao.Ori;

                case (0b0010011u, 0b111u):
                    return Operacao.Andi;

                case (0b0010011u, 0b001u):
                    if (campoSuperiorImediato == 0b0000000u)
                        return Operacao.Slli;

                    break;

                case (0b0010011u, 0b101u):
                    switch (campoSuperiorImediato)
                    {
                        case 0b0000000u:
                            return Operacao.Srli;

                        case 0b0100000u:
                            return Operacao.Srai;
                    }

                    break;
            }

            return Operacao.NaoReconhecida;
        }



        public FormatoInstrucao IdentificarFormato(uint opcode)
        {
            switch (opcode)
            {
                case 0b0110011u:
                    return FormatoInstrucao.R;

                case 0b0010011u:
                case 0b0000011u:
                case 0b1100111u:
                    return FormatoInstrucao.I;

                case 0b0100011u:
                    return FormatoInstrucao.S;

                case 0b1100011u:
                    return FormatoInstrucao.B;

                case 0b0110111u:
                case 0b0010111u:
                    return FormatoInstrucao.U;

                case 0b1101111u:
                    return FormatoInstrucao.J;

                case 0b0001011u:
                    return FormatoInstrucao.Custom; //bswap

                default:
                    return FormatoInstrucao.NaoReconhecido;
            }
        }
    }
}


