# Simulador RISC-V

Simulador educacional de uma CPU baseada na arquitetura RISC-V, desenvolvido em C# como projeto acadêmico de Ciência da Computação.

O objetivo é implementar e compreender progressivamente os principais componentes de um processador, explorando a busca, decodificação e execução de instruções de 32 bits.

**Status:** Em desenvolvimento.

## Funcionalidades implementadas

### Componentes da CPU

- **Memória de instruções:** armazenamento e leitura de instruções de 32 bits por endereço.
- **Contador de Programa (PC):** controle do endereço da próxima instrução, com avanço sequencial de 4 bytes.
- **Banco de registradores:** implementação dos 32 registradores de propósito geral (x0 a x31), incluindo a proteção do registrador x0, que permanece sempre em zero.
- **Decodificador:** identificação do formato da instrução pelo opcode e identificação das operações pelos campos de função.
- **ULA (Unidade Lógica e Aritmética):** execução de operações aritméticas, lógicas e de deslocamento.

### Instruções suportadas

Atualmente, o simulador executa 19 instruções dos formatos R e I da arquitetura RV32I.

**Formato R — Operações entre registradores**

| Instrução | Descrição |
|---|---|
| ADD | Adição |
| SUB | Subtração |
| SLL | Deslocamento lógico à esquerda |
| SLT | Comparação menor que (com sinal) |
| SLTU | Comparação menor que (sem sinal) |
| XOR | OU exclusivo |
| SRL | Deslocamento lógico à direita |
| SRA | Deslocamento aritmético à direita |
| OR | OU lógico |
| AND | E lógico |

**Formato I — Operações com imediato**

| Instrução | Descrição |
|---|---|
| ADDI | Adição com imediato |
| SLTI | Comparação menor que (com sinal) |
| SLTIU | Comparação menor que (sem sinal) |
| XORI | OU exclusivo com imediato |
| ORI | OU lógico com imediato |
| ANDI | E lógico com imediato |
| SLLI | Deslocamento lógico à esquerda com imediato |
| SRLI | Deslocamento lógico à direita com imediato |
| SRAI | Deslocamento aritmético à direita com imediato |

### Decodificação e execução

O fluxo atual de execução contempla:

1. Busca da instrução na memória utilizando o endereço indicado pelo PC.
2. Extração do opcode e identificação do formato da instrução.
3. Extração dos campos necessários para interpretar a instrução.
4. Identificação da operação pelo decodificador.
5. Leitura dos registradores de origem ou obtenção do imediato.
6. Execução da operação na ULA.
7. Escrita do resultado no registrador de destino.
8. Atualização do contador de programa.

O simulador também implementa a extensão de sinal dos imediatos de 12 bits para 32 bits e o tratamento específico do campo de deslocamento (shamt) nas instruções SLLI, SRLI e SRAI.

## Tecnologias utilizadas

- C#
- .NET 10
- Visual Studio
- Git e GitHub

## Como executar o projeto

### Pré-requisitos

- .NET 10 SDK instalado.
- Git instalado.

### Clonando o repositório

```bash
git clone https://github.com/Cksb7/SimuladorRiscV.git
```

### Acessando a pasta do projeto

```bash
cd SimuladorRiscV
```

### Executando o simulador

```bash
dotnet run --project SimuladorRiscV/SimuladorRiscV.csproj
```

As instruções utilizadas nos testes são definidas diretamente no programa, permitindo acompanhar a decodificação, os operandos, os resultados da ULA e o estado final dos registradores.

## Próximas implementações

O projeto está sendo desenvolvido progressivamente. Entre as próximas etapas estão:

- Implementação das instruções dos formatos S, B, U e J.
- Implementação de leitura e escrita na memória de dados.
- Expansão do fluxo de execução da CPU.
- Implementação da instrução especial BSWAP.
- Ampliação dos testes e validações.
- Organização e refatoração da estrutura principal do simulador.

**Observação:** o reconhecimento de alguns formatos adicionais pelo opcode já está previsto no decodificador, mas suas funcionalidades de execução ainda estão em desenvolvimento.

## Sobre o projeto

Projeto desenvolvido em grupo, com foco no estudo de arquitetura de computadores, organização de processadores, representação de instruções e funcionamento de uma CPU baseada na arquitetura RISC-V.
