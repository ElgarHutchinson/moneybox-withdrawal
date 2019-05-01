﻿using Moneybox.App.DataAccess;
using Moneybox.App.Domain.Services;
using System;

namespace Moneybox.App.Features
{
    public class TransferMoney
    {
        private IAccountRepository accountRepository;
        private INotificationService notificationService;

        public TransferMoney(IAccountRepository accountRepository, INotificationService notificationService)
        {
            this.accountRepository = accountRepository;
            this.notificationService = notificationService;
        }

        public void Execute(Guid fromAccountId, Guid toAccountId, decimal amount)
        {
            var from = this.accountRepository.GetAccountById(fromAccountId);
            var to = this.accountRepository.GetAccountById(toAccountId);

            from.Withdraw(amount);
            to.PayIn(amount);

            if (from.FundsLow())
            {
                notificationService.NotifyFundsLow(from.User.Email);
            }

            if (to.ApproachingPayInLimit())
            {
                notificationService.NotifyApproachingPayInLimit(to.User.Email);
            }
            
            this.accountRepository.Update(from);
            this.accountRepository.Update(to);
        }
    }
}
