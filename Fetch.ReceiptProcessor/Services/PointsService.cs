using Fetch.ReceiptProcessor.Entities;
using Fetch.ReceiptProcessor.Models;
using Fetch.ReceiptProcessor.Services.Interfaces;

namespace Fetch.ReceiptProcessor.Services;

public class PointsServiceAsync:IPointsServiceAsync
{
    private readonly ILogger logger;

    public PointsServiceAsync(ILogger logger)
    {
        this.logger = logger;
    }
    
    public async Task<int> calculatePointsAsync(Receipts receipts)
    {
        var tasks = new List<Task<int>>
        {
            Task.Run(() => CheckAlphanumericCharacters(receipts.Retailer)),
            Task.Run(() => CheckRoundAmount(receipts.Total)),
            Task.Run(() => CheckMultipleOf25(receipts.Total)),
            Task.Run(() => CheckItemsQuantity(receipts.ItemsList.Count)),
            Task.Run(() => CheckTrimmedLengthItemDescription(receipts.ItemsList)),
            Task.Run(() => CheckPurchaseDateIsOdd(receipts.PurchaseDate)),
            Task.Run(() => CheckTimeOfPurchase(receipts.PurchaseTime))
        };

        var results = await Task.WhenAll(tasks);
        return results.Sum();
    }

    private int CheckAlphanumericCharacters(string retailerName)
    {
        int sum = 0;

        foreach (char c in retailerName)
        {
            if (Char.IsLetterOrDigit(c))
            {
                sum++;
            }
        }
        logger.LogInformation($"Check AlphaNumeric Points-> {sum}");
        return sum;
    }

    private int CheckRoundAmount(decimal amount)
    {
        int sum = 0;

        if (amount % 1 == 0)
        {
            sum = 50;
        }
        logger.LogInformation($"Check RoundAmount Points-> {sum}");
        return sum;
    }

    private int CheckMultipleOf25(decimal amount)
    {
        int sum = 0;

        if (amount % .25M == 0)
        {
            sum = sum + 25;
        }

        logger.LogInformation($"Check Multipleof25 Points-> {sum}");
        return sum;
    }

    private int CheckItemsQuantity(int count)
    {
        int itemQuantity = count / 2;

        logger.LogInformation($"Check Items Quantity Points-> {itemQuantity *5}");
        return itemQuantity * 5;
    }

    private int CheckTrimmedLengthItemDescription(List<Items> items)
    {
        var sum = items.Where(item => item.ShortDescription.Trim().Length % 3 == 0)
            .Sum(item => Math.Ceiling(item.Price * 0.2m));

        logger.LogInformation($"Check Trimmed Length of Item Description Points-> {(int)sum} {sum}");
        return (int)sum;
    }

    private int CheckPurchaseDateIsOdd(DateTimeOffset date)
    {
        var dayOfMonth = date.Day;

        if (dayOfMonth % 2 == 1)
        {
            logger.LogInformation("Check Purchase Date is odd Points-> 6");
            return 6;
        }
           
        
        
        logger.LogInformation("Check Purchase Date is odd Points-> 0");
        return 0;
    }

    private int CheckTimeOfPurchase(TimeSpan timeOfPurchase)
    {
        TimeSpan startTime = new TimeSpan(14, 0, 0);
        TimeSpan endTime = new TimeSpan(16, 0, 0);

        if (timeOfPurchase >= startTime && timeOfPurchase <= endTime)
        {
            logger.LogInformation("Check Purchase Time is between Points-> 10");
            return 10;
        }

        logger.LogInformation("Check Purchase Time is between Points-> 00");
        return 0;
    }
}