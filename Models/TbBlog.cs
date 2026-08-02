using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace San_Pham_Do_An1.Models;

[Table("tb_Blog")]
public partial class TbBlog
{
    [Key]
    public int BlogId { get; set; }

    [StringLength(200)]
    public string? Title { get; set; }

    [StringLength(200)]
    public string? Alias { get; set; }

    public int? BlogCategoryId { get; set; }

    public string? Description { get; set; }

    public string? Detail { get; set; }

    [StringLength(200)]
    public string? Image { get; set; }

    public DateTime? CreatedDate { get; set; }

    [StringLength(100)]
    public string? CreatedBy { get; set; }

    public DateTime? ModifiedDate { get; set; }

    [StringLength(100)]
    public string? ModifiedBy { get; set; }

    public int? AccountId { get; set; }

    public bool IsActive { get; set; }

    [ForeignKey("AccountId")]
    public virtual TbAccount? Account { get; set; }

    [ForeignKey("BlogCategoryId")]
    public virtual TbBlogCategory? BlogCategory { get; set; }

    public virtual ICollection<TbBlogComment> TbBlogComments { get; set; } = new List<TbBlogComment>();
}
