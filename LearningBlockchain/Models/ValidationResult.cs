namespace LearningBlockchain.Models;

public record ValidationResult(bool IsValid, Block Block)
{
	public static ValidationResult Success(Block block) => new(true, block);
	public static ValidationResult Failure(Block block) => new(false, block);
}
