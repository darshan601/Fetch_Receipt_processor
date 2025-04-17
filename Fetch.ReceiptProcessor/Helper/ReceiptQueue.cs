using System.Collections.Concurrent;
using System.Threading.Channels;

namespace Fetch.ReceiptProcessor.Helper;

// changing from queues to channels
public class ReceiptQueue:IReceiptQueue
{
    // private readonly ConcurrentQueue<Guid> queue;
    private readonly Channel<Guid> channel;

    public ReceiptQueue()
    {
        this.channel = Channel.CreateUnbounded<Guid>();
    }
    
    public Task EnqueueAsync(Guid receiptId)
    {
        // queue.Enqueue(receiptId);
        if (!channel.Writer.TryWrite(receiptId))
        {
            // optional: log backpressure, or handle retries
        }

        return Task.CompletedTask;
        // await channel.Writer.WriteAsync(receiptId);
    }

    public async Task<Guid> DequeueAsync(CancellationToken cancellationToken)
    {
        return await channel.Reader.ReadAsync(cancellationToken);
    }
}