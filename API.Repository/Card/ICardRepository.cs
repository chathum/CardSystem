using System;
using System.Collections.Generic;
using System.Text;
using model = API.Model;

namespace API.Repository.Card
{
    public interface ICardRepository
    {
        Task<Guid> CreateAsync(model.Card card);
        Task<model.Card?> GetByIdAsync(Guid id);
    }
}
