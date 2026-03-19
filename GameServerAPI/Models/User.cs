using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace GameServerAPI.Models;

public class User
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string? Id { get; set; }

    [BsonElement("username")]
    public string Username { get; set; } = null!;

    [BsonElement("passwordHash")]
    public string PasswordHash { get; set; } = null!;

    [BsonElement("level")]
    public int Level { get; set; } = 1;
}

public class AuthRequest
{
    public string Username { get; set; } = null!;
    public string Password { get; set; } = null!;
}