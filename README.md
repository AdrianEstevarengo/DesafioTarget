# Desafio Técnico — Target Sistemas

Solução em **C# / .NET 8** para os três exercícios do desafio, organizada em camadas:

```
DesafioTarget.sln
├── src/
│   ├── DesafioTarget.Core/      # Regras de negócio (sem dependência de console ou arquivo)
│   │   ├── Comissoes/           # Exercício 1
│   │   ├── Estoque/             # Exercício 2
│   │   └── Juros/               # Exercício 3
│   └── DesafioTarget.App/       # Aplicação console com menu
│       ├── Dados/               # vendas.json e estoque.json do enunciado
│       └── Telas/               # Interação com o usuário
└── tests/
    └── DesafioTarget.Tests/     # Testes unitários (xUnit)
```

A lógica fica toda no projeto `Core`, isolada da interface. Assim ela pode ser testada
e reaproveitada (numa API REST, por exemplo) sem mexer em nada.

## Como executar

Requisito: [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0).

```bash
dotnet run --project src/DesafioTarget.App     # abre o menu
dotnet test                                    # roda os testes
```

## 1. Comissão dos vendedores

Lê `Dados/vendas.json`, aplica a regra **em cada venda** e agrupa por vendedor:

| Valor da venda               | Comissão |
|------------------------------|----------|
| abaixo de R$ 100,00          | 0%       |
| de R$ 100,00 a R$ 499,99     | 1%       |
| a partir de R$ 500,00        | 5%       |

Resultado com os dados do enunciado:

| Vendedor        | Vendas | Total vendido | Comissão  |
|-----------------|-------:|--------------:|----------:|
| João Silva      | 10     | R$ 10.754,70  | R$ 495,69 |
| Maria Souza     | 9      | R$ 9.874,30   | R$ 465,96 |
| Ana Lima        | 9      | R$ 8.763,95   | R$ 404,99 |
| Carlos Oliveira | 8      | R$ 7.928,35   | R$ 379,38 |
| **Total**       | **36** | **R$ 37.321,30** | **R$ 1.746,02** |

## 2. Movimentação de estoque

Carrega `Dados/estoque.json` e permite listar produtos, lançar movimentações e ver o histórico.
Cada lançamento:

- recebe um **ID único e sequencial** (gerado dentro de um `lock`, então não repete mesmo com chamadas concorrentes);
- tem **tipo** (Entrada/Saída) e uma **descrição** livre que identifica a operação (ex.: "Compra de fornecedor", "Venda", "Devolução");
- **retorna a quantidade final** do produto, junto com o estoque anterior.

Validações: produto inexistente, quantidade menor ou igual a zero, descrição vazia e saída maior
que o saldo (o estoque nunca fica negativo). Um lançamento recusado não altera o saldo nem entra no histórico.

## 3. Juros por atraso

Informa-se o valor e a data de vencimento; o programa calcula os juros **na data de hoje**:

```
juros = valor × 2,5% × dias de atraso
```

- Juros **simples** sobre o valor original, como o enunciado descreve a taxa "ao dia".
- Se o título vence hoje ou no futuro, não há juros.
- A data de hoje é injetável (`Func<DateOnly>`), para os testes não dependerem do dia em que rodam.

Exemplo: R$ 1.000,00 vencido há 10 dias → juros de R$ 250,00 → total R$ 1.250,00.

## Decisões técnicas

- **`decimal`** para todos os valores monetários (evita os erros de arredondamento do `double`).
- Arredondamento para centavos com `MidpointRounding.AwayFromZero` (o "arredondamento comercial": 0,005 → 0,01).
  Na comissão, o arredondamento é feito por venda, já que cada venda gera sua própria comissão.
- Entrada de valores aceita `1.500,75`, `1500,75` ou `1500.75`; datas em `dd/mm/aaaa`.
- Erros de regra de negócio usam uma exceção própria (`EstoqueException`), tratada na tela com uma mensagem amigável.
- Sem pacotes externos no código de produção: só a biblioteca padrão do .NET (`System.Text.Json`, LINQ).
