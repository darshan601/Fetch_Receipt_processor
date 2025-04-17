using System.Collections.Concurrent;
using Fetch.ReceiptProcessor.Entities;
using Fetch.ReceiptProcessor.Models;
using Fetch.ReceiptProcessor.Services.Interfaces;

namespace Fetch.ReceiptProcessor.Services;

public class ReceiptStorage: IReceiptStorage
{
    private readonly ILogger logger;

    private readonly ConcurrentDictionary<Guid, Receipts> receiptsStorage;
    
    private readonly ConcurrentDictionary<string, Guid> _hashToResponseMap;

    public ReceiptStorage(ILogger logger)
    {
        this.logger = logger;
        receiptsStorage = new ConcurrentDictionary<Guid, Receipts>();
        _hashToResponseMap = new ConcurrentDictionary<string, Guid>();
    }
    
    public void StoreReceipt(Receipts receipt)
    {
        logger.LogInformation("StoreReceipt called........");

        if (receiptsStorage.TryAdd(receipt.ReceiptId, receipt))
        {
            logger.LogInformation($"StoreReceipt added for Receipt ID {receipt.ReceiptId}....");
        }
        else
        {
            logger.LogWarning($"Error storing receipt: {receipt.ReceiptId}");
        }
    }

    public Receipts GetReceipts(Guid receiptId)
    {
       return receiptsStorage.TryGetValue(receiptId, out var receipt) ? receipt : null;
    }

    public void UpdateReceipts(Receipts receipts)
    {
        if (receiptsStorage.TryGetValue(receipts.ReceiptId, out var existing))
        {
            receiptsStorage.TryUpdate(receipts.ReceiptId, receipts, existing);
        }
        
    }

    public int GetPoints(Guid receiptId)
    {
        logger.LogInformation($"Retrieving total points for receipt ID: {receiptId} ...........");

        if (receiptsStorage.TryGetValue(receiptId, out var receipt))
        {
            logger.LogInformation($"Found Receipt ID: {receiptId} ...........");
            return receipt.Points;
        }

        logger.LogWarning($"Receipt Not Found for Receipt ID: {receiptId} ..........");
        return -1;
    }

    public void MarkFailed(Guid receiptId)
    {
        if (receiptsStorage.TryGetValue(receiptId, out var existing))
        {
            existing.Status = ProcessingStatus.Failed;
            receiptsStorage.TryUpdate(receiptId, existing, existing);
        }
        logger.LogWarning($"Receipt with ID {receiptId} failed to process......");
    }

    public bool ContainsHash(string hash)
    {
        return _hashToResponseMap.ContainsKey(hash);
    }
    
    public Guid GetByHash(string hash)
    {
        return _hashToResponseMap[hash];
    }

    public void StoreHash(string hash, Guid id)
    {
        if (_hashToResponseMap.TryAdd(hash, id))
        {
            logger.LogInformation($"Stored hash {hash}");
        }
        else
        {
            logger.LogWarning($"Hash already exists: {hash}");
        }
    }
}