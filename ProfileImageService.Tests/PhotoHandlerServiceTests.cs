using System.IO;
using System.Text;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Xunit;
using Moq;
using Moq.Protected;
using ProfileImageService.Features.FaceApi;
using ProfileImageService.Features.PhotoHandler;
using ProfileImageService.Features.RemoveBg;

namespace ProfileImageService.Tests
{
    public class PhotoHandlerServiceTests : IClassFixture<TestFixture>
    {
        private readonly TestFixture _testFixture;

        public PhotoHandlerServiceTests(TestFixture testFixture)
        {
            _testFixture = testFixture;
        }

        [Fact]
        public async Task PhotoHandlerService_Should_ProcessPhoto()
        {
            // Arrange
            var (removeBgHttpClient, _) = CreateMockHttpClient(new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = new StreamContent(new MemoryStream(File.ReadAllBytes("assets/removebg-response.png"))),
            });

            var (faceApiHttpClient, _) = CreateMockHttpClient(new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent(File.ReadAllText("assets/face-api-response.json"), Encoding.UTF8, "application/json"),
            });

            var removeBgClient = new RemoveBgClient(removeBgHttpClient, _testFixture.Settings);
            var faceApiClient = new FaceApiClient(faceApiHttpClient, _testFixture.Settings);
            var photoHandlerService = new PhotoHandlerService(faceApiClient, removeBgClient);
            
            var photoStream = new MemoryStream(File.ReadAllBytes("assets/adult-1868750_1280.jpg"));

            // Act
            var processedFaces = await photoHandlerService.ProcessPhoto(photoStream);

            // Assert
            foreach (var processedFace in processedFaces)
            {
                //using var rawFile = File.Create("test-output/actual-profile-image.png");
                //await processedFace.PhotoOfFaceWithoutBackgroundStream.CopyToAsync(rawFile);

                using var profileImageFile = File.Create("test-output/actual-profile-image.png");
                await processedFace.ProfileImageStream.CopyToAsync(profileImageFile);
                processedFace.ProfileImageStream.Seek(0, SeekOrigin.Begin);

                var expectedStream = new MemoryStream(File.ReadAllBytes("assets/expected-profile-image.png"));
                // TODO figured out how to assert
            }
        }

        private static (HttpClient, Mock<HttpMessageHandler>) CreateMockHttpClient(HttpResponseMessage responseMessage)
        {
            var httpMessageHandlerMock = new Mock<HttpMessageHandler>(MockBehavior.Strict);

            httpMessageHandlerMock.Protected().Setup<Task<HttpResponseMessage>>("SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(responseMessage)
                .Verifiable();


            //HttpClientFactory.Create(new DelegatingHandler()

            return (new HttpClient(httpMessageHandlerMock.Object), httpMessageHandlerMock);
        }
    }
}
