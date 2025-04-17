namespace Fetch.ReceiptProcessor.Helper;

public interface IReceiptQueue
{
    Task EnqueueAsync(Guid receiptId);
    
    Task<Guid> DequeueAsync(CancellationToken cancellationToken);
}