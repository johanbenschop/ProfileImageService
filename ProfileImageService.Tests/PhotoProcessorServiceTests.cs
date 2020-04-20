using System;
using System.IO;
using System.Text;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Xunit;
using Moq;
using Moq.Protected;
using FluentAssertions;
using ProfileImageService.Components.RemoveBg;
using ProfileImageService.Components.FaceApi;
using ProfileImageService.Components.PhotoProcessor;
using ProfileImageService.Designs;

namespace ProfileImageService.Tests
{
    public class PhotoProcessorServiceTests : IClassFixture<TestFixture>
    {
        private readonly TestFixture _testFixture;

        public PhotoProcessorServiceTests(TestFixture testFixture)
        {
            _testFixture = testFixture;
        }

        [Fact]
        public async Task PhotoProcessorService_Should_ProcessPhoto()
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
            var design = new LiveCodersDesign(); //SentiaDesign();
            var photoProcessorService = new PhotoProcessorService(faceApiClient, removeBgClient, design);
            
            var sourcePhoto = new ReadOnlyMemory<byte>(File.ReadAllBytes("assets/adult-1868750_1280.jpg"));

            // Act
            var processedFaces = await photoProcessorService.ProcessPhoto(sourcePhoto);

            // Assert
            foreach (var processedFace in processedFaces)
            {
                //using var rawFile = File.Create("test-output/actual-profile-image.png");
                //await processedFace.PhotoOfFaceWithoutBackgroundStream.CopyToAsync(rawFile);

                await using var debugImageFile = File.Create("test-output/debug-image.png");
                await debugImageFile.WriteAsync(processedFace.DebugImage);

                await using var croppedPhotoFile = File.Create("test-output/actual-cropped-photo.png");
                await croppedPhotoFile.WriteAsync(processedFace.CroppedPhoto);

                await using var profileImageFile = File.Create("test-output/actual-profile-image.png");
                await profileImageFile.WriteAsync(processedFace.ProfileImage);

                await using var rawProfileImageFile = File.Create("test-output/actual-raw-profile-image.png");
                await rawProfileImageFile.WriteAsync(processedFace.TransparentPhoto);

                var expected = new Memory<byte>(File.ReadAllBytes("assets/expected-profile-image.png"));

                processedFace.TransparentPhoto.Length.Should().BeGreaterThan(10000);
                processedFace.ProfileImage.Length.Should().BeGreaterThan(10000);

                // TODO do some tolerant image comparison
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

            return (new HttpClient(httpMessageHandlerMock.Object), httpMessageHandlerMock);
        }
    }
}
