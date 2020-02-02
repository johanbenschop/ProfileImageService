using System;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using ProfileImageService.Settings;

namespace ProfileImageService.Features.RemoveBg
{
    public class RemoveBgClient
    {
        private readonly HttpClient _httpClient;

        public RemoveBgClient(HttpClient httpClient, ProfileImageServiceSettings settings)
        {
            _httpClient = httpClient;
            _httpClient.BaseAddress = new Uri("https://api.remove.bg/");
            _httpClient.DefaultRequestHeaders.Add("X-Api-Key", settings.RemoveBgApiKey);
        }

        public async Task<Stream> RemoveBackground(MemoryStream stream)
        {
            using var formData = new MultipartFormDataContent
            {
                { new ByteArrayContent(stream.ToArray()), "image_file", "the_file.png" },
                { new StringContent("regular"), "size" },
                { new StringContent("png"), "format" }
            };

            var response = await _httpClient.PostAsync("/v1.0/removebg", formData);

            response.EnsureSuccessStatusCode();

            return await response.Content.ReadAsStreamAsync();
        }
    }
}