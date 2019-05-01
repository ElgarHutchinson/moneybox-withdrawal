using System;
using Moneybox.App.DataAccess;
using Moneybox.App.Domain.Services;
using Moneybox.App.Features;
using Moq;
using NUnit.Framework;

namespace Moneybox.App.Test.Features
{
    public class WithdrawMoneyTests
    {
        private Mock<IAccountRepository> _accountRepository;
        private Mock<INotificationService> _notificationService;
        private Mock<User> _user;

        private WithdrawMoney _withdrawMoney;
        
        private Guid _id;
        private Account _from;

        [SetUp]
        public void Setup()
        {
            _accountRepository = new Mock<IAccountRepository>();
            _notificationService = new Mock<INotificationService>();
            _user = new Mock<User>();

            _withdrawMoney = new WithdrawMoney(_accountRepository.Object, _notificationService.Object);

            _id = Guid.NewGuid();

            _from = new Account(_id, _user.Object, 1000);

            _accountRepository.Setup(o => o.GetAccountById(_id)).Returns(_from);
        }
        
        [Test]
        public void Execute_SuccessfulWithdrawal()
        {
            var balanceBefore = _from.Balance;
            _withdrawMoney.Execute(_id, 10);

            Assert.AreEqual(-10, _from.Withdrawn);
        }

        [Test]
        public void Execute_NotifyLowFunds()
        {
            _withdrawMoney.Execute(_id, _from.Balance);

            _notificationService.Verify(o => o.NotifyFundsLow(_user.Object.Email));
        }

        [Test]
        public void Execute_NotifyLowFunds_NotCalled()
        {
            _withdrawMoney.Execute(_id, 0);

            _notificationService.Verify(o => o.NotifyFundsLow(_user.Object.Email), Times.Never());
        }
    }
}
