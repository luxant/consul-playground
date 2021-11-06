using AspNetConsulIntegration.Models;
using Consul;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Primitives;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.Json;

namespace AspNetConsulIntegration;
public class ConsulConfigurationSource : IConfigurationSource
{
    public IConfigurationProvider Build(IConfigurationBuilder builder)
    {
        return new ConsultConfigurationProvider(
            new ConsulClient(),
            new HttpClient());
    }
}

public class ConsultConfigurationProvider : IConfigurationProvider
{
    private readonly ConsulClient _consulClient;
    private readonly HttpClient _httpClient;

    public ConsultConfigurationProvider(ConsulClient consulClient, HttpClient httpClient)
    {
        _consulClient = consulClient;
        _httpClient = httpClient;

        _httpClient.BaseAddress = new Uri("http://localhost:8500/v1/kv/");
    }

    public IEnumerable<string> GetChildKeys(IEnumerable<string> earlierKeys, string parentPath)
    {
        return Array.Empty<string>();
    }

    public IChangeToken GetReloadToken()
    {
        return NullChangeToken.Singleton;
    }

    public void Load()
    {
    }

    public void Set(string key, string value)
    {
        throw new NotImplementedException();
    }

    public bool TryGet(string key, out string value)
    {
        if (string.IsNullOrWhiteSpace(key))
        {
            value = string.Empty;
            return false;
        }

        var keyParts = key.Split(':');

        if (keyParts.Length == 1)
        {
            return TryGetValue(keyParts[0], out value);
        }

        // There are more that 1 sections in the key
        var configKey = keyParts[0];
        var configProperty = keyParts[1];

        try
        {
            if (!TryGetValue(configKey, out var section))
            {
                value = string.Empty;
                return false;
            }

            var json = JsonDocument.Parse(section);
            json.RootElement.TryGetProperty(configProperty, out var property);

            value = property.ToString();

            return property.ValueKind != JsonValueKind.Undefined;
        }
        catch (Exception)
        {
            value = string.Empty;
            return false;
        }
    }

    private bool TryGetValue(string configKey, out string value)
    {
        try
        {
            // var result = _httpClient.GetFromJsonAsync<object>(keyParts[0]).Result;
            QueryResult<KVPair> queryResult = _consulClient.KV.Get(configKey).Result;

            if (queryResult.Response == null)
            {
                value = string.Empty;
                return false;
            }

            var decodedResponse = Encoding.UTF8.GetString(queryResult.Response.Value);

            value = decodedResponse;

            return queryResult.StatusCode == System.Net.HttpStatusCode.OK;
        }
        catch (Exception)
        {
            value = string.Empty;
            return false;
        }
    }
}