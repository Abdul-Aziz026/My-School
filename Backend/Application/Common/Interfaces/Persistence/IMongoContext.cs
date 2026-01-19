
using MongoDB.Driver;

namespace Application.Common.Interfaces.Persistence;

public interface IMongoContext
{
    IMongoCollection<T> GetCollection<T>(string? name = null);
}
