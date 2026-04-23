using API.Repository.Card;
using API.Repository.CardTransaction;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;
using model=API.Model;

namespace API.Service.CardTransaction
{
    public class TransactionService : ITransactionService
    {
        private readonly ICardTransactionRepository _repository;
        private readonly ICardRepository _cardRepository;
        private readonly ILogger<TransactionService> _logger;

        public TransactionService(
            ICardTransactionRepository repository,
            ICardRepository cardRepository,
            ILogger<TransactionService> logger)
        {
            _repository = repository;
            _cardRepository = cardRepository;
            _logger = logger;
        }

        public async Task CreateAsync(model.CardTransaction cardTransaction)
        {
            try
            {
                if (cardTransaction is null)
                {
                    _logger.LogWarning("CreateAsync called with null request");
                    throw new ArgumentNullException(nameof(cardTransaction));
                }

                if (cardTransaction.Amount <= 0)
                {
                    _logger.LogWarning("Invalid amount: {Amount} for CardId: {CardId}",
                        cardTransaction.Amount, cardTransaction.CardId);

                    throw new ArgumentException("Amount must be greater than zero.");
                }

                var card = await _cardRepository.GetByIdAsync(cardTransaction.CardId);

                if (card is null)
                {
                    _logger.LogWarning("Card not found. CardId: {CardId}", cardTransaction.CardId);
                    throw new KeyNotFoundException($"Card {cardTransaction.CardId} not found.");
                }

                cardTransaction.Id = Guid.NewGuid();

                var result = await _repository.CreateAsync(cardTransaction);

                _logger.LogInformation("Transaction created successfully. TransactionId: {TransactionId}",
                    result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error while creating transaction for CardId: {CardId}",
                    cardTransaction?.CardId);

                throw new Exception("An error occurred while processing the transaction.", ex);
            }
        }
    }
}
