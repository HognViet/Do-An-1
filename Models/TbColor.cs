using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace San_Pham_Do_An1.Models;

[Table("tb_Color")]
public partial class TbColor
{
    [Key]
    public int ColorId { get; set; }

    [StringLength(100)]
    public string? ColorName { get; set; }

    [StringLength(50)]
    public string? ColorCode { get; set; }

    public bool? IsActive { get; set; }

    // Commented out to focus on homepage:
    // public virtual ICollection<TbProductVariant> TbProductVariants { get; set; } = new List<TbProductVariant>();
}
