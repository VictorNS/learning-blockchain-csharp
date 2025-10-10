using LearningBlockchain.Models;
using LearningBlockchain.Services;

var proofOfWorkSettings = new ProofOfWorkSettings(1, 1000000, 10, 10000);
var miningService = new MiningService(proofOfWorkSettings);
var blockchain = new Blockchain(proofOfWorkSettings, miningService);

while (true)
{
	Console.WriteLine("\n=== Learning Blockchain ===");
	Console.WriteLine("1. Add Block");
	Console.WriteLine("2. Show Blockchain");
	Console.WriteLine("3. Validate Chain");
	Console.WriteLine("4. Exit");
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
				await foreach (var block in blockchain.ValidateBlocks())
					Console.WriteLine($"{block.Index,3}|{block.TimestampUnixMs}|{block.PreviousHash}|{block.Hash}|{block.Difficulty,3}|{block.Nonce,3}|{block.Data}");
			}
			catch (Exception ex)
			{
				Console.WriteLine($"Error validating blockchain: {ex.Message}");
			}
			break;

		case "4":
			Console.WriteLine("Goodbye!");
			return;

		default:
			Console.WriteLine("Invalid option. Try again.");
			break;
	}
}
