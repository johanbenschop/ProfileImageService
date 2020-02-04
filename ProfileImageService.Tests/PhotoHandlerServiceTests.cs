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
using System;
using FluentAssertions;

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
            
            var sourcePhoto = new ReadOnlyMemory<byte>(File.ReadAllBytes("assets/adult-1868750_1280.jpg"));

            // Act
            var processedFaces = await photoHandlerService.ProcessPhoto(sourcePhoto);

            // Assert
            foreach (var processedFace in processedFaces)
            {
                //using var rawFile = File.Create("test-output/actual-profile-image.png");
                //await processedFace.PhotoOfFaceWithoutBackgroundStream.CopyToAsync(rawFile);

                using var profileImageFile = File.Create("test-output/actual-profile-image.png");
                await profileImageFile.WriteAsync(processedFace.ProfileImage);
                using var rawProfileImageFile = File.Create("test-output/actual-raw-profile-image.png");
                await rawProfileImageFile.WriteAsync(processedFace.PhotoOfFaceWithoutBackground);

                var expected = new Memory<byte>(File.ReadAllBytes("assets/expected-profile-image.png"));

                processedFace.PhotoOfFaceWithoutBackground.Length.Should().BeGreaterThan(10000);
                processedFace.ProfileImage.Length.Should().BeGreaterThan(10000);
                //processedFace.ProfileImage.Should().Be(expected);
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
