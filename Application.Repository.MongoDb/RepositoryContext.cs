using Application.Repository.MongoDb.Abstractions;
using Microsoft.Extensions.Options;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Conventions;
using MongoDB.Bson.Serialization.IdGenerators;
using MongoDB.Bson.Serialization.Serializers;
using MongoDB.Driver;
using MongoDB.Driver.Core.Clusters;

namespace Application.Repository.MongoDb
{
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class RepositoryContext<T> : IRepositoryContext<T> where T:Entity
    {
        /// <summary>
        /// 
        /// </summary>
        protected DatabaseSettings settings;
        
        private IMongoDatabase database;

        /// <summary>
        /// 
        /// </summary>
        public ICluster Cluster { get; private set; }
    
        /// <summary>
        /// 
        /// </summary>
        public string DatabaseName { get; protected set; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="settings"></param>
        public RepositoryContext(IOptions<DatabaseSettings> settings) 
        {
            this.settings = settings.Value;
        }

        /// <summary>
        /// 
        /// </summary>
        public void Setup() 
        {
            MapModel();
            MapConnection();

        }

        /// <summary>
        /// 
        /// </summary>
        protected virtual void MapConnection() 
        {
            MongoClient client = new MongoClient(settings.ConnectionString);
            this.database = client.GetDatabase(settings.DataBaseName);
            this.Cluster = client.Cluster;
        }

        private void MapModel()
        {
            var pack = new ConventionPack();
            pack.Add(new IgnoreExtraElementsConvention(true));
            ConventionRegistry.Register($"{typeof(T).Name} Convention", pack, t => true);

            if (!BsonClassMap.IsClassMapRegistered(typeof(Entity)))
            {
                BsonClassMap.RegisterClassMap<Entity>(cm =>
                {

                    cm.MapIdField((prod) => prod.Id).SetIdGenerator(StringObjectIdGenerator.Instance)
                                                    .SetSerializer(new StringSerializer(BsonType.ObjectId));
                    cm.AutoMap();
                });
            }
            
            if (!BsonClassMap.IsClassMapRegistered(typeof(T)))
            {
                BsonClassMap.RegisterClassMap<T>(cm =>
                {
                    this.MapDomainModel(cm);
                    
                });
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="cm"></param>
        protected virtual void MapDomainModel(BsonClassMap<T> cm) 
        {
            cm.AutoMap();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <param name="settings"></param>
        /// <returns></returns>
        public virtual IMongoCollection<T> GetCollection(string name, MongoCollectionSettings settings = null) 
        {
            return this.database.GetCollection<T>(name, settings);
        }


    }
}
