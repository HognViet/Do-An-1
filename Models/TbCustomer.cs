using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace San_Pham_Do_An1.Models;

[Table("tb_Customer")]
public partial class TbCustomer
{
    [Key]
    public int CustomerId { get; set; }

    [StringLength(100)]
    public string? Name { get; set; }

    [StringLength(20)]
    public string? Phone { get; set; }

    [StringLength(100)]
    public string? Email { get; set; }

    [StringLength(50)]
    public string? Username { get; set; }

    [StringLength(100)]
    public string? Password { get; set; }

    public DateTime? Birthday { get; set; }

    [StringLength(200)]
    public string? Avatar { get; set; }

    [StringLength(200)]
    public string? Location { get; set; }

    public DateTime? LastLogin { get; set; }

    public bool? IsActive { get; set; }

    public virtual ICollection<TbProductReview> TbProductReviews { get; set; } = new List<TbProductReview>();

    // Commented out to focus on homepage:
    // public virtual ICollection<TbBlogComment> TbBlogComments { get; set; } = new List<TbBlogComment>();
    // public virtual ICollection<TbOrder> TbOrders { get; set; } = new List<TbOrder>();
}
