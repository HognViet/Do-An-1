using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace San_Pham_Do_An1.Models;

[Table("tb_Menu")]
public partial class TbMenu
{
    [Key]
    public int MenuId { get; set; }

    [StringLength(200)]
    public string? Title { get; set; }

    [StringLength(200)]
    public string? Alias { get; set; }

    public string? Description { get; set; }

    public int? Levels { get; set; }

    public int? ParentId { get; set; }

    public int? Position { get; set; }

    public DateTime? CreatedDate { get; set; }

    [StringLength(100)]
    public string? CreatedBy { get; set; }

    public DateTime? ModifiedDate { get; set; }

    [StringLength(100)]
    public string? ModifiedBy { get; set; }

    public bool IsActive { get; set; }
}
