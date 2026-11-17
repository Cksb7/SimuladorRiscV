
using System;

namespace SimuladorRiscV
{
    internal class MemoriaDados
    {
        // RAM principal: 512 KiB
        private readonly byte[] ram = new byte[0x80000];

        // VRAM: 64 KiB
        private readonly byte[] vram = new byte[0x10000];


        // Armazena uma palavra de 32 bits na região correspondente
        public void EscreverPalavra(uint endereco, uint valor)
        {
            RegiaoMemoria regiao = ValidarEnderecoPalavra(endereco);

            byte[] memoria = ObterMemoria(regiao);

            int indice = ObterIndice(endereco, regiao);

            // Armazenamento em Little-Endian
            memoria[indice] = (byte)(valor & 0xFFu);

            memoria[indice + 1] = (byte)((valor >> 8) & 0xFFu);

            memoria[indice + 2] = (byte)((valor >> 16) & 0xFFu);

            memoria[indice + 3] = (byte)((valor >> 24) & 0xFFu);
        }


        // Lê uma palavra de 32 bits da região correspondente
        public uint LerPalavra(uint endereco)
        {
            RegiaoMemoria regiao = ValidarEnderecoPalavra(endereco);

            byte[] memoria = ObterMemoria(regiao);

            int indice = ObterIndice(endereco, regiao);

            // Reconstrução da palavra em Little-Endian
            uint valor =
                (uint)memoria[indice] |
                ((uint)memoria[indice + 1] << 8) |
                ((uint)memoria[indice + 2] << 16) |
                ((uint)memoria[indice + 3] << 24);

            return valor;
        }


        // Retorna a memória correspondente à região identificada
        private byte[] ObterMemoria(RegiaoMemoria regiao)
        {
            switch (regiao)
            {
                case RegiaoMemoria.RamPrincipal:
                    return ram;

                case RegiaoMemoria.Vram:
                    return vram;

                default:
                    throw new InvalidOperationException(
                        "Esta região não possui armazenamento acessível.");
            }
        }


        // Converte o endereço absoluto em índice do array
        private int ObterIndice(uint endereco, RegiaoMemoria regiao)
        {
            uint enderecoRelativo;

            if (regiao == RegiaoMemoria.RamPrincipal)
            {
                enderecoRelativo = endereco - MapaMemoria.InicioRam;
            }
            else
            {
                enderecoRelativo = endereco - MapaMemoria.InicioVram;
            }

            return (int)enderecoRelativo;
        }


        // Valida o endereço e identifica sua região
        private RegiaoMemoria ValidarEnderecoPalavra(uint endereco)
        {
            // Verifica o alinhamento de 4 bytes
            if (endereco % 4 != 0)
            {
                throw new ArgumentException(
                    "Endereco invalido: acesso desalinhado.");
            }

            // Identifica a região do endereço
            RegiaoMemoria regiao =
                MapaMemoria.IdentificarRegiao(endereco);

            // Verifica se a palavra inteira cabe na região
            switch (regiao)
            {
                case RegiaoMemoria.RamPrincipal:

                    if (endereco > MapaMemoria.FimRam - 3u)
                    {
                        throw new ArgumentOutOfRangeException(
                            nameof(endereco),
                            "Palavra ultrapassa os limites da RAM.");
                    }

                    break;


                case RegiaoMemoria.Vram:

                    if (endereco > MapaMemoria.FimVram - 3u)
                    {
                        throw new ArgumentOutOfRangeException(
                            nameof(endereco),
                            "Palavra ultrapassa os limites da VRAM.");
                    }

                    break;


                case RegiaoMemoria.Reservada:

                    throw new InvalidOperationException(
                        "Acesso à região reservada não permitido.");


                case RegiaoMemoria.Perifericos:

                    throw new NotSupportedException(
                        "Acesso aos periféricos ainda não implementado.");


                default:

                    throw new ArgumentOutOfRangeException(
                        nameof(endereco),
                        "Endereço fora do mapa de memória.");
            }

            return regiao;
        }
    }
}

