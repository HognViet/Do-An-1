using System;

namespace San_Pham_Do_An1.Models;

public partial class TbBlogComment
{
    public int CommentId { get; set; }

    public int? BlogId { get; set; }

    public int? CustomerId { get; set; }

    public string? Detail { get; set; }

    public DateTime? CreatedDate { get; set; }

    public bool? IsActive { get; set; }

    public virtual TbBlog? Blog { get; set; }

    public virtual TbCustomer? Customer { get; set; }
}
