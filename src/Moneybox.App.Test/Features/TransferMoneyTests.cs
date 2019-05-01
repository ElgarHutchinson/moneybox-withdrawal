using System;
using Moneybox.App.DataAccess;
using Moneybox.App.Domain.Services;
using Moneybox.App.Features;
using Moq;
using NUnit.Framework;

namespace Moneybox.App.Test.Features
{
    public class TransferMoneyTests
    {
        private Mock<IAccountRepository> _accountRepository;
        private Mock<INotificationService> _notificationService;
        private Mock<User> _user;

        private TransferMoney _transferMoney;

        private Guid _fromId;
        private Guid _toId;
        private Account _from;
        private Account _to;

        [SetUp]
        public void Setup()
        {
            _accountRepository = new Mock<IAccountRepository>();
            _notificationService = new Mock<INotificationService>();
            _user = new Mock<User>();

            _transferMoney = new TransferMoney(_accountRepository.Object, _notificationService.Object);

            _fromId = Guid.NewGuid();
            _toId = Guid.NewGuid();

            _from = new Account(_fromId, _user.Object, Account.PayInLimit);
            _to = new Account(_toId, _user.Object, 0);

            _accountRepository.Setup(o => o.GetAccountById(_fromId)).Returns(_from);
            _accountRepository.Setup(o => o.GetAccountById(_toId)).Returns(_to);
        }

        [Test]
        public void Execute_SuccessfulWithdrawal()
        {
            _transferMoney.Execute(_fromId, _toId, 5);

            Assert.AreEqual(-5, _from.Withdrawn);
        }

        [Test]
        public void Execute_SuccessfulPayIn()
        {
            _transferMoney.Execute(_fromId, _toId, 5);

            Assert.AreEqual(5, _to.PaidIn);
        }

        [Test]
        public void Execute_NotifyLowFunds()
        {
            _transferMoney.Execute(_fromId, _toId, _from.Balance);

            _notificationService.Verify(o => o.NotifyFundsLow(_user.Object.Email));
        }

        [Test]
        public void Execute_NotifyLowFunds_NotCalled()
        {
            _transferMoney.Execute(_fromId, _toId, 0);
            
            _notificationService.Verify(o => o.NotifyFundsLow(_user.Object.Email), Times.Never());
        }

        [Test]
        public void Execute_ApproachingPayInLimit()
        {
            _transferMoney.Execute(_fromId, _toId, _from.Balance);

            _notificationService.Verify(o => o.NotifyApproachingPayInLimit(_user.Object.Email));
        }

        [Test]
        public void Execute_ApproachingPayInLimit_NotCalled()
        {
            _transferMoney.Execute(_fromId, _toId, 0);

            _notificationService.Verify(o => o.NotifyApproachingPayInLimit(_user.Object.Email), Times.Never());
        }
    }
}
