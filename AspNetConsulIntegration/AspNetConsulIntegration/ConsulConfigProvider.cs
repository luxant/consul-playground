using Microsoft.Extensions.Primitives;

namespace AspNetConsulIntegration;
public class EntityConfigurationSource : IConfigurationSource
{
    private readonly string _connectionString;

    public EntityConfigurationSource(string connectionString) =>
        _connectionString = connectionString;

    public IConfigurationProvider Build(IConfigurationBuilder builder)
    {

        return new A();
    }

}

public class A : IConfigurationProvider
{
    public IEnumerable<string> GetChildKeys(IEnumerable<string> earlierKeys, string parentPath)
    {
        throw new NotImplementedException();
    }

    public IChangeToken GetReloadToken()
    {
        throw new NotImplementedException();
    }

    public void Load()
    {
        throw new NotImplementedException();
    }

    public void Set(string key, string value)
    {
        throw new NotImplementedException();
    }

    public bool TryGet(string key, out string value)
    {
        throw new NotImplementedException();
    }
}
