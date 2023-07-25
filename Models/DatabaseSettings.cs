namespace MediaApi.Models;

public class DatabaseSettings
{
    public string ConnectionString { get; set; } = null!;

    public string DatabaseName { get; set; } = null!;

    public string CommentsCollectionName { get; set; } = null!;
    public string UserCollectionName { get; set; } = null!;
}