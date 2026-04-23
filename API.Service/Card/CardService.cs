using API.Repository.Card;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;
using model = API.Model;    

namespace API.Service.Card
{
    public class CardService: ICardService
    {

        private readonly ICardRepository _repository;
        private readonly ILogger<CardService> _logger;

        public CardService(ICardRepository repository, ILogger<CardService> logger)
        {
            _repository = repository;
            _logger = logger;
        }

        public async Task<model.Card> GetCardByIdAsync(Guid id)
        {
            try
            {
                if (id == Guid.Empty)
                    throw new ArgumentException("Invalid card id");

                var card = await _repository.GetByIdAsync(id);

                if (card is null)
                    throw new NotSupportedException($"Card {id} not found");

                return card;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex,
                    "Unexpected error while getting card. CardId: {CardId}", id);

                throw new ApplicationException("An unexpected error occurred");
            }
        }

        public async Task<Guid> CreateCardAsync(model.Card card)
        {
            if (card is null)
                throw new ArgumentNullException(nameof(card));

            try
            {
                card.Id = Guid.NewGuid();
                card.CreatedAt = DateTime.UtcNow;

                return await _repository.CreateAsync(card);
            }
            catch (Exception ex)
            {                
                _logger.LogError(ex, "Error occurred while creating card");
                throw new InvalidOperationException("Failed to create card due to an unexpected error.", ex);
            }
        }


    }
}
