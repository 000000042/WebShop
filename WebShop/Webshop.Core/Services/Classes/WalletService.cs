using Webshop.Core.DTOs;
using Webshop.Core.Services.Interfaces;
using Webshop.DataLayer.Context;
using Webshop.DataLayer.Entities.Wallet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Webshop.Core.Services.Classes
{
    public class WalletService : IWalletService
    {
        #region Dependencies

        private WebshopContext _context;

        public WalletService(WebshopContext context)
        {
            _context = context;
        }

        #endregion

        public int AddWallet(Wallet wallet)
        {
            _context.Wallets.Add(wallet);
            _context.SaveChanges();
            return wallet.WalletId;
        }

        public int ChargeWallet(int userId, int amount, string description, bool isPay = false)
        {
            Wallet chargeWallet = new Wallet()
            {
                Amount = amount,
                CreateDate = DateTime.Now,
                Description = description,
                IsPay = isPay,
                TypeId = 1,
                UserId = userId
            };

            return AddWallet(chargeWallet);
        }

        public List<CheckWalletViewModel> GetUserWallets(int userId)
        {
            return _context.Wallets
                .Where(w => w.UserId == userId && w.IsPay)
                .Select(w => new CheckWalletViewModel()
                {
                    Amount = w.Amount,
                    Type = w.TypeId,
                    DateTime = w.CreateDate,
                    Description = w.Description
                }).ToList();
        }

        public Wallet GetWalletByWalletId(int walletId)
        {
            return _context.Wallets.Single(w => w.WalletId == walletId);
        }

        public void UpdateWallet(Wallet wallet)
        {
            _context.Wallets.Update(wallet);
            _context.SaveChanges();
        }

        public int UserWalletAmount(int userId)
        {
            List<Wallet> deposit = _context.Wallets
                .Where(w => w.UserId == userId && w.TypeId == 1 && w.IsPay)
                .ToList();

            List<Wallet> withdraw = _context.Wallets
                .Where(w => w.UserId == userId && w.TypeId == 2 && w.IsPay)
                .ToList();

            return (deposit.Sum(w => w.Amount) - withdraw.Sum(w => w.Amount));
        }
    }
}
