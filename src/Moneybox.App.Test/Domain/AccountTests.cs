using System;
using Moq;
using NUnit.Framework;

namespace Moneybox.App.Test.Domain
{
    public class AccountTests
    {
        private Mock<User> _user;

        [SetUp]
        public void Setup()
        {
            _user = new Mock<User>();
        }

        [Test]
        public void WithdrawValid()
        {
            var account = new Account(Guid.NewGuid(), _user.Object, 100);

            account.Withdraw(50);
        }

        [Test]
        public void Withdraw_Maximum()
        {
            var withdrawalAmount = 100;
            var account = new Account(Guid.NewGuid(), _user.Object, withdrawalAmount);
            
            account.Withdraw(withdrawalAmount);
        }

        [Test]
        public void Withdraw_Overdraw()
        {
            var account = new Account(Guid.NewGuid(), _user.Object, 100);

            Assert.Throws<InvalidOperationException>(() => account.Withdraw(100.01m));
        }

        [Test]
        public void Withdraw_Multiple()
        {
            var account = new Account(Guid.NewGuid(), _user.Object, 100);

            account.Withdraw(40);
            account.Withdraw(20);
        }

        [Test]
        public void Withdraw_MultipleOverdraw()
        {
            var account = new Account(Guid.NewGuid(), _user.Object, 100);

            account.Withdraw(40);
            Assert.Throws<InvalidOperationException>(() => account.Withdraw(60.01m));
        }

        [Test]
        public void Withdraw_CorrectBalance()
        {
            var account = new Account(Guid.NewGuid(), _user.Object, 100);

            account.Withdraw(10);
            account.Withdraw(15);

            Assert.AreEqual(75, account.Balance);
        }

        [Test]
        public void Withdraw_CorrectWithdrawal()
        {
            var account = new Account(Guid.NewGuid(), _user.Object, 100);

            account.Withdraw(10);
            account.Withdraw(20);

            Assert.AreEqual(-30, account.Withdrawn);
        }

        [Test]
        public void PayIn()
        {
            var account = new Account(Guid.NewGuid(), _user.Object);

            account.PayIn(1);
        }

        [Test]
        public void PayIn_Maximum()
        {
            var account = new Account(Guid.NewGuid(), _user.Object);

            account.PayIn(Account.PayInLimit);
        }

        [Test]
        public void PayIn_OverLimit()
        {
            var account = new Account(Guid.NewGuid(), _user.Object);

            Assert.Throws<InvalidOperationException>(() => account.PayIn(Account.PayInLimit + 0.01m));
        }

        [Test]
        public void PayIn_Multiple()
        {
            var account = new Account(Guid.NewGuid(), _user.Object);

            account.PayIn(Account.PayInLimit - 1);
            account.PayIn(1);
        }

        [Test]
        public void PayIn_MultipleOverLimit()
        {
            var account = new Account(Guid.NewGuid(), _user.Object);

            account.PayIn(Account.PayInLimit);
            Assert.Throws<InvalidOperationException>(() => account.PayIn(0.01m));
        }

        [Test]
        public void PayIn_CorrectBalance()
        {
            var account = new Account(Guid.NewGuid(), _user.Object, 10);

            account.PayIn(10);
            account.PayIn(15);

            Assert.AreEqual(35, account.Balance);
        }

        [Test]
        public void PayIn_CorrectPaidIn()
        {
            var account = new Account(Guid.NewGuid(), _user.Object, 50);

            account.PayIn(10);
            account.PayIn(5);

            Assert.AreEqual(15, account.PaidIn);
        }

        [Test]
        public void FundsLow_False()
        {
            var account = new Account(Guid.NewGuid(), _user.Object, Account.NotificationAmount);

            Assert.IsFalse(account.FundsLow());
        }

        [Test]
        public void FundsLow_True()
        {
            var account = new Account(Guid.NewGuid(), _user.Object, Account.NotificationAmount - 0.01m);

            Assert.IsTrue(account.FundsLow());
        }

        [Test]
        public void ApproachingPayInLimit_False()
        {
            var payInAmount = Account.PayInLimit - Account.NotificationAmount;
            var account = new Account(Guid.NewGuid(), _user.Object, payInAmount, 0, payInAmount);

            Assert.IsFalse(account.ApproachingPayInLimit());
        }

        [Test]
        public void ApproachingPayInLimit_True()
        {
            var payInAmount = Account.PayInLimit - Account.NotificationAmount + 0.01m;
            var account = new Account(Guid.NewGuid(), _user.Object, payInAmount, 0, payInAmount);

            Assert.IsTrue(account.ApproachingPayInLimit());
        }
    }
}