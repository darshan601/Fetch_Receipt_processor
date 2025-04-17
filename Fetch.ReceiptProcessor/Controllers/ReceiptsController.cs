using System.Diagnostics;
using System.Text.Json;
using Fetch.ReceiptProcessor.Entities;
using Fetch.ReceiptProcessor.Helper;
using Fetch.ReceiptProcessor.Models;
using Fetch.ReceiptProcessor.Models.RequestModels;
using Fetch.ReceiptProcessor.Services;
using Fetch.ReceiptProcessor.Services.Interfaces;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Exception = System.Exception;

namespace Fetch.ReceiptProcessor.Controllers;

[ApiController]
[Route("receipts")]
public class ReceiptsController:ControllerBase
{
    private readonly IReceiptStorage receiptStorage;

    private readonly IPointsServiceAsync pointsServiceAsync;
    
    private readonly IReceiptService receiptService;
    
    private readonly ILogger<ReceiptsController> logger;
    
    private readonly IReceiptQueue queue;

    public ReceiptsController(IReceiptStorage receiptStorage, IReceiptService receiptService, IPointsServiceAsync pointsService, 
        ILogger<ReceiptsController> logger, IReceiptQueue queue)
    {
        this.receiptStorage = receiptStorage;
        this.pointsServiceAsync = pointsService;
        this.receiptService = receiptService;
        this.logger = logger;
        this.queue = queue;
    }

    [HttpPost("process")]
    public async Task<ActionResult<ProcessResponse>> ProcessReceipt([FromBody] ReceiptRequest request)
    {
        // var stopwatch = Stopwatch.StartNew();
        
        
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        try
        {
            var serializedRequest = JsonSerializer.Serialize(request);
            var requestHash=Extensions.ComputeHash(serializedRequest);
            
            if (receiptStorage.ContainsHash(requestHash))
            {
                // logger.LogInformation($"Receipt exists in Hash Dictionary, returning the same");
                return Ok(new ProcessResponse{ Id = receiptStorage.GetByHash(requestHash) });
            }
            
            
            var receipts=receiptService.CreateReceipt(request);
            receipts.Status = ProcessingStatus.Pending;
            
            // receipts.Points=await pointsServiceAsync.calculatePointsAsync(receipts);
            // logger.LogInformation($"Adding receipt into Dictionaries");
            receiptStorage.StoreReceipt(receipts);
            receiptStorage.StoreHash(requestHash, receipts.ReceiptId);
            
            // logger.LogInformation($"Pushing Receipt Id:{receipts.ReceiptId} into Queue");
            queue.EnqueueAsync(receipts.ReceiptId);
        
            // stopwatch.Stop();
            // logger.LogInformation($"Latency: {stopwatch.ElapsedMilliseconds} ms");
            return Ok(receiptService.ToProcessResponse(receipts));
        }
        catch(Exception e)
        {
            logger.LogError(e, e.Message);
            // stopwatch.Stop();
            // logger.LogInformation($"Latency: {stopwatch.ElapsedMilliseconds} ms");
            return StatusCode(500, "Error Processing Receipt");
        }
        
    }


    [HttpGet("{id}/points")]
    public async Task<ActionResult> GetPoints(Guid id)
    {
        
        // var stopwatch = Stopwatch.StartNew();
        // logger.LogInformation($"Getting receipt from Storage");
        var receipts = await Task.FromResult(receiptStorage.GetReceipts(id));

        if (receipts==null)
        {
            return NotFound(new {Error = "Receipt not found"});
        }
        // stopwatch.Stop();
        // logger.LogInformation($"Latency: {stopwatch.ElapsedMilliseconds} ms");
        
        // logger.LogInformation($"Checking status of the receipt and returning response");
        return receipts.Status switch
        {
            ProcessingStatus.Completed => Ok(receiptService.ToPointsResponse(receipts)),
            ProcessingStatus.Pending => Accepted(new { Message = "Processing in progress" }),
            ProcessingStatus.Failed => StatusCode(503, new { 
                Error = "Processing failed after multiple retries" 
            }),
            _ => BadRequest()
        };
    }
}