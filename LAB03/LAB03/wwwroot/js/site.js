$(document).ready(function () {
    // Cập nhật tổng tiền khi thay đổi số lượng sản phẩm
    $(".quantity-input").on("input", function () {
        let totalAmount = 0;

        // Duyệt qua từng sản phẩm trong giỏ hàng
        $(".quantity-input").each(function () {
            let productId = $(this).data("product-id");
            let quantity = parseInt($(this).val());

            // Lấy giá từ cột Price (không có dấu "Vnd")
            let price = parseFloat($(".price[data-product-id='" + productId + "']").text().trim().replace(',', '').replace('Vnd', ''));

            // Kiểm tra nếu giá trị hợp lệ và tính tổng tiền cho mỗi sản phẩm
            if (!isNaN(quantity) && !isNaN(price)) {
                let totalPrice = quantity * price;

                // Cập nhật lại tổng tiền cho từng sản phẩm
                $(".total-price[data-product-id='" + productId + "']").text(totalPrice.toLocaleString("vi-VN", { minimumFractionDigits: 0, maximumFractionDigits: 0 }) + " Vnd");

                // Cộng vào tổng giỏ hàng
                totalAmount += totalPrice;
            }
        });

        // Hiển thị tổng tiền giỏ hàng
        if (!isNaN(totalAmount)) {
            $("#totalAmountDisplay").val(totalAmount.toLocaleString("vi-VN", { minimumFractionDigits: 0, maximumFractionDigits: 0 }) + " Vnd");
        } else {
            $("#totalAmountDisplay").val("Lỗi tính toán!");
        }
    });

    // Gọi hàm tính toán khi trang load
    $(".quantity-input").trigger("input");
});


$(document).ready(function () {
    // Hiển thị nút cuộn lên đầu trang khi người dùng cuộn xuống hơn 100px
    $(window).scroll(function () {
        if ($(this).scrollTop() > 100) { // Khi cuộn xuống hơn 100px
            $('#scrollTopBtn').fadeIn();
        } else {
            $('#scrollTopBtn').fadeOut();
        }
    });

    // Cuộn lên đầu trang khi người dùng nhấp vào nút
    $('#scrollTopBtn').click(function () {
        $('html, body').animate({ scrollTop: 0 }, 600); // Cuộn lên đầu trang
        return false;
    });
});


// Kiểm tra và áp dụng chế độ lưu trữ trước đó (nếu có)
if (localStorage.getItem('theme') === 'dark') {
    document.body.classList.add('dark-theme');
    document.getElementById('toggleThemeBtn').innerHTML = '<i class="bi bi-sun"></i> ';
} else {
    document.body.classList.remove('dark-theme');
    document.getElementById('toggleThemeBtn').innerHTML = '<i class="bi bi-moon"></i> ';
}

document.getElementById('toggleThemeBtn').addEventListener('click', function () {
    // Chuyển đổi giữa chế độ sáng và tối
    document.body.classList.toggle('dark-theme');

    // Lưu trạng thái chế độ vào localStorage
    if (document.body.classList.contains('dark-theme')) {
        localStorage.setItem('theme', 'dark');
        document.getElementById('toggleThemeBtn').innerHTML = '<i class="bi bi-sun"></i> ';
    } else {
        localStorage.setItem('theme', 'light');
        document.getElementById('toggleThemeBtn').innerHTML = '<i class="bi bi-moon"></i> ';
    }
});

$(document).ready(function () {
    $(".add-to-cart-btn")("click", function () {  // Sử dụng .one() để sự kiện chỉ được gọi một lần
        var productId = $(this).data("product-id");
        var quantity = $(this).data("quantity"); // Lấy số lượng từ thuộc tính data-quantity

        $.ajax({
            url: '/ShoppingCart/AddToCartAjax',  // Đảm bảo URL này đúng
            type: 'POST',
            data: {
                productId: productId,
                quantity: quantity
            },
            success: function (response) {
                if (response.success) {
                    // Hiển thị thông báo thành công
                    alert(response.message);
                    // Cập nhật số lượng giỏ hàng
                    $("#cartCount").text(response.cartCount);  // Giả sử bạn có #cartCount để hiển thị số lượng
                } else {
                    alert(response.message);
                }
            },
            error: function () {
                alert("Có lỗi xảy ra, vui lòng thử lại!");
            }
        });
    });
});

