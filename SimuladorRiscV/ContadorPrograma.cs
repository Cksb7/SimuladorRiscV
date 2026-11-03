
namespace SimuladorRiscV
{
    internal class ContadorPrograma
    {
        private uint enderecoAtual = 0;

        // le o endereco atual
        public uint Ler()
        {
            return enderecoAtual;
        }

        // avanca para a proxima instrucao
        public void Avancar()
        {
            enderecoAtual += 4;
        }

        // define um novo endereco
        public void Definir(uint novoEndereco)
        {
            enderecoAtual = novoEndereco;
        }
    }
}
