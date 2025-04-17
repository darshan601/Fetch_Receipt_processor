using Fetch.ReceiptProcessor.Entities;
using Fetch.ReceiptProcessor.Helper;
using Fetch.ReceiptProcessor.Services.Interfaces;

namespace Fetch.ReceiptProcessor.Services;

public class ReceiptProcessorWorker:BackgroundService
{
    private readonly ILogger logger;

    private readonly IReceiptQueue queue;

    private readonly IReceiptStorage storage;
    
    private readonly IPointsServiceAsync pointsService;

    public ReceiptProcessorWorker(ILogger logger, IReceiptQueue queue, IReceiptStorage storage, IPointsServiceAsync pointsService)
    {
        this.logger = logger;
        this.queue = queue;
        this.storage = storage;
        this.pointsService = pointsService;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        // logger.LogInformation("Starting receipt processor worker...............");
        while (!stoppingToken.IsCancellationRequested)
        {
            var receiptId =await queue.DequeueAsync(stoppingToken);
            // logger.LogInformation($"receipt id: {receiptId}.............");

            if (receiptId != Guid.Empty)
            {
                try
                {
                    var receipt = storage.GetReceipts(receiptId);
                    if (receipt?.Status == ProcessingStatus.Pending)
                    {
                        var points = await pointsService.calculatePointsAsync(receipt);
                        
                        receipt.Points = points;
                        receipt.Status = ProcessingStatus.Completed;
                        
                        // logger.LogInformation($"Updating the Receipt Processing Status: {receipt.Status}");
                        
                        storage.UpdateReceipts(receipt);
                        
                    }
                }
                catch (Exception e)
                {
                    logger.LogError(e, "error processing receipt........", receiptId);
                    storage.MarkFailed(receiptId);
                }
            }
        }
    }
}