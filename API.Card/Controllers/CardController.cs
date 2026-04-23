using Microsoft.AspNetCore.Mvc;
using model = API.Model;
using API.Service.Card;

namespace API.Card.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CardController : ControllerBase
    {
        private readonly ICardService _cardService;

        public CardController(ICardService cardService)
        {
            _cardService = cardService;
        }

        // GET api/card/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<model.Card>> Get(Guid id)
        {
            try
            {
                var card = await _cardService.GetCardByIdAsync(id);
                return Ok(card);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (NotSupportedException ex)
            {
                return NotFound(ex.Message);
            }
            catch (Exception)
            {
                return StatusCode(500, "An unexpected error occurred.");
            }
        }

        // POST api/card
        [HttpPost]
        public async Task<ActionResult<Guid>> Post([FromBody] model.Card card)
        {
            try
            {
                var id = await _cardService.CreateCardAsync(card);
                return CreatedAtAction(nameof(Get), new { id }, id);
            }
            catch (ArgumentNullException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (InvalidOperationException ex)
            {
                return StatusCode(500, ex.Message);
            }
            catch (Exception)
            {
                return StatusCode(500, "An unexpected error occurred.");
            }
        }
    }
}
