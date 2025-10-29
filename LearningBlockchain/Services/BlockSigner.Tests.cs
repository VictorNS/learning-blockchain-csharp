using System.Security.Cryptography;
using System.Text;
using LearningBlockchain.Models;
using NSubstitute;
using Xunit;

namespace LearningBlockchain.Services;

public class BlockSignerTests
{
	private readonly Block _defaultBlock = new(1, DateTimeOffset.UtcNow.ToUnixTimeMilliseconds(), "0", "Text", 0, "", 0, "");
	private RSA _rsa;
	private readonly IKeyStorageService _keyStorageService = Substitute.For<IKeyStorageService>();

	public BlockSignerTests()
	{
		_rsa = RSA.Create(2048);
		_keyStorageService.LoadOrGenerateKey().Returns(_rsa);
	}

	[Fact]
	public void VerifyBlock_ReturnsTrueForValidSignature()
	{
		// Arrange
		var block = _defaultBlock with { Difficulty = 1, Nonce = 1 };
		var blockData = Encoding.UTF8.GetBytes(block.GetCanonical());
		var signature = _rsa.SignData(blockData, HashAlgorithmName.SHA256, RSASignaturePadding.Pkcs1);
		var signedBlock = block with { Signature = Convert.ToBase64String(signature) };

		var service = new BlockSigner(_keyStorageService);

		// Act
		var actual = service.VerifyBlock(signedBlock);

		// Assert
		Assert.True(actual);
	}

	[Fact]
	public void VerifyBlock_ReturnsFalseForInvalidSignature()
	{
		// Arrange
		var block = _defaultBlock with { Difficulty = 1, Nonce = 1, Signature = "YWJjZGVmZ2g=" };

		var service = new BlockSigner(_keyStorageService);

		// Act
		var actual = service.VerifyBlock(block);

		// Assert
		Assert.False(actual);
	}

	[Fact]
	public void SignAndVerify_RoundTripTest()
	{
		// Arrange
		var block = _defaultBlock with { Difficulty = 1, Nonce = 1, Hash = "0abcdef" };

		var service = new BlockSigner(_keyStorageService);

		// Act
		var signedBlock = block with { Signature = service.SignBlock(block) };
		var actual = service.VerifyBlock(signedBlock);

		// Assert
		Assert.True(actual);
	}

	[Fact]
	public void VerifyBlock_ReturnsFalseForModifiedBlock()
	{
		// Arrange
		var block = _defaultBlock with { Difficulty = 1, Nonce = 1, Hash = "0abcdef" };

		var service = new BlockSigner(_keyStorageService);

		// Act
		var signedBlock = block with { Signature = service.SignBlock(block) };
		var changedBlock = block with { Nonce = 0 };
		var actual = service.VerifyBlock(changedBlock);

		// Assert
		Assert.False(actual);
	}
}
