namespace SimuladorRiscV
{
    internal class Program
    {
        static void Main(string[] args)
        {
            // define as instrucoes do programa
            uint[] programa =
            {
                0b0000000_00010_00001_000_00101_0110011u,
                0b0100000_00010_00001_000_00110_0110011u
            };

            // cria a memoria de instrucoes
            MemoriaInstrucoes memoria = new MemoriaInstrucoes(programa);

            // cria o contador de programa
            ContadorPrograma pc = new ContadorPrograma();

            // cria o banco de registradores
            BancoRegistradores banco = new BancoRegistradores();

            banco.Escrever(1, 10);
            banco.Escrever(2, 5);
            banco.Escrever(5, 100);

            // testa a proteção do x0
            banco.Escrever(0, 50);

            // cria o decodificador
            Decodificador decodificador = new Decodificador();

            // cria a ULA
            ULA ula = new ULA();

            // percorre as instrucoes do programa
            for (int i = 0; i < programa.Length; i++)
            {
                // recebe a instrução
                uint enderecoAtual = pc.Ler();
                uint instrucaoAtual = memoria.Ler(enderecoAtual);

                // extrai os campos
                uint opcode = instrucaoAtual & 0b1111111u;

                uint rd = (instrucaoAtual >> 7) & 0b11111u;
                uint funct3 = (instrucaoAtual >> 12) & 0b111u;
                uint rs1 = (instrucaoAtual >> 15) & 0b11111u;
                uint rs2 = (instrucaoAtual >> 20) & 0b11111u;
                uint funct7 = (instrucaoAtual >> 25) & 0b1111111u;

                // identifica a operação
                Operacao operacaoIdentificada = decodificador.IdentificarOperacao(opcode, funct3, funct7);

                // verifica se a instrução foi reconhecida
                if (operacaoIdentificada == Operacao.NaoReconhecida)
                {
                    Console.WriteLine("Instrução não reconhecida.");
                    return;
                }

                // le os registradores de origem
                uint valorRs1 = banco.Ler((int)rs1);
                uint valorRs2 = banco.Ler((int)rs2);

                // executa a operação
                uint resultado = ula.Executar(operacaoIdentificada, valorRs1, valorRs2);

                // armazena o resultado no registrador de destino
                banco.Escrever((int)rd, resultado);

                Console.WriteLine($"\n--- INSTRUÇÃO {i + 1} ---");

                Console.WriteLine($"Endereço: 0x{enderecoAtual:X8}");
                Console.WriteLine($"Instrução recebida: 0x{instrucaoAtual:X8}");
                Console.WriteLine($"Operação identificada: {operacaoIdentificada}");

                Console.WriteLine($"Opcode: {opcode}");
                Console.WriteLine($"Funct3: {funct3}");
                Console.WriteLine($"Funct7: {funct7}");

                Console.WriteLine($"\nRegistrador destino: x{rd}");
                Console.WriteLine($"Registrador fonte 1: x{rs1}");
                Console.WriteLine($"Registrador fonte 2: x{rs2}");

                Console.WriteLine("\n--- OPERANDOS ---");

                Console.WriteLine($"Conteúdo de x{rs1}: {valorRs1}");
                Console.WriteLine($"Conteúdo de x{rs2}: {valorRs2}");

                Console.WriteLine("\n--- RESULTADO DA EXECUÇÃO ---");

                Console.WriteLine($"Resultado da ULA: {resultado}");
                Console.WriteLine($"Registrador atualizado: x{rd} = {banco.Ler((int)rd)}");

                // avanca para a proxima instrucao
                pc.Avancar();

                Console.WriteLine($"Próximo endereço do PC: 0x{pc.Ler():X8}");
            }

            Console.WriteLine("\n--- BANCO DE REGISTRADORES ---");

            Console.WriteLine($"x0: {banco.Ler(0)}");
            Console.WriteLine($"x1: {banco.Ler(1)}");
            Console.WriteLine($"x2: {banco.Ler(2)}");
            Console.WriteLine($"x5: {banco.Ler(5)}");
            Console.WriteLine($"x6: {banco.Ler(6)}");

            Console.WriteLine($"\nPC final: 0x{pc.Ler():X8}");
        }
    }
}



