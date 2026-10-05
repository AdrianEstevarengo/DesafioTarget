using DesafioTarget.Core.Juros;

namespace DesafioTarget.Tests;

public class JurosCalculatorTests
{
    private static readonly DateOnly Hoje = new(2026, 10, 5);

    [Fact]
    public void Titulo_vencido_cobra_2_5_por_cento_ao_dia()
    {
        var r = JurosCalculator.Calcular(1000m, new DateOnly(2026, 9, 25), Hoje);

        Assert.Equal(10, r.DiasAtraso);
        Assert.Equal(250m, r.ValorJuros);        // 1000 × 2,5% × 10
        Assert.Equal(1250m, r.ValorAtualizado);
    }

    [Fact]
    public void Um_dia_de_atraso()
    {
        var r = JurosCalculator.Calcular(100m, Hoje.AddDays(-1), Hoje);

        Assert.Equal(1, r.DiasAtraso);
        Assert.Equal(2.50m, r.ValorJuros);
    }

    [Theory]
    [InlineData(0)]   // vence hoje
    [InlineData(5)]   // vence no futuro
    public void Titulo_em_dia_nao_tem_juros(int diasParaVencer)
    {
        var r = JurosCalculator.Calcular(500m, Hoje.AddDays(diasParaVencer), Hoje);

        Assert.Equal(0, r.DiasAtraso);
        Assert.Equal(0m, r.ValorJuros);
        Assert.Equal(500m, r.ValorAtualizado);
    }

    [Fact]
    public void Juros_sao_arredondados_para_centavos()
    {
        var r = JurosCalculator.Calcular(123.45m, Hoje.AddDays(-3), Hoje);

        Assert.Equal(9.26m, r.ValorJuros);   // 123,45 × 0,075 = 9,25875
    }

    [Fact]
    public void Usa_a_data_de_hoje_quando_nenhuma_data_e_informada()
    {
        var calculadora = new JurosCalculator(() => Hoje);

        var r = calculadora.Calcular(200m, Hoje.AddDays(-2));

        Assert.Equal(Hoje, r.DataCalculo);
        Assert.Equal(10m, r.ValorJuros);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-10)]
    public void Valor_invalido_e_rejeitado(int valor)
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => JurosCalculator.Calcular(valor, Hoje, Hoje));
    }
}
