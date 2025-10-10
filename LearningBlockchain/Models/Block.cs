namespace LearningBlockchain.Models;

public record Block(int Index, long TimestampUnixMs, string PreviousHash, string Data, uint Difficulty, string Hash, ulong Nonce)
{
	public Block GetMinedBlock(string hash, ulong nonce)
	{
		return this with { Hash = hash, Nonce = nonce };
	}
}
