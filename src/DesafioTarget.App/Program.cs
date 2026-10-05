using System.Globalization;
using System.Text;
using DesafioTarget.App.Telas;

Console.OutputEncoding = Encoding.UTF8;
Console.InputEncoding = Encoding.UTF8;
CultureInfo.DefaultThreadCurrentCulture = CultureInfo.DefaultThreadCurrentUICulture = new CultureInfo("pt-BR");
CultureInfo.CurrentCulture = CultureInfo.CurrentUICulture = new CultureInfo("pt-BR");

var estoqueTela = new EstoqueTela(Path.Combine(AppContext.BaseDirectory, "Dados", "estoque.json"));

while (true)
{
    Console.WriteLine();
    Console.WriteLine("==============================================");
    Console.WriteLine("        DESAFIO TÉCNICO — TARGET SISTEMAS      ");
    Console.WriteLine("==============================================");
    Console.WriteLine(" 1 - Comissão dos vendedores");
    Console.WriteLine(" 2 - Movimentação de estoque");
    Console.WriteLine(" 3 - Cálculo de juros por atraso");
    Console.WriteLine(" 0 - Sair");
    Console.Write("Escolha uma opção: ");

    var opcao = Console.ReadLine()?.Trim();
    if (opcao is null) break; // fim da entrada (ex.: Ctrl+Z / pipe encerrado)

    try
    {
        switch (opcao)
        {
            case "1": ComissoesTela.Executar(Path.Combine(AppContext.BaseDirectory, "Dados", "vendas.json")); break;
            case "2": estoqueTela.Executar(); break;
            case "3": JurosTela.Executar(); break;
            case "0": return;
            default: Console.WriteLine("Opção inválida."); break;
        }
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Erro inesperado: {ex.Message}");
    }
}
