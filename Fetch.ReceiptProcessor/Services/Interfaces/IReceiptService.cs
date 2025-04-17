using Fetch.ReceiptProcessor.Entities;
using Fetch.ReceiptProcessor.Models;
using Fetch.ReceiptProcessor.Models.RequestModels;

namespace Fetch.ReceiptProcessor.Services.Interfaces;

public interface IReceiptService
{
    
    Receipts CreateReceipt(ReceiptRequest request);

    ProcessResponse ToProcessResponse(Receipts receipt);
    
    PointsResponse ToPointsResponse(Receipts receipt);
}