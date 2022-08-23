using Webshop.Core.DTOs.Order;
using Webshop.Core.Services.Interfaces;
using Webshop.DataLayer.Context;
using Webshop.DataLayer.Entities.Course;
using Webshop.DataLayer.Entities.Order;
using Webshop.DataLayer.Entities.Wallet;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Webshop.Core.Services.Classes
{
    public class OrderService : IOrderService
    {
        private WebshopContext _context;
        private IWalletService _walletService;

        public OrderService(WebshopContext context, IWalletService walletService)
        {
            _context = context;
            _walletService = walletService;
        }

        public void AddDiscount(Discount discount)
        {
            if (discount != null)
            {
                _context.Discounts.Add(discount);
                _context.SaveChanges();
            }
        }

        public int AddOrder(string userName, int courseId)
        {
            int userId = _context.Users.FirstOrDefault(u => u.UserName == userName).UserId;

            Course course = _context.Courses.FirstOrDefault(c => c.CourseId == courseId);
            Order order = _context.Orders.FirstOrDefault(u => u.UserId == userId && !u.IsFinally);
            if (order != null && course != null)
            {
                OrderDetail detail = _context.OrderDetails.FirstOrDefault(d => d.OrderId == order.OrderId && d.CourseId == course.CourseId);
                if (detail != null)
                {
                    detail.Count += 1;
                    _context.OrderDetails.Update(detail);
                }
                else
                {
                    detail = new OrderDetail()
                    {
                        CourseId = course.CourseId,
                        OrderId = order.OrderId,
                        Price = course.CoursePrice,
                        Count = 1
                    };
                    _context.OrderDetails.Add(detail);
                }
                _context.SaveChanges();
                OrderPriceSum(order.OrderId);
            }
            else
            {
                order = new Order()
                {
                    UserId = userId,
                    PriceSum = course.CoursePrice,
                    CreateDate = DateTime.Now,
                    IsFinally = false,
                    OrderDetails = new List<OrderDetail>()
                    {
                        new OrderDetail()
                        {
                            CourseId = courseId,
                            Count = 1,
                            Price = course.CoursePrice
                        }
                    }
                };
                _context.Orders.Add(order);
                _context.SaveChanges();
            }

            return order.OrderId;
        }

        public bool FinallyOrder(string userName, int orderId)
        {
            int userId = _context.Users.FirstOrDefault(u => u.UserName == userName).UserId;
            Order order = _context.Orders.Include(d => d.OrderDetails)
                .ThenInclude(c => c.Course)
                .FirstOrDefault(o => o.OrderId == orderId);
            if (order == null || order.IsFinally)
            {
                return false;
            }
            if (_walletService.UserWalletAmount(userId) >= order.PriceSum)
            {
                order.IsFinally = true;
                _walletService.AddWallet(new Wallet()
                {
                    Amount = order.PriceSum,
                    CreateDate = DateTime.Now,
                    Description = "فاکتور شماره ی " + order.OrderId,
                    IsPay = true,
                    TypeId = 2,
                    UserId = userId
                });
                _context.Orders.Update(order);
                foreach (var detail in order.OrderDetails)
                {
                    _context.UserCourses.Add(new UserCourse()
                    {
                        UserId = userId,
                        CourseId = detail.CourseId
                    });
                }
                _context.SaveChanges();

                return true;
            }
            return false;
        }

        public List<Discount> GetAllDiscounts()
        {
            return _context.Discounts.ToList();
        }

        public Discount GetDiscountById(int discountId)
        {
            return _context.Discounts.SingleOrDefault(d => d.DiscountId == discountId);
        }

        public Order GetOrderForUserPanel(string userName, int orderId)
        {
            int userId = _context.Users.FirstOrDefault(u => u.UserName == userName).UserId;

            Order order = _context.Orders.Include(o => o.OrderDetails)
                .ThenInclude(od => od.Course)
                .FirstOrDefault(o => o.OrderId == orderId && o.UserId == userId);

                return order;
        }

        public List<Order> GetOrdersForShow(string userName)
        {
            int userId = _context.Users.SingleOrDefault(u => u.UserName == userName).UserId;
            return _context.Orders.Where(o => o.UserId == userId).ToList();
        }

        public bool IsCodeExist(string code)
        {
            return _context.Discounts.Any(d => d.DiscountCode == code);
        }

        public bool IsUserInCourse(string userName, int courseId)
        {
            int userId = _context.Users.SingleOrDefault(u => u.UserName == userName).UserId;
            return _context.UserCourses.Any(uc => uc.UserId == userId && uc.CourseId == courseId);
        }

        public void OrderPriceSum(int orderId)
        {
            Order order = _context.Orders.Find(orderId);
            if (order != null)
            {
                List<OrderDetail> orderDetails = _context.OrderDetails.Where(d => d.OrderId == orderId).ToList();
                int detailPriceSum = 0;
                foreach (var detail in orderDetails)
                {
                    detailPriceSum += detail.Count * detail.Price;
                }
                order.PriceSum = detailPriceSum;
                _context.Orders.Update(order);
                _context.SaveChanges();
            }
        }

        public void UpdateDiscount(Discount discount)
        {
            _context.Discounts.Update(discount);
            _context.SaveChanges();
        }

        public DiscountType UseDiscount(string userName, int orderId, string discountCode)
        {
            int userId = _context.Users.SingleOrDefault(d => d.UserName == userName).UserId;
            var order = _context.Orders.Include(od => od.OrderDetails).SingleOrDefault(o => o.OrderId == orderId && o.UserId == userId);

            if (order == null)
                return DiscountType.NotFound;

            var discount = _context.Discounts.SingleOrDefault(d => d.DiscountCode == discountCode);

            if (discount == null)
                return DiscountType.NotFound;

            if (discount.UsableCount != null && discount.UsableCount < 1)
                return DiscountType.Finished;

            if (discount.StartDate != null && discount.StartDate >= DateTime.Now)
                return DiscountType.ExpireDate;

            if (discount.EndDate != null && discount.EndDate <= DateTime.Now)
                return DiscountType.ExpireDate;

            bool checkIfUsed = _context.UserDiscounts.Any(ud => ud.UserId == userId && ud.DiscountId == discount.DiscountId);
            if (checkIfUsed)
                return DiscountType.Used;

            int discountAmount = (order.PriceSum * discount.DiscountPercent) / 100;
            order.PriceSum = order.PriceSum - discountAmount;

            foreach (var detail in order.OrderDetails)
            {
                int discountAmountForDetail = (detail.Price * discount.DiscountPercent) / 100;
                detail.Price = detail.Price - discountAmountForDetail;
            }
            _context.UserDiscounts.Add(new DataLayer.Entities.User.UserDiscount()
            {
                DiscountId = discount.DiscountId,
                UserId = userId
            });

            _context.Orders.Update(order);

            if (discount.UsableCount != null)
            {
                discount.UsableCount -= 1;
            }
            _context.Discounts.Update(discount);

            _context.SaveChanges();

            return DiscountType.Success;
        }
    }
}
