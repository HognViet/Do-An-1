using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace San_Pham_Do_An1.Models;

[Table("tb_ProductVariant")]
public partial class TbProductVariant
{
    [Key]
    public int VariantId { get; set; }

    public int? ProductId { get; set; }

    public int? ColorId { get; set; }

    public int? SizeId { get; set; }

    [StringLength(200)]
    public string? Image { get; set; }

    [StringLength(100)]
    public string? Sku { get; set; }

    [Column(TypeName = "decimal(18, 2)")]
    public decimal? Price { get; set; }

    [Column(TypeName = "decimal(18, 2)")]
    public decimal? PriceSale { get; set; }

    public int? Quantity { get; set; }

    public bool? IsActive { get; set; }

    [ForeignKey("ColorId")]
    public virtual TbColor? Color { get; set; }

    [ForeignKey("ProductId")]
    public virtual TbProduct? Product { get; set; }

    [ForeignKey("SizeId")]
    public virtual TbSize? Size { get; set; }
}
