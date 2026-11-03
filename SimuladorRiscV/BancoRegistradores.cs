
namespace SimuladorRiscV
{
    internal class BancoRegistradores
    {
        private uint[] registradores = new uint[32];


        public uint Ler(int indice)
        {
            if (indice < 0 || indice >= 32)
            {
                throw new ArgumentOutOfRangeException(nameof(indice));
            }

            return registradores[indice];
        }


        public void Escrever(int indice, uint valor)
        {
            if (indice < 0 || indice >= 32)
            {
                throw new ArgumentOutOfRangeException(nameof(indice));
            }

            // O registrador x0 é sempre 0 e n pode ser alterado
            if (indice == 0)
            {
                return;
            }

            registradores[indice] = valor;
        }
    }
}

