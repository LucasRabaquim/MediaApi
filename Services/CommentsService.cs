using MediaApi.Models;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace MediaApi.Services;

public class CommentsService
{
    private readonly IMongoCollection<Comments> _commentsCollection;

    private readonly IMongoCollection<User> _usersCollection;

    public CommentsService(IOptions<DatabaseSettings> databaseSettings)
    {
        var mongoClient = new MongoClient(databaseSettings.Value.ConnectionString);

        var mongoDatabase = mongoClient.GetDatabase(databaseSettings.Value.DatabaseName);

        _commentsCollection = mongoDatabase.GetCollection<Comments>(databaseSettings.Value.CommentsCollectionName);

        _usersCollection = mongoDatabase.GetCollection<User>(databaseSettings.Value.UserCollectionName);
    }

    public async Task<List<Comments>> GetAsync() =>
        await _commentsCollection.Find(_ => true).ToListAsync();

    public async Task<Comments?> GetAsync(int id) =>
        await _commentsCollection.Find(x => x.Id == id).FirstOrDefaultAsync();

    public async Task<List<Comments>> GetAsync(string email)
    {
        User userId = await _usersCollection.Find(x => x.Email == email).FirstOrDefaultAsync();
        List<Comments> comments = await _commentsCollection.Find(x => x.userId == userId.Id).ToListAsync();
        return comments;
    }


    public async Task CreateAsync(Comments newBook) =>
        await _commentsCollection.InsertOneAsync(newBook);

    public async Task UpdateAsync(int id, Comments updatedBook) =>
        await _commentsCollection.ReplaceOneAsync(x => x.Id == id, updatedBook);

    public async Task RemoveAsync(int id) =>
        await _commentsCollection.DeleteOneAsync(x => x.Id == id);
}