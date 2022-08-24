using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Webshop.DataLayer.Entities.Order
{
    public class OrderDetail
    {
        [Key]
        public int DetailId { get; set; }

        [Required]
        public int CourseId { get; set; }

        [Required]
        public int OrderId { get; set; }

        [Required]
        public int Price { get; set; }

        [Required]
        public int Count { get; set; }


        #region Navigation Props

        [ForeignKey("CourseId")]
        public Course.Course Course { get; set; }

        [ForeignKey("OrderId")]
        public Order Order { get; set; }

        #endregion
    }
}
