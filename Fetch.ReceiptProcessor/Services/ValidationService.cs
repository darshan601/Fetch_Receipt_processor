using System.Globalization;
using System.Text.RegularExpressions;
using Fetch.ReceiptProcessor.Models.RequestModels;
using Fetch.ReceiptProcessor.Services.Interfaces;

namespace Fetch.ReceiptProcessor.Services;

public class ValidationService:IValidationService
{
    public Task<bool> ValidateRequestAsync(ReceiptRequest request)
    {
        var retailerValid = IsValidRetailer(request.Retailer);
        if (!retailerValid) return Task.FromResult(false);

        return ValidateRemainingAsync(request);
    }

    public async Task<bool> ValidateRemainingAsync(ReceiptRequest request)
    {
        var dateTimeTask = Task.Run(()=> IsValidPurchaseDateTime(request.PurchaseDate, request.PurchaseTime));
        var totalTask = Task.Run(()=> IsValidTotal(request.Total));
        var itemTask = Task.Run(()=> IsValidItemsListParallel(request.ItemsList));
        
        await Task.WhenAll(dateTimeTask, totalTask, itemTask);
        
        return dateTimeTask.Result && totalTask.Result && itemTask.Result;
    }


    private static readonly Regex RetailerRegex = 
        new Regex(@"^[\w\s\-&]+$", RegexOptions.Compiled);

    private static bool IsValidRetailer(string retailer)
    {
        return !string.IsNullOrWhiteSpace(retailer) && 
               RetailerRegex.IsMatch(retailer);
    }
    
    private static bool IsValidPurchaseDateTime(string date, string time)
    {
        // Combined date/time validation with exact format
        return DateTime.TryParseExact(
            $"{date} {time}",
            "yyyy-MM-dd HH:mm",
            CultureInfo.InvariantCulture,
            DateTimeStyles.None,
            out _
        );
    }
    
    private static bool IsValidTotal(string total)
    {
        // Regex + decimal parse combo
        return Regex.IsMatch(total, @"^\d+\.\d{2}$") &&
               decimal.TryParse(total, NumberStyles.AllowDecimalPoint, 
                   CultureInfo.InvariantCulture, out var parsed) &&
               parsed > 0;
    }
    

    private static bool IsValidItemsListParallel(List<ItemRequest> items)
    {
        if (items == null || items.Count < 1) return false;
        
        var validFlags = new bool[items.Count];
        Parallel.For(0, items.Count, i => {
            validFlags[i] = IsValidItem(items[i]);
        });
        
        return Array.TrueForAll(validFlags, x => x);
    }
    
    private static bool IsValidItem(ItemRequest item)
    {
        // item validation logic here
        return !string.IsNullOrWhiteSpace(item.ShortDescription) &&
               decimal.TryParse(item.Price, NumberStyles.AllowDecimalPoint,
                   CultureInfo.InvariantCulture, out _);
    }

}