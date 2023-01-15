
namespace Ironclad.Entities
{
    class Loan
    {
        public int Rounds { get; set; }
        public int InterestRate { get; set; }
        public int Payment { get; set; }
        public int Amount { get; set; }
        public string ID { get; set; }

        public Loan(int rounds, int interestRate, int payment, int amount, int id)
        {
            Rounds = rounds > 0 ? rounds : 0;
            InterestRate = interestRate > 0 ? interestRate : 0;
            Payment = payment > 0 ? payment : 0;
            Amount = amount > 0 ? amount : 0;
            ID = $"loan{id}";
        }
    }
}
