using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using API.Model;
using Dapper;
using model = API.Model;

namespace API.Repository.Card
{

    public class CardRepository : ICardRepository
    {
        private readonly IDbConnection _dbConnection;
        public CardRepository(IDbConnection dbConnection)
        {
            _dbConnection = dbConnection;
        }

        public async Task<Guid> CreateAsync(model.Card card)
        {
            var sql = @"INSERT INTO Cards (Id, CreditLimit, CreatedAt)
                    VALUES (@Id, @CreditLimit, @CreatedAt)";

            await _dbConnection.ExecuteAsync(sql, card);
            return card.Id;
        }

        public async Task<model.Card?> GetByIdAsync(Guid id)
        {
            var sql = "SELECT * FROM Cards WHERE Id = @Id";
            return await _dbConnection.QueryFirstOrDefaultAsync<model.Card>(sql, new { Id = id });
        }
    }
}
