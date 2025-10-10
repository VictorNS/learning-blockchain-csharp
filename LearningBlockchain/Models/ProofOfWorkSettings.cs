namespace LearningBlockchain.Models;

public record ProofOfWorkSettings(uint Difficulty, uint MaxNonceAttempts, uint RetargetInterval, uint TargetBlockTimeMs);
