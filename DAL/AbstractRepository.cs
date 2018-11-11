using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using MongoDB.Bson;
using MongoDB.Driver;

namespace Q.Squid.DAL
{
    public abstract class AbstractRepository<T> where T : SquidBaseModel
    {
        private MongoClient _client;
        private IMongoDatabase _database;
        private IMongoCollection<T> _collection;

        public AbstractRepository(string connectionString, string databaseName, string collectionName)
        {
            _client = new MongoClient(connectionString);
            _database = _client.GetDatabase(databaseName);
            _collection = _database.GetCollection<T>($"squid.{collectionName}");
        }

        public async Task<List<T>> GetAll()
        {
            return await _collection.Find(new BsonDocument()).ToListAsync();
        }

        public async Task<T> Get(string id)
        {
            var filter = Builders<T>.Filter.Eq("_id", id);
            var documents = await _collection.FindAsync(filter);
            return await documents.FirstOrDefaultAsync();
        }

        public async Task<T> Add(T entity)
        {
            await _collection.InsertOneAsync(entity);
            return entity;
        }

        public async Task Update(T entity)
        {
            var filter = Builders<T>.Filter.Eq("_id", entity.Id);
            await _collection.ReplaceOneAsync(filter, entity);
        }

        public async Task Delete(string id)
        {
            var filter = Builders<T>.Filter.Eq("_id", id);
            await _collection.DeleteOneAsync(filter);
        }
    }
}