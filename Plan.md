# Learning Plan

## Stage 1. Preparation

- Define the data structure for a block and the blockchain.
- Decide how the chain will be stored (in memory or as a JSON file).

## Stage 2. Block and Hashing

- Implement SHA-256 hashing for a block.
- Verify that the hash changes when any data changes.

## Stage 3. Chain and Validation

- Link blocks through the PrevHash field.
- Add a method to validate the entire chain's integrity.

## Stage 4. Proof-of-Work

- Add a Nonce field and a rule like "the hash must start with N zeros."
- Implement a simple search loop to find the valid hash.

## Stage 5. Interaction

- Create a console menu or minimal API for adding blocks.
- Add a command to check the chain's validity.

## Stage 6. Storage and Signatures

- Save the chain to a file.
- Optionally, implement basic message signing with RSA or ECDSA.
