// Please see documentation at https://learn.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.

function formatCurrency(value) {
  return Number(value || 0).toLocaleString("vi-VN");
}

function updateCartTotalDisplays(total) {
  var formatted = formatCurrency(total);
  document.querySelectorAll("#minicart-total").forEach(function (el) {
    el.textContent = formatted;
  });
}

function updateCartRowTotal(rowElement, quantity) {
  if (!rowElement) return;
  var priceElement = rowElement.querySelector("[data-unit-price]");
  if (!priceElement) return;
  var unitPrice = parseFloat(priceElement.getAttribute("data-unit-price")) || 0;
  var lineTotalElement = rowElement.querySelector(".cart-line-total");
  if (lineTotalElement) {
    lineTotalElement.textContent = formatCurrency(unitPrice * quantity);
  }
}

(function () {
  var MINICART_PANEL_SELECTOR = ".offCanvas__minicart";
  var BODY_ACTIVE_CLASS = "offCanvas__minicart_active";

  function openMinicart() {
    var panel = document.querySelector(MINICART_PANEL_SELECTOR);
    if (panel && !panel.classList.contains("active")) {
      panel.classList.add("active");
    }
    document.body.classList.add(BODY_ACTIVE_CLASS);
  }

  function closeMinicart() {
    var panel = document.querySelector(MINICART_PANEL_SELECTOR);
    if (panel && panel.classList.contains("active")) {
      panel.classList.remove("active");
    }
    document.body.classList.remove(BODY_ACTIVE_CLASS);
  }

  document.addEventListener("click", function (e) {
    var openBtn = e.target.closest && e.target.closest(".minicart__open--btn");
    if (openBtn) {
      e.preventDefault();
      openMinicart();
      return;
    }

    var closeBtn =
      e.target.closest && e.target.closest(".minicart__close--btn");
    if (closeBtn) {
      e.preventDefault();
      closeMinicart();
    }
  });
})();

// Removed: jQuery handler for .add__to--cart
// Đã sửa: Script.js (vanilla JS) xử lý thêm sản phẩm vào modal
$(document).on("click", "#quickview-add-to-cart", function (e) {
  e.preventDefault();

  // Get product ID from the form or modal data
  // Since we're inside the modal, get the form and extract product ID from action or data attribute
  var modal = $(this).closest("#modal1");
  var form = modal.find("form");

  // We need to extract product ID - it's not directly in the form, so we'll need to pass it
  // For now, we'll modify ProductPreview to include a hidden field with ProductId
  var productId = form.find('input[name="ProductId"]').val();

  if (!productId) {
    // Alternative: get from window or modal data if set by script.js
    productId = window.currentProductId;
  }

  var quantity = form.find(".quantity__number").val() || 1;
});

// Mini cart: increase / decrease quantity
// Chỉ xử lý buttons trong minicart để tránh conflict với script.js
// Event listener được thêm ngay lập tức để chạy trước script.js
(function () {
  // Thêm event listener ngay lập tức (không đợi DOM ready) để đảm bảo chạy trước script.js
  document.addEventListener(
    "click",
    function (e) {
      // Tìm button quantity (có thể click vào text node bên trong button)
      var button = e.target.closest && e.target.closest(".quantity__value");
      if (!button) return;

      // Chỉ xử lý nếu là button increase hoặc decrease
      if (
        !button.classList ||
        (!button.classList.contains("increase") &&
          !button.classList.contains("decrease"))
      ) {
        return;
      }

      var minicartContainer =
        button.closest && button.closest(".minicart__quantity");
      var cartRow =
        button.closest && button.closest(".cart__table--body__items");
      var isMiniCart = !!minicartContainer;
      var isCartPage = !!cartRow;

      if (!isMiniCart && !isCartPage) return;

      e.preventDefault();
      e.stopPropagation();
      e.stopImmediatePropagation();

      var productId = parseInt(button.getAttribute("data-id"), 10);
      if (!productId) {
        console.error("ProductId not found");
        return;
      }

      var wrapper = button.closest && button.closest(".quantity__box");
      var input = wrapper && wrapper.querySelector(".quantity__number");
      if (!input) {
        console.error("Input not found for productId:", productId);
        return;
      }

      var current = parseInt(input.value, 10) || 1;
      if (button.classList.contains("increase")) {
        current++;
      } else {
        current = Math.max(1, current - 1);
      }

      // Gọi API để update quantity
      fetch("/Cart/UpdateQuantity", {
        method: "POST",
        headers: {
          "Content-Type": "application/json",
        },
        body: JSON.stringify({ id: productId, quantity: current }),
      })
        .then(function (res) {
          return res.json();
        })
        .then(function (res) {
          if (res.success) {
            input.value = current; // Update input field
            if (isCartPage) {
              updateCartRowTotal(cartRow, current);
            }
            updateCartTotalDisplays(res.total);
            if (
              typeof window.setCartCount === "function" &&
              typeof res.cartCount !== "undefined"
            ) {
              window.setCartCount(res.cartCount);
            }
          } else {
            console.error("Update failed:", res.message);
          }
        })
        .catch(function (error) {
          console.error("Error updating quantity:", error);
        });
    },
    true
  ); // Use capture phase để chạy trước script.js
})();

// Mini cart: manual quantity change
$(document).on("change", ".quantity__number", function (e) {
  var $input = $(this);
  var productId = $input.data("id");
  var qty = parseInt($input.val()) || 1;
  if (qty < 1) qty = 1;
  $.ajax({
    url: "/Cart/UpdateQuantity",
    method: "POST",
    contentType: "application/json",
    data: JSON.stringify({ id: productId, quantity: qty }),
    success: function (res) {
      if (res.success) {
        updateCartTotalDisplays(res.total);
        var row = $input.closest(".cart__table--body__items");
        if (row.length) {
          updateCartRowTotal(row.get(0), qty);
        }
        if (
          typeof window.setCartCount === "function" &&
          typeof res.cartCount !== "undefined"
        ) {
          window.setCartCount(res.cartCount);
        }
      }
    },
  });
});

// Mini cart: remove item
$(document).on("click", ".minicart__product--remove", function (e) {
  e.preventDefault();
  var productId = $(this).data("id");
  $.ajax({
    url: "/Cart/RemoveFromCart",
    method: "POST",
    contentType: "application/json",
    data: JSON.stringify({ id: productId }),
    success: function (res) {
      if (res.success) {
        $.get("/Cart/MiniCart", function (html) {
          var minicartWrap = $(".minicart__product").parent();
          if (minicartWrap.length > 0) {
            minicartWrap.html(html);
          }
        });
        if (
          typeof window.setCartCount === "function" &&
          typeof res.cartCount !== "undefined"
        ) {
          window.setCartCount(res.cartCount);
        }
      }
    },
  });
});

function updateMiniCartTotal() {
  var total = 0;
  $(".minicart__product--items").each(function () {
    var price = parseFloat($(this).find(".current__price").data("price")) || 0;
    var qty = parseInt($(this).find(".quantity__number").val()) || 0;
    total += price * qty;
  });
  $("#minicart-total").text(total.toLocaleString("vi-VN"));
  return total;
}
function validateBeforeAdd(container) {
    const sizeInputs = container.find("input[name='size']");
    const colorInputs = container.find("input[name='color']");

    const size = sizeInputs.length ? sizeInputs.filter(":checked").val() : null;
    const color = colorInputs.length ? colorInputs.filter(":checked").val() : null;

    if (sizeInputs.length && !size) {
        alert("Bạn phải chọn size");
        return false;
    }
    if (colorInputs.length && !color) {
        alert("Bạn phải chọn màu");
        return false;
    }

    return true;
}
$(document).off("click", ".quickview__value--quantity").on("click", ".quickview__value--quantity", function () {

    const qtyBox = $(this).closest(".quantity__box");
    const input = qtyBox.find(".quickview__value--number");
    let value = parseInt(input.val());

    if ($(this).hasClass("increase")) {
        value++;
    }
    else if ($(this).hasClass("decrease") && value > 1) {
        value--;
    }

    input.val(value);
});
$(document).off("click", ".quickview__cart--btn").on("click", ".quickview__cart--btn", function (e) {
    if (e && e.preventDefault) e.preventDefault();
    const container = $(this).closest(".product__variant");
    if (!validateBeforeAdd(container)) {
        return;
    }

    let productId = $(this).data("id");
    let quantity = parseInt(container.find(".quickview__value--number").val()) || 1;

    let color = container.find("input[name='color']:checked").val() || null;
    let size = container.find("input[name='size']:checked").val() || null;
    
    addToCart(productId, size, color, quantity);
});

$(document).off("click", ".variant__buy--now__btn").on("click", ".variant__buy--now__btn", function (e) {
    if (e && e.preventDefault) e.preventDefault();
    const container = $(this).closest(".product__variant");
    if (!validateBeforeAdd(container)) {
        return;
    }

    let productId = $(this).data("id");
    let quantity = parseInt(container.find(".quickview__value--number").val()) || 1;

    let color = container.find("input[name='color']:checked").val() || null;
    let size = container.find("input[name='size']:checked").val() || null;

    fetch("/Checkout/BuyNow", {
        method: "POST",
        headers: {
            "Content-Type": "application/json",
        },
        body: JSON.stringify({
            productId: productId,
            quantity: quantity,
            size: size,
            color: color,
        }),
    })
        .then((res) => res.json())
        .then((data) => {
            if (data && data.success && data.redirect) {
                window.location = data.redirect;
            } else {
                alert(data?.message || "Không thể mua ngay. Vui lòng thử lại.");
            }
        })
        .catch(() => {
            alert("Không thể mua ngay. Vui lòng thử lại.");
        });
});