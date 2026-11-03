
namespace SimuladorRiscV
{
    internal class ULA
    {
        public uint Executar(Operacao operacao, uint valorRs1, uint valorRs2)
        {
            switch (operacao)
            {
                case Operacao.Add:
                    return valorRs1 + valorRs2;

                case Operacao.Sub:
                    return valorRs1 - valorRs2;

                default:
                    throw new InvalidOperationException("Operação não suportada pela ULA.");
            }
        }
    }
}
