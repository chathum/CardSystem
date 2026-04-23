using System;
using System.Threading.Tasks;
using Xunit;
using Moq;
using API.Service.TransactionConversion;
using API.Repository.CardTransaction;
using API.Utils;
using API.Repository.Card;
using Microsoft.Extensions.Logging;
using API.Model.Response;

namespace API.Test.Unit
{
    public class TransactionConversionServiceTests
    {
        [Fact]
        public async Task ConvertTransactionAsync_ReturnsConverted_WhenRateAvailable()
        {
            // Arrange
            var txId = Guid.NewGuid();
            var tx = new API.Model.CardTransaction
            {
                Id = txId,
                Amount = 100m,
                Currency = "USD",
                TransactionDate = DateTime.UtcNow,
                Description = "Test"
            };

            var txRepoMock = new Mock<ICardTransactionRepository>();
            txRepoMock.Setup(r => r.GetByIdAsync(txId)).ReturnsAsync(tx);

            var treasuryMock = new Mock<ITreasuryClient>();
            treasuryMock.Setup(t => t.GetBestRateAsync("EUR", It.IsAny<DateTime>())).ReturnsAsync(0.9m);

            var cardRepoMock = new Mock<ICardRepository>();

            var loggerMock = new Mock<ILogger<TransactionConversionService>>();

            var service = new TransactionConversionService(txRepoMock.Object, treasuryMock.Object, cardRepoMock.Object, loggerMock.Object);

            // Act
            var result = await service.ConvertTransactionAsync(txId, "EUR");

            // Assert
            Assert.NotNull(result);
            Assert.Equal(txId, result.TransactionId);
            Assert.Equal("EUR", result.TargetCurrency);
            Assert.Equal(90m, result.ConvertedAmount);
        }

        [Fact]
        public async Task GetAvailableBalanceAsync_ReturnsBalance_WhenNoTransactions()
        {
            // Arrange
            var cardId = Guid.NewGuid();
            var card = new API.Model.Card { Id = cardId, CreditLimit = 1000m };

            var txRepoMock = new Mock<ICardTransactionRepository>();
            txRepoMock.Setup(r => r.GetByCardIdAsync(cardId)).ReturnsAsync(Array.Empty<API.Model.CardTransaction>());

            var treasuryMock = new Mock<ITreasuryClient>();

            var cardRepoMock = new Mock<ICardRepository>();
            cardRepoMock.Setup(c => c.GetByIdAsync(cardId)).ReturnsAsync(card);

            var loggerMock = new Mock<ILogger<TransactionConversionService>>();

            var service = new TransactionConversionService(txRepoMock.Object, treasuryMock.Object, cardRepoMock.Object, loggerMock.Object);

            // Act
            var result = await service.GetAvailableBalanceAsync(cardId, "USD");

            // Assert
            Assert.NotNull(result);
            Assert.Equal(cardId, result.CardId);
            Assert.Equal(1000m, result.AvailableBalance);
        }
    }
}
