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
    
    public async Task EnqueueAsync(Guid receiptId)
    {
        // queue.Enqueue(receiptId);
        await channel.Writer.WriteAsync(receiptId);
    }

    public async Task<Guid> DequeueAsync(CancellationToken cancellationToken)
    {
        return await channel.Reader.ReadAsync(cancellationToken);
    }
}