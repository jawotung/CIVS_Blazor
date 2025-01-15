using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebAPI;

namespace Application.Interfaces
{
    public interface IBackgroundTaskQueue
    {
        ValueTask QueueBackgroundWorkItemAsync(ChequeAccountDetail workItem);

        ValueTask<ChequeAccountDetail> DequeueOneAsync(CancellationToken cancellationToken);

        ValueTask<List<ChequeAccountDetail>> DequeueBatchAsync(CancellationToken cancellationToken);
    }
}
