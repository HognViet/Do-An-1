using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace San_Pham_Do_An1.Models;

[Table("tb_BlogCategory")]
public partial class TbBlogCategory
{
    [Key]
    public int BlogCategoryId { get; set; }

    [StringLength(200)]
    public string? Title { get; set; }

    [StringLength(200)]
    public string? Alias { get; set; }

    public string? Description { get; set; }

    public DateTime? CreatedDate { get; set; }

    [StringLength(200)]
    public string? Image { get; set; }

    public virtual ICollection<TbBlog> TbBlogs { get; set; } = new List<TbBlog>();
}
