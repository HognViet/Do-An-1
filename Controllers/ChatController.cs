using San_Pham_Do_An1.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Text.RegularExpressions;
using System.Text.Json;
using System.Text;
using System.Net.Http.Headers;

namespace San_Pham_Do_An1.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Produces("application/json")]
    public class ChatController : ControllerBase
    {
        private readonly WedQuanAoDbContext _context;
        private readonly ILogger<ChatController> _logger;
        private readonly IConfiguration _configuration;
        private readonly HttpClient _httpClient;

        private const string GroqApiEndpoint = "https://api.groq.com/openai/v1/chat/completions";

        private string GetGroqModelName()
        {
            return _configuration["GroqAI:ModelName"] ?? "llama-3.3-70b-versatile";
        }

        public ChatController(
            WedQuanAoDbContext context,
            ILogger<ChatController> logger,
            IConfiguration configuration,
            IHttpClientFactory httpClientFactory)
        {
            _context = context;
            _logger = logger;
            _configuration = configuration;
            _httpClient = httpClientFactory.CreateClient();
            _httpClient.Timeout = TimeSpan.FromSeconds(30);
        }
        [HttpGet("messages")]
        public async Task<IActionResult> GetMessages()
        {
            try
            {
                await CleanOldMessagesAsync();

                var userId = GetUserId();
                var guestToken = GetGuestToken();

                IQueryable<TbChatMessage> query = _context.TbChatMessages;

                if (userId.HasValue)
                {
                    query = query.Where(m => m.UserId == userId);
                }
                else if (!string.IsNullOrEmpty(guestToken))
                {
                    query = query.Where(m => m.GuestToken == guestToken);
                }
                else
                {
                    return Ok(new List<object>());
                }

                var messages = await query
                    .OrderBy(m => m.CreatedDate)
                    .Select(m => new
                    {
                        m.MessageId,
                        m.UserId,
                        m.GuestToken,
                        m.Sender,
                        m.Message,
                        m.CreatedDate
                    })
                    .ToListAsync();

                return Ok(messages);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi lấy lịch sử chat");
                return StatusCode(500, new { error = "Lỗi server khi lấy lịch sử chat" });
            }
        }
        [HttpPost("send")]
        public async Task<IActionResult> SendMessage([FromBody] SendMessageRequest request)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(request.Message))
                {
                    return BadRequest(new { error = "Tin nhắn không được để trống" });
                }

                var userId = GetUserId();
                var guestToken = GetOrCreateGuestToken();
                var userMsg = await SaveUserMessage(userId, guestToken, request.Message);
                var aiResponse = await GetAIResponse(userId, guestToken, request.Message);
                var botMsg = await SaveBotMessage(userId, guestToken, aiResponse);

                return Ok(BuildResponse(userMsg, botMsg, null));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi gửi tin nhắn: {Message}", ex.Message);
                _logger.LogError(ex, "Stack trace: {StackTrace}", ex.StackTrace);
                return StatusCode(500, new
                {
                    error = "Lỗi server khi xử lý tin nhắn. Vui lòng thử lại sau.",
                    details = ex.Message
                });
            }
        }
        private async Task<string> GetAIResponse(int? userId, string guestToken, string userMessage)
        {
            try
            {
                var storeContext = await BuildStoreContext(userMessage);

                var chatHistory = await GetChatHistory(userId, guestToken, 10);

                var apiKey = _configuration["GroqAI:ApiKey"];
                if (string.IsNullOrEmpty(apiKey))
                {
                    _logger.LogError("GroqAI:ApiKey không được cấu hình trong appsettings.json");
                    return GetFallbackResponse(userMessage);
                }

            
                var groqRequest = new
                {
                    model = GetGroqModelName(),
                    messages = BuildGroqMessages(storeContext, chatHistory, userMessage),
                    temperature = 0.8,
                    max_tokens = 500,
                    top_p = 0.9
                };

                var jsonRequest = JsonSerializer.Serialize(groqRequest);
                var content = new StringContent(jsonRequest, Encoding.UTF8, "application/json");

                using var httpRequest = new HttpRequestMessage(HttpMethod.Post, GroqApiEndpoint)
                {
                    Content = content
                };
                httpRequest.Headers.Authorization = new AuthenticationHeaderValue("Bearer", apiKey);

                var response = await _httpClient.SendAsync(httpRequest);

                if (!response.IsSuccessStatusCode)
                {
                    var errorBody = await response.Content.ReadAsStringAsync();
                    _logger.LogWarning("Groq API returned {StatusCode}: {Body}", response.StatusCode, errorBody);
                    return GetFallbackResponse(userMessage);
                }

                var responseBody = await response.Content.ReadAsStringAsync();
                var groqResponse = JsonSerializer.Deserialize<JsonElement>(responseBody);

                var aiText = groqResponse
                    .GetProperty("choices")[0]
                    .GetProperty("message")
                    .GetProperty("content")
                    .GetString() ?? GetFallbackResponse(userMessage);

                return CleanAIResponse(aiText);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi gọi Groq API");
                return GetFallbackResponse(userMessage);
            }
        }

        private List<object> BuildGroqMessages(string systemContext, List<TbChatMessage> history, string currentMessage)
        {
            var messages = new List<object>();

        
            messages.Add(new
            {
                role = "system",
                content = systemContext
            });

            foreach (var msg in history)
            {
                messages.Add(new
                {
                    role = msg.Sender == "user" ? "user" : "assistant",
                    content = msg.Message ?? ""
                });
            }

    
            messages.Add(new
            {
                role = "user",
                content = currentMessage
            });

            return messages;
        }

        private async Task<string> BuildStoreContext(string userMessage)
        {
            var sb = new StringBuilder();

            sb.AppendLine("Bạn là trợ lý bán hàng của cửa hàng thời trang NETMARK, đang trò chuyện trực tiếp với khách trên website. " +
                          "Nói chuyện tự nhiên, thân thiện như một nhân viên tư vấn thực thụ, không máy móc, không rập khuôn. " +
                          "Dưới đây là dữ liệu thật lấy từ hệ thống cửa hàng — chỉ dùng những thông tin này để trả lời, không tự bịa sản phẩm/giá không có trong dữ liệu.");
            sb.AppendLine();

            var categories = await _context.TbProductCategories
                .Include(c => c.TbProducts.Where(p => p.IsActive == true))
                .Where(c => c.TbProducts.Any(p => p.IsActive == true))
                .Select(c => new
                {
                    c.Title,
                    ProductCount = c.TbProducts.Count(p => p.IsActive == true)
                })
                .ToListAsync();

            sb.AppendLine("=== DANH MỤC SẢN PHẨM ===");
            foreach (var c in categories)
            {
                sb.AppendLine($"- {c.Title} ({c.ProductCount} sản phẩm)");
            }
            sb.AppendLine();

            var keywords = ExtractProductKeywords(userMessage.ToLower());
            var validKeywords = keywords.Where(kw => kw.Length >= 2).ToList();

            var activeProducts = await _context.TbProducts
                .Include(p => p.CategoryProduct)
                .Where(p => p.IsActive == true && p.Title != null)
                .ToListAsync();

            List<TbProduct> relevantProducts;
            if (validKeywords.Any())
            {
                relevantProducts = activeProducts
                    .Where(p => validKeywords.Any(kw =>
                        p.Title.ToLower().Contains(kw) ||
                        (p.Description != null && p.Description.ToLower().Contains(kw))))
                    .OrderByDescending(p => p.IsBestSeller)
                    .ThenByDescending(p => p.Star)
                    .Take(10)
                    .ToList();
            }
            else
            {
                relevantProducts = new List<TbProduct>();
            }

            if (!relevantProducts.Any())
            {
                relevantProducts = activeProducts
                    .Where(p => p.IsBestSeller == true)
                    .OrderByDescending(p => p.Star)
                    .Take(8)
                    .ToList();
            }

            sb.AppendLine("=== SẢN PHẨM LIÊN QUAN (dùng để trả lời câu hỏi hiện tại của khách) ===");
            if (relevantProducts.Any())
            {
                foreach (var p in relevantProducts)
                {
                    var price = FormatPrice(p.PriceSale ?? p.Price);
                    var cat = p.CategoryProduct?.Title ?? "Khác";
                    var desc = TruncateDescription(p.Description, 100);
                    sb.AppendLine($"- {p.Title} | Danh mục: {cat} | Giá: {price} | Mô tả: {desc}");
                }
            }
            else
            {
                sb.AppendLine("(Không có sản phẩm nào khớp — nếu khách hỏi về sản phẩm cụ thể, hãy nói thật là chưa tìm thấy và gợi ý họ xem thêm trên website hoặc mô tả rõ hơn nhu cầu.)");
            }
            sb.AppendLine();

            sb.AppendLine("=== CÁC TÍNH NĂNG HỖ TRỢ KHÁC ===");
            sb.AppendLine("- Khách có thể tra cứu đơn hàng bằng mã đơn (ví dụ: #123)");
            sb.AppendLine("- Khách có thể yêu cầu tư vấn theo tiêu chí (giới tính, phong cách, mức giá)");

            return sb.ToString();
        }

  
        private async Task<List<TbChatMessage>> GetChatHistory(int? userId, string guestToken, int limit)
        {
            var query = _context.TbChatMessages
                .Where(m => userId.HasValue ? m.UserId == userId : m.GuestToken == guestToken)
                .OrderByDescending(m => m.CreatedDate)
                .Take(limit)
                .OrderBy(m => m.CreatedDate);

            return await query.ToListAsync();
        }

    
        [HttpPost("track-order")]
        public async Task<IActionResult> TrackOrder([FromBody] TrackOrderRequest request)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(request.OrderId))
                {
                    return BadRequest(new { error = "Mã đơn hàng không được để trống" });
                }

                var userId = GetUserId();
                var guestToken = GetOrCreateGuestToken();

                var orderIdStr = Regex.Replace(request.OrderId, @"[^0-9]", "");

                if (string.IsNullOrEmpty(orderIdStr) || !int.TryParse(orderIdStr, out int orderId))
                {
                    var botMsg = await SaveBotMessage(userId, guestToken,
                        "Mã đơn hàng không hợp lệ. Vui lòng nhập mã đơn hàng (ví dụ: #123 hoặc 123).");
                    return Ok(new { bot = botMsg });
                }

                var order = await _context.TbOrders
                    .Include(o => o.Customer)
                    .Include(o => o.OrderStatus)
                    .Include(o => o.TbOrderDetails).ThenInclude(od => od.Product)
                    .FirstOrDefaultAsync(o => o.OrderId == orderId);

                if (order == null)
                {
                    var botMsg = await SaveBotMessage(userId, guestToken,
                        $"Không tìm thấy đơn hàng #{orderId}. Vui lòng kiểm tra lại mã đơn hàng.");
                    return Ok(new { bot = botMsg });
                }

                var orderMessage = BuildOrderMessage(order);
                var botMsg2 = await SaveBotMessage(userId, guestToken, orderMessage);

        
                var orderData = BuildOrderData(order);

                return Ok(new { bot = botMsg2, order = orderData });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi tra cứu đơn hàng");
                return StatusCode(500, new { error = "Lỗi server khi tra cứu đơn hàng" });
            }
        }

        private string BuildOrderMessage(TbOrder order)
        {
            var statusLabel = order.OrderStatus?.Name ?? "Chưa xác định";
            var statusEmoji = GetStatusEmoji(statusLabel);

            var message = $"📦 **Thông tin đơn hàng #{order.OrderId}**\n\n";
            message += $"**Trạng thái:** {statusEmoji} {statusLabel}\n";
            message += $"**Ngày đặt:** {order.CreatedDate:dd/MM/yyyy HH:mm}\n";
            message += $"**Tổng tiền:** {FormatPrice(order.TotalAmount)}\n\n";

            if (!string.IsNullOrEmpty(order.ShippingAddress) && order.Customer != null)
            {
                message += $"**Địa chỉ giao hàng:**\n";
                message += $"{order.Customer.Name} - {order.Customer.Phone}\n";
                message += $"{order.ShippingAddress}\n\n";
            }

            message += "**Sản phẩm:**\n";
            var index = 1;
            foreach (var item in order.TbOrderDetails)
            {
                var productName = item.Product?.Title ?? "Sản phẩm";
                var total = (item.Price ?? 0) * (item.Quantity ?? 0);
                message += $"{index++}. {productName} x{item.Quantity} - {FormatPrice(total)}\n";
            }

            return message;
        }

        private object BuildOrderData(TbOrder order)
        {
            var statusLabel = order.OrderStatus?.Name ?? "Chưa xác định";
            return new
            {
                id = order.OrderId,
                status = statusLabel,
                status_label = statusLabel,
                status_emoji = GetStatusEmoji(statusLabel),
                created_at = order.CreatedDate?.ToString("dd/MM/yyyy HH:mm"),
                total_price = FormatPrice(order.TotalAmount),
                subtotal = FormatPrice(order.TbOrderDetails.Sum(od => (od.Price ?? 0) * (od.Quantity ?? 0))),
                shipping_fee = "25.000 ₫",
                discount_amount = "0 ₫",
                shipping_address = order.Customer != null ? new
                {
                    full_name = order.Customer.Name,
                    phone = order.Customer.Phone,
                    address = order.ShippingAddress
                } : null,
                items = order.TbOrderDetails.Select(od => new
                {
                    product_name = od.Product?.Title ?? "Sản phẩm",
                    quantity = od.Quantity ?? 0,
                    price = FormatPrice(od.Price),
                    total = FormatPrice((od.Price ?? 0) * (od.Quantity ?? 0))
                }),
                detail_url = $"/order/{order.OrderId}"
            };
        }

   
        [HttpPost("perfume-advisor")]
        public async Task<IActionResult> PerfumeAdvisor([FromBody] PerfumeAdvisorRequest request)
        {
            try
            {
                var userId = GetUserId();
                var guestToken = GetOrCreateGuestToken();

                var query = _context.TbProducts
                    .Include(p => p.CategoryProduct)
                    .Include(p => p.TbOrderDetails)
                    .Where(p => p.IsActive == true)
                    .AsQueryable();

                var searchKeywords = new List<string>();

           
                if (!string.IsNullOrEmpty(request.Gender))
                {
                    var genderMap = new Dictionary<string, string[]>
                    {
                        { "nam", new[] { "nam", "men", "homme", "for him" } },
                        { "nữ", new[] { "nữ", "women", "femme", "for her", "lady" } },
                        { "unisex", new[] { "unisex", "for all", "chung" } }
                    };

                    if (genderMap.TryGetValue(request.Gender.ToLower(), out var keywords))
                    {
                        searchKeywords.AddRange(keywords);
                    }
                }

                if (!string.IsNullOrEmpty(request.Style)) searchKeywords.Add(request.Style);
                if (!string.IsNullOrEmpty(request.Note)) searchKeywords.Add(request.Note);

            
                if (searchKeywords.Any())
                {
                    query = query.Where(p =>
                        searchKeywords.Any(keyword =>
                            (p.Title != null && p.Title.ToLower().Contains(keyword)) ||
                            (p.Description != null && p.Description.ToLower().Contains(keyword))
                        )
                    );
                }

         
                if (!string.IsNullOrEmpty(request.PriceRange))
                {
                    var range = request.PriceRange.Split('-');
                    if (range.Length == 2 &&
                        decimal.TryParse(range[0], out decimal minPrice) &&
                        decimal.TryParse(range[1], out decimal maxPrice))
                    {
                        query = query.Where(p => p.Price >= minPrice && p.Price <= maxPrice);
                    }
                }

                var products = await query.ToListAsync();

                var sortedProducts = products
                    .OrderByDescending(p => p.TbOrderDetails.Sum(od => od.Quantity ?? 0))
                    .Take(6)
                    .Select(p => new
                    {
                        id = p.ProductId,
                        name = p.Title,
                        slug = p.Alias ?? p.Title?.ToLower().Replace(" ", "-"),
                        price = p.PriceSale ?? p.Price,
                        price_formatted = FormatPrice(p.PriceSale ?? p.Price),
                        description = TruncateDescription(p.Description, 150),
                        image_url = p.Image ?? "/assets/img/default-product.png",
                        detail_url = $"/product/{(p.Alias ?? p.ProductId.ToString())}-{p.ProductId}.html"
                    })
                    .ToList();

         
                var criteria = new List<string>();
                if (!string.IsNullOrEmpty(request.Gender)) criteria.Add($"giới tính: {request.Gender}");
                if (!string.IsNullOrEmpty(request.Style)) criteria.Add($"phong cách: {request.Style}");
                if (!string.IsNullOrEmpty(request.Note)) criteria.Add($"nốt hương: {request.Note}");
                if (!string.IsNullOrEmpty(request.PriceRange))
                {
                    var range = request.PriceRange.Split('-');
                    if (range.Length == 2)
                    {
                        var priceText = $"{FormatPrice(decimal.Parse(range[0]))} - {FormatPrice(decimal.Parse(range[1]))}";
                        criteria.Add($"mức giá: {priceText}");
                    }
                }

                var criteriaText = string.Join(", ", criteria);
                var message = sortedProducts.Any()
                    ? $"Dựa trên tiêu chí của bạn ({criteriaText}), chúng tôi gợi ý {sortedProducts.Count} sản phẩm phù hợp nhất: 🌸"
                    : $"Xin lỗi, hiện tại chúng tôi chưa có sản phẩm phù hợp với tiêu chí ({criteriaText}). Bạn có thể thử điều chỉnh lại tiêu chí. 🌸";

                var botMsg = await SaveBotMessage(userId, guestToken, message);

                return Ok(new
                {
                    bot = new
                    {
                        botMsg.MessageId,
                        botMsg.Sender,
                        botMsg.Message,
                        botMsg.CreatedDate
                    },
                    products = sortedProducts,
                    is_specific_product = sortedProducts.Any()
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi tư vấn sản phẩm");
                return StatusCode(500, new { error = "Lỗi server khi tư vấn sản phẩm" });
            }
        }



        private async Task<TbChatMessage> SaveUserMessage(int? userId, string guestToken, string message)
        {
            try
            {
                var userMsg = new TbChatMessage
                {
                    UserId = userId,
                    GuestToken = userId.HasValue ? null : guestToken,
                    Sender = "user",
                    Message = message.Trim(),
                    CreatedDate = DateTime.Now
                };

                _context.TbChatMessages.Add(userMsg);
                await _context.SaveChangesAsync();
                return userMsg;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi lưu tin nhắn người dùng: {Message}", ex.Message);
                throw;
            }
        }

        private async Task<TbChatMessage> SaveBotMessage(int? userId, string guestToken, string message)
        {
            try
            {
                var botMsg = new TbChatMessage
                {
                    UserId = userId,
                    GuestToken = userId.HasValue ? null : guestToken,
                    Sender = "bot",
                    Message = CleanAIResponse(message),
                    CreatedDate = DateTime.Now
                };

                _context.TbChatMessages.Add(botMsg);
                await _context.SaveChangesAsync();
                return botMsg;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi lưu tin nhắn bot: {Message}", ex.Message);
                throw;
            }
        }

        private object BuildResponse(TbChatMessage userMsg, TbChatMessage botMsg, object? additionalData)
        {
            var response = new
            {
                user = new
                {
                    userMsg.MessageId,
                    userMsg.Sender,
                    userMsg.Message,
                    userMsg.CreatedDate
                },
                bot = new
                {
                    botMsg.MessageId,
                    botMsg.Sender,
                    botMsg.Message,
                    botMsg.CreatedDate
                }
            };

            if (additionalData != null)
            {
       
                var dict = new Dictionary<string, object?>
                {
                    ["user"] = response.user,
                    ["bot"] = response.bot
                };

                foreach (var prop in additionalData.GetType().GetProperties())
                {
                    dict[prop.Name] = prop.GetValue(additionalData);
                }

                return dict;
            }

            return response;
        }

        private string CleanAIResponse(string text)
        {
   
            text = Regex.Replace(text, @"\*\*(.*?)\*\*", "$1");
       
            text = Regex.Replace(text, @"^\s*[-*•]\s+", "", RegexOptions.Multiline);
        
            text = Regex.Replace(text, @"\n{3,}", "\n\n");
            return text.Trim();
        }

        private string GetFallbackResponse(string userMessage)
        {
            var lower = userMessage.ToLower();

            if (lower.Contains("xin chào") || lower.Contains("hello") || lower.Contains("hi"))
                return "Xin chào! Tôi có thể giúp gì cho bạn về sản phẩm của chúng tôi? 😊";

            if (lower.Contains("cảm ơn") || lower.Contains("thank"))
                return "Rất vui được hỗ trợ bạn! Chúc bạn có trải nghiệm mua sắm tuyệt vời! 🎉";

            if (lower.Contains("tạm biệt") || lower.Contains("bye"))
                return "Tạm biệt! Hẹn gặp lại bạn sớm! 👋";

            return "Tôi có thể giúp bạn tìm hiểu về sản phẩm, danh mục hoặc tra cứu đơn hàng. Bạn cần hỗ trợ gì? 🛍️";
        }

        private int? GetUserId()
        {
      
            return null;
        }

        private string? GetGuestToken()
        {
            return Request.Cookies["chat_token"];
        }

        private string GetOrCreateGuestToken()
        {
            var token = GetGuestToken();
            if (string.IsNullOrEmpty(token))
            {
                token = "guest_" + Guid.NewGuid().ToString("N");
                Response.Cookies.Append("chat_token", token, new CookieOptions
                {
                    Expires = DateTimeOffset.Now.AddDays(180),
                    HttpOnly = true,
                    SameSite = SameSiteMode.Lax
                });
            }
            return token;
        }

        private async Task CleanOldMessagesAsync()
        {
            try
            {
                var cutoffDate = DateTime.Now.AddDays(-1);
                var oldMessages = await _context.TbChatMessages
                    .Where(m => m.CreatedDate < cutoffDate)
                    .ToListAsync();

                _context.TbChatMessages.RemoveRange(oldMessages);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Không thể xóa tin nhắn cũ");
            }
        }

        private List<string> ExtractProductKeywords(string message)
        {
            var stopwords = new[] {
                "có", "không", "bạn", "tôi", "mình", "cho", "của", "về", "với", "và", "hay",
                "thì", "là", "được", "đã", "sẽ", "đang", "bị", "nào", "gì", "như", "thế",
                "này", "đó", "kia", "những", "các", "một", "hai", "ba", "muốn", "cần",
                "tìm", "xem", "mua", "giá", "sản", "phẩm"
            };

            var cleanMessage = Regex.Replace(message, @"[^\p{L}\p{N}\s]", " ");
            var words = cleanMessage.ToLower()
                .Split(new[] { ' ', '\t', '\n' }, StringSplitOptions.RemoveEmptyEntries);

            return words
                .Where(word => word.Length >= 2 && !stopwords.Contains(word))
                .Distinct()
                .ToList();
        }

        private string TruncateDescription(string? description, int maxLength)
        {
            if (string.IsNullOrEmpty(description))
                return "Sản phẩm chất lượng cao";

            return description.Length <= maxLength
                ? description
                : description.Substring(0, maxLength) + "...";
        }

        private string FormatPrice(decimal? price)
        {
            return price.HasValue
                ? price.Value.ToString("N0") + " ₫"
                : "Liên hệ";
        }

        private string GetStatusEmoji(string status)
        {
            return status.ToLower() switch
            {
                "pending" or "chờ xử lý" => "⏳",
                "processing" or "đang xử lý" => "✅",
                "delivered" or "đã giao" => "✅",
                "completed" or "hoàn thành" => "🎉",
                "canceled" or "đã hủy" => "❌",
                _ => "ℹ️"
            };
        }
    }

   
    public class SendMessageRequest
    {
        public string Message { get; set; } = string.Empty;
    }

    public class TrackOrderRequest
    {
        public string OrderId { get; set; } = string.Empty;
    }

    public class PerfumeAdvisorRequest
    {
        public string? Gender { get; set; }
        public string? Style { get; set; }
        public string? Note { get; set; }
        public string? PriceRange { get; set; }
    }
}