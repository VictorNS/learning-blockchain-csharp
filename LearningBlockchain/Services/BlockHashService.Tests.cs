using LearningBlockchain.Models;
using Xunit;

namespace LearningBlockchain.Services;

public class BlockHashServiceTests
{
	[Fact]
	public void ComputeBlockHash_SensitiveToDataChanges()
	{
		// Arrange
		var service = new BlockHashService();
		var timestampUnixMs = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
		var block1 = new Block(0, timestampUnixMs, "0", "0001", 0, "", 0, "");
		var block2 = new Block(0, timestampUnixMs, "0", "0002", 0, "", 0, "");

		// Act
		var hash1 = service.ComputeBlockHash(block1);
		var hash2 = service.ComputeBlockHash(block2);

		// Assert
		Assert.NotEqual(hash1, hash2);
	}

	[Fact]
	public void ComputeBlockHash_BothOverloadsReturnSameResult()
	{
		// Arrange
		var service = new BlockHashService();
		var timestampUnixMs = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
		var block = new Block(0, timestampUnixMs, "0", "0001", 0, "", 0, "");

		// Act
		var hash1 = service.ComputeBlockHash(block);
		var hash2 = service.ComputeBlockHash(block, block.Nonce);

		// Assert
		Assert.Equal(hash1, hash2);
	}

	[Fact]
	public void ComputeBlockHash_ReturnsLowercaseHexString()
	{
		// Arrange
		var service = new BlockHashService();
		var timestampUnixMs = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
		var block = new Block(0, timestampUnixMs, "0", "test", 0, "", 0, "");

		// Act
		var hash = service.ComputeBlockHash(block);

		// Assert
		Assert.Equal(64, hash.Length); // SHA256 => 32 bytes => 64 hex chars
		Assert.True(hash.All(c => char.IsDigit(c) || (c >= 'a' && c <= 'f')));
		Assert.Contains(hash, c => char.IsDigit(c) || (c >= 'a' && c <= 'f'));
		Assert.DoesNotContain(hash, c => c >= 'A' && c <= 'F');
	}

	[Fact]
	public void ComputeBlockHash_SensitiveToNonceChanges()
	{
		// Arrange
		var service = new BlockHashService();
		var timestampUnixMs = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
		var block1 = new Block(0, timestampUnixMs, "0", "0001", 0, "", 0, "");
		var block2 = new Block(0, timestampUnixMs, "0", "0001", 0, "", 1, "");

		// Act
		var hash1 = service.ComputeBlockHash(block1);
		var hash2 = service.ComputeBlockHash(block2);

		// Assert
		Assert.NotEqual(hash1, hash2);
	}
}
