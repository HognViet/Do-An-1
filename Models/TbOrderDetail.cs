using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace San_Pham_Do_An1.Models;

[Table("tb_OrderDetail")]
public partial class TbOrderDetail
{
    public int OrderId { get; set; }

    public int ProductId { get; set; }

    [Column(TypeName = "decimal(18, 2)")]
    public decimal? Price { get; set; }

    public int? Quantity { get; set; }

    [ForeignKey("OrderId")]
    public virtual TbOrder Order { get; set; } = null!;

    [ForeignKey("ProductId")]
    public virtual TbProduct Product { get; set; } = null!;
}
