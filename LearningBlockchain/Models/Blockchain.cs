using LearningBlockchain.Services;

namespace LearningBlockchain.Models;

internal class Blockchain
{
	private readonly ProofOfWorkSettings _settings;
	private readonly IMiningService _miningService;
	private readonly IBlockSigner _blockSigner;
	private List<Block> Chain { get; set; } = [];

	public Blockchain(ProofOfWorkSettings proofOfWorkSettings, IMiningService miningService, IBlockSigner blockSigner)
	{
		_settings = proofOfWorkSettings;
		_miningService = miningService;
		_blockSigner = blockSigner;
		AddGenesisBlock();
	}

	private void AddGenesisBlock()
	{
		var block = new Block(0, DateTimeOffset.UtcNow.ToUnixTimeMilliseconds(), "0", "Genesis Block", 0, "", 0, "");
		Chain.Add(_miningService.MineBlock(block));
	}

	public void AddBlock(string data)
	{
		var block = new Block(Chain.Count, DateTimeOffset.UtcNow.ToUnixTimeMilliseconds(), Chain[^1].Hash, data, _settings.Difficulty, "", 0, "");
		var minedBlock = _miningService.MineBlock(block);
		var signedBlock = minedBlock with { Signature = _blockSigner.SignBlock(minedBlock) };
		Chain.Add(signedBlock);
	}

	public IReadOnlyList<Block> GetBlocks()
	{
		return Chain.AsReadOnly();
	}

	public async IAsyncEnumerable<ValidationResult> ValidateEntireChain()
	{
		string previousHash = "0";

		for (int i = 0; i < Chain.Count; i++)
		{
			var block = Chain[i];

			if (block.Index != i || block.PreviousHash != previousHash)
			{
				await Task.Yield();
				yield return ValidationResult.Failure(block, block.Index != i ?
					$"Block index should be {i}, but was {block.Index}" :
					$"Block previousHash should be '{previousHash}', but was '{block.PreviousHash}'");
				break;
			}

			previousHash = block.Hash;

			if (!_miningService.VerifyBlock(block))
			{
				await Task.Yield();
				yield return ValidationResult.Failure(block, "Block hash is invalid.");
				break;
			}
			else if (block.Index > 0 && !_blockSigner.VerifyBlock(block))
			{
				await Task.Yield();
				yield return ValidationResult.Failure(block, "Signature hash is invalid.");
				break;
			}
			else
			{
				await Task.Yield();
				yield return ValidationResult.Success(block);
			}
		}
	}

	public async IAsyncEnumerable<ValidationResult> ValidateBlocksIndividually()
	{
		for (int i = 0; i < Chain.Count; i++)
		{
			var block = Chain[i];

			if (!_miningService.VerifyBlock(block))
			{
				await Task.Yield();
				yield return ValidationResult.Failure(block, "Block hash is invalid.");
				break;
			}
			else if (block.Index > 0 && !_blockSigner.VerifyBlock(block))
			{
				await Task.Yield();
				yield return ValidationResult.Failure(block, "Signature hash is invalid.");
				break;
			}
			else
			{
				await Task.Yield();
				yield return ValidationResult.Success(block);
			}
		}
	}

	public ValidationResult ValidateChainIntegrity()
	{
		var block = Chain[0];

		if (block.Index != 0 || block.PreviousHash != "0")
			return ValidationResult.Failure(block, block.Index != 0 ?
				$"Genesis block index should be 0, but was {block.Index}" :
				$"Genesis block previousHash should be '0', but was '{block.PreviousHash}'");

		var previousHash = block.Hash;

		for (int i = 1; i < Chain.Count; i++)
		{
			block = Chain[i];

			if (block.Index != i || block.PreviousHash != previousHash)
				return ValidationResult.Failure(block, block.Index != i ?
					$"Block index should be {i}, but was {block.Index}" :
					$"Block previousHash should be '{previousHash}', but was '{block.PreviousHash}'");

			previousHash = block.Hash;
		}

		return ValidationResult.Success(Chain[0]);
	}
}
