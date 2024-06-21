using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace LearnAPI.Repos.Models
{
    [Table("tbl_product")]
    public partial class TblProduct
    {
        [Key]
        [Column("code")]
        [StringLength(50)]
        [Unicode(false)]
        public string Code { get; set; } = null!;

        [Column("name")]
        [Unicode(false)]
        public string? Name { get; set; }

        [Column("price", TypeName = "decimal(18, 2)")]
        public decimal? Price { get; set; }
    }
}
