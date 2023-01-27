using Q_LESS.Models;

namespace Q_LESS.Services
{
    public interface ICardService
    {
        Card CreateCard(Card card);
        Card TopUpCard(string cardNumber, decimal amount, decimal cashValue);
        Card DeductFare(string cardNumber);
        Card CheckBalance(string cardNumber);
    }
}
