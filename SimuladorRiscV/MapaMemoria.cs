
namespace SimuladorRiscV
{
    // Identifica as regiões existentes no mapa de memória
    internal enum RegiaoMemoria
    {
        RamPrincipal,
        Vram,
        Reservada,
        Perifericos,
        ForaDoMapa
    }


    internal static class MapaMemoria
    {
        // RAM principal: 512 KiB
        public const uint InicioRam = 0x00000;
        public const uint FimRam = 0x7FFFF;

        // VRAM: 64 KiB
        public const uint InicioVram = 0x80000;
        public const uint FimVram = 0x8FFFF;

        // Área reservada: 63 KiB
        public const uint InicioReservada = 0x90000;
        public const uint FimReservada = 0x9FBFF;

        // Periféricos: 1 KiB
        public const uint InicioPerifericos = 0x9FC00;
        public const uint FimPerifericos = 0x9FFFF;


        // Identifica a região correspondente ao endereço recebido
        public static RegiaoMemoria IdentificarRegiao(uint endereco)
        {
            if (endereco >= InicioRam && endereco <= FimRam)
                return RegiaoMemoria.RamPrincipal;

            if (endereco >= InicioVram && endereco <= FimVram)
                return RegiaoMemoria.Vram;

            if (endereco >= InicioReservada && endereco <= FimReservada)
                return RegiaoMemoria.Reservada;

            if (endereco >= InicioPerifericos && endereco <= FimPerifericos)
                return RegiaoMemoria.Perifericos;

            return RegiaoMemoria.ForaDoMapa;
        }
    }
}
