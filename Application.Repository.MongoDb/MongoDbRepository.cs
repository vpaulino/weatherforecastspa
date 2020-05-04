
using Application.Repository.MongoDb.Abstractions;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;

namespace Application.Repository.MongoDb
{
 
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="TEntity"></typeparam>
    /// <typeparam name="TPartialUpdate"></typeparam>
    public abstract class MongoDbRepository<TEntity, TPartialUpdate> : IReadRepository<string, TEntity>, IWriteRepository<string, TEntity, TPartialUpdate> where TEntity : Entity
    {
        IMongoCollection<TEntity> collection;
        FilterDefinitionBuilder<TEntity> filterBuilder = new FilterDefinitionBuilder<TEntity>();

        /// <summary>
        /// 
        /// </summary>
        /// <param name="options"></param>
        public MongoDbRepository(IOptions<DatabaseSettings> options)
        {

            this.collection = new MongoClient(options.Value.ConnectionString).GetDatabase(options.Value.DataBaseName).GetCollection<TEntity>(options.Value.SetName);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="options"></param>
        public MongoDbRepository(DatabaseSettings options)
        {

            this.collection = new MongoClient(options.ConnectionString).GetDatabase(options.DataBaseName).GetCollection<TEntity>(options.SetName);
        }

        /// <summary>
        /// Create all the indexes
        /// </summary>
        protected abstract void TryCreateIndexes();

        /// <summary>
        /// create new entity
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        public async Task Create(TEntity entity, CancellationToken token)
        {
            await this.collection.InsertOneAsync(entity,new InsertOneOptions() {  BypassDocumentValidation = true }, token);
        }

        /// <summary>
        /// Delete entity
        /// </summary>
        /// <param name="id"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        public async Task Delete(string id, CancellationToken token)
        {
           
            FilterDefinition<TEntity> definition = filterBuilder.Eq<string>((entity) => entity.Id, id);

            await this.collection.DeleteOneAsync(definition, token);
        }

        /// <summary>
        /// Get by id
        /// </summary>
        /// <param name="id"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        public async Task<TEntity> Get(string id, CancellationToken token)
        {
            FilterDefinition<TEntity> definition = filterBuilder.Eq<string>((entity) => entity.Id, id);
            var cursor = await this.collection.FindAsync<TEntity>(definition);

            return await cursor.FirstOrDefaultAsync(token);
        }

         
        /// <summary>
        /// Get by tags
        /// </summary>
        /// <param name="containingTags"></param>
        /// <param name="take"></param>
        /// <param name="skip"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        public async Task<IEnumerable<TEntity>> GetByTags(IEnumerable<string> containingTags, int take, int skip, CancellationToken token)
        {
            var filterDefinition = filterBuilder.ElemMatch((entity) => entity.Tags, (tag) => containingTags.Contains(tag));

            var cursor = await this.collection.FindAsync<TEntity>(filterDefinition, new FindOptions<TEntity, TEntity>() { Skip = skip, Limit = take });

            return await cursor.ToListAsync(token);

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="containingTags"></param>
        /// <param name="CreatedBiggerThen"></param>
        /// <param name="CreatedlessThen"></param>
        /// <param name="take"></param>
        /// <param name="skip"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        public async Task<IEnumerable<TEntity>> GetByTagsTimeInterval(IEnumerable<string> containingTags, DateTime CreatedBiggerThen, DateTime CreatedlessThen , int take, int skip, CancellationToken token = default)
        {
            var tagsFilterDefinition = filterBuilder.ElemMatch((entity) => entity.Tags, (tag) => containingTags.Contains(tag));
            var datesInterval = filterBuilder.And(filterBuilder.Gte<DateTime>((entity) => entity.Created, CreatedBiggerThen), filterBuilder.Lte<DateTime>((entity) => entity.Created, CreatedlessThen));
         
            return await FindMany(take, skip, filterBuilder.And(tagsFilterDefinition, datesInterval), token);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="CreatedBiggerThen"></param>
        /// <param name="CreatedlessThen"></param>
        /// <param name="take"></param>
        /// <param name="skip"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        public async Task<IEnumerable<TEntity>> GetByTimeInterval(DateTime CreatedBiggerThen, DateTime CreatedlessThen, int take, int skip, CancellationToken token = default)
        {
            
            var datesInterval = filterBuilder.And(filterBuilder.Gte<DateTime>((entity) => entity.Created, CreatedBiggerThen), filterBuilder.Lte<DateTime>((entity) => entity.Created, CreatedlessThen));
            return await FindMany(take, skip, datesInterval, token);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="take"></param>
        /// <param name="skip"></param>
        /// <param name="orderBy"></param>
        /// <param name="orderDirection"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        public async Task<IEnumerable<TEntity>> GetList(int take, int skip, string orderBy, int orderDirection, CancellationToken token)
        {

            var filter = FilterDefinition<TEntity>.Empty;

            return await FindMany(take, skip, filter, token);
        }

        private async Task<IEnumerable<TEntity>> FindMany(int take, int skip, FilterDefinition<TEntity> filter, CancellationToken token)
        {
            var searchOptions = new FindOptions<TEntity, TEntity>()
            {
                Skip = skip,
                Limit = take,
                Sort = Builders<TEntity>.Sort.Descending((entity) => entity.Created),
            };

            var cursor = await this.collection.FindAsync<TEntity>(filter, searchOptions);

            return (await cursor.ToListAsync(token)).Skip(skip).Take(take);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <param name="entity"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        public async Task CreateOrUpdate(string id, TPartialUpdate entity, CancellationToken token)
        {
            UpdateDefinitionBuilder<TEntity> updateDefinitionBuilder = new UpdateDefinitionBuilder<TEntity>();

            UpdateDefinition<TEntity> updateDefinition = this.GetUpdateDefinition(entity);

            FilterDefinition<TEntity> filterById = filterBuilder.Eq<string>((_entity) => _entity.Id, id);

            await this.collection.UpdateOneAsync(filterById, updateDefinition, new UpdateOptions() { IsUpsert = true });
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        protected abstract UpdateDefinition<TEntity> GetUpdateDefinition(TPartialUpdate entity);

        
    }
}
