namespace SimuladorRiscV
{
    internal class Program
    {
        static void Main(string[] args)
        {
            uint[] programa =
            {
                // FORMATO R

                // ADD x5, x1, x2
                0b0000000_00010_00001_000_00101_0110011u,

                // SUB x6, x1, x2
                0b0100000_00010_00001_000_00110_0110011u,

                // SLL x7, x1, x2
                0b0000000_00010_00001_001_00111_0110011u,

                // SLT x8, x1, x2
                0b0000000_00010_00001_010_01000_0110011u,

                // SLTU x9, x1, x2
                0b0000000_00010_00001_011_01001_0110011u,

                // XOR x10, x1, x2
                0b0000000_00010_00001_100_01010_0110011u,

                // SRL x11, x1, x2
                0b0000000_00010_00001_101_01011_0110011u,

                // SRA x12, x1, x2
                0b0100000_00010_00001_101_01100_0110011u,

                // OR x13, x1, x2
                0b0000000_00010_00001_110_01101_0110011u,

                // AND x14, x1, x2
                0b0000000_00010_00001_111_01110_0110011u,


                // FORMATO I

                // ADDI x15, x1, 3
                0b0000000_00011_00001_000_01111_0010011u,

                // SLTI x16, x1, 3
                0b0000000_00011_00001_010_10000_0010011u,

                // SLTIU x17, x1, 3
                0b0000000_00011_00001_011_10001_0010011u,

                // XORI x18, x1, 3
                0b0000000_00011_00001_100_10010_0010011u,

                // ORI x19, x1, 3
                0b0000000_00011_00001_110_10011_0010011u,

                // ANDI x20, x1, 3
                0b0000000_00011_00001_111_10100_0010011u,

                // SLLI x21, x1, 3
                0b0000000_00011_00001_001_10101_0010011u,

                // SRLI x22, x1, 3
                0b0000000_00011_00001_101_10110_0010011u,

                // SRAI x23, x1, 3
                0b0100000_00011_00001_101_10111_0010011u
            };


            // Criação dos componentes da CPU
            MemoriaInstrucoes memoria = new MemoriaInstrucoes(programa);

            ContadorPrograma pc = new ContadorPrograma();

            BancoRegistradores banco = new BancoRegistradores();

            // Valores iniciais dos registradores
            banco.Escrever(1, 0xFFFFFFF5u);
            banco.Escrever(2, 3);

            Decodificador decodificador = new Decodificador();

            ULA ula = new ULA();


            // Ciclo de execução das instruções
            for (int i = 0; i < programa.Length; i++)
            {
                // Busca da instrução
                uint enderecoAtual = pc.Ler();
                uint instrucaoAtual = memoria.Ler(enderecoAtual);

                // Extração do opcode
                uint opcode = instrucaoAtual & 0b1111111u;

                // Identificação do formato
                FormatoInstrucao formatoIdentificado =
                    decodificador.IdentificarFormato(opcode);

                Console.WriteLine($"\n--- INSTRUÇÃO {i + 1} ---");
                Console.WriteLine($"Endereço: 0x{enderecoAtual:X8}");
                Console.WriteLine($"Instrução recebida: 0x{instrucaoAtual:X8}");
                Console.WriteLine($"Formato identificado: {formatoIdentificado}");
                Console.WriteLine($"Opcode: {opcode}");


                // Interpretação conforme o formato identificado
                switch (formatoIdentificado)
                {
                    case FormatoInstrucao.R:
                        {
                            // Extração dos campos do formato R
                            uint rd = (instrucaoAtual >> 7) & 0b11111u;
                            uint funct3 = (instrucaoAtual >> 12) & 0b111u;
                            uint rs1 = (instrucaoAtual >> 15) & 0b11111u;
                            uint rs2 = (instrucaoAtual >> 20) & 0b11111u;
                            uint funct7 = (instrucaoAtual >> 25) & 0b1111111u;

                            // Identificação da operação
                            Operacao operacaoIdentificada =
                                decodificador.IdentificarOperacao(
                                    opcode, funct3, funct7);

                            if (operacaoIdentificada == Operacao.NaoReconhecida)
                            {
                                Console.WriteLine("Operação R não reconhecida.");
                                return;
                            }

                            // Leitura dos registradores
                            uint valorRs1 = banco.Ler((int)rs1);
                            uint valorRs2 = banco.Ler((int)rs2);

                            // Execução na ULA
                            uint resultado = ula.Executar(
                                operacaoIdentificada, valorRs1, valorRs2);

                            // Escrita do resultado no registrador de destino
                            banco.Escrever((int)rd, resultado);

                            Console.WriteLine($"Operação identificada: {operacaoIdentificada}");
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

                            break;
                        }


                    case FormatoInstrucao.I:
                        {
                            // Extração dos campos do formato I
                            uint rd = (instrucaoAtual >> 7) & 0b11111u;
                            uint funct3 = (instrucaoAtual >> 12) & 0b111u;
                            uint rs1 = (instrucaoAtual >> 15) & 0b11111u;

                            // Extração do imediato de 12 bits
                            uint imediato12 = (instrucaoAtual >> 20) & 0xFFFu;

                            // Extração dos bits superiores do imediato [31:25]
                            uint campoSuperiorImediato =
                                (instrucaoAtual >> 25) & 0b1111111u;

                            // Extensão de sinal para 32 bits
                            uint imediatoEstendido = imediato12;

                            if ((imediato12 & 0x800u) != 0)
                            {
                                imediatoEstendido |= 0xFFFFF000u;
                            }

                            // Identificação da operação
                            Operacao operacaoIdentificada =
                                decodificador.IdentificarOperacaoI(
                                    opcode, funct3, campoSuperiorImediato);

                            if (operacaoIdentificada == Operacao.NaoReconhecida)
                            {
                                Console.WriteLine("Operação I não reconhecida.");
                                return;
                            }

                            // Leitura do registrador de origem
                            uint valorRs1 = banco.Ler((int)rs1);

                            // Identificação do segundo operando
                            uint segundoOperando = imediatoEstendido;

                            // Nas instruções de deslocamento, utilizamos somente o shamt
                            if (operacaoIdentificada == Operacao.Slli ||
                                operacaoIdentificada == Operacao.Srli ||
                                operacaoIdentificada == Operacao.Srai)
                            {
                                segundoOperando = imediato12 & 0b11111u;
                            }

                            // Execução na ULA utilizando o imediato
                            uint resultado = ula.Executar(
                                operacaoIdentificada, valorRs1, segundoOperando);

                            // Escrita do resultado no registrador de destino
                            banco.Escrever((int)rd, resultado);

                            Console.WriteLine($"Operação identificada: {operacaoIdentificada}");
                            Console.WriteLine($"Funct3: {funct3}");

                            Console.WriteLine($"\nRegistrador destino: x{rd}");
                            Console.WriteLine($"Registrador fonte 1: x{rs1}");

                            Console.WriteLine("\n--- OPERANDOS ---");
                            Console.WriteLine($"Conteúdo de x{rs1}: {valorRs1}");

                            // Exibição dos operandos conforme a operação
                            if (operacaoIdentificada == Operacao.Slli ||
                                operacaoIdentificada == Operacao.Srli ||
                                operacaoIdentificada == Operacao.Srai)
                            {
                                Console.WriteLine($"Campo superior [31:25]: {campoSuperiorImediato}");
                                Console.WriteLine($"Quantidade de deslocamento (shamt): {segundoOperando}");
                            }
                            else
                            {
                                Console.WriteLine($"Imediato original (12 bits): {imediato12}");
                                Console.WriteLine($"Imediato estendido (32 bits): {imediatoEstendido}");
                            }

                            Console.WriteLine("\n--- RESULTADO DA EXECUÇÃO ---");
                            Console.WriteLine($"Resultado da ULA: {resultado}");
                            Console.WriteLine($"Registrador atualizado: x{rd} = {banco.Ler((int)rd)}");

                            break;
                        }


                    default:
                        Console.WriteLine("Formato de instrução ainda não implementado.");
                        return;
                }


                // Avanço do contador de programa
                pc.Avancar();

                Console.WriteLine($"Próximo endereço do PC: 0x{pc.Ler():X8}");
            }


            // Exibição final do banco de registradores
            Console.WriteLine("\n--- BANCO DE REGISTRADORES ---");

            for (int registrador = 0; registrador <= 23; registrador++)
            {
                Console.WriteLine($"x{registrador}: {banco.Ler(registrador)}");
            }

            // PC final
            Console.WriteLine($"\nPC final: 0x{pc.Ler():X8}");
        }
    }
}



