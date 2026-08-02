using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace San_Pham_Do_An1.Models;

[Table("tb_OrderStatus")]
public partial class TbOrderStatus
{
    [Key]
    public int OrderStatusId { get; set; }

    [StringLength(50)]
    public string? Name { get; set; }

    [StringLength(200)]
    public string? Description { get; set; }

    public virtual ICollection<TbOrder> TbOrders { get; set; } = new List<TbOrder>();
}
