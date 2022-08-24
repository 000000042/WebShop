using Webshop.DataLayer.Entities.Order;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Webshop.DataLayer.Entities.User
{
    public class UserDiscount
    {
        [Key]
        public int UD_Id { get; set; }

        public int UserId { get; set; }

        public int DiscountId { get; set; }


        #region Navigation Props

        [ForeignKey("UserId")]
        public User User { get; set; }

        [ForeignKey("DiscountId")]
        public Discount Discount { get; set; }

        #endregion
    }
}
