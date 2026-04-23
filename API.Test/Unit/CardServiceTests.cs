using System;
using System.Threading.Tasks;
using Xunit;
using Moq;
using API.Service.Card;
using API.Repository.Card;
using Microsoft.Extensions.Logging;
using model = API.Model;

namespace API.Test.Unit
{
    public class CardServiceTests
    {
        [Fact]
        public async Task GetCardByIdAsync_ReturnsCard_WhenExists()
        {
            // Arrange
            var cardId = Guid.NewGuid();
            var card = new model.Card { Id = cardId, CreditLimit = 1000m };

            var repoMock = new Mock<ICardRepository>();
            repoMock.Setup(r => r.GetByIdAsync(cardId)).ReturnsAsync(card);

            var loggerMock = new Mock<ILogger<CardService>>();

            var service = new CardService(repoMock.Object, loggerMock.Object);

            // Act
            var result = await service.GetCardByIdAsync(cardId);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(cardId, result.Id);
        }

        [Fact]
        public async Task GetCardByIdAsync_Throws_WhenNotFound()
        {
            // Arrange
            var cardId = Guid.NewGuid();

            var repoMock = new Mock<ICardRepository>();
            repoMock.Setup(r => r.GetByIdAsync(cardId)).ReturnsAsync((model.Card?)null);

            var loggerMock = new Mock<ILogger<CardService>>();

            var service = new CardService(repoMock.Object, loggerMock.Object);

            // Act & Assert
            await Assert.ThrowsAsync<NotSupportedException>(() => service.GetCardByIdAsync(cardId));
        }

        [Fact]
        public async Task CreateCardAsync_CreatesCard()
        {
            // Arrange
            var card = new model.Card { CreditLimit = 500m };

            var repoMock = new Mock<ICardRepository>();
            repoMock.Setup(r => r.CreateAsync(It.IsAny<model.Card>())).ReturnsAsync(Guid.NewGuid());

            var loggerMock = new Mock<ILogger<CardService>>();

            var service = new CardService(repoMock.Object, loggerMock.Object);

            // Act
            var id = await service.CreateCardAsync(card);

            // Assert
            Assert.NotEqual(Guid.Empty, id);
            repoMock.Verify(r => r.CreateAsync(It.IsAny<model.Card>()), Times.Once);
        }
    }
}
