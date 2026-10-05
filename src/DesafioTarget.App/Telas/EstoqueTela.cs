using System.Text.Json;
using DesafioTarget.Core.Estoque;

namespace DesafioTarget.App.Telas;

/// <summary>
/// Tela de movimentação de estoque. O serviço é criado uma única vez,
/// então as movimentações ficam acumuladas enquanto o programa estiver aberto.
/// </summary>
internal sealed class EstoqueTela
{
    private sealed record ProdutoJson(int CodigoProduto, string DescricaoProduto, int Estoque);
    private sealed record ArquivoEstoque(List<ProdutoJson> Estoque);

    private readonly EstoqueService _service;

    public EstoqueTela(string caminhoJson)
    {
        var arquivo = JsonSerializer.Deserialize<ArquivoEstoque>(
                          File.ReadAllText(caminhoJson),
                          new JsonSerializerOptions { PropertyNameCaseInsensitive = true })
                      ?? throw new InvalidDataException("Arquivo de estoque vazio ou inválido.");

        _service = new EstoqueService(
            arquivo.Estoque.Select(p => new Produto(p.CodigoProduto, p.DescricaoProduto, p.Estoque)));
    }

    public void Executar()
    {
        while (true)
        {
            Console.WriteLine();
            Console.WriteLine("--- Movimentação de estoque ---");
            Console.WriteLine(" 1 - Listar produtos");
            Console.WriteLine(" 2 - Lançar movimentação");
            Console.WriteLine(" 3 - Histórico de movimentações");
            Console.WriteLine(" 0 - Voltar");

            switch (Entrada.LerTexto("Opção: "))
            {
                case "1": ListarProdutos(); break;
                case "2": LancarMovimentacao(); break;
                case "3": ListarHistorico(); break;
                case "0" or null: return;
                default: Console.WriteLine("Opção inválida."); break;
            }
        }
    }

    private void ListarProdutos()
    {
        Console.WriteLine();
        Console.WriteLine($"{"Código",-7} {"Produto",-28} {"Estoque",8}");
        foreach (var p in _service.Produtos)
            Console.WriteLine($"{p.CodigoProduto,-7} {p.DescricaoProduto,-28} {p.Estoque,8}");
    }

    private void LancarMovimentacao()
    {
        ListarProdutos();
        Console.WriteLine();

        var codigo = Entrada.LerInteiro("Código do produto: ");
        if (codigo is null) return;

        var tipoTexto = Entrada.LerTexto("Tipo (E = Entrada / S = Saída): ")?.ToUpperInvariant();
        TipoMovimentacao? tipo = tipoTexto switch
        {
            "E" or "ENTRADA" => TipoMovimentacao.Entrada,
            "S" or "SAIDA" or "SAÍDA" => TipoMovimentacao.Saida,
            _ => null
        };
        if (tipo is null)
        {
            Console.WriteLine("Tipo inválido. Use E ou S.");
            return;
        }

        var quantidade = Entrada.LerInteiro("Quantidade: ");
        if (quantidade is null) return;

        var descricao = Entrada.LerTexto("Descrição (ex.: Compra de fornecedor, Venda, Devolução): ") ?? "";

        try
        {
            var mov = _service.Lancar(codigo.Value, tipo.Value, quantidade.Value, descricao);
            var produto = _service.ObterProduto(mov.CodigoProduto);

            Console.WriteLine();
            Console.WriteLine($"Movimentação #{mov.Id} registrada: {mov.Tipo.Descricao()} de {mov.Quantidade} un. — \"{mov.Descricao}\"");
            Console.WriteLine($"Produto: {produto.CodigoProduto} - {produto.DescricaoProduto}");
            Console.WriteLine($"Estoque anterior: {mov.EstoqueAnterior} | ESTOQUE FINAL: {mov.EstoqueFinal}");
        }
        catch (EstoqueException ex)
        {
            Console.WriteLine($"Não foi possível lançar: {ex.Message}");
        }
    }

    private void ListarHistorico()
    {
        var movs = _service.Movimentacoes;
        if (movs.Count == 0)
        {
            Console.WriteLine("Nenhuma movimentação lançada ainda.");
            return;
        }

        Console.WriteLine();
        Console.WriteLine($"{"ID",-4} {"Data/Hora",-19} {"Prod.",-6} {"Tipo",-8} {"Qtde",6} {"Final",7}  Descrição");
        foreach (var m in movs)
            Console.WriteLine($"{m.Id,-4} {m.DataHora:dd/MM/yyyy HH:mm:ss} {m.CodigoProduto,-6} {m.Tipo.Descricao(),-8} {m.Quantidade,6} {m.EstoqueFinal,7}  {m.Descricao}");
    }
}
