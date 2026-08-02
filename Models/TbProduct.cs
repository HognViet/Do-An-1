using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace San_Pham_Do_An1.Models;

[Table("tb_Product")]
public partial class TbProduct
{
    [Key]
    public int ProductId { get; set; }

    [StringLength(200)]
    public string? Title { get; set; }

    [StringLength(200)]
    public string? Alias { get; set; }

    public int? CategoryProductId { get; set; }

    public string? Description { get; set; }

    public string? Detail { get; set; }

    [StringLength(200)]
    public string? Image { get; set; }

    [Column(TypeName = "decimal(18, 2)")]
    public decimal? Price { get; set; }

    [Column(TypeName = "decimal(18, 2)")]
    public decimal? PriceSale { get; set; }

    public DateTime? CreatedDate { get; set; }

    [StringLength(100)]
    public string? CreatedBy { get; set; }

    public DateTime? ModifiedDate { get; set; }

    [StringLength(100)]
    public string? ModifiedBy { get; set; }

    public bool IsNew { get; set; }

    public bool IsBestSeller { get; set; }

    public bool IsActive { get; set; }

    public int? Quantity { get; set; }

    public double? Star { get; set; }

    [ForeignKey("CategoryProductId")]
    public virtual TbProductCategory? CategoryProduct { get; set; }

    public virtual ICollection<TbProductReview> TbProductReviews { get; set; } = new List<TbProductReview>();

    public virtual ICollection<TbOrderDetail> TbOrderDetails { get; set; } = new List<TbOrderDetail>();
    public virtual ICollection<TbProductVariant> TbProductVariants { get; set; } = new List<TbProductVariant>();
}
