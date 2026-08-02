using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace San_Pham_Do_An1.Models;

[Table("tb_Size")]
public partial class TbSize
{
    [Key]
    public int SizeId { get; set; }

    [StringLength(50)]
    public string? SizeName { get; set; }

    public int? SizeOrder { get; set; }

    public bool? IsActive { get; set; }

    public virtual ICollection<TbProductVariant> TbProductVariants { get; set; } = new List<TbProductVariant>();
}
