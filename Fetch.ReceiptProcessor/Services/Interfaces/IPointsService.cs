using Fetch.ReceiptProcessor.Entities;
using Fetch.ReceiptProcessor.Models;

namespace Fetch.ReceiptProcessor.Services.Interfaces;

public interface IPointsServiceAsync
{
    Task<int> calculatePointsAsync(Receipts receipts);
}