namespace DesafioTarget.Core.Estoque;

/// <summary>Erro de regra de negócio ao movimentar o estoque.</summary>
public sealed class EstoqueException(string message) : Exception(message);
