using System;

namespace Moneybox.App
{
    public class Account
    {
        public const decimal PayInLimit = 4000m;
        public const decimal NotificationAmount = 500m;

        public Account(Guid id, User user, decimal balance, decimal withdrawn, decimal paidIn)
            : this(id, user, balance)
        {
            Withdrawn = withdrawn;
            PaidIn = paidIn;
        }

        public Account(Guid id, User user, decimal balance)
            : this(id, user)
        {
            Balance = balance;
        }

        public Account(Guid id, User user)
        {
            Id = id;
            User = user;
        }

        public Guid Id { get; private set; }

        public User User { get; private set; }

        public decimal Balance { get; private set; }

        public decimal Withdrawn { get; private set; }

        public decimal PaidIn { get; private set; }
        
        public bool FundsLow()
        {
            return Balance < NotificationAmount;
        }

        public bool ApproachingPayInLimit()
        {
            return PayInLimit - PaidIn < NotificationAmount;
        }

        public void Withdraw(decimal amount)
        {
            var remainingBalance = Balance - amount;

            if (remainingBalance < 0m)
            {
                throw new InvalidOperationException("Insufficient funds for withdrawal");
            }

            Balance = remainingBalance;
            Withdrawn = Withdrawn - amount;
        }

        public void PayIn(decimal amount)
        {
            var newPayIn = PaidIn + amount;

            if (newPayIn > PayInLimit)
            {
                throw new InvalidOperationException("Account pay in limit reached");
            }

            Balance = Balance + amount;
            PaidIn = newPayIn;
        }
    }
}
