namespace DesafioTarget.Core.Estoque;

/// <summary>Registro imutável de uma movimentação de estoque já efetivada.</summary>
public sealed record Movimentacao(
    int Id,
    int CodigoProduto,
    TipoMovimentacao Tipo,
    int Quantidade,
    string Descricao,
    int EstoqueAnterior,
    int EstoqueFinal,
    DateTime DataHora);
