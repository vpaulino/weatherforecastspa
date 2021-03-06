﻿using MongoDB.Driver;

namespace Application.Repository.MongoDb
{
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IRepositoryContext<T>
    {
        /// <summary>
        /// 
        /// </summary>
        string DatabaseName { get; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <param name="settings"></param>
        /// <returns></returns>
        IMongoCollection<T> GetCollection(string name, MongoCollectionSettings settings = null);
    }
}