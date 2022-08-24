using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Webshop.DataLayer.Entities.Order
{
    public class Order
    {
        [Key]
        public int OrderId { get; set; }

        [Required]
        public int UserId { get; set; }

        [Required]
        public int PriceSum { get; set; }

        [Required]
        public bool IsFinally { get; set; }

        [Required]
        public DateTime CreateDate { get; set; }


        #region Navigation Props

        [ForeignKey("UserId")]
        public User.User User { get; set; }

        public List<OrderDetail> OrderDetails { get; set; }

        #endregion

    }
}
