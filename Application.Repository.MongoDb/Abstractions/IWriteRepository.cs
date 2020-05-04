using System.Threading;
using System.Threading.Tasks;

namespace Application.Repository.MongoDb.Abstractions
{
    /// <summary>
    /// Represents a set of write operations to a repository
    /// </summary>
    /// <typeparam name="TId"></typeparam>
    /// <typeparam name="TEntity"></typeparam>
    /// <typeparam name="TPartialUpdate"></typeparam>
    public interface IWriteRepository<TId, TEntity, TPartialUpdate> where TEntity : Entity
    {
        /// <summary>
        /// Creates an instance of object. if that entity already exists if should throw exception 
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        Task Create(TEntity entity, CancellationToken token);

        /// <summary>
        /// Creates an instance and if already exists then it will be updated
        /// </summary>
        /// <param name="id"></param>
        /// <param name="entity"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        Task CreateOrUpdate(TId id, TPartialUpdate entity, CancellationToken token);

        /// <summary>
        /// Delete an entity by is Id
        /// </summary>
        /// <param name="id"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        Task Delete(TId id, CancellationToken token);
    }
}
