using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace LearnAPI.Repos.Models
{
    [Table("tbl_menu")]
    public partial class TblMenu
    {
        [Key]
        [Column("code")]
        [StringLength(50)]
        public string Code { get; set; } = null!;

        [Column("name")]
        [StringLength(200)]
        public string Name { get; set; } = null!;

        [Column("status")]
        public bool? Status { get; set; }
    }
}
