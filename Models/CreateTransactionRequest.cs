
using System;
using System.ComponentModel.DataAnnotations;

namespace HisaabApi.Models
{
    public class CreateTransactionRequest
    {
        [Required]
        [RegularExpression("^(Income|Expense)$")]
        public string Type { get; set; } = "Expense";

        [Range(0.01, 999999999)]
        public decimal Amount { get; set; }

        [Range(1, int.MaxValue)]
        public int CategoryId { get; set; }

        public DateTime Date { get; set; } = DateTime.Now;

        [MaxLength(500)]
        public string? Note { get; set; }
    }
}
