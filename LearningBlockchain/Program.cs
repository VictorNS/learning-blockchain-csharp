using LearningBlockchain.Models;
using LearningBlockchain.Services;

var appDataFolder = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
var appFolder = Path.Combine(appDataFolder, "LearningBlockchain");
var proofOfWorkSettings = new ProofOfWorkSettings(1, 1000000, 10, 10000);

var fileKeyStorageService= new FileKeyStorageService(appFolder);
var blockSigner = new BlockSigner(fileKeyStorageService);
var blockHashService = new BlockHashService();
var miningService = new MiningService(proofOfWorkSettings, blockHashService);
var blockchain = new Blockchain(proofOfWorkSettings, miningService, blockSigner);

while (true)
{
	Console.WriteLine("\n=== Learning Blockchain ===");
	Console.WriteLine("1. Add Block");
	Console.WriteLine("2. Show Blockchain");
	Console.WriteLine("3. Validate Entire Chain");
	Console.WriteLine("4. Validate Blocks Only");
	Console.WriteLine("5. Validate Chain Integrity");
	Console.WriteLine("6. Exit");
	Console.Write("Choose option: ");

	var choice = Console.ReadLine();

	switch (choice)
	{
		case "1":
			try
			{
				Console.Write("Enter block data: ");
				var data = Console.ReadLine();
				if (string.IsNullOrWhiteSpace(data))
					continue;

				blockchain.AddBlock(data);
				Console.WriteLine("Block added and mined successfully!");
			}
			catch (Exception ex)
			{
				Console.WriteLine($"Error adding block: {ex.Message}");
				throw;
			}
			break;

		case "2":
			try
			{
				foreach (var block in blockchain.GetBlocks())
					Console.WriteLine($"{block.Index,3}|{block.TimestampUnixMs}|{block.PreviousHash}|{block.Hash}|{block.Difficulty,3}|{block.Nonce,3}|{block.Data}");
			}
			catch (Exception ex)
			{
				Console.WriteLine($"Error displaying blockchain: {ex.Message}");
				throw;
			}
			break;

		case "3":
			try
			{
				var hasError = false;

				await foreach (var result in blockchain.ValidateEntireChain())
				{
					var block = result.Block;
					Console.WriteLine($"{block.Index,3}|{block.TimestampUnixMs}|{block.PreviousHash}|{block.Hash}|{block.Difficulty,3}|{block.Nonce,3}|{block.Data}");

					if (!result.IsValid)
					{
						hasError = true;
						Console.WriteLine($"✗ Chain integrity failed at block {block.Index}: {result.Message}");
					}
				}

				if (!hasError)
				{
					Console.WriteLine("✓ All blocks are valid.");
				}
			}
			catch (Exception ex)
			{
				Console.WriteLine($"Error validating blockchain: {ex.Message}");
			}
			break;

		case "4":
			try
			{
				var hasError = false;

				await foreach (var result in blockchain.ValidateBlocksIndividually())
				{
					var block = result.Block;
					Console.WriteLine($"{block.Index,3}|{block.TimestampUnixMs}|{block.PreviousHash}|{block.Hash}|{block.Difficulty,3}|{block.Nonce,3}|{block.Data}");

					if (!result.IsValid)
					{
						hasError = true;
						Console.WriteLine($"✗ Block {block.Index} is invalid!");
					}
				}

				if (!hasError)
				{
					Console.WriteLine("✓ All blocks are valid.");
				}
			}
			catch (Exception ex)
			{
				Console.WriteLine($"Error validating blockchain: {ex.Message}");
			}
			break;

		case "5":
			try
			{
				var result = blockchain.ValidateChainIntegrity();

				if (result.IsValid)
				{
					Console.WriteLine("✓ Chain integrity is valid - all blocks are properly linked.");
				}
				else
				{
					var block = result.Block;
					Console.WriteLine($"✗ Chain integrity failed at block {block.Index}: {result.Message}");
					Console.WriteLine($"Block: {block.Index,3}|{block.TimestampUnixMs}|{block.PreviousHash}|{block.Hash}|{block.Difficulty,3}|{block.Nonce,3}|{block.Data}");
				}
			}
			catch (Exception ex)
			{
				Console.WriteLine($"Error validating blockchain: {ex.Message}");
			}
			break;

		case "6":
			Console.WriteLine("Goodbye!");
			return;

		default:
			Console.WriteLine("Invalid option. Try again.");
			break;
	}
}
