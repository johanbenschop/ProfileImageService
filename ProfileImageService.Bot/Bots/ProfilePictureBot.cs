using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Schema;
using ProfileImageService.Components.AzureStorage;
using ProfileImageService.Components.PhotoProcessor;

namespace ProfileImageService.Bot.Bots
{
    public class ProfilePictureBot : ActivityHandler
    {
        private readonly PhotoProcessorService _photoProcessorService;
        private readonly AzureBlobStorageService _azureBlobStorageService;
        private readonly HttpClient _httpClient;

        public ProfilePictureBot(PhotoProcessorService photoProcessorService, AzureBlobStorageService azureBlobStorageService, IHttpClientFactory httpClientFactory)
        {
            _photoProcessorService = photoProcessorService;
            _azureBlobStorageService = azureBlobStorageService;
            _httpClient = httpClientFactory.CreateClient();
        }

        protected override async Task OnMembersAddedAsync(IList<ChannelAccount> membersAdded, ITurnContext<IConversationUpdateActivity> turnContext, CancellationToken cancellationToken)
        {
            foreach (var member in membersAdded)
            {
                if (member.Id != turnContext.Activity.Recipient.Id)
                {
                    var welcomeText = $"Hi there {member.Name}! Give me any photo and I will spark the perfect profile image into existance.";
                    await turnContext.SendActivityAsync(MessageFactory.Text(welcomeText), cancellationToken);
                }
            }
        }

        protected override async Task OnMessageActivityAsync(ITurnContext<IMessageActivity> turnContext, CancellationToken cancellationToken)
        {
            foreach (var attachment in turnContext.Activity.Attachments)
            {
                if (attachment.ContentType != "image/jpeg" && attachment.ContentType != "image/png") continue;

                await turnContext.SendActivityAsync("Thank you! Please standby, this only takes a moment.", cancellationToken: cancellationToken);

                var sourcePhotoBytes = await _httpClient.GetByteArrayAsync(attachment.ContentUrl);
                var sourcePhoto = new ReadOnlyMemory<byte>(sourcePhotoBytes);

                var processedFaces = await _photoProcessorService.ProcessPhoto(sourcePhoto);

                foreach (var processedFace in processedFaces)
                {
                    var url = await _azureBlobStorageService.SaveProcessedFaceAsync(processedFace, cancellationToken);

                    await turnContext.SendActivityAsync(MessageFactory.Attachment(new Attachment("image/png", url)), cancellationToken);
                }
            }
        }
    }
}
