using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Fetch.ReceiptProcessor.Models.RequestModels;

public class ItemRequest
{
    [Required]
    [JsonPropertyName("shortDescription")]
    public string ShortDescription { get; set; } = string.Empty;

    [Required]
    [RegularExpression(@"^\d+\.\d{2}$", ErrorMessage = "Price must have 2 decimal places")]
    [JsonPropertyName("price")]
    public string Price { get; set; } = string.Empty;
}