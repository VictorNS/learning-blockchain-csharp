using LearningBlockchain.Services;
using NSubstitute;
using Xunit;

namespace LearningBlockchain.Models;

public class BlockchainTests
{
	private readonly ProofOfWorkSettings _defaultSettings = new(0, 0, 0, 0);
	private readonly Block _defaultBlock = new(1, DateTimeOffset.UtcNow.ToUnixTimeMilliseconds(), "0", "Text", 0, "", 0, "");
	private readonly IBlockchainStorage _blockchainStorage = Substitute.For<IBlockchainStorage>();

	public BlockchainTests()
	{
		_blockchainStorage.Load().Returns([]);
	}

	[Fact]
	public async Task Constructor_CreatesGenesisBlock()
	{
		// Arrange
		var miningService = Substitute.For<IMiningService>();

		miningService.MineBlock(Arg.Any<Block>())
			.Returns(_defaultBlock with { Index = 0, Hash = "genesis_hash", Difficulty = 0, Nonce = 0 });

		var blockSigner = Substitute.For<IBlockSigner>();

		blockSigner.SignBlock(Arg.Any<Block>())
			.Returns("fake_signature");

		var service = new Blockchain(
			_defaultSettings with { MaxNonceAttempts = 0 },
			miningService,
			blockSigner,
			_blockchainStorage);
		await service.Initialize();

		// Act
		var actual = service.GetBlocks();
		var block = actual[0];

		// Assert
		Assert.Single(actual);
		Assert.Equal(0, block.Index);
		Assert.Equal("genesis_hash", block.Hash);
		Assert.Equal(0UL, block.Difficulty);
		Assert.Equal(0UL, block.Nonce);
		miningService.Received(1).MineBlock(Arg.Any<Block>());
	}

	[Fact]
	public async Task AddBlock_CreatesBlockWithCorrectPreviousHashAndIndex()
	{
		// Arrange
		var miningService = Substitute.For<IMiningService>();

		miningService.MineBlock(Arg.Is<Block>(b => b.Index == 0))
			.Returns(callInfo => callInfo.Arg<Block>() with { Hash = "genesis_hash", Nonce = 0 });
		miningService.MineBlock(Arg.Is<Block>(b => b.Index == 1))
			.Returns(callInfo => callInfo.Arg<Block>() with { Hash = "block1_hash", Nonce = 1 });
		miningService.MineBlock(Arg.Is<Block>(b => b.Index == 2))
			.Returns(callInfo => callInfo.Arg<Block>() with { Hash = "block2_hash", Nonce = 2 });

		var blockSigner = Substitute.For<IBlockSigner>();

		blockSigner.SignBlock(Arg.Any<Block>())
			.Returns("fake_signature");

		var service = new Blockchain(
			_defaultSettings with { Difficulty = 1, MaxNonceAttempts = 2 },
			miningService,
			blockSigner,
			_blockchainStorage);
		await service.Initialize();

		// Act
		service.AddBlock("block1");
		service.AddBlock("block2");
		var actual = service.GetBlocks();

		// Assert
		Assert.Collection(actual,
			block => {
				Assert.Equal(0, block.Index);
				Assert.Equal("0", block.PreviousHash);
				Assert.Equal("genesis_hash", block.Hash);
				Assert.Equal(0UL, block.Nonce);
			},
			block => {
				Assert.Equal(1, block.Index);
				Assert.Equal("genesis_hash", block.PreviousHash);
				Assert.Equal("block1_hash", block.Hash);
				Assert.Equal(1UL, block.Nonce);
			},
			block => {
				Assert.Equal(2, block.Index);
				Assert.Equal("block1_hash", block.PreviousHash);
				Assert.Equal("block2_hash", block.Hash);
				Assert.Equal(2UL, block.Nonce);
			}
		);
		miningService.Received(3).MineBlock(Arg.Any<Block>());
	}

	[Fact]
	public async Task ValidateEntireChain_ReturnsValidBlocksAsyncEnumerable()
	{
		// Arrange
		var miningService = Substitute.For<IMiningService>();

		miningService.MineBlock(Arg.Is<Block>(b => b.Index == 0))
			.Returns(callInfo => callInfo.Arg<Block>() with { Hash = "genesis_hash", Nonce = 0 });
		miningService.MineBlock(Arg.Is<Block>(b => b.Index == 1))
			.Returns(callInfo => callInfo.Arg<Block>() with { Hash = "block1_hash", Nonce = 1 });
		miningService.MineBlock(Arg.Is<Block>(b => b.Index == 2))
			.Returns(callInfo => callInfo.Arg<Block>() with { Hash = "block2_hash", Nonce = 2 });

		miningService.VerifyBlock(Arg.Is<Block>(b => b.Index == 0))
			.Returns(true);
		miningService.VerifyBlock(Arg.Is<Block>(b => b.Index == 1))
			.Returns(true);
		miningService.VerifyBlock(Arg.Is<Block>(b => b.Index == 2))
			.Returns(false);

		var blockSigner = Substitute.For<IBlockSigner>();

		blockSigner.SignBlock(Arg.Any<Block>())
			.Returns("fake_signature");
		blockSigner.VerifyBlock(Arg.Any<Block>())
			.Returns(true);

		var service = new Blockchain(
			_defaultSettings with { Difficulty = 1, MaxNonceAttempts = 2 },
			miningService,
			blockSigner,
			_blockchainStorage);
		await service.Initialize();

		// Act
		service.AddBlock("block1");
		service.AddBlock("block2");
		var actual = new List<ValidationResult>();
		await foreach (var result in service.ValidateEntireChain())
			actual.Add(result);

		// Assert
		Assert.Collection(actual,
			result => {
				Assert.Equal(0, result.Block.Index);
				Assert.True(result.IsValid);
			},
			result => {
				Assert.Equal(1, result.Block.Index);
				Assert.True(result.IsValid);
			},
			result => {
				Assert.Equal(2, result.Block.Index);
				Assert.False(result.IsValid);
			}
		);
		miningService.Received(3).MineBlock(Arg.Any<Block>());
		miningService.Received(3).VerifyBlock(Arg.Any<Block>());
	}

	[Fact]
	public async Task ValidateBlocksIndividually_ReturnsValidBlocksAsyncEnumerable()
	{
		// Arrange
		var miningService = Substitute.For<IMiningService>();

		miningService.MineBlock(Arg.Is<Block>(b => b.Index == 0))
			.Returns(callInfo => callInfo.Arg<Block>() with { Hash = "genesis_hash", Nonce = 0 });
		miningService.MineBlock(Arg.Is<Block>(b => b.Index == 1))
			.Returns(callInfo => callInfo.Arg<Block>() with { Hash = "block1_hash", Nonce = 1 });
		miningService.MineBlock(Arg.Is<Block>(b => b.Index == 2))
			.Returns(callInfo => callInfo.Arg<Block>() with { Hash = "block2_hash", Nonce = 2 });

		miningService.VerifyBlock(Arg.Is<Block>(b => b.Index == 0))
			.Returns(true);
		miningService.VerifyBlock(Arg.Is<Block>(b => b.Index == 1))
			.Returns(true);
		miningService.VerifyBlock(Arg.Is<Block>(b => b.Index == 2))
			.Returns(false);

		var blockSigner = Substitute.For<IBlockSigner>();

		blockSigner.SignBlock(Arg.Any<Block>())
			.Returns("fake_signature");
		blockSigner.VerifyBlock(Arg.Any<Block>())
			.Returns(true);

		var service = new Blockchain(
			_defaultSettings with { Difficulty = 1, MaxNonceAttempts = 2 },
			miningService,
			blockSigner,
			_blockchainStorage);
		await service.Initialize();

		// Act
		service.AddBlock("block1");
		service.AddBlock("block2");
		var actual = new List<ValidationResult>();
		await foreach (var result in service.ValidateBlocksIndividually())
			actual.Add(result);

		// Assert
		Assert.Collection(actual,
			result => {
				Assert.Equal(0, result.Block.Index);
				Assert.True(result.IsValid);
			},
			result => {
				Assert.Equal(1, result.Block.Index);
				Assert.True(result.IsValid);
			},
			result => {
				Assert.Equal(2, result.Block.Index);
				Assert.False(result.IsValid);
			}
		);
		miningService.Received(3).MineBlock(Arg.Any<Block>());
		miningService.Received(3).VerifyBlock(Arg.Any<Block>());
	}

	[Fact]
	public async Task ValidateChainIntegrity_ReturnsValidBlock()
	{
		// Arrange
		var miningService = Substitute.For<IMiningService>();

		miningService.MineBlock(Arg.Is<Block>(b => b.Index == 0))
			.Returns(callInfo => callInfo.Arg<Block>() with { Hash = "genesis_hash", Nonce = 0 });
		miningService.MineBlock(Arg.Is<Block>(b => b.Index == 1))
			.Returns(callInfo => callInfo.Arg<Block>() with { Hash = "block1_hash", Nonce = 1 });
		miningService.MineBlock(Arg.Is<Block>(b => b.Index == 2))
			.Returns(callInfo => callInfo.Arg<Block>() with { Hash = "block2_hash", Nonce = 2 });

		var blockSigner = Substitute.For<IBlockSigner>();

		blockSigner.SignBlock(Arg.Any<Block>())
			.Returns("fake_signature");

		var service = new Blockchain(
			_defaultSettings with { Difficulty = 1, MaxNonceAttempts = 2 },
			miningService,
			blockSigner,
			_blockchainStorage);
		await service.Initialize();

		// Act
		service.AddBlock("block1");
		service.AddBlock("block2");
		var actual = service.ValidateChainIntegrity();

		// Assert
		Assert.True(actual.IsValid);
		miningService.Received(3).MineBlock(Arg.Any<Block>());
		miningService.DidNotReceive().VerifyBlock(Arg.Any<Block>());
	}
}
