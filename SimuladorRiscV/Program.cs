
using System;

namespace SimuladorRiscV
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("--- TESTE DO MAPA DE MEMORIA ---");

            uint[] enderecosTesteMapa =
            {
                0x00000u,
                0x7FFFFu,
                0x80000u,
                0x8FFFFu,
                0x90000u,
                0x9FBFFu,
                0x9FC00u,
                0x9FFFFu,
                0xA0000u
            };

            foreach (uint enderecoTeste in enderecosTesteMapa)
            {
                Console.WriteLine(
                    $"0x{enderecoTeste:X5} -> " +
                    MapaMemoria.IdentificarRegiao(enderecoTeste));
            }

            uint[] programa =
            {
                // ======================================
                // TESTES ANTERIORES DA CPU
                // ======================================

                // LUI x5, 0x12345
                0x123452B7u,

                // AUIPC x6, 0x00012
                0x00012317u,

                // JAL x7, 8
                0x008003EFu,

                // ADDI x8, x0, 99 (instrução ignorada pelo salto)
                0x06300413u,

                // BSWAP x9, x1
                0x0000848Bu,

                // ADDI x10, x0, 1
                0x00100513u,

                // BEQ x10, x0, 12
                0x00050663u,

                // ADDI x10, x0, 0
                0x00000513u,

                // JAL x11, -8
                0xFF9FF5EFu,

                // ADDI x12, x0, 7
                0x00700613u,


                // ======================================
                // TESTE SW / LW NA RAM PRINCIPAL
                // ======================================

                // ADDI x13, x0, 100 -> endereço RAM 100
                0x06400693u,

                // LUI x15, 0x12345
                0x123457B7u,

                // ADDI x15, x15, 0x678 -> x15 = 0x12345678
                0x67878793u,

                // SW x15, 0(x13) -> RAM[100] = 0x12345678
                0x00F6A023u,

                // LW x16, 0(x13) -> x16 = RAM[100]
                0x0006A803u,


                // ======================================
                // TESTE SW / LW NA VRAM
                // ======================================

                // LUI x17, 0x80 -> endereço base 0x00080000
                0x000808B7u,

                // LUI x18, 0x55667
                0x55667937u,

                // ADDI x18, x18, 0x788 -> x18 = 0x55667788
                0x78890913u,

                // SW x18, 0(x17) -> VRAM[0x80000] = 0x55667788
                0x0128A023u,

                // LW x19, 0(x17) -> x19 = VRAM[0x80000]
                0x0008A983u
            };


            // Criação dos componentes da CPU
            MemoriaInstrucoes memoria = new MemoriaInstrucoes(programa);
            MemoriaDados memoriaDados = new MemoriaDados();


            // ==========================================
            // TESTE DA RAM PRINCIPAL
            // ==========================================

            Console.WriteLine("\n--- TESTE DA RAM PRINCIPAL ---");

            memoriaDados.EscreverPalavra(100u, 0xDEADBEEFu);

            uint valorRam = memoriaDados.LerPalavra(100u);

            Console.WriteLine($"Endereço: 100");
            Console.WriteLine($"Valor esperado: 0xDEADBEEF");
            Console.WriteLine($"Valor recuperado: 0x{valorRam:X8}");


            // ==========================================
            // TESTE DA VRAM
            // ==========================================

            Console.WriteLine("\n--- TESTE DA VRAM ---");

            memoriaDados.EscreverPalavra(0x80000u, 0x41424344u);

            uint valorVram = memoriaDados.LerPalavra(0x80000u);

            Console.WriteLine($"Endereço: 0x80000");
            Console.WriteLine($"Valor esperado: 0x41424344");
            Console.WriteLine($"Valor recuperado: 0x{valorVram:X8}");


            // ==========================================
            // TESTE DOS LIMITES DA MEMORIA
            // ==========================================

            Console.WriteLine("\n--- TESTE DOS LIMITES DA MEMORIA ---");

            // Última palavra válida da RAM
            memoriaDados.EscreverPalavra(0x7FFFCu, 0xAABBCCDDu);

            uint ultimaRam = memoriaDados.LerPalavra(0x7FFFCu);

            Console.WriteLine($"Última palavra da RAM: 0x{ultimaRam:X8}");


            // Última palavra válida da VRAM
            memoriaDados.EscreverPalavra(0x8FFFCu, 0x11223344u);

            uint ultimaVram = memoriaDados.LerPalavra(0x8FFFCu);

            Console.WriteLine($"Última palavra da VRAM: 0x{ultimaVram:X8}");


            // Testa se os acessos inválidos são bloqueados
            void TestarAcessoBloqueado(
                string descricao, uint endereco, Type tipoEsperado)
            {
                try
                {
                    memoriaDados.LerPalavra(endereco);

                    Console.WriteLine($"FALHA: {descricao} foi permitido.");
                }
                catch (Exception erro)
                {
                    if (erro.GetType() == tipoEsperado)
                    {
                        Console.WriteLine(
                            $"OK: {descricao} bloqueado ({erro.GetType().Name}).");
                    }
                    else
                    {
                        Console.WriteLine(
                            $"FALHA: {descricao}. Esperado {tipoEsperado.Name}, " +
                            $"recebido {erro.GetType().Name}.");
                    }
                }
            }


            // Acesso desalinhado: deve gerar ArgumentException
            TestarAcessoBloqueado(
                "Acesso desalinhado",
                0x80001u,
                typeof(ArgumentException));

            // Região reservada: deve gerar InvalidOperationException
            TestarAcessoBloqueado(
                "Área reservada",
                0x90000u,
                typeof(InvalidOperationException));

            // Periféricos: ainda não implementados
            TestarAcessoBloqueado(
                "Região de periféricos",
                0x9FC00u,
                typeof(NotSupportedException));

            // Endereço fora do mapa: deve gerar ArgumentOutOfRangeException
            TestarAcessoBloqueado(
                "Endereço fora do mapa",
                0xA0000u,
                typeof(ArgumentOutOfRangeException));


            // ==========================================
            // CRIACAO DOS COMPONENTES RESTANTES DA CPU
            // ==========================================

            ContadorPrograma pc = new ContadorPrograma();

            BancoRegistradores banco = new BancoRegistradores();

            // Valores iniciais dos registradores
            banco.Escrever(1, 0x12345678u);

            Decodificador decodificador = new Decodificador();

            ULA ula = new ULA();


            Console.WriteLine("\n--- TESTE DE INTEGRACAO SW/LW PELA CPU ---");


            // ==========================================
            // CICLO DE EXECUCAO DAS INSTRUCOES
            // ==========================================

            int numeroInstrucao = 0;

            while (pc.Ler() < (uint)(programa.Length * 4))
            {
                numeroInstrucao++;

                // Busca da instrução
                uint enderecoAtual = pc.Ler();
                uint instrucaoAtual = memoria.Ler(enderecoAtual);

                // Controla se o PC foi atualizado por desvio ou salto
                bool pcFoiAtualizado = false;

                // Extração do opcode
                uint opcode = instrucaoAtual & 0b1111111u;

                // Identificação do formato
                FormatoInstrucao formatoIdentificado =
                    decodificador.IdentificarFormato(opcode);

                Console.WriteLine($"\n--- INSTRUÇÃO {numeroInstrucao} ---");
                Console.WriteLine($"Endereço: 0x{enderecoAtual:X8}");
                Console.WriteLine($"Instrução recebida: 0x{instrucaoAtual:X8}");
                Console.WriteLine($"Formato identificado: {formatoIdentificado}");
                Console.WriteLine($"Opcode: {opcode}");


                // Interpretação conforme o formato identificado
                switch (formatoIdentificado)
                {

                    // ======================================
                    // FORMATO R
                    // ======================================

                    case FormatoInstrucao.R:
                        {
                            uint rd = (instrucaoAtual >> 7) & 0b11111u;
                            uint funct3 = (instrucaoAtual >> 12) & 0b111u;
                            uint rs1 = (instrucaoAtual >> 15) & 0b11111u;
                            uint rs2 = (instrucaoAtual >> 20) & 0b11111u;
                            uint funct7 = (instrucaoAtual >> 25) & 0b1111111u;

                            Operacao operacaoIdentificada =
                                decodificador.IdentificarOperacao(
                                    opcode, funct3, funct7);

                            if (operacaoIdentificada == Operacao.NaoReconhecida)
                            {
                                Console.WriteLine("Operação R não reconhecida.");
                                return;
                            }

                            uint valorRs1 = banco.Ler((int)rs1);
                            uint valorRs2 = banco.Ler((int)rs2);

                            uint resultado = ula.Executar(
                                operacaoIdentificada, valorRs1, valorRs2);

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


                    // ======================================
                    // FORMATO I
                    // ======================================

                    case FormatoInstrucao.I:
                        {
                            uint rd = (instrucaoAtual >> 7) & 0b11111u;
                            uint funct3 = (instrucaoAtual >> 12) & 0b111u;
                            uint rs1 = (instrucaoAtual >> 15) & 0b11111u;

                            uint imediato12 = (instrucaoAtual >> 20) & 0xFFFu;

                            uint campoSuperiorImediato =
                                (instrucaoAtual >> 25) & 0b1111111u;

                            uint imediatoEstendido = imediato12;

                            if ((imediato12 & 0x800u) != 0)
                            {
                                imediatoEstendido |= 0xFFFFF000u;
                            }

                            Operacao operacaoIdentificada =
                                decodificador.IdentificarOperacaoI(
                                    opcode, funct3, campoSuperiorImediato);

                            if (operacaoIdentificada == Operacao.NaoReconhecida)
                            {
                                Console.WriteLine("Operação I não reconhecida.");
                                return;
                            }

                            uint valorRs1 = banco.Ler((int)rs1);


                            // EXECUÇÃO DA INSTRUÇÃO LW
                            if (operacaoIdentificada == Operacao.Lw)
                            {
                                uint enderecoMemoria = ula.Executar(
                                    Operacao.Add, valorRs1, imediatoEstendido);

                                uint valorCarregado =
                                    memoriaDados.LerPalavra(enderecoMemoria);

                                banco.Escrever((int)rd, valorCarregado);

                                Console.WriteLine($"Operação identificada: {operacaoIdentificada}");

                                Console.WriteLine($"\nRegistrador destino: x{rd}");
                                Console.WriteLine($"Registrador fonte 1: x{rs1}");

                                Console.WriteLine("\n--- ACESSO À MEMÓRIA ---");
                                Console.WriteLine($"Endereço calculado pela ULA: 0x{enderecoMemoria:X8}");
                                Console.WriteLine($"Região acessada: {MapaMemoria.IdentificarRegiao(enderecoMemoria)}");
                                Console.WriteLine($"Valor recuperado da memória: {valorCarregado} (0x{valorCarregado:X8})");

                                Console.WriteLine("\n--- RESULTADO DA EXECUÇÃO ---");
                                Console.WriteLine($"Registrador atualizado: x{rd} = {banco.Ler((int)rd)}");

                                break;
                            }


                            uint segundoOperando = imediatoEstendido;

                            if (operacaoIdentificada == Operacao.Slli ||
                                operacaoIdentificada == Operacao.Srli ||
                                operacaoIdentificada == Operacao.Srai)
                            {
                                segundoOperando = imediato12 & 0b11111u;
                            }

                            uint resultado = ula.Executar(
                                operacaoIdentificada, valorRs1, segundoOperando);

                            banco.Escrever((int)rd, resultado);

                            Console.WriteLine($"Operação identificada: {operacaoIdentificada}");
                            Console.WriteLine($"Funct3: {funct3}");

                            Console.WriteLine($"\nRegistrador destino: x{rd}");
                            Console.WriteLine($"Registrador fonte 1: x{rs1}");

                            Console.WriteLine("\n--- OPERANDOS ---");
                            Console.WriteLine($"Conteúdo de x{rs1}: {valorRs1}");

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


                    // ======================================
                    // FORMATO S
                    // ======================================

                    case FormatoInstrucao.S:
                        {
                            uint funct3 = (instrucaoAtual >> 12) & 0b111u;
                            uint rs1 = (instrucaoAtual >> 15) & 0b11111u;
                            uint rs2 = (instrucaoAtual >> 20) & 0b11111u;

                            uint imediatoSuperior =
                                (instrucaoAtual >> 25) & 0b1111111u;

                            uint imediatoInferior =
                                (instrucaoAtual >> 7) & 0b11111u;

                            uint imediato12 =
                                (imediatoSuperior << 5) | imediatoInferior;

                            uint imediatoEstendido = imediato12;

                            if ((imediato12 & 0x800u) != 0)
                            {
                                imediatoEstendido |= 0xFFFFF000u;
                            }

                            Operacao operacaoIdentificada =
                                decodificador.IdentificarOperacaoS(opcode, funct3);

                            if (operacaoIdentificada == Operacao.NaoReconhecida)
                            {
                                Console.WriteLine("Operação S não reconhecida.");
                                return;
                            }

                            uint valorRs1 = banco.Ler((int)rs1);
                            uint valorRs2 = banco.Ler((int)rs2);

                            uint enderecoMemoria = ula.Executar(
                                Operacao.Add, valorRs1, imediatoEstendido);

                            if (operacaoIdentificada == Operacao.Sw)
                            {
                                memoriaDados.EscreverPalavra(enderecoMemoria, valorRs2);
                            }

                            uint valorArmazenado = memoriaDados.LerPalavra(enderecoMemoria);

                            Console.WriteLine($"Operação identificada: {operacaoIdentificada}");
                            Console.WriteLine($"Funct3: {funct3}");

                            Console.WriteLine($"\nRegistrador fonte 1: x{rs1}");
                            Console.WriteLine($"Registrador fonte 2: x{rs2}");

                            Console.WriteLine("\n--- OPERANDOS ---");
                            Console.WriteLine($"Conteúdo de x{rs1}: {valorRs1}");
                            Console.WriteLine($"Conteúdo de x{rs2}: {valorRs2}");

                            Console.WriteLine($"Imediato reconstruído: {imediato12}");
                            Console.WriteLine($"Imediato estendido: {unchecked((int)imediatoEstendido)}");

                            Console.WriteLine("\n--- ACESSO À MEMÓRIA ---");
                            Console.WriteLine($"Endereço calculado pela ULA: 0x{enderecoMemoria:X8}");
                            Console.WriteLine($"Região acessada: {MapaMemoria.IdentificarRegiao(enderecoMemoria)}");
                            Console.WriteLine($"Valor armazenado na memória: {valorArmazenado} (0x{valorArmazenado:X8})");

                            break;
                        }


                    // ======================================
                    // FORMATO B
                    // ======================================

                    case FormatoInstrucao.B:
                        {
                            uint funct3 = (instrucaoAtual >> 12) & 0b111u;
                            uint rs1 = (instrucaoAtual >> 15) & 0b11111u;
                            uint rs2 = (instrucaoAtual >> 20) & 0b11111u;

                            uint imediato12 = (instrucaoAtual >> 31) & 0b1u;
                            uint imediato11 = (instrucaoAtual >> 7) & 0b1u;
                            uint imediato105 = (instrucaoAtual >> 25) & 0b111111u;
                            uint imediato41 = (instrucaoAtual >> 8) & 0b1111u;

                            uint imediato13 =
                                (imediato12 << 12) |
                                (imediato11 << 11) |
                                (imediato105 << 5) |
                                (imediato41 << 1);

                            uint imediatoEstendido = imediato13;

                            if ((imediato13 & 0x1000u) != 0)
                            {
                                imediatoEstendido |= 0xFFFFE000u;
                            }

                            Operacao operacaoIdentificada =
                                decodificador.IdentificarOperacaoB(opcode, funct3);

                            if (operacaoIdentificada == Operacao.NaoReconhecida)
                            {
                                Console.WriteLine("Operação B não reconhecida.");
                                return;
                            }

                            uint valorRs1 = banco.Ler((int)rs1);
                            uint valorRs2 = banco.Ler((int)rs2);

                            uint resultadoComparacao = ula.Executar(
                                operacaoIdentificada, valorRs1, valorRs2);

                            bool condicaoVerdadeira = resultadoComparacao != 0;

                            uint enderecoDestino = unchecked(
                                enderecoAtual + imediatoEstendido);

                            if (condicaoVerdadeira)
                            {
                                pc.Definir(enderecoDestino);
                                pcFoiAtualizado = true;
                            }

                            Console.WriteLine($"Operação identificada: {operacaoIdentificada}");
                            Console.WriteLine($"Funct3: {funct3}");

                            Console.WriteLine($"\nRegistrador fonte 1: x{rs1}");
                            Console.WriteLine($"Registrador fonte 2: x{rs2}");

                            Console.WriteLine("\n--- OPERANDOS ---");
                            Console.WriteLine($"Conteúdo de x{rs1}: {valorRs1}");
                            Console.WriteLine($"Conteúdo de x{rs2}: {valorRs2}");

                            Console.WriteLine("\n--- IMEDIATO ---");
                            Console.WriteLine($"Imediato reconstruído: {imediato13}");
                            Console.WriteLine($"Imediato estendido: {unchecked((int)imediatoEstendido)}");

                            Console.WriteLine("\n--- DESVIO ---");
                            Console.WriteLine($"Resultado da comparação: {resultadoComparacao}");
                            Console.WriteLine($"Condição satisfeita: {condicaoVerdadeira}");

                            if (condicaoVerdadeira)
                            {
                                Console.WriteLine("Desvio realizado!");
                                Console.WriteLine($"Endereço de destino: 0x{enderecoDestino:X8}");
                            }
                            else
                            {
                                Console.WriteLine("Desvio não realizado. Execução sequencial.");
                            }

                            break;
                        }


                    // ======================================
                    // FORMATO U
                    // ======================================

                    case FormatoInstrucao.U:
                        {
                            uint rd = (instrucaoAtual >> 7) & 0b11111u;

                            // O imediato já está posicionado nos bits 31 até 12
                            uint imediatoU = instrucaoAtual & 0xFFFFF000u;

                            Operacao operacaoIdentificada =
                                decodificador.IdentificarOperacaoU(opcode);

                            if (operacaoIdentificada == Operacao.NaoReconhecida)
                            {
                                Console.WriteLine("Operação U não reconhecida.");
                                return;
                            }

                            uint resultado;

                            if (operacaoIdentificada == Operacao.Lui)
                            {
                                // LUI: carrega o imediato superior
                                resultado = imediatoU;
                            }
                            else
                            {
                                // AUIPC: soma o imediato ao PC atual
                                resultado = unchecked(enderecoAtual + imediatoU);
                            }

                            banco.Escrever((int)rd, resultado);

                            Console.WriteLine($"Operação identificada: {operacaoIdentificada}");
                            Console.WriteLine($"Registrador destino: x{rd}");

                            Console.WriteLine("\n--- IMEDIATO U ---");
                            Console.WriteLine($"Imediato: 0x{imediatoU:X8}");

                            Console.WriteLine("\n--- RESULTADO DA EXECUÇÃO ---");
                            Console.WriteLine($"Resultado: 0x{resultado:X8}");
                            Console.WriteLine($"Registrador atualizado: x{rd} = 0x{banco.Ler((int)rd):X8}");

                            break;
                        }


                    // ======================================
                    // FORMATO J
                    // ======================================

                    case FormatoInstrucao.J:
                        {
                            uint rd = (instrucaoAtual >> 7) & 0b11111u;

                            // Extração das quatro partes do imediato
                            uint imediato20 = (instrucaoAtual >> 31) & 0b1u;

                            uint imediato101 =
                                (instrucaoAtual >> 21) & 0b1111111111u;

                            uint imediato11 = (instrucaoAtual >> 20) & 0b1u;

                            uint imediato1912 =
                                (instrucaoAtual >> 12) & 0b11111111u;

                            // Reconstrução do imediato de 21 bits
                            uint imediato21 =
                                (imediato20 << 20) |
                                (imediato1912 << 12) |
                                (imediato11 << 11) |
                                (imediato101 << 1);

                            // Extensão de sinal para 32 bits
                            uint imediatoEstendido = imediato21;

                            if ((imediato21 & 0x100000u) != 0)
                            {
                                imediatoEstendido |= 0xFFE00000u;
                            }

                            Operacao operacaoIdentificada =
                                decodificador.IdentificarOperacaoJ(opcode);

                            if (operacaoIdentificada != Operacao.Jal)
                            {
                                Console.WriteLine("Operação J não reconhecida.");
                                return;
                            }

                            // Endereço de retorno
                            uint enderecoRetorno = unchecked(enderecoAtual + 4u);

                            // Endereço de destino
                            uint enderecoDestino = unchecked(
                                enderecoAtual + imediatoEstendido);

                            // Salva o endereço de retorno em rd
                            banco.Escrever((int)rd, enderecoRetorno);

                            // Atualiza o PC para o endereço do salto
                            pc.Definir(enderecoDestino);

                            pcFoiAtualizado = true;

                            Console.WriteLine($"Operação identificada: {operacaoIdentificada}");
                            Console.WriteLine($"Registrador destino: x{rd}");

                            Console.WriteLine("\n--- IMEDIATO J ---");
                            Console.WriteLine($"Imediato reconstruído: {imediato21}");
                            Console.WriteLine($"Imediato estendido: {unchecked((int)imediatoEstendido)}");

                            Console.WriteLine("\n--- SALTO ---");
                            Console.WriteLine($"Endereço de retorno: 0x{enderecoRetorno:X8}");
                            Console.WriteLine($"Endereço de destino: 0x{enderecoDestino:X8}");
                            Console.WriteLine($"Registrador atualizado: x{rd} = {banco.Ler((int)rd)}");

                            break;
                        }


                    // ======================================
                    // FORMATO CUSTOMIZADO
                    // ======================================

                    case FormatoInstrucao.Custom:
                        {
                            // Codificação customizada para BSWAP:
                            // rd, rs1 e campos restantes reservados em zero

                            uint rd = (instrucaoAtual >> 7) & 0b11111u;
                            uint funct3 = (instrucaoAtual >> 12) & 0b111u;
                            uint rs1 = (instrucaoAtual >> 15) & 0b11111u;
                            uint rs2 = (instrucaoAtual >> 20) & 0b11111u;
                            uint funct7 = (instrucaoAtual >> 25) & 0b1111111u;

                            Operacao operacaoIdentificada =
                                decodificador.IdentificarOperacaoCustom(opcode);

                            if (operacaoIdentificada != Operacao.Bswap ||
                                funct3 != 0 ||
                                rs2 != 0 ||
                                funct7 != 0)
                            {
                                Console.WriteLine("Instrução customizada não reconhecida.");
                                return;
                            }

                            // BSWAP utiliza somente rs1
                            uint valorRs1 = banco.Ler((int)rs1);

                            // Execução da inversão dos quatro bytes
                            uint resultado = ula.Executar(
                                operacaoIdentificada, valorRs1, 0);

                            // Escrita no registrador de destino
                            banco.Escrever((int)rd, resultado);

                            Console.WriteLine($"Operação identificada: {operacaoIdentificada}");

                            Console.WriteLine($"\nRegistrador destino: x{rd}");
                            Console.WriteLine($"Registrador fonte: x{rs1}");

                            Console.WriteLine("\n--- OPERANDOS ---");
                            Console.WriteLine($"Valor original: 0x{valorRs1:X8}");

                            Console.WriteLine("\n--- RESULTADO DA EXECUÇÃO ---");
                            Console.WriteLine($"Resultado do BSWAP: 0x{resultado:X8}");
                            Console.WriteLine($"Registrador atualizado: x{rd} = 0x{banco.Ler((int)rd):X8}");

                            break;
                        }


                    default:
                        {
                            Console.WriteLine("Formato de instrução ainda não implementado.");
                            return;
                        }
                }


                // Avança o PC somente se ele não foi atualizado por desvio ou salto
                if (!pcFoiAtualizado)
                {
                    pc.Avancar();
                }

                Console.WriteLine($"Próximo endereço do PC: 0x{pc.Ler():X8}");
            }


            // ==========================================
            // EXIBICAO FINAL DO BANCO DE REGISTRADORES
            // ==========================================

            Console.WriteLine("\n--- BANCO DE REGISTRADORES ---");

            for (int registrador = 0; registrador <= 23; registrador++)
            {
                Console.WriteLine($"x{registrador}: {banco.Ler(registrador)}");
            }


            // ==========================================
            // RESULTADO FINAL DA INTEGRACAO SW / LW
            // ==========================================

            uint valorFinalRam = memoriaDados.LerPalavra(100u);
            uint valorFinalVram = memoriaDados.LerPalavra(0x80000u);

            bool testeRamCpu =
                banco.Ler(16) == 0x12345678u &&
                valorFinalRam == 0x12345678u;

            bool testeVramCpu =
                banco.Ler(19) == 0x55667788u &&
                valorFinalVram == 0x55667788u;

            Console.WriteLine("\n--- RESULTADO DA INTEGRACAO SW / LW ---");

            Console.WriteLine(
                $"RAM: x16 = 0x{banco.Ler(16):X8} | " +
                $"Memória[100] = 0x{valorFinalRam:X8} | " +
                $"{(testeRamCpu ? "APROVADO" : "FALHOU")}");

            Console.WriteLine(
                $"VRAM: x19 = 0x{banco.Ler(19):X8} | " +
                $"Memória[0x80000] = 0x{valorFinalVram:X8} | " +
                $"{(testeVramCpu ? "APROVADO" : "FALHOU")}");


            // PC final
            Console.WriteLine($"\nPC final: 0x{pc.Ler():X8}");
        }
    }
}


