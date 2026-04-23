using Dapper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using model = API.Model;

namespace API.Repository.CardTransaction
{
    public class CardTransactionRepository: ICardTransactionRepository
    {
        private readonly IDbConnection _dbConnection;

        public CardTransactionRepository(IDbConnection dbConnection)
        {
            _dbConnection = dbConnection;
        }

        public async Task<Guid> CreateAsync(model.CardTransaction transaction)
        {
            const string sql = @"
            INSERT INTO Transactions
            (Id, CardId, Description, Amount, TransactionDate, Currency)
            VALUES
            (@Id, @CardId, @Description, @Amount, @TransactionDate, @Currency);
        ";

            transaction.Id = Guid.NewGuid();

            await _dbConnection.ExecuteAsync(sql, transaction);

            return transaction.Id;
        }

        public async Task<IEnumerable<model.CardTransaction>> GetByCardIdAsync(Guid cardId)
        {
            const string sql = @"
            SELECT *
            FROM Transactions
            WHERE CardId = @CardId
            ORDER BY TransactionDate DESC;
        ";

            return await _dbConnection.QueryAsync<model.CardTransaction>(sql, new { CardId = cardId });
        }

        public async Task<model.CardTransaction?> GetByIdAsync(Guid id)
        {
            const string sql = @"
            SELECT *
            FROM Transactions
            WHERE Id = @Id;
        ";

            return await _dbConnection.QueryFirstOrDefaultAsync<model.CardTransaction>(sql, new { Id = id });
        }
    
    }
}
