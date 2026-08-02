using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace San_Pham_Do_An1.Models;

[Table("tb_Account")]
public partial class TbAccount
{
    [Key]
    public int AccountId { get; set; }

    [StringLength(100)]
    public string? Username { get; set; }

    [StringLength(100)]
    public string? Password { get; set; }

    [StringLength(100)]
    public string? FullName { get; set; }

    [StringLength(20)]
    public string? Phone { get; set; }

    [StringLength(100)]
    public string? Email { get; set; }

    public int? RoleId { get; set; }

    public DateTime? LastLogin { get; set; }

    public bool? IsActive { get; set; }

    [ForeignKey("RoleId")]
    public virtual TbRole? Role { get; set; }

    public virtual ICollection<TbBlog> TbBlogs { get; set; } = new List<TbBlog>();

    public virtual ICollection<TbChatMessage> TbChatMessages { get; set; } = new List<TbChatMessage>();
}
