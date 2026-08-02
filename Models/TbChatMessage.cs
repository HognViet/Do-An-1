using System;

namespace San_Pham_Do_An1.Models;

public partial class TbChatMessage
{
    public int MessageId { get; set; }

    public int? UserId { get; set; }

    public string? GuestToken { get; set; }

    public string Sender { get; set; } = null!;

    public string Message { get; set; } = null!;

    public DateTime? CreatedDate { get; set; }

    public virtual TbAccount? User { get; set; }
}
