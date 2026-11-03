
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

                default:
                    return Operacao.NaoReconhecida;
            }
        }
    }
}
