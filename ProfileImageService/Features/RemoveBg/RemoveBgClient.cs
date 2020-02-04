using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using ProfileImageService.Features.RemoveBg.Models;
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

        public async Task<ReadOnlyMemory<byte>> RemoveBackground(MemoryStream stream)
        {
            using var formData = new MultipartFormDataContent
            {
                { new ByteArrayContent(stream.ToArray()), "image_file", "the_file.png" },
                { new StringContent("regular"), "size" },
                { new StringContent("png"), "format" }
            };

            var response = await _httpClient.PostAsync("/v1.0/removebg", formData);

            if (response.StatusCode == HttpStatusCode.BadRequest)
            {
                var errorResponse = await response.Content.ReadAsAsync<ErrorResponse>();
                throw new RemoveBgException(errorResponse);
            }

            response.EnsureSuccessStatusCode();

            return new ReadOnlyMemory<byte>(await response.Content.ReadAsByteArrayAsync());
        }
    }
}
