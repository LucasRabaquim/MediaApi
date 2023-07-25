using MediaApi.Models;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace MediaApi.Services;

public class UserService
{
    private readonly IMongoCollection<User> _userCollection;

    public UserService(IOptions<DatabaseSettings> databaseSettings){
          var mongoClient = new MongoClient(databaseSettings.Value.ConnectionString);

        var mongoDatabase = mongoClient.GetDatabase(databaseSettings.Value.DatabaseName);

        _userCollection = mongoDatabase.GetCollection<User>(databaseSettings.Value.UserCollectionName);
    }

    public async Task<List<User>> GetAsync(int id) =>
        await _userCollection.Find(_ => true).ToListAsync();

    public async Task<List<User>> GetAsync() =>
        await _userCollection.Find(_ => true).ToListAsync();

    public async Task<User> PostAsync(string email) =>
        await _userCollection.Find(x => x.Email == email).FirstOrDefaultAsync();

    public async Task CreateAsync(User newBook) =>
        await _userCollection.InsertOneAsync(newBook);

    public async Task UpdateAsync(int id, User updatedBook) =>
        await _userCollection.ReplaceOneAsync(x => x.Id == id, updatedBook);

    public async Task RemoveAsync(int id) =>
        await _userCollection.DeleteOneAsync(x => x.Id == id);
}