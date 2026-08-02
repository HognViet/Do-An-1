// Đảm bảo code chỉ chạy khi DOM đã sẵn sàng
(function() {
    // Nếu DOM đã sẵn sàng, chạy ngay lập tức
    if (document.readyState === 'loading') {
        document.addEventListener("DOMContentLoaded", initChatbot);
    } else {
        // DOM đã sẵn sàng, chạy ngay
        initChatbot();
    }
    
    function initChatbot() {
    const chatBody = document.querySelector(".chat-body");
    const messageInput = document.querySelector(".message-input");
    const sendMessageButton = document.querySelector("#send-message");
    const fileInput = document.querySelector("#file-input");
    const fileUploadWrapper = document.querySelector(".file-upload-wrapper");
    const fileCancelButton = document.querySelector("#file-cancel");
    const chatbotToggler = document.querySelector("#chatbot-toggler");
    const closeChatbot = document.querySelector("#close-chatbot");

    // Kiểm tra xem các phần tử bắt buộc có tồn tại không
    if (!chatBody || !messageInput || !chatbotToggler || !closeChatbot) {
        console.error("Chatbot elements not found", {
            chatBody: !!chatBody,
            messageInput: !!messageInput,
            chatbotToggler: !!chatbotToggler,
            closeChatbot: !!closeChatbot
        });
        return;
    }
    
    console.log("Chatbot initialized successfully");
    
    // Kiểm tra các phần tử tùy chọn
    if (!fileInput || !fileUploadWrapper || !fileCancelButton || !sendMessageButton) {
        console.warn("Some chatbot optional elements not found");
    }

    // API setup - Gọi qua backend để bảo vệ API key
    // API key được lưu trong appsettings.json (không commit lên GitHub)
    const API_URL = "/api/Chat/send";

    const userData = {
        message: null,
        file: {
            data: null,
            mime_type: null
        }
    };

    const chatHistory = [];

    const initialInputHeight = messageInput.scrollHeight;

// Create message element with dynamic classes and return it
const createMessageElement = (content, ...classes) => {
    const div = document.createElement("div");
    div.classList.add("message", ...classes);
    div.innerHTML = content;
    return div;
};

// Generate bot response using API - Gọi qua backend
const generateBotResponse = async (incomingMessageDiv) => {
    const messageElement = incomingMessageDiv.querySelector(".message-text");

    // API request options - Gửi message đến backend
    const requestOptions = {
        method: "POST",
        headers: { 
            "Content-Type": "application/json",
            "RequestVerificationToken": document.querySelector('input[name="__RequestVerificationToken"]')?.value || ""
        },
        body: JSON.stringify({
            message: userData.message
        })
    }

    try {
        // Fetch bot response from backend API
        const response = await fetch(API_URL, requestOptions);
        
        // Đọc response text trước để có thể xử lý cả JSON và non-JSON
        const responseText = await response.text();
        let data;
        
        // Kiểm tra Content-Type và thử parse JSON
        const contentType = response.headers.get("content-type") || "";
        if (contentType.includes("application/json")) {
            try {
                data = JSON.parse(responseText);
            } catch (jsonError) {
                console.error("Failed to parse JSON response:", responseText);
                throw new Error("Server trả về dữ liệu không hợp lệ. Vui lòng thử lại.");
            }
        } else {
            // Nếu không phải JSON, có thể là HTML error page
            console.error("Non-JSON response received:", responseText.substring(0, 200));
            
            // Kiểm tra xem có phải là HTML error page không
            if (responseText.includes("<html") || responseText.includes("<!DOCTYPE")) {
                throw new Error("Server trả về trang lỗi HTML. Vui lòng kiểm tra lại cấu hình server.");
            } else {
                // Có thể là plain text error
                throw new Error("Server trả về: " + responseText.substring(0, 100));
            }
        }
        
        if (!response.ok) {
            // Xử lý các loại lỗi khác nhau
            let errorMessage = data.error || "Đã xảy ra lỗi không xác định";
            const errorCode = response.status;
            
            // Phát hiện lỗi quota/rate limit
            if (errorMessage.includes("quota") || errorMessage.includes("Quota exceeded") || 
                errorMessage.includes("rate limit") || errorMessage.includes("rate-limits") ||
                errorMessage.includes("limit: 0") || errorCode === 429) {
                errorMessage = "⚠️ API đã hết quota hoặc vượt quá giới hạn.\n\n" +
                    "Nguyên nhân có thể:\n" +
                    "• Model đang dùng không có trong free tier\n" +
                    "• Đã vượt quá giới hạn sử dụng miễn phí\n" +
                    "• API key chưa được kích hoạt đầy đủ\n\n" +
                    "Giải pháp:\n" +
                    "• Kiểm tra quota tại: https://ai.dev/usage?tab=rate-limit\n" +
                    "• Đợi quota reset (thường reset theo ngày/tháng)\n" +
                    "• Hoặc nâng cấp plan tại Google Cloud Console";
            } 
            // Phát hiện lỗi API key
            else if (errorMessage.includes("API key") || errorMessage.includes("invalid") || 
                     errorCode === 401 || errorCode === 403) {
                errorMessage = "🔑 Lỗi xác thực API key.\n\n" +
                    "Vui lòng kiểm tra lại API key trong appsettings.json.";
            }
            // Phát hiện lỗi network
            else if (errorMessage.includes("network") || errorMessage.includes("fetch")) {
                errorMessage = "🌐 Lỗi kết nối mạng.\n\n" +
                    "Vui lòng kiểm tra kết nối internet và thử lại.";
            }
            // Lỗi server
            else if (errorCode >= 500) {
                errorMessage = "🔧 Lỗi server.\n\n" +
                    "Vui lòng thử lại sau hoặc liên hệ admin.";
            }
            // Lỗi khác
            else {
                errorMessage = "❌ Lỗi: " + errorMessage + "\n\n" +
                    "Vui lòng thử lại sau hoặc liên hệ hỗ trợ.";
            }
            
            throw new Error(errorMessage);
        }

        // Extract and display bot's response text từ backend response
        // Backend trả về format: { user: {...}, bot: { Message: "..." }, products: [...] }
        const apiResponseText = (data.bot?.Message || data.bot?.message || "").replace(/\*\*(.*?)\*\*/g, "$1").trim();
        
        if (!apiResponseText) {
            throw new Error("Không nhận được phản hồi từ server");
        }
        
        // Kiểm tra xem có products không
        const products = data.products || [];
        
        if (products.length > 0) {
            // Render product cards
            let productCardsHTML = `<div class="bot-message-text">${apiResponseText}</div>`;
            productCardsHTML += '<div class="chatbot-products-container" style="margin-top: 15px; display: flex; flex-direction: column; gap: 12px;">';
            
            products.forEach(product => {
                const productUrl = product.detail_url || `/product/${product.slug || product.id}-${product.id}.html`;
                const productImage = product.image_url || '/assets/img/default-product.png';
                const productName = product.name || 'Sản phẩm';
                
                // Format price - nếu đã có price_formatted thì dùng, nếu không thì format số
                let productPrice = product.price_formatted || '';
                if (!productPrice && product.price) {
                    const priceNum = typeof product.price === 'number' ? product.price : parseFloat(product.price);
                    productPrice = new Intl.NumberFormat('vi-VN').format(priceNum);
                }
                if (!productPrice) productPrice = '0';
                
                const productDescription = product.description || '';
                
                // Escape HTML để tránh XSS
                const escapeHtml = (text) => {
                    const div = document.createElement('div');
                    div.textContent = text;
                    return div.innerHTML;
                };
                
                productCardsHTML += `
                    <a href="${escapeHtml(productUrl)}" class="chatbot-product-card">
                        <img src="${escapeHtml(productImage)}" alt="${escapeHtml(productName)}" onerror="this.src='/assets/img/default-product.png'">
                        <div style="flex: 1; min-width: 0;">
                            <h4>${escapeHtml(productName)}</h4>
                            ${productDescription ? `<p>${escapeHtml(productDescription)}</p>` : ''}
                            <div class="product-price">${escapeHtml(productPrice)} VNĐ</div>
                        </div>
                    </a>
                `;
            });
            
            productCardsHTML += '</div>';
            messageElement.innerHTML = productCardsHTML;
        } else {
            // Chỉ hiển thị text nếu không có products
            messageElement.innerText = apiResponseText;
        }
        
        // Thêm vào chat history để giữ context
        chatHistory.push({
            role: "user",
            parts: [{ text: userData.message }]
        });
        chatHistory.push({
            role: "model",
            parts: [{ text: apiResponseText }]
        });
    } catch (error) {
        // Hiển thị lỗi với định dạng đẹp hơn
        messageElement.innerHTML = error.message.replace(/\n/g, "<br>");
        messageElement.style.color = "#ff0000";
        messageElement.style.whiteSpace = "pre-line";
        messageElement.style.lineHeight = "1.6";
        
        // Không thêm lỗi vào chat history
        console.error("Chatbot API Error:", error);
    } finally {
        userData.file = {};
        incomingMessageDiv.classList.remove("thinking");
        chatBody.scrollTo({ behavior: "smooth", top: chatBody.scrollHeight });
    }
};

// Handle outgoing user message
const handleOutgoingMessage = (e) => {
    e.preventDefault();
    userData.message = messageInput.value.trim();
    messageInput.value = "";
    if (fileUploadWrapper) {
        fileUploadWrapper.classList.remove("file-uploaded");
    }
    messageInput.dispatchEvent(new Event("input"));

    // Create and display user message
    const messageContent = `<div class="message-text"></div>
                            ${userData.file.data ? `<img src="data:${userData.file.mime_type};base64,${userData.file.data}" class="attachment" />` : ""}`;

    const outgoingMessageDiv = createMessageElement(messageContent, "user-message");
    outgoingMessageDiv.querySelector(".message-text").innerText = userData.message;
    chatBody.appendChild(outgoingMessageDiv);
    chatBody.scrollTop = chatBody.scrollHeight;

    // Simulate bot response with thinking indicator after a delay
    setTimeout(() => {
        const messageContent = `<svg class="bot-avatar" xmlns="http://www.w3.org/2000/svg" width="50" height="50" viewBox="0 0 1024 1024">
                    <path d="M738.3 287.6H285.7c-59 0-106.8 47.8-106.8 106.8v303.1c0 59 47.8 106.8 106.8 106.8h81.5v111.1c0 .7.8 1.1 1.4.7l166.9-110.6 41.8-.8h117.4l43.6-.4c59 0 106.8-47.8 106.8-106.8V394.5c0-59-47.8-106.9-106.8-106.9zM351.7 448.2c0-29.5 23.9-53.5 53.5-53.5s53.5 23.9 53.5 53.5-23.9 53.5-53.5 53.5-53.5-23.9-53.5-53.5zm157.9 267.1c-67.8 0-123.8-47.5-132.3-109h264.6c-8.6 61.5-64.5 109-132.3 109zm110-213.7c-29.5 0-53.5-23.9-53.5-53.5s23.9-53.5 53.5-53.5 53.5 23.9 53.5 53.5-23.9 53.5-53.5 53.5zM867.2 644.5V453.1h26.5c19.4 0 35.1 15.7 35.1 35.1v121.1c0 19.4-15.7 35.1-35.1 35.1h-26.5zM95.2 609.4V488.2c0-19.4 15.7-35.1 35.1-35.1h26.5v191.3h-26.5c-19.4 0-35.1-15.7-35.1-35.1zM561.5 149.6c0 23.4-15.6 43.3-36.9 49.7v44.9h-30v-44.9c-21.4-6.5-36.9-26.3-36.9-49.7 0-28.6 23.3-51.9 51.9-51.9s51.9 23.3 51.9 51.9z"></path>
                </svg>
                <div class="message-text">
                    <div class="thinking-indicator">
                        <div class="dot"></div>
                        <div class="dot"></div>
                        <div class="dot"></div>
                    </div>
                </div>`;

        const incomingMessageDiv = createMessageElement(messageContent, "bot-message", "thinking");
        chatBody.appendChild(incomingMessageDiv);
        chatBody.scrollTo({ behavior: "smooth", top: chatBody.scrollHeight });
        generateBotResponse(incomingMessageDiv);
    }, 600);
};

// Handle Enter key press for sending messages
messageInput.addEventListener("keydown", (e) => {
    const userMessage = e.target.value.trim();
    if (e.key === "Enter" && userMessage && !e.shiftKey && window.innerWidth > 768) {
        handleOutgoingMessage(e);
    }
});

messageInput.addEventListener("input", (e) => {
    messageInput.style.height = `${initialInputHeight}px`;
    messageInput.style.height = `${messageInput.scrollHeight}px`;
    const form = document.querySelector(".chat-form");
    if (form) {
        form.style.boderRadius = messageInput.scrollHeight > initialInputHeight ? "15px" : "32px";
    }
});

// Handle file input change event
if (fileInput) {
    fileInput.addEventListener("change", (e) => {
    const file = e.target.files[0];
    if (!file) return;
    const reader = new FileReader();
    reader.onload = (e) => {
        fileUploadWrapper.querySelector("img").src = e.target.result;
        fileUploadWrapper.classList.add("file-uploaded");
        const base64String = e.target.result.split(",")[1];

        // Store file data in userData
        userData.file = {
            data: base64String,
            mime_type: file.type
        };

        fileInput.value = "";
    };

    reader.readAsDataURL(file);
    });
}

if (fileCancelButton && fileUploadWrapper) {
    fileCancelButton.addEventListener("click", (e) => {
        userData.file = {};
        fileUploadWrapper.classList.remove("file-uploaded");
    });
}

const chatForm = document.querySelector(".chat-form");
if (chatForm && typeof EmojiMart !== 'undefined') {
    const picker = new EmojiMart.Picker({
        theme: "light",
        showSkinTones: "none",
        previewPosition: "none",
        onEmojiSelect: (emoji) => {
            const { selectionStart: start, selectionEnd: end } = messageInput;
            messageInput.setRangeText(emoji.native, start, end, "end");
            messageInput.focus();
        },
        onClickOutside: (e) => {
            if (e.target.id === "emoji-picker") {
                document.body.classList.toggle("show-emoji-picker");
            } else {
                document.body.classList.remove("show-emoji-picker");
            }
        },
    });
    chatForm.appendChild(picker);
}

if (fileInput && fileUploadWrapper) {
    fileInput.addEventListener("change", async (e) => {
    const file = e.target.files[0];
    if (!file) return;
    const validImageTypes = ['image/jpeg', 'image/png', 'image/gif', 'image/webp'];
    if (!validImageTypes.includes(file.type)) {
        await Swal.fire({
            icon: 'error',
            title: 'Lỗi',
            text: 'Chỉ chấp nhận file ảnh (JPEG, PNG, GIF, WEBP)',
            confirmButtonText: 'OK'
        });
        resetFileInput();
        return;
    }
    const reader = new FileReader();
    reader.onload = (e) => {
        fileUploadWrapper.querySelector("img").src = e.target.result;
        fileUploadWrapper.classList.add("file-uploaded");
        const base64String = e.target.result.split(",")[1];
        userData.file = {
            data: base64String,
            mime_type: file.type
        };
    };
    reader.readAsDataURL(file);
    });
}

function resetFileInput() {
    if (fileInput && fileUploadWrapper) {
        fileInput.value = "";
        fileUploadWrapper.classList.remove("file-uploaded");
        const img = fileUploadWrapper.querySelector("img");
        if (img) img.src = "#";
        userData.file = { data: null, mime_type: null };
        const form = document.querySelector(".chat-form");
        if (form) form.reset();
    }
}

    if (sendMessageButton) {
        sendMessageButton.addEventListener("click", (e) => handleOutgoingMessage(e));
    }
    
    const fileUploadBtn = document.querySelector("#file-upload");
    if (fileUploadBtn && fileInput) {
        fileUploadBtn.addEventListener("click", (e) => fileInput.click());
    }
    
    chatbotToggler.addEventListener("click", function(e) {
        e.preventDefault();
        e.stopPropagation();
        console.log("Chatbot toggler clicked");
        document.body.classList.toggle("show-chatbot");
        console.log("Chatbot state:", document.body.classList.contains("show-chatbot") ? "shown" : "hidden");
    });
    
    closeChatbot.addEventListener("click", function(e) {
        e.preventDefault();
        e.stopPropagation();
        document.body.classList.remove("show-chatbot");
    });
    
    }
})();