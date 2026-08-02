using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace San_Pham_Do_An1.Models;

[Table("tb_Role")]
public partial class TbRole
{
    [Key]
    public int RoleId { get; set; }

    [StringLength(100)]
    public string? RoleName { get; set; }

    [StringLength(200)]
    public string? Description { get; set; }

    public virtual ICollection<TbAccount> TbAccounts { get; set; } = new List<TbAccount>();
}
