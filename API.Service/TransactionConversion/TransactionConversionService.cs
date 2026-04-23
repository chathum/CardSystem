using API.Model.Response;
using API.Repository.Card;
using API.Repository.CardTransaction;
using API.Service.CardTransaction;
using API.Utils;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;

namespace API.Service.TransactionConversion
{
    public class TransactionConversionService: ITransactionConversionService
    {
        private readonly ICardTransactionRepository _cardTransactionRepository;
        private readonly ITreasuryClient _treasuryClient;
        private readonly ICardRepository _cardRepository;        
        private readonly ILogger<TransactionConversionService> _logger;

        public TransactionConversionService(
            ICardTransactionRepository cardTransactionRepository,
            ITreasuryClient treasuryClient,
            ICardRepository cardRepository,
        ILogger<TransactionConversionService> logger)
        {
            _cardTransactionRepository = cardTransactionRepository;
            _treasuryClient = treasuryClient;
            _cardRepository = cardRepository;
            _logger = logger;
        }

        public async Task<ConvertedTransactionResponse> ConvertTransactionAsync(Guid transactionId, string targetCurrency)
        {
            try
            {
              
                var cardTransaction = await _cardTransactionRepository.GetByIdAsync(transactionId);

                if (cardTransaction is null)
                {
                    _logger.LogWarning("Transaction not found. TransactionId: {TransactionId}", transactionId);
                    throw new KeyNotFoundException($"Transaction {transactionId} not found.");
                }

                if (string.IsNullOrWhiteSpace(targetCurrency))
                {
                    _logger.LogWarning("Invalid target currency provided. TransactionId: {TransactionId}", transactionId);
                    throw new ArgumentException("Target currency cannot be null or empty.", nameof(targetCurrency));
                }

               
                if (string.Equals(cardTransaction.Currency, targetCurrency, StringComparison.OrdinalIgnoreCase))
                {
                    _logger.LogInformation("No conversion needed. Same currency for TransactionId: {TransactionId}", transactionId);

                    return new ConvertedTransactionResponse
                    {
                        TransactionId = cardTransaction.Id,
                        Description = cardTransaction.Description,
                        TransactionDate = cardTransaction.TransactionDate,
                        OriginalAmount = cardTransaction.Amount,
                        OriginalCurrency = cardTransaction.Currency,
                        TargetCurrency = targetCurrency,
                        ExchangeRate = 1,
                        ConvertedAmount = cardTransaction.Amount,
                        RateDateUsed = cardTransaction.TransactionDate
                    };
                }

                _logger.LogInformation("Fetching exchange rate. Currency: {TargetCurrency}, TransactionDate: {Date}",
                    targetCurrency, cardTransaction.TransactionDate);

                var rate = await _treasuryClient.GetBestRateAsync(
                    targetCurrency,
                    cardTransaction.TransactionDate);

                if (rate is null)
                {
                    _logger.LogWarning("Exchange rate not found. Currency: {TargetCurrency}", targetCurrency);
                    throw new InvalidOperationException(
                        $"No exchange rate found within allowed range for {targetCurrency}");
                }

                var convertedAmount = cardTransaction.Amount * rate;

                _logger.LogInformation("Conversion successful. TransactionId: {TransactionId}, Rate: {Rate}, ConvertedAmount: {ConvertedAmount}",
                    transactionId, rate, convertedAmount);

                return new ConvertedTransactionResponse
                {
                    TransactionId = cardTransaction.Id,
                    Description = cardTransaction.Description,
                    TransactionDate = cardTransaction.TransactionDate,
                    OriginalAmount = cardTransaction.Amount,
                    OriginalCurrency = cardTransaction.Currency,
                    TargetCurrency = targetCurrency,
                    ExchangeRate = rate,
                    ConvertedAmount = convertedAmount
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex,
                    "Error occurred while converting transaction. TransactionId: {TransactionId}, TargetCurrency: {TargetCurrency}",
                    transactionId, targetCurrency);

                throw;
            }
        }

        public async Task<BalanceResponse> GetAvailableBalanceAsync(Guid cardId, string targetCurrency)
        {
            try
            {
                _logger.LogInformation("Calculating available balance. CardId: {CardId}, TargetCurrency: {Currency}",
                    cardId, targetCurrency);

                if (string.IsNullOrWhiteSpace(targetCurrency))
                    throw new ArgumentException("Target currency is required.", nameof(targetCurrency));

                targetCurrency = targetCurrency.ToUpperInvariant();

                var card = await _cardRepository.GetByIdAsync(cardId);
                if (card is null)
                    throw new KeyNotFoundException($"Card {cardId} not found.");

                var transactions = await _cardTransactionRepository.GetByCardIdAsync(cardId);

                if (transactions == null || !transactions.Any())
                {
                    _logger.LogInformation("No transactions found. CardId: {CardId}", cardId);

                    return new BalanceResponse
                    {
                        CardId = card.Id,
                        Currency = targetCurrency,
                        AvailableBalance = card.CreditLimit,
                        ExchangeRate = 1,
                        ConvertedBalance = card.CreditLimit,
                        RateDateUsed = DateTime.UtcNow
                    };
                }

                decimal? totalSpentInTarget = 0;

                foreach (var tx in transactions)
                {
                    decimal? amount = tx.Amount;

                    if (!string.Equals(tx.Currency, targetCurrency, StringComparison.OrdinalIgnoreCase))
                    {
                        var rate = await _treasuryClient.GetBestRateAsync(targetCurrency, tx.TransactionDate);

                        if (rate is null)
                        {
                            _logger.LogWarning("Missing FX rate. From: {From} To: {To}", tx.Currency, targetCurrency);
                            throw new InvalidOperationException($"Missing FX rate for {tx.Currency} -> {targetCurrency}");
                        }

                        amount *= rate;
                    }

                    totalSpentInTarget += amount;
                }

                decimal? availableBalance = card.CreditLimit - totalSpentInTarget;

                _logger.LogInformation("Balance calculated. CardId: {CardId}, TotalSpent: {Spent}, Available: {Available}",
                    cardId, totalSpentInTarget, availableBalance);

                return new BalanceResponse
                {
                    CardId = card.Id,
                    Currency = targetCurrency,
                    AvailableBalance = availableBalance,
                    ConvertedBalance = availableBalance,
                    RateDateUsed = DateTime.UtcNow
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex,
                    "Error calculating available balance. CardId: {CardId}, TargetCurrency: {Currency}",
                    cardId, targetCurrency);

                throw;
            }
        }
    }
}
