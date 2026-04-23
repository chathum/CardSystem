using System;
using System.Collections.Generic;
using System.Text;
using model= API.Model; 

namespace API.Repository.CardTransaction
{
    public interface ICardTransactionRepository
    {
        Task<Guid> CreateAsync(model.CardTransaction transaction);

        Task<IEnumerable<model.CardTransaction>> GetByCardIdAsync(Guid cardId);

        Task<model.CardTransaction?> GetByIdAsync(Guid id);
    }
}
