using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace San_Pham_Do_An1.Models;

[Table("tb_ProductCategory")]
public partial class TbProductCategory
{
    [Key]
    public int CategoryProductId { get; set; }

    [StringLength(200)]
    public string? Title { get; set; }

    [StringLength(200)]
    public string? Alias { get; set; }

    public string? Description { get; set; }

    [StringLength(200)]
    public string? Icon { get; set; }

    public int? Position { get; set; }

    public DateTime? CreatedDate { get; set; }

    public virtual ICollection<TbProduct> TbProducts { get; set; } = new List<TbProduct>();
}
