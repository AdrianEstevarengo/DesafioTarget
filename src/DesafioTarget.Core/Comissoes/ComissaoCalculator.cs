namespace DesafioTarget.Core.Comissoes;

/// <summary>
/// Calcula comissões aplicando a regra por venda:
/// <list type="bullet">
///   <item>abaixo de R$ 100,00 → sem comissão;</item>
///   <item>de R$ 100,00 até abaixo de R$ 500,00 → 1%;</item>
///   <item>a partir de R$ 500,00 → 5%.</item>
/// </list>
/// </summary>
public sealed class ComissaoCalculator
{
    public const decimal LimiteSemComissao = 100.00m;
    public const decimal LimiteFaixaMaior = 500.00m;
    public const decimal PercentualFaixaMenor = 0.01m;
    public const decimal PercentualFaixaMaior = 0.05m;

    /// <summary>Retorna o percentual de comissão aplicável ao valor de uma venda.</summary>
    public static decimal ObterPercentual(decimal valorVenda) => valorVenda switch
    {
        < LimiteSemComissao => 0m,
        < LimiteFaixaMaior => PercentualFaixaMenor,
        _ => PercentualFaixaMaior
    };

    /// <summary>Calcula a comissão de uma única venda, arredondada para centavos.</summary>
    public static decimal CalcularComissao(decimal valorVenda)
    {
        if (valorVenda < 0)
            throw new ArgumentOutOfRangeException(nameof(valorVenda), "O valor da venda não pode ser negativo.");

        return Math.Round(valorVenda * ObterPercentual(valorVenda), 2, MidpointRounding.AwayFromZero);
    }

    /// <summary>Agrupa as vendas por vendedor e soma vendas e comissões.</summary>
    public IReadOnlyList<ComissaoVendedor> CalcularPorVendedor(IEnumerable<Venda> vendas)
    {
        ArgumentNullException.ThrowIfNull(vendas);

        return vendas
            .GroupBy(v => v.Vendedor.Trim())
            .Select(g => new ComissaoVendedor(
                Vendedor: g.Key,
                QuantidadeVendas: g.Count(),
                TotalVendido: g.Sum(v => v.Valor),
                TotalComissao: g.Sum(v => CalcularComissao(v.Valor))))
            .OrderByDescending(c => c.TotalComissao)
            .ToList();
    }
}
