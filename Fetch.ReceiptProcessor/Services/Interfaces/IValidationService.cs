using Fetch.ReceiptProcessor.Models.RequestModels;

namespace Fetch.ReceiptProcessor.Services.Interfaces;

public interface IValidationService
{
    Task<bool> ValidateRequestAsync(ReceiptRequest request);

    Task<bool> ValidateRemainingAsync(ReceiptRequest request);

    bool IsValidGuid(string id);
}