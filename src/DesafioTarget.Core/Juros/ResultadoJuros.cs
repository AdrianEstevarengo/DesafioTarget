namespace DesafioTarget.Core.Juros;

public sealed record ResultadoJuros(
    decimal ValorOriginal,
    DateOnly DataVencimento,
    DateOnly DataCalculo,
    int DiasAtraso,
    decimal ValorJuros,
    decimal ValorAtualizado);
