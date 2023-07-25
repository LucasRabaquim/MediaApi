using MongoDB.Bson;
using System.Text.Json.Serialization;
using MongoDB.Bson.Serialization.Attributes;

namespace MediaApi.Models;

public class Comments{

    [BsonId]
    public int Id {get; set;}
    public int userId {get; set;}
    public required string likes {get; set;}
    public string message {get; set;} = null!;
    public DateTime postedDate = DateTime.UtcNow;
    public DateTime alteredDate {get; set;}
    public int? replyToCommentId {get; set;}
}