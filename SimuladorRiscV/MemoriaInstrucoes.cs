
namespace SimuladorRiscV
{
    internal class MemoriaInstrucoes
    {
        // array responsável por armazenar as instruções do programa
        private uint[] instrucoes;

        // construtor: recebe o array de instruções e guarda sua referência
        // para que a classe possa consultar as instruções posteriormente
        public MemoriaInstrucoes(uint[] instrucoes)
        {
            this.instrucoes = instrucoes;
        }

        // recebe um endereço e retorna a instrução armazenada nele
        public uint Ler(uint endereco)
        {
            // verifica se o endereço é múltiplo de 4,
            // pois cada instrução ocupa 4 bytes
            if (endereco % 4 != 0)
            {
                throw new ArgumentException("Endereco invalido.");
            }

            // converte o endereço (em bytes) para o índice correspondente no array
            // exemplo: endereço 4 / 4 = índice 1
            uint indice = endereco / 4;

            // verifica se o índice calculado existe dentro do array
            // exemplo: se houver 2 instruções, os índices válidos são 0 e 1
            if (indice >= (uint)instrucoes.Length)
            {
                throw new ArgumentOutOfRangeException(nameof(endereco));
            }

            // retorna a instrução completa de 32 bits armazenada no índice encontrado
            return instrucoes[(int)indice];
        }
    }
}

