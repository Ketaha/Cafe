using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

#nullable disable

namespace IT_Homework.Models
{
    [Table("Profit")]
    public partial class Profit
    {
        [Key]
        public int Id { get; set; }
        [Column("Profit", TypeName = "money")]
        public decimal? Profit1 { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? EarnedOn { get; set; }
    }
}
