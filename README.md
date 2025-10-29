# Learning Blockchain in C#

A simple educational project to explore how blockchain works under the hood.

Includes basic block structure, cryptographic hashing, proof-of-work consensus, digital signatures, and chain validation.

> **Note:** This is a learning project designed for educational purposes. Not intended for production use.

## ✨ Features

- **🔗 Block Creation & Linking** - Complete block structure with proper chain linking
- **🔐 SHA-256 Cryptographic Hashing** - Secure block hashing with integrity verification
- **⛏️ Proof-of-Work Mining** - Configurable difficulty with nonce-based mining algorithm
- **✍️ Digital Signatures** - RSA-based block signing and verification
- **✅ Multi-Level Validation** - Individual block validation, chain integrity, and complete chain verification
- **💾 Persistent Storage** - JSON file-based blockchain persistence
- **🖥️ Interactive Console** - Full-featured console interface for blockchain operations
- **🧪 Comprehensive Testing** - Unit tests for all core components

## 🚀 Quick Start

### Prerequisites
- [.NET 8.0 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)

### Installation & Running
```bash
git clone https://github.com/VictorNS/learning-blockchain-csharp.git
cd learning-blockchain-csharp/LearningBlockchain
dotnet run
```

### Usage Example
```
=== Learning Blockchain ===
1. Add Block
2. Show Blockchain
3. Validate Entire Chain
4. Validate Blocks Only
5. Validate Chain Integrity
6. Exit
Choose option: 1
Enter block data: Hello, Blockchain!
Block added and mined successfully!
```

## 🏗️ Project Structure

```
LearningBlockchain/
├── Models/
│   ├── Block.cs                     # Block data structure
│   ├── Blockchain.cs                # Main blockchain implementation
│   │   └─ Blockchain.Tests.cs
│   ├── ProofOfWorkSettings.cs       # Mining configuration
│   └── ValidationResult.cs          # Validation result model
├── Services/
│   ├── BlockHashService.cs          # SHA-256 hashing service
│   │   └─ BlockHashService.Tests.cs
│   ├── BlockSigner.cs               # RSA digital signature service
│   │   └─ BlockSigner.Tests.cs
│   ├── FileBlockchainStorage.cs     # JSON persistence service
│   ├── FileKeyStorageService.cs     # RSA key management
│   ├── MiningService.cs             # Proof-of-work mining service
│   │   └─ MiningService.Tests.cs
└── Program.cs                       # Console application entry point
```

### 🧪 Testing Approach

This project uses an **embedded testing structure** where test files are co-located with their corresponding implementation files:

- **Embedded Tests**: Each component has its tests in a separate `.Tests.cs` file in the same directory
- **Conditional Compilation**: Test files are only compiled in Debug configuration
- **Visual Studio Integration**: Tests appear nested under their parent classes in Solution Explorer
- **Production Builds**: Release builds automatically exclude all test code

**Project Configuration:**
```xml
<!-- Exclude test files from Release builds -->
<ItemGroup Condition=" '$(Configuration)' != 'Debug' ">
    <Compile Remove="**/*Tests.cs" />
</ItemGroup>

<!-- Nest test files under their parent classes in IDE -->
<ItemGroup>
    <Compile Update="**\*.Tests.cs">
        <DependentUpon>$([System.String]::Copy(%(Filename)).Replace('.Tests', '.cs'))</DependentUpon>
    </Compile>
</ItemGroup>
```

## 🔍 Key Learning Concepts Demonstrated

1. **Immutability** - How changing any block data invalidates the hash
2. **Chain Integrity** - How blocks are cryptographically linked
3. **Consensus Mechanism** - Basic proof-of-work implementation
4. **Digital Authentication** - RSA signatures for block authenticity
5. **Data Persistence** - Blockchain state management
6. **Validation Strategies** - Different approaches to ensuring chain validity

## 🛠️ Configuration

The blockchain uses configurable proof-of-work settings:
- **Initial Difficulty**: 1 (number of leading zeros)
- **Max Iterations**: 1,000,000 per mining attempt
- **Difficulty Adjustment**: Every 10 blocks
- **Max Mining Time**: 10 seconds per block

## 🚧 Educational Limitations

This implementation focuses on core blockchain concepts and intentionally omits:
- Network/distributed functionality
- Transaction pools and UTXO model
- Advanced consensus algorithms (PoS, etc.)
- Merkle trees for transaction verification
- Production-grade security measures

## 📝 License

This project is for educational purposes. Feel free to use and modify for learning.

---

**Built with ❤️ for learning blockchain fundamentals**
