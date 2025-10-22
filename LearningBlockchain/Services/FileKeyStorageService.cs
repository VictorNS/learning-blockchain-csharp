using System.Security.Cryptography;

namespace LearningBlockchain.Services;

public interface IKeyStorageService
{
	RSA LoadOrGenerateKey();
	string GetPublicKey();
}

public class FileKeyStorageService : IKeyStorageService, IDisposable
{
	private const string KeyFileName = "blockchain_private_key.xml";
	private readonly string _appFolder;
	private readonly string _keyPath;
	private RSA? _rsa;
	private bool disposedValue;

	public FileKeyStorageService(string appFolder)
	{
		_appFolder = appFolder;
		_keyPath = Path.Combine(_appFolder, KeyFileName);
	}

	public RSA LoadOrGenerateKey()
	{
		if (_rsa != null)
			return _rsa;

		Directory.CreateDirectory(_appFolder);
		_rsa = ReadFile();
		_rsa ??= GenerateAndSaveKey();

		if (_rsa == null)
			throw new InvalidOperationException("Failed to load or generate RSA key.");

		return _rsa;
	}

	public string GetPublicKey()
	{
		if (_rsa == null)
			throw new InvalidOperationException("RSA key not loaded.");

		var publicKeyBytes = _rsa.ExportRSAPublicKey();
		return Convert.ToBase64String(publicKeyBytes);
	}

	private RSA? ReadFile()
	{
		RSA? rsa = null;

		try
		{
			if (File.Exists(_keyPath))
			{
				var keyXml = File.ReadAllText(_keyPath);
				rsa = RSA.Create();
				rsa.FromXmlString(keyXml);
				return rsa;
			}
		}
		catch
		{
			rsa?.Dispose();
		}

		return null;
	}

	private RSA? GenerateAndSaveKey()
	{
		RSA? rsa = null;

		try
		{
			rsa = RSA.Create(2048);
			var keyXml = rsa.ToXmlString(true);
			File.WriteAllText(_keyPath, keyXml);
			return rsa;
		}
		catch
		{
			rsa?.Dispose();
		}

		return null;
	}

	#region IDisposable Support
	protected virtual void Dispose(bool disposing)
	{
		if (!disposedValue)
		{
			if (disposing)
			{
				if (_rsa != null)
				{
					_rsa.Dispose();
					_rsa = null;
				}
			}

			disposedValue = true;
		}
	}

	public void Dispose()
	{
		// Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
		Dispose(disposing: true);
		GC.SuppressFinalize(this);
	}
	#endregion IDisposable Support
}
