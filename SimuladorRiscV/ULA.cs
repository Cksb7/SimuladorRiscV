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

                case Operacao.Sll:
                case Operacao.Slli:
                    return valorRs1 << (int)(valorRs2 & 0b11111u); //Desloca bits para a esquerda (e preenche com zeros)

                case Operacao.Slt:
                case Operacao.Slti:
                    return (int)valorRs1 < (int)valorRs2 ? 1u : 0u; // vdd ou falso de inteiros com sinal

                case Operacao.Sltu:
                case Operacao.Sltiu:
                    return valorRs1 < valorRs2 ? 1u : 0u; // vdd ou falso de inteiros sem sinal

                case Operacao.Xor:
                case Operacao.Xori:
                    return valorRs1 ^ valorRs2; // diferentes 1, iguais 0

                case Operacao.Srl:
                case Operacao.Srli:
                    return valorRs1 >> (int)(valorRs2 & 0b11111u); // Desloca bits para a direita preenchendo com zeros

                case Operacao.Sra:
                case Operacao.Srai:
                    return (uint)((int)valorRs1 >> (int)(valorRs2 & 0b11111u)); // Desloca bits para a direita preservando o sinal

                case Operacao.Or:
                case Operacao.Ori:
                    return valorRs1 | valorRs2;

                case Operacao.And:
                case Operacao.Andi:
                    return valorRs1 & valorRs2;

                default:
                    throw new InvalidOperationException(
                        "Operação não suportada pela ULA.");
            }
        }
    }
}


