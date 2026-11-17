# Simulador RISC-V

Simulador educacional de uma CPU baseada na arquitetura RISC-V de 32 bits (RV32I), desenvolvido em C#.

O projeto tem como objetivo compreender e implementar progressivamente os principais componentes de um processador, desde a busca e decodificação até a execução de instruções, incluindo operações aritméticas, lógicas, desvios e acesso à memória.

**Status:** Em desenvolvimento.

## Funcionalidades implementadas

### Componentes da CPU

- **Contador de Programa (PC):** controle do endereço da próxima instrução, com avanço sequencial e suporte a desvios e saltos.
- **Banco de registradores:** implementação dos 32 registradores de propósito geral (x0 a x31), incluindo a proteção do registrador x0.
- **Decodificador:** identificação dos formatos e das operações por meio dos campos da instrução.
- **ULA (Unidade Lógica e Aritmética):** execução de operações aritméticas, lógicas, comparações e deslocamentos.
- **Memória de instruções:** armazenamento e leitura de instruções de 32 bits.
- **Memória de dados:** suporte a leitura e escrita de palavras de 32 bits.
- **Mapa de memória:** identificação e validação das regiões de memória disponíveis.

### Instruções suportadas

O simulador implementa 30 instruções da base RV32I, além de uma instrução personalizada.

| Formato | Instruções |
|---|---|
| R | ADD, SUB, SLL, SLT, SLTU, XOR, SRL, SRA, OR, AND |
| I | ADDI, SLTI, SLTIU, XORI, ORI, ANDI, SLLI, SRLI, SRAI, LW |
| S | SW |
| B | BEQ, BNE, BLT, BGE, BLTU, BGEU |
| U | LUI, AUIPC |
| J | JAL |
| Custom-0 | BSWAP |

**Instrução personalizada BSWAP:** inverte a ordem dos quatro bytes de um registrador de 32 bits. Exemplo: `0x12345678` resulta em `0x78563412`.

## Sistema de memória

O simulador possui um mapa de endereçamento com as seguintes regiões:

| Região | Endereço inicial | Endereço final | Capacidade |
|---|---|---|---|
| RAM principal | 0x00000 | 0x7FFFF | 512 KiB |
| VRAM | 0x80000 | 0x8FFFF | 64 KiB |
| Reservada | 0x90000 | 0x9FBFF | — |
| Periféricos | 0x9FC00 | 0x9FFFF | — |

Endereços a partir de `0xA0000` estão fora do mapa de memória.

### Características da memória

- Armazenamento de palavras de 32 bits.
- Organização Little-Endian.
- Verificação de alinhamento de endereços.
- Validação dos limites das regiões de memória.
- Bloqueio de acessos a regiões reservadas, periféricos não implementados e endereços fora do mapa.

### Integração SW/LW

As instruções SW e LW estão integradas à execução da CPU, permitindo armazenar e recuperar valores tanto na RAM principal quanto na VRAM.

## Testes e validações

O programa principal contém cenários de teste para verificar:

- Identificação das regiões do mapa de memória.
- Leitura e escrita na RAM principal e na VRAM.
- Acesso aos limites das regiões de memória.
- Bloqueio de acessos desalinhados.
- Tratamento de regiões reservadas e periféricos.
- Integração das instruções SW e LW com a CPU.
- Execução de instruções dos formatos R, I, S, B, U e J.
- Execução da instrução personalizada BSWAP.

## Tecnologias utilizadas

- C#
- .NET 10
- Visual Studio
- Git e GitHub

## Como executar o projeto

**Pré-requisito:** .NET 10 SDK instalado.

Clone o repositório:

```bash
git clone https://github.com/Cksb7/SimuladorRiscV.git
```

Entre na pasta do projeto:

```bash
cd SimuladorRiscV
```

Execute o simulador:

```bash
dotnet run --project SimuladorRiscV/SimuladorRiscV.csproj
```

## Equipe

Projeto desenvolvido em grupo.

