using System.Text;
using System.Text.Json;
using PlatformService.Dtos;

namespace PlatformService.SyncDataServices.Http;

public class HttpCommandDataClient : ICommandDataClient
{

  private readonly HttpClient _httpClient;
  private readonly IConfiguration _configuration;
  public HttpCommandDataClient(IConfiguration configuration, HttpClient httpClient)
  {
    _httpClient = httpClient;
    _configuration = configuration;
  }
  public async Task SendPlatformToCommand(ReadPlatformDto plat)
  {
    var httpContent = new StringContent(
      JsonSerializer.Serialize(plat),
      Encoding.UTF8,
      "application/json"
    );

    var response = await _httpClient.PostAsync($"{_configuration["CommandService"]}/c/platform/", httpContent);

    if (response.IsSuccessStatusCode)
    {
      Console.WriteLine($"----> Sync POST to Command Services Was Ok!");
    }
    else
    {
      Console.WriteLine($"----> Sync POST to Command Services Was Not Ok!");
    }
  }
}
