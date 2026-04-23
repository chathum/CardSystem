using API.Repository.Card;
using API.Repository.CardTransaction;
using API.Service.Card;
using API.Service.CardTransaction;
using API.Service.TransactionConversion;
using API.Utils;
using System.Data;
using System.Data.SqlClient;

var builder = WebApplication.CreateBuilder(args);

// configure services
builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();



var configuration = builder.Configuration;
var connectionString = configuration.GetConnectionString("DefaultConnection") ?? string.Empty;

builder.Services.AddTransient<IDbConnection>(sp => new SqlConnection(connectionString));

builder.Services.AddTransient<ICardRepository, CardRepository>();
builder.Services.AddTransient<ICardService, CardService>();

builder.Services.AddTransient<ICardTransactionRepository, CardTransactionRepository>();
builder.Services.AddTransient<ITransactionService, TransactionService>();

builder.Services.AddTransient<ITransactionConversionService, TransactionConversionService>();

builder.Services.AddHttpClient<ITreasuryClient, TreasuryClient>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
