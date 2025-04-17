using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using Fetch.ReceiptProcessor.Entities;

namespace Fetch.ReceiptProcessor.Models.RequestModels;

public class ReceiptRequest
{
    [Required]
    [RegularExpression("^[\\w\\s\\-&]+$")]
    public string Retailer { get; set; } = string.Empty;

    [Required]
    [JsonPropertyName("purchaseDate")]
    [RegularExpression(@"^\d{4}-\d{2}-\d{2}$", ErrorMessage = "Date format must be YYYY-MM-DD")]
    public string PurchaseDate { get; set; }

    [Required]
    [JsonPropertyName("purchaseTime")]
    [RegularExpression(@"^([01]\d|2[0-3]):[0-5]\d$", ErrorMessage = "Time format must be HH:mm")]
    public string PurchaseTime { get; set; }

    [Required] 
    [JsonPropertyName("items")]
    [MinLength(1)] 
    public List<ItemRequest> ItemsList { get; set; } = new();

    [Required]
    [Range(0, double.MaxValue)]
    [RegularExpression("^\\d+\\.\\d{2}$", ErrorMessage = "Total must have 2 decimal places")]
    public string Total { get; set; } = string.Empty;

}