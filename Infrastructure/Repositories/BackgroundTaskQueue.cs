using Application.Interfaces;
using Application.Models;
using Infrastructure.Common;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Channels;
using System.Threading.Tasks;
using WebAPI;

namespace Infrastructure.Repositories
{
    public sealed class BackgroundTaskQueue : IBackgroundTaskQueue
    {
        private readonly Channel<ChequeAccountDetail> _queueAcctDetails;
        private readonly int maxBatchSize;
        public BackgroundTaskQueue(int maxQueue, int maxBatch)
        {
            maxBatchSize = maxBatch;
            BoundedChannelOptions options = new BoundedChannelOptions(maxQueue)
            {
                FullMode = BoundedChannelFullMode.Wait
            };
            _queueAcctDetails = Channel.CreateBounded<ChequeAccountDetail>(options);
        }

        public async ValueTask QueueBackgroundWorkItemAsync(ChequeAccountDetail workItem)
        {
            if (workItem == null)
                throw new ArgumentNullException(nameof(workItem));

            await _queueAcctDetails.Writer.WriteAsync(workItem);
            Console.WriteLine($"Queued: {workItem.ChequeImageLinkedKey}");
        }

        public async ValueTask<ChequeAccountDetail> DequeueOneAsync(CancellationToken cancellationToken)
        {
            ChequeAccountDetail workItem =
                await _queueAcctDetails.Reader.ReadAsync(cancellationToken);

            Console.WriteLine($"DequeueOne: {workItem.ChequeImageLinkedKey}");
            return workItem;
        }

        public async ValueTask<List<ChequeAccountDetail>> DequeueBatchAsync(CancellationToken cancellationToken)
        {
            Console.WriteLine($"Waiting to dequeue");
            await _queueAcctDetails.Reader.WaitToReadAsync(cancellationToken);
            var batchDetails = new List<ChequeAccountDetail>();
            while (batchDetails.Count < maxBatchSize && _queueAcctDetails.Reader.TryRead(out ChequeAccountDetail details))
            {
                batchDetails.Add(details);
            }

            Console.WriteLine($"DequeueBatch count of : {batchDetails.Count}");
            return batchDetails;
        }
    }
}
