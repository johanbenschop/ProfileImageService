using System.IO;
using Microsoft.Extensions.Configuration;
using ProfileImageService.Settings;

namespace ProfileImageService.Tests
{
    public sealed class TestFixture
    {
        public ProfileImageServiceSettings Settings { get; set; }

        public TestFixture()
        {
            var configuration = new ConfigurationBuilder()
               .AddJsonFile("appsettings.json", true)
               .AddUserSecrets("8A5141D1-33BC-4B1E-8051-F57FB87FC5C5")
               .Build();

            Settings = new ProfileImageServiceSettings();

            configuration.Bind(Settings);

            Directory.CreateDirectory("test-output");
        }
    }
}
