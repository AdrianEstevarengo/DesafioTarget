namespace DesafioTarget.Core.Estoque;

/// <summary>
/// Controla o estoque do depósito e registra cada movimentação
/// (entrada ou saída) com um identificador único e uma descrição.
/// </summary>
public sealed class EstoqueService
{
    private readonly Dictionary<int, Produto> _produtos;
    private readonly List<Movimentacao> _movimentacoes = [];
    private readonly object _lock = new();
    private int _ultimoId;

    public EstoqueService(IEnumerable<Produto> produtos)
    {
        ArgumentNullException.ThrowIfNull(produtos);

        _produtos = new Dictionary<int, Produto>();
        foreach (var produto in produtos)
        {
            if (!_produtos.TryAdd(produto.CodigoProduto, produto))
                throw new EstoqueException($"Código de produto duplicado: {produto.CodigoProduto}.");
        }
    }

    public IReadOnlyCollection<Produto> Produtos => _produtos.Values.OrderBy(p => p.CodigoProduto).ToList();

    public IReadOnlyList<Movimentacao> Movimentacoes
    {
        get { lock (_lock) return _movimentacoes.ToList(); }
    }

    public Produto ObterProduto(int codigoProduto) =>
        _produtos.TryGetValue(codigoProduto, out var produto)
            ? produto
            : throw new EstoqueException($"Produto {codigoProduto} não encontrado.");

    /// <summary>
    /// Lança uma movimentação e devolve o registro criado,
    /// incluindo a quantidade final em estoque do produto.
    /// </summary>
    public Movimentacao Lancar(int codigoProduto, TipoMovimentacao tipo, int quantidade, string descricao)
    {
        if (quantidade <= 0)
            throw new EstoqueException("A quantidade deve ser maior que zero.");

        if (string.IsNullOrWhiteSpace(descricao))
            throw new EstoqueException("Informe uma descrição para a movimentação.");

        if (!Enum.IsDefined(tipo))
            throw new EstoqueException("Tipo de movimentação inválido.");

        lock (_lock)
        {
            var produto = ObterProduto(codigoProduto);
            var estoqueAnterior = produto.Estoque;

            var estoqueFinal = tipo == TipoMovimentacao.Entrada
                ? estoqueAnterior + quantidade
                : estoqueAnterior - quantidade;

            if (estoqueFinal < 0)
                throw new EstoqueException(
                    $"Estoque insuficiente para \"{produto.DescricaoProduto}\": disponível {estoqueAnterior}, solicitado {quantidade}.");

            produto.Estoque = estoqueFinal;

            var movimentacao = new Movimentacao(
                Id: ++_ultimoId,
                CodigoProduto: produto.CodigoProduto,
                Tipo: tipo,
                Quantidade: quantidade,
                Descricao: descricao.Trim(),
                EstoqueAnterior: estoqueAnterior,
                EstoqueFinal: estoqueFinal,
                DataHora: DateTime.Now);

            _movimentacoes.Add(movimentacao);
            return movimentacao;
        }
    }
}
