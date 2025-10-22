namespace LearningBlockchain.Models;

public record ValidationResult(bool IsValid, Block Block, string Message)
{
	public static ValidationResult Success(Block block) => new(true, block, "");
	public static ValidationResult Failure(Block block, string message) => new(false, block, message);
}
