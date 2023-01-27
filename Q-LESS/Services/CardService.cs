using Q_LESS.Data;
using Q_LESS.Models;
using System.Text.RegularExpressions;

namespace Q_LESS.Services
{
    public class CardService : ICardService
    {
        private readonly ApplicationDbContext _dbContext;

        public CardService(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public Card CreateCard(Card card)
        {
            //Validate card
            if (card.CardType == CardType.Discounted)
            {
                if (string.IsNullOrEmpty(card.IdentificationNumber))
                {
                    string message = card.DiscountType == DiscountType.PWD ? "PWD Identification Number is Required." : "Senior Citizen Identification Number is Required.";
                    throw new Exception(message);
                }
                else
                {
                    int expectedLength = card.DiscountType == DiscountType.PWD ? 10 : 12;
                    string pattern = card.DiscountType == DiscountType.PWD ? @"^[a-zA-Z0-9]{2}-[a-zA-Z0-9]{4}-[a-zA-Z0-9]{4}$" : @"^[a-zA-Z0-9]{4}-[a-zA-Z0-9]{4}-[a-zA-Z0-9]{4}$";
                    if (card.IdentificationNumber.Length != expectedLength || !Regex.IsMatch(card.IdentificationNumber, pattern))
                    {
                        string message = card.DiscountType == DiscountType.PWD ? "Invalid PWD Identification Number" : "Invalid Senior Citizen Identification Number";
                        throw new Exception(message);
                    }
                }
            }

            int count = 0;
            // if card type is regular
            if (card.CardType == CardType.Regular)
            {
                // get the number of existing regular cards
                count = _dbContext.Cards.Count(c => c.CardType == CardType.Regular);
                // format the card number
                card.CardNumber = "REG-" + DateTime.Now.ToString("MMddyyyy") + "-" + (count + 1);
                card.InitialLoad = 100;
                card.ValidUntil = DateTime.Now.AddYears(5);
                card.IdentificationNumber = "";
            }
            else
            {
                // get the number of existing discounted cards
                count = _dbContext.Cards.Count(c => c.CardType == CardType.Discounted);
                // format the card number
                card.CardNumber = "DISC-" + DateTime.Now.ToString("MMddyyyy") + "-" + (count + 1);
                card.InitialLoad = 500;
                card.ValidUntil = DateTime.Now.AddYears(3);
            }
            // set defaults
            card.TripsTaken = 0;
            card.DiscountPercent= 0;

            //set balance
            card.Balance = card.InitialLoad;

            //save to db
            _dbContext.Cards.Add(card);
            _dbContext.SaveChanges();
            return card;
        }
        public Card TopUpCard(string cardNumber, decimal amountToLoad, decimal cashValue)
        {
            var card = _dbContext.Cards.FirstOrDefault(c => c.CardNumber == cardNumber);
            if (card == null)
                throw new Exception("Card not found");

            if (amountToLoad < 100 || amountToLoad > 1000)
                throw new Exception("Invalid amount to load. Min amount is 100 and max amount is 1000");

            if (card.Balance + amountToLoad > 10000)
                throw new Exception("Card balance exceeded the maximum of 10000");

            if (amountToLoad > cashValue)
                throw new Exception("Not enough cash value");

            var change = cashValue - amountToLoad;
            var newBalance = card.Balance + amountToLoad;

            //update balance
            card.Balance = newBalance;
            card.LastUsedDate = CardUsed(card).LastUsedDate;
            card.ValidUntil = CardUsed(card).ValidUntil;

            //save to db
            _dbContext.Cards.Update(card);
            _dbContext.SaveChanges();

            card.AmountToLoad = amountToLoad;
            card.CashValue = cashValue;
            card.Change = change;
            card.NewBalance = newBalance;

            return card;
        }

        public Card DeductFare(string cardNumber)
        {
            var card = _dbContext.Cards.FirstOrDefault(c => c.CardNumber == cardNumber);
            if (card == null)
                throw new Exception("Card not found");
            if (card.ValidUntil < DateTime.Now)
                throw new Exception("Card has expired");
            if (card.Balance < 15 && card.CardType == CardType.Regular)
                throw new Exception("Insufficient balance");
            if (card.Balance < 10 && card.CardType == CardType.Discounted)
                throw new Exception("Insufficient balance");
            // deduct fare
            // Implement Section C Discount Definition
            #region Discount Definition Section
            if (card.CardType == CardType.Discounted)
            {
                // Check if the card was last used today
                if (card.LastUsedDate.Date == DateTime.Now.Date)
                {
                    // Check if the card has reached the maximum number of trips for the day
                    if (card.TripsTaken >= 4)
                    {
                        // If maximum number of trips reached, apply base discount of 20%
                        card.DiscountPercent = 0.20m;
                    }
                    else
                    {
                        // If not, apply the base discount plus additional 3% for each trip taken
                        card.DiscountPercent = 0.20m + (0.03m * card.TripsTaken);
                    }
                }
                else
                {
                    // If card was last used on a different day, apply base discount of 20%
                    card.DiscountPercent = 0.20m;
                }
                // Increase the number of trips taken for the card
                card.TripsTaken++;
            }
            else
            {
                // If the card is not a discounted card, set the discount to 0
                card.DiscountPercent = 0;
            }
            #endregion
            card.Balance -= (10 * (1 - (card.DiscountPercent)));
            card.LastUsedDate = CardUsed(card).LastUsedDate;
            card.ValidUntil = CardUsed(card).ValidUntil;
            _dbContext.Cards.Update(card);
            _dbContext.SaveChanges();
            return card;
        }


        public Card CheckBalance(string cardNumber)
        {
            var card = _dbContext.Cards.FirstOrDefault(c => c.CardNumber == cardNumber);
            if (card == null)
                throw new Exception("Card not found");
            return card;
        }

        private static Card CardUsed(Card card)
        {
            card.ValidUntil = card.CardType == CardType.Regular ? DateTime.Now.AddYears(5) : DateTime.Now.AddYears(3);
            card.LastUsedDate = DateTime.Now;

            return card;
        }
    }
}