using System.Text;
using LearningBlockchain.Models;

namespace LearningBlockchain.Services;

public interface IBlockHashService
{
	string ComputeBlockHash(Block block);
	string ComputeBlockHash(Block block, ulong nonce);
}

public class BlockHashService : IBlockHashService
{
	public string ComputeBlockHash(Block block) => ComputeBlockHash(block, block.Nonce);

	public string ComputeBlockHash(Block block, ulong nonce)
	{
		var canonical = $"{block.Index}|{block.TimestampUnixMs}|{block.PreviousHash}|{block.Data}|{block.Difficulty}|{nonce}";
		var bytes = Encoding.UTF8.GetBytes(canonical);
		var hashBytes = System.Security.Cryptography.SHA256.HashData(bytes);
		return ToHexString(hashBytes);
	}

	/// <summary>
	/// Converts byte array to lowercase hexadecimal string.
	/// Lowercase hex is the standard format for blockchain hashes.
	/// </summary>
	private static string ToHexString(byte[] hashBytes)
	{
		var sb = new StringBuilder(hashBytes.Length * 2);

		for (int i = 0; i < hashBytes.Length; i++)
		{
			byte b = hashBytes[i];
			// Using ToString("x2") gives lowercase hex and pads with leading zero.
			sb.Append(b.ToString("x2"));
		}

		return sb.ToString();
	}
}
