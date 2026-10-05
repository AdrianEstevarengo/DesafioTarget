using System.Text.Json;
using DesafioTarget.Core.Comissoes;

namespace DesafioTarget.App.Telas;

internal static class ComissoesTela
{
    private sealed record ArquivoVendas(List<Venda> Vendas);

    private static readonly JsonSerializerOptions JsonOptions = new() { PropertyNameCaseInsensitive = true };

    public static void Executar(string caminhoJson)
    {
        var arquivo = JsonSerializer.Deserialize<ArquivoVendas>(File.ReadAllText(caminhoJson), JsonOptions)
                      ?? throw new InvalidDataException("Arquivo de vendas vazio ou inválido.");

        var resultado = new ComissaoCalculator().CalcularPorVendedor(arquivo.Vendas);

        Console.WriteLine();
        Console.WriteLine("Regra: < R$ 100,00 = 0% | < R$ 500,00 = 1% | >= R$ 500,00 = 5% (por venda)");
        Console.WriteLine();
        Console.WriteLine($"{"Vendedor",-18} {"Vendas",6} {"Total vendido",16} {"Comissão",13}");
        Console.WriteLine(new string('-', 56));

        foreach (var c in resultado)
            Console.WriteLine($"{c.Vendedor,-18} {c.QuantidadeVendas,6} {c.TotalVendido,16:C2} {c.TotalComissao,13:C2}");

        Console.WriteLine(new string('-', 56));
        Console.WriteLine($"{"TOTAL",-18} {resultado.Sum(c => c.QuantidadeVendas),6} " +
                          $"{resultado.Sum(c => c.TotalVendido),16:C2} {resultado.Sum(c => c.TotalComissao),13:C2}");
    }
}
