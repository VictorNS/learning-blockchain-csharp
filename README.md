# learning-blockchain-csharp
A simple educational project to explore how blockchain works under the hood.
Includes basic block structure, hashing, validation, and a minimal Proof-of-Work implementation.
Built in C# as a learning exercise — not for production use.

## Planned features

- Block creation and linking
- Chain validation
- Simple Proof-of-Work
- JSON file persistence
- Console interaction

## Learning Plan

- Define the data structure for a block and the blockchain.
- Link blocks through the PrevHash field.
- Implement SHA-256 hashing for a block.
- Add a Nonce field and a rule like "the hash must start with N zeros."
- Implement a simple search loop to find the valid hash.
- Implement basic message signing with RSA or ECDSA.
- Verify that the hash changes when any data changes.
- Add a method to validate the entire chain's integrity.
- Create a console menu or minimal API for adding blocks.
- Add a command to check the chain's validity.
- Save the chain to a file.
