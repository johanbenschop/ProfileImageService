using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Web;
using ProfileImageService.Components.FaceApi.Models;
using ProfileImageService.Settings;

namespace ProfileImageService.Components.FaceApi
{
    public class FaceApiClient
    {
        private readonly HttpClient _httpClient;

        public FaceApiClient(HttpClient httpClient, ProfileImageServiceSettings settings)
        {
            _httpClient = httpClient;
            _httpClient.BaseAddress = new Uri(settings.ComputerVisionBaseUrl);
            _httpClient.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", settings.ComputerVisionKey2);
        }

        public async Task<Face[]> DedectFaces(ReadOnlyMemory<byte> sourcePhoto)
        {
            // Request parameters
            var queryString = HttpUtility.ParseQueryString(string.Empty);
            queryString["returnFaceAttributes"] = "age,gender,headPose,smile,facialHair,glasses,emotion,hair,makeup,occlusion,accessories,blur,exposure,noise";
            queryString["recognitionModel"] = "recognition_02";

            // Request body
            using var content = new ReadOnlyMemoryContent(sourcePhoto);
            content.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");

            using var response = await _httpClient.PostAsync("/face/v1.0/detect?" + queryString, content);
            if (response.StatusCode == HttpStatusCode.BadRequest)
            {
                var errorResponse = await response.Content.ReadAsAsync<ErrorResponse>();
                throw new FaceApiException(errorResponse);
            }

            response.EnsureSuccessStatusCode();

            return await response.Content.ReadAsAsync<Face[]>();
        }
    }
}
