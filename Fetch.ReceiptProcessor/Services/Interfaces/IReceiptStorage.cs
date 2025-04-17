using Fetch.ReceiptProcessor.Entities;
using Fetch.ReceiptProcessor.Models;

namespace Fetch.ReceiptProcessor.Services.Interfaces;

public interface IReceiptStorage
{
    void StoreReceipt(Receipts receipt);
    
    Receipts GetReceipts(Guid id);
    
    void UpdateReceipts(Receipts receipts);

    int GetPoints(Guid receiptId);
    
    void MarkFailed(Guid receiptId);
    
    
// HAShFunctions
    bool ContainsHash(string hash);
    Guid GetByHash(string hash);
    
    void StoreHash(string hash, Guid id);
}