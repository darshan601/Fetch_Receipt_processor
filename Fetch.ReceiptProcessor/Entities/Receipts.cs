using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Fetch.ReceiptProcessor.Entities;

public enum ProcessingStatus
{
    Pending, Completed, Failed
}

public class Receipts
{
   
    public Guid ReceiptId { get; set; }
    
    public ProcessingStatus Status { get; set; }

    public string Retailer { get; set; } = string.Empty;
    
    public DateTimeOffset PurchaseDate { get; set; }
    
    public TimeSpan PurchaseTime { get; set; }
    
    public List<Items> ItemsList { get; set; } = new();
    
    public decimal Total { get; set; }
    
    public int Points { get; set; }
}