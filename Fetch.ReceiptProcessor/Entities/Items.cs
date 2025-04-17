using System.ComponentModel.DataAnnotations;

namespace Fetch.ReceiptProcessor.Entities;

public class Items
{
    
    public string ShortDescription { get; set; } = string.Empty;
    
    public decimal Price { get; set; }
}