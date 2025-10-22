namespace LearningBlockchain.Models;

public record Block(int Index, long TimestampUnixMs, string PreviousHash, string Data, uint Difficulty, string Hash, ulong Nonce, string Signature)
{
	public Block GetMinedBlock(string hash, ulong nonce)
	{
		return this with { Hash = hash, Nonce = nonce };
	}

	public string GetCanonical() => GetCanonical(Nonce);

	public string GetCanonical(ulong nonce) => $"{Index}|{TimestampUnixMs}|{PreviousHash}|{Data}|{Difficulty}|{nonce}";
}
