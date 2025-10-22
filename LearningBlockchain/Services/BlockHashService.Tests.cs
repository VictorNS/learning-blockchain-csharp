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
}
