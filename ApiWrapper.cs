using ApiSdk;
using ApiSdk.Models;
using Microsoft.Kiota.Abstractions.Authentication;
using Microsoft.Kiota.Http.HttpClientLibrary;

namespace MovieSearchBot;

public class ApiWrapper
{
    KinoPoiskDev _client;

    public ApiWrapper(string apiKey)
    {
        ApiKeyAuthenticationProvider authProvider = new(apiKey, "X-API-KEY", ApiKeyAuthenticationProvider.KeyLocation.Header);
        HttpClientRequestAdapter adapter = new(authProvider);
        _client = new(adapter);
    }

    public async Task<List<SearchMovieDtoV1_4>?> FindMovie(string query)
    {
        var result = await _client.V14.Movie.Search.GetAsync(configuration =>
        {
            configuration.QueryParameters.Query = query;
        });
        return result?.Docs?.ToList();
    }
}