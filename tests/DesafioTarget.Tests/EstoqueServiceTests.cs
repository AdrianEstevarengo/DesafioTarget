using DesafioTarget.Core.Estoque;

namespace DesafioTarget.Tests;

public class EstoqueServiceTests
{
    private static EstoqueService CriarServico() => new(
    [
        new Produto(101, "Caneta Azul", 150),
        new Produto(102, "Caderno Universitário", 75),
    ]);

    [Fact]
    public void Entrada_soma_ao_estoque_e_retorna_quantidade_final()
    {
        var service = CriarServico();

        var mov = service.Lancar(101, TipoMovimentacao.Entrada, 50, "Compra de fornecedor");

        Assert.Equal(150, mov.EstoqueAnterior);
        Assert.Equal(200, mov.EstoqueFinal);
        Assert.Equal(200, service.ObterProduto(101).Estoque);
    }

    [Fact]
    public void Saida_subtrai_do_estoque()
    {
        var service = CriarServico();

        var mov = service.Lancar(102, TipoMovimentacao.Saida, 75, "Venda");

        Assert.Equal(0, mov.EstoqueFinal);
    }

    [Fact]
    public void Cada_movimentacao_recebe_um_id_unico_e_sequencial()
    {
        var service = CriarServico();

        var a = service.Lancar(101, TipoMovimentacao.Entrada, 1, "Ajuste");
        var b = service.Lancar(102, TipoMovimentacao.Saida, 1, "Venda");
        var c = service.Lancar(101, TipoMovimentacao.Saida, 1, "Avaria");

        Assert.Equal(new[] { 1, 2, 3 }, new[] { a.Id, b.Id, c.Id });
        Assert.Equal(3, service.Movimentacoes.Count);
    }

    [Fact]
    public void Saida_maior_que_o_estoque_e_bloqueada_e_nao_altera_nada()
    {
        var service = CriarServico();

        var ex = Assert.Throws<EstoqueException>(() => service.Lancar(102, TipoMovimentacao.Saida, 76, "Venda"));

        Assert.Contains("insuficiente", ex.Message);
        Assert.Equal(75, service.ObterProduto(102).Estoque);
        Assert.Empty(service.Movimentacoes);
    }

    [Theory]
    [InlineData(999, 10, "Compra")]   // produto inexistente
    [InlineData(101, 0, "Compra")]    // quantidade zero
    [InlineData(101, -5, "Compra")]   // quantidade negativa
    [InlineData(101, 10, "   ")]      // sem descrição
    public void Lancamentos_invalidos_sao_rejeitados(int codigo, int quantidade, string descricao)
    {
        var service = CriarServico();

        Assert.Throws<EstoqueException>(() => service.Lancar(codigo, TipoMovimentacao.Entrada, quantidade, descricao));
        Assert.Empty(service.Movimentacoes);
    }

    [Fact]
    public void Produtos_com_codigo_duplicado_nao_sao_aceitos()
    {
        Assert.Throws<EstoqueException>(() => new EstoqueService(
        [
            new Produto(101, "A", 1),
            new Produto(101, "B", 2),
        ]));
    }
}
