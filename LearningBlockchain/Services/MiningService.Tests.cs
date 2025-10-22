using LearningBlockchain.Models;
using NSubstitute;
using Xunit;

namespace LearningBlockchain.Services;

public class MiningServiceTests
{
	private readonly ProofOfWorkSettings _defaultSettings = new(0, 0, 0, 0);
	private readonly Block _defaultBlock = new(1, DateTimeOffset.UtcNow.ToUnixTimeMilliseconds(), "0", "Text", 0, "", 0, "");

	[Fact]
	public void MineBlock_FindsValidNonceForDifficulty1()
	{
		// Arrange
		var blockHashService = Substitute.For<IBlockHashService>();

		blockHashService.ComputeBlockHash(Arg.Any<Block>(), 0)
			.Returns("1abcdef");
		blockHashService.ComputeBlockHash(Arg.Any<Block>(), 1) // Simulate a hash that starts with '0' when nonce is 1
			.Returns("0abcdef");

		var service = new MiningService(
			_defaultSettings with { MaxNonceAttempts = 2 },
			blockHashService);

		// Act
		var actual = service.MineBlock(_defaultBlock with { Difficulty = 1 });

		// Assert
		Assert.Equal(1UL, actual.Nonce);
		Assert.Equal("0abcdef", actual.Hash);
		blockHashService.Received().ComputeBlockHash(Arg.Any<Block>(), 0);
		blockHashService.Received().ComputeBlockHash(Arg.Any<Block>(), 1);
		blockHashService.DidNotReceive().ComputeBlockHash(Arg.Any<Block>(), 2);
	}

	[Fact]
	public void MineBlock_ThrowsExceptionWhenMaxNonceAttemptsExceeded()
	{
		// Arrange
		var blockHashService = Substitute.For<IBlockHashService>();

		blockHashService.ComputeBlockHash(Arg.Any<Block>(), 0)
			.Returns("1abcdef");
		blockHashService.ComputeBlockHash(Arg.Any<Block>(), 1)
			.Returns("2abcdef");

		var service = new MiningService(
			_defaultSettings with { MaxNonceAttempts = 2 },
			blockHashService);

		// Act & Assert
		Assert.Throws<InvalidOperationException>(() => service.MineBlock(_defaultBlock with { Difficulty = 1 }));
		blockHashService.Received().ComputeBlockHash(Arg.Any<Block>(), 0);
		blockHashService.Received().ComputeBlockHash(Arg.Any<Block>(), 1);
		blockHashService.DidNotReceive().ComputeBlockHash(Arg.Any<Block>(), 2);
	}

	[Fact]
	public void IsValidBlock_ReturnsTrueForValidBlock()
	{
		// Arrange
		var blockHashService = Substitute.For<IBlockHashService>();

		blockHashService.ComputeBlockHash(Arg.Any<Block>(), 11)
			.Returns("000abcdef");

		var service = new MiningService(
			_defaultSettings with { MaxNonceAttempts = 1 },
			blockHashService);

		// Act
		var actual = service.VerifyBlock(_defaultBlock with { Hash = "000abcdef", Difficulty = 3, Nonce = 11 });

		// Assert
		Assert.True(actual);
		blockHashService.Received(1).ComputeBlockHash(Arg.Any<Block>(), 11);
	}

	[Fact]
	public void IsValidBlock_ReturnsFalseForInvalidBlock()
	{
		// Arrange
		var blockHashService = Substitute.For<IBlockHashService>();

		blockHashService.ComputeBlockHash(Arg.Any<Block>(), 11)
			.Returns("000abcdef");

		var service = new MiningService(
			_defaultSettings with { MaxNonceAttempts = 1 },
			blockHashService);

		// Act
		var actual = service.VerifyBlock(_defaultBlock with { Hash = "000abracadabra", Difficulty = 3, Nonce = 11 });

		// Assert
		Assert.False(actual);
		blockHashService.Received(1).ComputeBlockHash(Arg.Any<Block>(), 11);
	}
}
