using Microsoft.AspNetCore.Mvc;
using API.Service.TransactionConversion;
using API.Model.Response;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace API.Finance.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class FinanceController : ControllerBase
    {
        private readonly ITransactionConversionService _conversionService;
        private readonly ILogger<FinanceController> _logger;

        public FinanceController(ITransactionConversionService conversionService, ILogger<FinanceController> logger)
        {
            _conversionService = conversionService;
            _logger = logger;
        }

        [HttpGet("transactions/{transactionId}/convert")]
        public async Task<IActionResult> ConvertTransactionAsync(Guid transactionId, [FromQuery] string targetCurrency)
        {
            try
            {
                var result = await _conversionService.ConvertTransactionAsync(transactionId, targetCurrency);
                return Ok(result);
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogWarning(ex, "Conversion could not be performed. TransactionId: {TransactionId}", transactionId);
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error converting transaction. TransactionId: {TransactionId}", transactionId);
                return StatusCode(500, "An unexpected error occurred while converting the transaction.");
            }
        }

        [HttpGet("cards/{cardId}/balance")]
        public async Task<IActionResult> GetAvailableBalanceAsync(Guid cardId, [FromQuery] string targetCurrency)
        {
            try
            {
                var result = await _conversionService.GetAvailableBalanceAsync(cardId, targetCurrency);
                return Ok(result);
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogWarning(ex, "Failed to calculate balance. CardId: {CardId}", cardId);
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error calculating balance. CardId: {CardId}", cardId);
                return StatusCode(500, "An unexpected error occurred while calculating the balance.");
            }
        }
    }
}
