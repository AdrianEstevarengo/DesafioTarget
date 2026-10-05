namespace DesafioTarget.Core.Juros;

/// <summary>
/// Calcula os juros de um título vencido usando juros simples de 2,5% ao dia
/// sobre o valor original: juros = valor × 2,5% × dias de atraso.
/// Títulos pagos até o vencimento não têm juros.
/// </summary>
public sealed class JurosCalculator
{
    public const decimal TaxaDiaria = 0.025m;

    private readonly Func<DateOnly> _hoje;

    /// <param name="hoje">Fonte da data atual (permite fixar a data em testes).</param>
    public JurosCalculator(Func<DateOnly>? hoje = null)
    {
        _hoje = hoje ?? (() => DateOnly.FromDateTime(DateTime.Today));
    }

    /// <summary>Calcula os juros na data de hoje.</summary>
    public ResultadoJuros Calcular(decimal valor, DateOnly dataVencimento) =>
        Calcular(valor, dataVencimento, _hoje());

    /// <summary>Calcula os juros em uma data de referência específica.</summary>
    public static ResultadoJuros Calcular(decimal valor, DateOnly dataVencimento, DateOnly dataCalculo)
    {
        if (valor <= 0)
            throw new ArgumentOutOfRangeException(nameof(valor), "O valor deve ser maior que zero.");

        var diasAtraso = Math.Max(0, dataCalculo.DayNumber - dataVencimento.DayNumber);
        var juros = Math.Round(valor * TaxaDiaria * diasAtraso, 2, MidpointRounding.AwayFromZero);

        return new ResultadoJuros(
            ValorOriginal: valor,
            DataVencimento: dataVencimento,
            DataCalculo: dataCalculo,
            DiasAtraso: diasAtraso,
            ValorJuros: juros,
            ValorAtualizado: valor + juros);
    }
}
