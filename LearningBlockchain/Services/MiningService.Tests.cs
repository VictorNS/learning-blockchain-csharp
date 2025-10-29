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
	public void MineBlock_FindsValidNonceForDifficulty2()
	{
		// Arrange
		var blockHashService = Substitute.For<IBlockHashService>();

		blockHashService.ComputeBlockHash(Arg.Any<Block>(), 0)
			.Returns("11abcdef");
		blockHashService.ComputeBlockHash(Arg.Any<Block>(), 1) // Simulate a hash that starts with '0' when nonce is 1
			.Returns("01abcdef");
		blockHashService.ComputeBlockHash(Arg.Any<Block>(), 2) // Simulate a hash that starts with '00' when nonce is 2
			.Returns("00abcdef");

		var service = new MiningService(
			_defaultSettings with { MaxNonceAttempts = 3 },
			blockHashService);

		// Act
		var actual = service.MineBlock(_defaultBlock with { Difficulty = 2 });

		// Assert
		Assert.Equal(2UL, actual.Nonce);
		Assert.Equal("00abcdef", actual.Hash);
		blockHashService.Received().ComputeBlockHash(Arg.Any<Block>(), 0);
		blockHashService.Received().ComputeBlockHash(Arg.Any<Block>(), 1);
		blockHashService.Received().ComputeBlockHash(Arg.Any<Block>(), 2);
		blockHashService.DidNotReceive().ComputeBlockHash(Arg.Any<Block>(), 3);
	}

	[Fact]
	public void MineBlock_SucceedsWithMaxNonceAttemptsOne()
	{
		// Arrange
		var blockHashService = Substitute.For<IBlockHashService>();

		blockHashService.ComputeBlockHash(Arg.Any<Block>(), 0)
			.Returns("0abcdef");

		var service = new MiningService(
			_defaultSettings with { MaxNonceAttempts = 1 },
			blockHashService);

		// Act
		var actual = service.MineBlock(_defaultBlock with { Difficulty = 1 });

		// Assert
		Assert.Equal(0UL, actual.Nonce);
		Assert.Equal("0abcdef", actual.Hash);
		blockHashService.Received(1).ComputeBlockHash(Arg.Any<Block>(), 0);
		blockHashService.DidNotReceive().ComputeBlockHash(Arg.Any<Block>(), 1);
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
	public void VerifyBlock_ReturnsTrueForValidBlock()
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
	public void VerifyBlock_ReturnsFalseForInvalidBlock()
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

	[Fact]
	public void VerifyBlock_ReturnsTrueForDifficultyZero()
	{
		// Arrange
		var blockHashService = Substitute.For<IBlockHashService>();

		blockHashService.ComputeBlockHash(Arg.Any<Block>(), 11)
			.Returns("1abcdef");

		var service = new MiningService(
			_defaultSettings with { MaxNonceAttempts = 1 },
			blockHashService);

		// Act
		var actual = service.VerifyBlock(_defaultBlock with { Hash = "1abcdef", Difficulty = 0, Nonce = 11 });

		// Assert
		Assert.True(actual);
		blockHashService.Received(1).ComputeBlockHash(Arg.Any<Block>(), 11);
	}
}
