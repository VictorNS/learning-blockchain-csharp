using LearningBlockchain.Models;

namespace LearningBlockchain.Services;

internal class MiningService
{
	private readonly ProofOfWorkSettings _settings;

	public MiningService(ProofOfWorkSettings currentSettings)
	{
		_settings = currentSettings;
	}

	public Block MineBlock(Block block)
	{
		var requiredPrefix = new string('0', (int)block.Difficulty);

		for (ulong nonce = 0; nonce < _settings.MaxNonceAttempts; nonce++)
		{
			var hash = BlockHashService.ComputeBlockHash(block, nonce);

			if (hash.StartsWith(requiredPrefix, StringComparison.Ordinal))
				return block.GetMinedBlock(hash, nonce);
		}

		throw new InvalidOperationException("Failed to mine block: nonce space exhausted.");
	}

	public bool IsValidBlock(Block block)
	{
		var hash = BlockHashService.ComputeBlockHash(block, block.Nonce);
		return hash == block.Hash && hash.StartsWith(new string('0', (int)block.Difficulty), StringComparison.Ordinal);
	}
}
