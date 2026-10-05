namespace DesafioTarget.Core.Comissoes;

/// <summary>Resumo consolidado da comissão de um vendedor.</summary>
public sealed record ComissaoVendedor(
    string Vendedor,
    int QuantidadeVendas,
    decimal TotalVendido,
    decimal TotalComissao);
