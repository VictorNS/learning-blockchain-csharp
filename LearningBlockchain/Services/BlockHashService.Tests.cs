using System.Text;
using LearningBlockchain.Models;
using Xunit;

namespace LearningBlockchain.Services;

public class BlockHashServiceTests
{
	[Fact]
	public void ComputeBlockHash_SensitiveToDataChanges()
	{
		// Arrange
		var timestampUnixMs = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
		var block1 = new Block(0, timestampUnixMs, "0", "0001", 0, "", 0);
		var block2 = new Block(0, timestampUnixMs, "0", "0002", 0, "", 0);

		// Act
		var hash1 = BlockHashService.ComputeBlockHash(block1);
		var hash2 = BlockHashService.ComputeBlockHash(block2);

		// Assert
		Assert.NotEqual(hash1, hash2);
	}
}
