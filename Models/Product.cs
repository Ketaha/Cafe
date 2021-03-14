using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

#nullable disable

namespace IT_Homework.Models
{
    public partial class Product
    {
        public Product()
        {
            Orders = new HashSet<Order>();
        }

        [Key]
        public byte Id { get; set; }
        [StringLength(30)]
        public string Name { get; set; }
        [Column(TypeName = "money")]
        public decimal? Price { get; set; }

        [InverseProperty(nameof(Order.Product))]
        public virtual ICollection<Order> Orders { get; set; }
    }
}
