using Fetch.ReceiptProcessor.Entities;
using Fetch.ReceiptProcessor.Models;
using Fetch.ReceiptProcessor.Models.RequestModels;
using Fetch.ReceiptProcessor.Services.Interfaces;

namespace Fetch.ReceiptProcessor.Services;

public class ReceiptService:IReceiptService
{

    public Receipts CreateReceipt(ReceiptRequest request)
    {
        return new Receipts
        {
            ReceiptId = Guid.NewGuid(),
            Retailer = request.Retailer.Trim(),
            PurchaseDate = DateTimeOffset.TryParse(request.PurchaseDate, out var pd)? pd: default,
            PurchaseTime = TimeSpan.TryParse(request.PurchaseTime,out var pt) ? pt : default,
            ItemsList = request.ItemsList.Select(ConvertItems).ToList() ?? new List<Items>(),
            Total = decimal.TryParse(request.Total, out var t)? t:0m
        };
    }

    private Items ConvertItems(ItemRequest itemRequest)
    {
        return new Items
        {
            ShortDescription = itemRequest.ShortDescription.Trim(),
            Price = decimal.Parse(itemRequest.Price)
        };
    }

    public ProcessResponse ToProcessResponse(Receipts receipt)
    {
        return new ProcessResponse
        {
            Id = receipt.ReceiptId,
        };
    }

    public PointsResponse ToPointsResponse(Receipts receipt)
    {
        return new PointsResponse
        {
            Points = receipt.Points
        };
    }
}