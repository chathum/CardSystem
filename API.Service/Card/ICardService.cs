using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using model= API.Model;

namespace API.Service.Card
{
    public interface ICardService
    {
        Task<model.Card> GetCardByIdAsync(Guid id);
        Task<Guid> CreateCardAsync(model.Card card);
    }
}
