# SimuladorRiscV

O projeto tem como objetivo implementar e simular o funcionamento dos principais componentes de um processador, desde a busca e decodificação das instruções até sua execução.

## Funcionalidades implementadas

- **Decodificação de instruções:** extração e identificação dos campos de instruções de 32 bits.
- **Banco de registradores:** implementação dos 32 registradores da arquitetura, incluindo a proteção do registrador x0.
- **ULA (Unidade Lógica e Aritmética):** execução das operações ADD e SUB.
- **Decodificador:** identificação das operações utilizando enum e switch.
- **Contador de Programa (PC):** controle do endereço da próxima instrução, com avanço sequencial de 4 bytes.
- **Memória de instruções:** armazenamento e busca de instruções por endereço.

## Tecnologias utilizadas

- C#
- .NET 10
- Visual Studio

## Como executar o projeto

**Pré-requisitos:**

- .NET 10 SDK instalado.

**1. Clone o repositório:**

```bash
git clone https://github.com/Cksb7/SimuladorRiscV.git
```

**2. Entre na pasta do projeto:**

```bash
cd SimuladorRiscV
```

**3. Execute o simulador:**

```bash
dotnet run --project SimuladorRiscV/SimuladorRiscV.csproj
```

## Próximas implementações

- Implementação da instrução especial BSWAP.
- Expansão do conjunto de instruções suportadas.
- Evolução dos componentes e testes integrados do processador.

## Equipe

Projeto desenvolvido em grupo.
