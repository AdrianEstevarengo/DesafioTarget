namespace DesafioTarget.Core.Estoque;

/// <summary>Produto do depósito com sua quantidade atual em estoque.</summary>
public sealed class Produto
{
    public Produto(int codigoProduto, string descricaoProduto, int estoque)
    {
        if (estoque < 0)
            throw new ArgumentOutOfRangeException(nameof(estoque), "O estoque inicial não pode ser negativo.");

        CodigoProduto = codigoProduto;
        DescricaoProduto = descricaoProduto;
        Estoque = estoque;
    }

    public int CodigoProduto { get; }
    public string DescricaoProduto { get; }
    public int Estoque { get; internal set; }
}
