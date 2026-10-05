using DesafioTarget.Core.Juros;

namespace DesafioTarget.App.Telas;

internal static class JurosTela
{
    public static void Executar()
    {
        Console.WriteLine();
        Console.WriteLine("--- Cálculo de juros (2,5% ao dia, juros simples) ---");

        var valor = Entrada.LerDecimal("Valor do título (R$): ");
        if (valor is null) return;
        if (valor <= 0)
        {
            Console.WriteLine("O valor deve ser maior que zero.");
            return;
        }

        var vencimento = Entrada.LerData("Data de vencimento (dd/mm/aaaa): ");
        if (vencimento is null) return;

        var r = new JurosCalculator().Calcular(valor.Value, vencimento.Value);

        Console.WriteLine();
        Console.WriteLine($"Data do cálculo : {r.DataCalculo:dd/MM/yyyy}");
        Console.WriteLine($"Vencimento      : {r.DataVencimento:dd/MM/yyyy}");
        Console.WriteLine($"Dias em atraso  : {r.DiasAtraso}");
        Console.WriteLine($"Valor original  : {r.ValorOriginal:C2}");
        Console.WriteLine($"Juros           : {r.ValorJuros:C2}");
        Console.WriteLine($"Valor atualizado: {r.ValorAtualizado:C2}");

        if (r.DiasAtraso == 0)
            Console.WriteLine("Título dentro do prazo: não há juros a cobrar.");
    }
}
