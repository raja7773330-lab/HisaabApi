
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace HisaabApi.Models;

public class Transaction
{
    public int Id { get; set; }

    [Required]
    [MaxLength(20)]
    public string Type { get; set; } = "Expense";

    [Range(0.01, 999999999)]
    public decimal Amount { get; set; }

    [Required]
    public int CategoryId { get; set; }

    [JsonIgnore]
    public Category Category { get; set; } = null!;

    public DateTime Date { get; set; } = DateTime.Now;

    public DateTime CreatedAt { get; set; } = DateTime.Now;

    [MaxLength(500)]
    public string? Note { get; set; }
}
