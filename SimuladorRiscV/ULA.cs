
namespace SimuladorRiscV
{
    internal class ULA
    {
        public uint Executar(Operacao operacao, uint valorRs1, uint valorRs2)
        {
            switch (operacao)
            {
                case Operacao.Add:
                case Operacao.Addi:
                    return unchecked(valorRs1 + valorRs2);

                case Operacao.Sub:
                    return unchecked(valorRs1 - valorRs2);


                // Deslocamento para a esquerda
                case Operacao.Sll:
                case Operacao.Slli:
                    return valorRs1 << (int)(valorRs2 & 0b11111u);


                // Comparação de inteiros com sinal
                case Operacao.Slt:
                case Operacao.Slti:
                    return (int)valorRs1 < (int)valorRs2 ? 1u : 0u;


                // Comparação de inteiros sem sinal
                case Operacao.Sltu:
                case Operacao.Sltiu:
                    return valorRs1 < valorRs2 ? 1u : 0u;


                // Comparações do formato B

                // BEQ: desvia se os valores forem iguais
                case Operacao.Beq:
                    return valorRs1 == valorRs2 ? 1u : 0u;

                // BNE: desvia se os valores forem diferentes
                case Operacao.Bne:
                    return valorRs1 != valorRs2 ? 1u : 0u;

                // BLT: menor que (com sinal)
                case Operacao.Blt:
                    return (int)valorRs1 < (int)valorRs2 ? 1u : 0u;

                // BGE: maior ou igual (com sinal)
                case Operacao.Bge:
                    return (int)valorRs1 >= (int)valorRs2 ? 1u : 0u;

                // BLTU: menor que (sem sinal)
                case Operacao.Bltu:
                    return valorRs1 < valorRs2 ? 1u : 0u;

                // BGEU: maior ou igual (sem sinal)
                case Operacao.Bgeu:
                    return valorRs1 >= valorRs2 ? 1u : 0u;


                // Operação XOR
                case Operacao.Xor:
                case Operacao.Xori:
                    return valorRs1 ^ valorRs2;


                // Deslocamento lógico para a direita
                case Operacao.Srl:
                case Operacao.Srli:
                    return valorRs1 >> (int)(valorRs2 & 0b11111u);


                // Deslocamento aritmético para a direita
                case Operacao.Sra:
                case Operacao.Srai:
                    return (uint)((int)valorRs1 >> (int)(valorRs2 & 0b11111u));


                // Operação OR
                case Operacao.Or:
                case Operacao.Ori:
                    return valorRs1 | valorRs2;


                // Operação AND
                case Operacao.And:
                case Operacao.Andi:
                    return valorRs1 & valorRs2;


                // BSWAP: inverte a ordem dos quatro bytes
                case Operacao.Bswap:
                    return
                        ((valorRs1 & 0x000000FFu) << 24) |
                        ((valorRs1 & 0x0000FF00u) << 8) |
                        ((valorRs1 & 0x00FF0000u) >> 8) |
                        ((valorRs1 & 0xFF000000u) >> 24);


                default:
                    throw new InvalidOperationException(
                        "Operação não suportada pela ULA.");
            }
        }
    }
}



