using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using RestSharp;

namespace PredictionAppApiClient
{
  public class ApiClient
  {
    private readonly RestClient _client;
    private readonly string _tokenFilePath;

    public string Token { get; private set; }

    public ApiClient(string baseUrl)
    {
      _client = new RestClient(baseUrl);
      _tokenFilePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "PredictionAppApiClient", "token.txt");
      LoadTokenFromFile();
    }

    public void SetToken(string token)
    {
      Token = token;
      if (!string.IsNullOrWhiteSpace(token))
      {
        var existing = _client.DefaultParameters?.FirstOrDefault(p => string.Equals(p.Name, "Authorization", StringComparison.OrdinalIgnoreCase));
        if (existing != null)
        {
          _client.DefaultParameters.Remove(existing);
        }
        _client.AddDefaultHeader("Authorization", "Bearer " + token);
        SaveTokenToFile();
      }
    }

    public void ClearToken()
    {
      Token = null;
      var existing = _client.DefaultParameters?.FirstOrDefault(p => string.Equals(p.Name, "Authorization", StringComparison.OrdinalIgnoreCase));
      if (existing != null)
        _client.DefaultParameters.Remove(existing);
      try { if (File.Exists(_tokenFilePath)) File.Delete(_tokenFilePath); } catch { }
    }

    private void EnsureTokenDirectory()
    {
      try
      {
        var dir = Path.GetDirectoryName(_tokenFilePath);
        if (!Directory.Exists(dir)) Directory.CreateDirectory(dir);
      }
      catch { }
    }

    private void SaveTokenToFile()
    {
      try
      {
        EnsureTokenDirectory();
        File.WriteAllText(_tokenFilePath, Token ?? string.Empty);
      }
      catch { }
    }

    private void LoadTokenFromFile()
    {
      try
      {
        if (File.Exists(_tokenFilePath))
        {
          var t = File.ReadAllText(_tokenFilePath).Trim();
          if (!string.IsNullOrWhiteSpace(t))
          {
            Token = t;
            _client.AddDefaultHeader("Authorization", "Bearer " + Token);
          }
        }
      }
      catch { }
    }

   

    public async Task<IRestResponse> LoginAsync(string loginEndpoint, string email, string password)
    {

      var res = await GetAsync<LoginResult>(loginEndpoint, new { email, password });
      if (res != null && res.IsSuccessful)
      {
        var token = res.Data.AccessToken;
        if (!string.IsNullOrWhiteSpace(token))
        {
          SetToken(token);
        }
      }
      return res;
    }

    public async Task<IRestResponse<TResponse>> GetAsync<TResponse>(string endpoint, object query = null)
    {
      var req = new RestRequest(endpoint, Method.GET);
      if (query != null)
        AddQueryParameters(req, query);
      return await _client.ExecuteAsync<TResponse>(req).ConfigureAwait(false);
    }

    public async Task<IRestResponse<TResponse>> PostAsync<TResponse>(string endpoint, object body = null)
    {
      var req = new RestRequest(endpoint, Method.POST);
      if (body != null) req.AddJsonBody(body);
      return await _client.ExecuteAsync<TResponse>(req).ConfigureAwait(false);
    }

    public async Task<IRestResponse<TResponse>> PostAsync<TRequest, TResponse>(string endpoint, TRequest body = default)
    {
      var req = new RestRequest(endpoint, Method.POST);
      if (body != null) req.AddJsonBody(body);
      return await _client.ExecuteAsync<TResponse>(req).ConfigureAwait(false);
    }

    public async Task<IRestResponse<TResponse>> PutAsync<TResponse>(string endpoint, object body = null)
    {
      var req = new RestRequest(endpoint, Method.PUT);
      if (body != null) req.AddJsonBody(body);
      return await _client.ExecuteAsync<TResponse>(req).ConfigureAwait(false);
    }

    public async Task<IRestResponse<TResponse>> PutAsync<TRequest, TResponse>(string endpoint, TRequest body = default)
    {
      var req = new RestRequest(endpoint, Method.PUT);
      if (body != null) req.AddJsonBody(body);
      return await _client.ExecuteAsync<TResponse>(req).ConfigureAwait(false);
    }

    public async Task<IRestResponse<TResponse>> DeleteAsync<TResponse>(string endpoint, object bodyOrQuery = null)
    {
      var req = new RestRequest(endpoint, Method.DELETE);
      if (bodyOrQuery != null)
      {
        if (IsSimpleType(bodyOrQuery.GetType()))
          req.AddParameter("id", bodyOrQuery);
        else
          req.AddJsonBody(bodyOrQuery);
      }
      return await _client.ExecuteAsync<TResponse>(req).ConfigureAwait(false);
    }

    private void AddQueryParameters(RestRequest req, object query)
    {
      foreach (var p in query.GetType().GetProperties(BindingFlags.Instance | BindingFlags.Public))
      {
        var val = p.GetValue(query);
        if (val != null)
          req.AddParameter(p.Name, val);
      }
    }

    private bool IsSimpleType(Type t)
    {
      return t.IsPrimitive || t.IsEnum || t == typeof(string) || t == typeof(decimal) || t == typeof(DateTime) || t == typeof(Guid);
    }
  }
}
