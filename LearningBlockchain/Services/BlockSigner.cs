using System.Security.Cryptography;
using System.Text;
using LearningBlockchain.Models;

namespace LearningBlockchain.Services;

public interface IBlockSigner
{
	string SignBlock(Block block);
	bool VerifyBlock(Block block);
}

public class BlockSigner : IBlockSigner
{
	private readonly IKeyStorageService _keyStorage;

	public BlockSigner(IKeyStorageService keyStorage)
	{
		_keyStorage = keyStorage;
	}

	public string SignBlock(Block block)
	{
		var rsa = _keyStorage.LoadOrGenerateKey();
		var blockData = Encoding.UTF8.GetBytes(block.GetCanonical());
		var signature = rsa.SignData(blockData, HashAlgorithmName.SHA256, RSASignaturePadding.Pkcs1);
		return Convert.ToBase64String(signature);
	}

	public bool VerifyBlock(Block block)
	{
		var rsa = _keyStorage.LoadOrGenerateKey();
		var blockData = Encoding.UTF8.GetBytes(block.GetCanonical());
		var signatureBytes = Convert.FromBase64String(block.Signature);
		return rsa.VerifyData(blockData, signatureBytes, HashAlgorithmName.SHA256, RSASignaturePadding.Pkcs1);
	}
}
