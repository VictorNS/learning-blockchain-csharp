using LearningBlockchain.Services;

namespace LearningBlockchain.Models;

internal class Blockchain
{
	private readonly ProofOfWorkSettings _settings;
	private readonly MiningService _miningService;
	private List<Block> Chain { get; set; } = [];

	public Blockchain(ProofOfWorkSettings proofOfWorkSettings, MiningService miningService)
	{
		_settings = proofOfWorkSettings ?? throw new ArgumentNullException(nameof(proofOfWorkSettings));
		_miningService = miningService ?? throw new ArgumentNullException(nameof(miningService));
		AddGenesisBlock();
	}

	private void AddGenesisBlock()
	{
		var block = new Block(0, DateTimeOffset.UtcNow.ToUnixTimeMilliseconds(), "0", "Genesis Block", 0, "", 0);
		Chain.Add(_miningService.MineBlock(block));
	}

	public void AddBlock(string data)
	{
		var block = new Block(Chain.Count, DateTimeOffset.UtcNow.ToUnixTimeMilliseconds(), Chain[^1].Hash, data, _settings.Difficulty, "", 0);
		Chain.Add(_miningService.MineBlock(block));
	}

	public IReadOnlyList<Block> GetBlocks()
	{
		return Chain.AsReadOnly();
	}

	public async IAsyncEnumerable<Block> ValidateBlocks()
	{
		for (int i = 0; i < Chain.Count; i++)
		{
			await Task.Yield();
			var block = Chain[i];

			if ( _miningService.IsValidBlock(block))
				yield return block;
			else
				throw new InvalidOperationException($"Block {i} has invalid hash.");
		}
	}
}
