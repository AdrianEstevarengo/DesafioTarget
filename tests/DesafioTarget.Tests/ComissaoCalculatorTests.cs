using DesafioTarget.Core.Comissoes;

namespace DesafioTarget.Tests;

public class ComissaoCalculatorTests
{
    [Theory]
    [InlineData(0.00, 0.00)]
    [InlineData(99.99, 0.00)]     // abaixo de 100 → sem comissão
    [InlineData(100.00, 1.00)]    // limite inferior da faixa de 1%
    [InlineData(499.99, 5.00)]    // 1% = 4,9999 → arredonda para 5,00
    [InlineData(500.00, 25.00)]   // "a partir de" 500 → 5%
    [InlineData(1200.50, 60.03)]  // 60,025 → 60,03
    public void CalcularComissao_aplica_a_faixa_correta(double valor, double esperado)
    {
        Assert.Equal((decimal)esperado, ComissaoCalculator.CalcularComissao((decimal)valor));
    }

    [Fact]
    public void CalcularComissao_rejeita_valor_negativo()
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => ComissaoCalculator.CalcularComissao(-1m));
    }

    [Fact]
    public void CalcularPorVendedor_agrupa_e_soma_por_vendedor()
    {
        var vendas = new[]
        {
            new Venda("Ana", 50m),     // 0
            new Venda("Ana", 200m),    // 2,00
            new Venda("Ana", 1000m),   // 50,00
            new Venda("Bruno", 500m),  // 25,00
        };

        var resultado = new ComissaoCalculator().CalcularPorVendedor(vendas);

        Assert.Equal(2, resultado.Count);
        var ana = resultado.Single(r => r.Vendedor == "Ana");
        Assert.Equal(3, ana.QuantidadeVendas);
        Assert.Equal(1250m, ana.TotalVendido);
        Assert.Equal(52m, ana.TotalComissao);
        Assert.Equal(25m, resultado.Single(r => r.Vendedor == "Bruno").TotalComissao);
    }

    [Fact]
    public void CalcularPorVendedor_com_os_dados_do_desafio()
    {
        var resultado = new ComissaoCalculator().CalcularPorVendedor(DadosDesafio.Vendas);

        Assert.Equal(495.69m, resultado.Single(r => r.Vendedor == "João Silva").TotalComissao);
        Assert.Equal(465.96m, resultado.Single(r => r.Vendedor == "Maria Souza").TotalComissao);
        Assert.Equal(379.38m, resultado.Single(r => r.Vendedor == "Carlos Oliveira").TotalComissao);
        Assert.Equal(404.99m, resultado.Single(r => r.Vendedor == "Ana Lima").TotalComissao);
    }
}
