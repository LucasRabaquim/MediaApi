using MongoDB.Bson;
using System.Text.Json.Serialization;
using MongoDB.Bson.Serialization.Attributes;

namespace MediaApi.Models;

public class User{
    [BsonId]
    public int Id {get; set;}
    public required string Name {get; set;}
    public required string Email {get; set;}
    public required string Password {get; set;}
}