using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace San_Pham_Do_An1.Models;

[Table("tb_ProductReview")]
public partial class TbProductReview
{
    [Key]
    public int ProductReviewId { get; set; }

    public int? CustomerId { get; set; }

    public string? Detail { get; set; }

    public int? Star { get; set; }

    public int? ProductId { get; set; }

    public DateTime? CreatedDate { get; set; }

    public bool? IsActive { get; set; }

    [ForeignKey("CustomerId")]
    public virtual TbCustomer? Customer { get; set; }

    [ForeignKey("ProductId")]
    public virtual TbProduct? Product { get; set; }
}
