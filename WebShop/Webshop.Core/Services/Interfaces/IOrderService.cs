using Webshop.Core.DTOs.Order;
using Webshop.DataLayer.Entities.Order;
using System;
using System.Collections.Generic;
using System.Text;

namespace Webshop.Core.Services.Interfaces
{
    public interface IOrderService
    {
        int AddOrder(string userName, int courseId);

        void OrderPriceSum(int orderId);

        Order GetOrderForUserPanel(string userName, int orderId);

        bool FinallyOrder(string userName, int orderId);

        DiscountType UseDiscount(string userName, int orderId, string discountCode);

        List<Order> GetOrdersForShow(string userName);

        #region Discounts

        void AddDiscount(Discount discount);

        List<Discount> GetAllDiscounts();

        Discount GetDiscountById(int discountId);

        void UpdateDiscount(Discount discount);

        bool IsCodeExist(string code);

        bool IsUserInCourse(string userName, int courseId);

        #endregion
    }
}
