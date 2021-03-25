using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

#nullable disable

namespace IT_Homework.Models
{
    public partial class Order
    {
        [Key]
        public int Id { get; set; }
        public byte ProductId { get; set; }
        public byte TableId { get; set; }
        [Column("isServed")]
        public bool? IsServed { get; set; }

        [ForeignKey(nameof(ProductId))]
        [InverseProperty("Orders")]
        public virtual Product Product { get; set; }
    }
}