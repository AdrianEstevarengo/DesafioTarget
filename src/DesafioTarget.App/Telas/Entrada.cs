using System.Globalization;

namespace DesafioTarget.App.Telas;

/// <summary>Leitura de dados do console com validação e repetição até um valor válido.</summary>
internal static class Entrada
{
    private static readonly CultureInfo PtBr = new("pt-BR");

    public static string? LerTexto(string rotulo)
    {
        Console.Write(rotulo);
        return Console.ReadLine()?.Trim();
    }

    public static int? LerInteiro(string rotulo)
    {
        while (true)
        {
            var texto = LerTexto(rotulo);
            if (texto is null) return null;
            if (int.TryParse(texto, out var valor)) return valor;
            Console.WriteLine("  Valor inválido. Digite um número inteiro.");
        }
    }

    /// <summary>Aceita "1.234,56", "1234,56" ou "1234.56".</summary>
    public static decimal? LerDecimal(string rotulo)
    {
        while (true)
        {
            var texto = LerTexto(rotulo);
            if (texto is null) return null;
            texto = texto.Replace("R$", "").Trim();

            var ehFormatoInvariante = texto.Contains('.') && !texto.Contains(',') && texto.Count(c => c == '.') == 1;
            var cultura = ehFormatoInvariante ? CultureInfo.InvariantCulture : PtBr;

            if (decimal.TryParse(texto, NumberStyles.Number, cultura, out var valor)) return valor;
            Console.WriteLine("  Valor inválido. Exemplo: 1500,75");
        }
    }

    public static DateOnly? LerData(string rotulo)
    {
        string[] formatos = ["dd/MM/yyyy", "d/M/yyyy", "yyyy-MM-dd"];
        while (true)
        {
            var texto = LerTexto(rotulo);
            if (texto is null) return null;
            if (DateOnly.TryParseExact(texto, formatos, PtBr, DateTimeStyles.None, out var data)) return data;
            Console.WriteLine("  Data inválida. Use o formato dd/mm/aaaa.");
        }
    }
}
