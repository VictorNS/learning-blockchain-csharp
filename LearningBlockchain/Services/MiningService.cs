using LearningBlockchain.Models;

namespace LearningBlockchain.Services;

public interface IMiningService
{
	bool IsValidBlock(Block block);
	Block MineBlock(Block block);
}

public class MiningService : IMiningService
{
	private readonly ProofOfWorkSettings _settings;
	private readonly IBlockHashService _blockHashService;

	public MiningService(ProofOfWorkSettings settings, IBlockHashService blockHashService)
	{
		_settings = settings;
		_blockHashService = blockHashService;
	}

	public Block MineBlock(Block block)
	{
		var requiredPrefix = new string('0', (int)block.Difficulty);

		for (ulong nonce = 0; nonce < _settings.MaxNonceAttempts; nonce++)
		{
			var hash = _blockHashService.ComputeBlockHash(block, nonce);

			if (hash.StartsWith(requiredPrefix, StringComparison.Ordinal))
				return block.GetMinedBlock(hash, nonce);
		}

		throw new InvalidOperationException("Failed to mine block: nonce space exhausted.");
	}

	public bool IsValidBlock(Block block)
	{
		var hash = _blockHashService.ComputeBlockHash(block, block.Nonce);
		return hash == block.Hash && hash.StartsWith(new string('0', (int)block.Difficulty), StringComparison.Ordinal);
	}
}
