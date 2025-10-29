using System.Text.Json;
using LearningBlockchain.Models;

namespace LearningBlockchain.Services;

public interface IBlockchainStorage
{
	List<Block> Load();
	void Save(IReadOnlyList<Block> blocks);
}

public class FileBlockchainStorage : IBlockchainStorage
{
	private const string BlockchainName = "blockchain.json";
	private readonly JsonSerializerOptions _jsonSerializerOptions = new() { WriteIndented = true };
	private readonly string _appFolder;
	private readonly string _filePath;

	public FileBlockchainStorage(string appFolder)
	{
		_appFolder = appFolder;
		_filePath = Path.Combine(_appFolder, BlockchainName);
	}

	public List<Block> Load()
	{
		try
		{
			if (File.Exists(_filePath))
			{
				var file = File.ReadAllText(_filePath);
				var blocks = JsonSerializer.Deserialize<List<Block>>(file);
				return blocks ?? [];
			}
		}
		catch (Exception ex)
		{
			Console.WriteLine($"Error loading blockchain from file: {ex.Message}");
		}

		return [];
	}

	public void Save(IReadOnlyList<Block> blocks)
	{
		try
		{
			Directory.CreateDirectory(_appFolder);
			var json = JsonSerializer.Serialize(blocks, _jsonSerializerOptions);
			File.WriteAllText(_filePath, json);
		}
		catch (Exception ex)
		{
			Console.WriteLine($"Error saving blockchain to file: {ex.Message}");
		}
	}
}
