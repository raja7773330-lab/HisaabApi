
using System.ComponentModel.DataAnnotations;

namespace HisaabApi.Models;

public class Category
{
    public int Id { get; set; }

    [Required]
    [MaxLength(100)]
    public string Name { get; set; } = string.Empty;

    [Required]
    [MaxLength(20)]
    public string Type { get; set; } = "Expense";

    [MaxLength(50)]
    public string Icon { get; set; } = "category";

    public bool IsActive { get; set; } = true;

    public DateTime CreatedAt { get; set; } = DateTime.Now;

    public ICollection<Transaction> Transactions { get; set; }
        = new List<Transaction>();
}
