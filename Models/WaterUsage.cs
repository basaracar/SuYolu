using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SuYolu.Models
{
    public class WaterUsage
    {
       
        [Required]
        public String UserId { get; set; }

        [Required]
        public double AmountInLiters { get; set; }
    }
}