using BudgetTracker.Dtos.Transactions;
using BudgetTracker.Services;

namespace BudgetTracker.Endpoints;

public static class TransactionEndpoints
{
    public static void MapTransactionEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/transactions");

        group.MapGet("/", async (ITransactionService transactionService) =>
        {
            var transactions = await transactionService.ListAsync();
            var dtos = transactions.Select(p => new TransactionResponseDto(
                p.Id,
                p.UserId,
                p.Description,
                p.Amount,
                p.Type,
                p.Timestamp,
                p.Date
            ));
            return TypedResults.Ok(dtos);
        }).Produces(200);

        group.MapPost("/", async (CreateTransactionDto dto, ITransactionService transactionService, HttpContext context) =>
        {
            var transaction = await transactionService.CreateAsync(dto.UserId, dto.Description, dto.Amount, dto.Type);
            var transactionDto = new TransactionResponseDto(
                transaction.Id,
                transaction.UserId,
                transaction.Description,
                transaction.Amount,
                transaction.Type,
                transaction.Timestamp,
                transaction.Date
            );

            var location = $"{context.Request.Scheme}://{context.Request.Host}/transactions/{transaction.Id}";
            return Results.Created(location, transactionDto);
        }).Produces(200);

        group.MapPatch("/{id:guid}", async (Guid id, UpdateTransactionDto dto, ITransactionService transactionService) =>
        {
            var transaction = await transactionService.UpdateAsync(id, dto.Description, dto.Amount, dto.Type);
            if (transaction is null) return Results.NotFound();

            var transactionDto = new TransactionResponseDto(
                transaction.Id,
                transaction.UserId,
                transaction.Description,
                transaction.Amount,
                transaction.Type,
                transaction.Timestamp,
                transaction.Date
            );
            return TypedResults.Ok(transactionDto);
        }).Produces(200);

        group.MapGet("/{id:guid}", async (Guid id, ITransactionService transactionService) =>
        {
            var transaction = await transactionService.GetAsync(id);
            if (transaction is null) return Results.NotFound();

            var dto = new TransactionResponseDto(
                transaction.Id,
                transaction.UserId,
                transaction.Description,
                transaction.Amount,
                transaction.Type,
                transaction.Timestamp,
                transaction.Date
            );
            return TypedResults.Ok(dto);
        }).Produces(200);

        group.MapDelete("/{id:guid}", async (Guid id, ITransactionService transactionService) =>
        {
            var deleted = await transactionService.DeleteAsync(id);
            return deleted ? Results.NoContent() : Results.NotFound();
        }).Produces(200);
    }
}