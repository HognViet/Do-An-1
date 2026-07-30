using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace San_Pham_Do_An1.Models;

[Table("tb_Order")]
public partial class TbOrder
{
    [Key]
    public int OrderId { get; set; }

    [StringLength(50)]
    public string? Code { get; set; }

    public int? CustomerId { get; set; }

    [StringLength(200)]
    public string? ShippingAddress { get; set; }

    [Column(TypeName = "decimal(18, 2)")]
    public decimal? TotalAmount { get; set; }

    public int? OrderStatusId { get; set; }

    public DateTime? CreatedDate { get; set; }

    public DateTime? ModifiedDate { get; set; }

    public string? Note { get; set; }

    [StringLength(100)]
    public string? PaymentMethod { get; set; }

    [ForeignKey("CustomerId")]
    public virtual TbCustomer? Customer { get; set; }

    [ForeignKey("OrderStatusId")]
    public virtual TbOrderStatus? OrderStatus { get; set; }

    public virtual ICollection<TbOrderDetail> TbOrderDetails { get; set; } = new List<TbOrderDetail>();
}
