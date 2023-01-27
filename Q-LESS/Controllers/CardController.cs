using Microsoft.AspNetCore.Cors.Infrastructure;
using Microsoft.AspNetCore.Mvc;
using Q_LESS.Models;
using Q_LESS.Services;

namespace Q_LESS.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class CardController : Controller
    {
        private readonly ICardService _cardService;

        public CardController(ICardService cardService)
        {
            _cardService = cardService;
        }

        [HttpPost]
        [Route("create")]
        public IActionResult CreateCard(Card card)
        {
            var newCard = _cardService.CreateCard(card);
            if (newCard == null)
                return BadRequest("Failed to create card");
            return Ok(newCard);
        }

        [HttpPut]
        [Route("topup")]
        public IActionResult TopUpCard(string cardNumber, decimal amount, decimal cashValue)
        {
            var updatedCard = _cardService.TopUpCard(cardNumber, amount, cashValue);
            if (updatedCard == null)
                return BadRequest("Failed to top up card");
            return Ok(updatedCard);
        }

        [HttpPut]
        [Route("deduct")]
        public IActionResult DeductFare(string cardNumber)
        {
            var updatedCard = _cardService.DeductFare(cardNumber);
            if (updatedCard == null)
                return BadRequest("Failed to deduct fare");
            return Ok(updatedCard);
        }

        [HttpGet]
        [Route("checkbalance")]
        public IActionResult CheckBalance(string cardNumber)
        {
            var card = _cardService.CheckBalance(cardNumber);
            if (card == null)
                return NotFound("Card not found");
            return Ok(card);
        }
    }
}
