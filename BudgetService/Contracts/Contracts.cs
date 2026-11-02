namespace BudgetService.Contracts
{
    // --- FAMILY ---
    public record CreateFamilyRequest(string Name);
    public record UpdateFamilyRequest(string Name);
    public record FamilyResponse(Guid Id, string Name, DateTime CreatedAt);

    // --- ACCOUNT ---
    public record CreateAccountRequest(Guid FamilyId, string Name, string Currency);
    public record UpdateAccountRequest(string Name, string Currency);
    public record AccountResponse(Guid Id, Guid FamilyId, string Name, string Currency, decimal Balance, bool IsClosed);

    // --- CATEGORY ---
    // Type: 1 - Expense, 2 - Income
    public record CreateCategoryRequest(Guid? FamilyId, string Name, int Type, Guid? ParentCategoryId);
    public record UpdateCategoryRequest(string Name, int Type);
    public record CategoryResponse(Guid Id, Guid? FamilyId, string Name, int Type, Guid? ParentCategoryId);

    // --- TRANSACTION ---
    public record CreateTransactionRequest(
        Guid AccountId,
        Guid CategoryId,
        decimal Amount,
        int Type, // 1 - Expense, 2 - Income
        DateTime Date,
        string? Comment
    );
    public record TransactionResponse(
        Guid Id,
        Guid AccountId,
        Guid CategoryId,
        decimal Amount,
        int Type,
        DateTime Date,
        string? Comment,
        DateTime CreatedAt
    );
}
