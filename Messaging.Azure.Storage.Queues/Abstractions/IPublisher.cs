
using Messaging.Abstractions;
using System.Threading;
using System.Threading.Tasks;

namespace Messaging.Azure.Storage.Queues
{
    /// <summary>
    /// 
    /// </summary>
    public  interface IPublisher<T>
    {
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="payload"></param>
        /// <param name="path"></param>
        /// <param name="options"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        Task<PublishResult> PublishAsync(T payload, string path, MessageOptions options, CancellationToken token);

        
    }
}
