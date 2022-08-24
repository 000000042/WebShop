using Webshop.Core.DTOs;
using Webshop.DataLayer.Entities.Wallet;
using System;
using System.Collections.Generic;
using System.Text;

namespace Webshop.Core.Services.Interfaces
{
    public interface IWalletService
    {
        #region Get

        List<CheckWalletViewModel> GetUserWallets(int userId);
        int UserWalletAmount(int userId);
        Wallet GetWalletByWalletId(int walletId);

        #endregion

        #region Add

        int ChargeWallet(int userId, int amount, string description, bool isPay = false);
        int AddWallet(Wallet wallet);

        #endregion

        #region Edit

        void UpdateWallet(Wallet wallet);

        #endregion

        #region Remove



        #endregion
    }
}
