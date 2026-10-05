namespace DesafioTarget.Core.Estoque;

public enum TipoMovimentacao
{
    Entrada = 1,
    Saida = 2
}

public static class TipoMovimentacaoExtensions
{
    public static string Descricao(this TipoMovimentacao tipo) => tipo switch
    {
        TipoMovimentacao.Entrada => "Entrada",
        TipoMovimentacao.Saida => "Saída",
        _ => tipo.ToString()
    };
}
