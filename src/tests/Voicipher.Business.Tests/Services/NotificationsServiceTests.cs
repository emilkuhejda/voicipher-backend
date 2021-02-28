using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using Moq;
using Serilog;
using Voicipher.Business.Services;
using Voicipher.DataAccess;
using Voicipher.Domain.Enums;
using Voicipher.Domain.Interfaces.Repositories;
using Voicipher.Domain.Models;
using Voicipher.Domain.Settings;
using Xunit;

namespace Voicipher.Business.Tests.Services
{
    public class NotificationsServiceTests
    {
        [Fact]
        public async Task SendNotificationsService()
        {
            // Arrange
            var userDeviceRepositoryMock = new Mock<IUserDeviceRepository>();
            var unitOfWorkMock = new Mock<IUnitOfWork>();
            var optionsMock = new Mock<IOptions<AppSettings>>();
            var appSettings = new AppSettings();

            userDeviceRepositoryMock
                .Setup(x => x.GetPlatformSpecificInstallationIdsAsync(It.IsAny<RuntimePlatform>(), It.IsAny<Language>(), It.IsAny<Guid>(), default))
                .ReturnsAsync(new[] { Guid.NewGuid() });
            optionsMock.Setup(x => x.Value).Returns(appSettings);

            var notificationsService = new NotificationsService(
                userDeviceRepositoryMock.Object,
                unitOfWorkMock.Object,
                optionsMock.Object,
                Mock.Of<ILogger>());

            var informationMessage = new InformationMessage
            {
                LanguageVersions = new List<LanguageVersion>
                {
                    new LanguageVersion
                    {
                        Id = Guid.NewGuid(),
                        InformationMessageId = Guid.NewGuid(),
                        Title = "Title",
                        Message = "Message",
                        Description = "Description",
                        Language = Language.English
                    }
                }
            };

            // Act
            await notificationsService.SendAsync(informationMessage);

            // Assert
        }
    }
}
