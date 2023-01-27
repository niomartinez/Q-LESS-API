using System.ComponentModel.DataAnnotations;

namespace Q_LESS.Models
{
    public class Card
    {
        [Key]
        public string CardNumber { get; set; }
        public CardType CardType { get; set; }
        public decimal InitialLoad { get; set; }
        public DateTime ValidUntil { get; set; }
        public DateTime LastUsedDate { get; set; }
        public decimal Balance { get; set; }
        public DiscountType? DiscountType { get; set; }
        public string IdentificationNumber { get; set; }
        public int TripsTaken { get; set; }
        public decimal DiscountPercent { get; set; }
        public decimal AmountToLoad { get; set; }
        public decimal CashValue { get; set; }
        public decimal Change { get; set; }
        public decimal NewBalance { get; set; }
    }
    public enum CardType
    {
        Regular,
        Discounted
    }
    public enum DiscountType
    {
        SeniorCitizen,
        PWD
    }
}
