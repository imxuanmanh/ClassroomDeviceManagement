// src/utils/toast.js
import Swal from "sweetalert2";

// Cấu hình Toast chung
const ToastMixin = Swal.mixin({
  toast: true,
  position: "top-end",
  showConfirmButton: false,
  timer: 1500,
  timerProgressBar: true,
  background: "#222831",
  color: "#eeeeee",
  didOpen: (toast) => {
    toast.addEventListener("mouseenter", Swal.stopTimer);
    toast.addEventListener("mouseleave", Swal.resumeTimer);
  },
});

// Xuất các hàm dùng nhanh
export const toast = {
  success: (message) => {
    ToastMixin.fire({
      icon: "success",
      title: message,
      iconColor: "#10b981", // ✅ Màu xanh lá cho success
    });
  },
  error: (message) => {
    ToastMixin.fire({
      icon: "error",
      title: message,
      iconColor: "#ef4444", // ❌ Màu đỏ cho lỗi
    });
  },
  warning: (message) => {
    ToastMixin.fire({
      icon: "warning",
      title: message,
      iconColor: "#f59e0b", // ⚠️ Màu vàng cam
    });
  },
  info: (message) => {
    ToastMixin.fire({
      icon: "info",
      title: message,
      iconColor: "#00adb5", // ℹ️ Màu xanh theme
    });
  },
  // Hàm hiển thị hộp thoại xác nhận (Confirm Dialog)
  confirm: async (title, text, confirmText = "Đồng ý") => {
    const result = await Swal.fire({
      title: title,
      text: text,
      icon: "warning",
      showCancelButton: true,
      confirmButtonColor: "#ef4444",
      cancelButtonColor: "#393e46",
      confirmButtonText: confirmText,
      cancelButtonText: "Hủy bỏ",
      background: "#222831",
      color: "#eeeeee",
    });
    return result.isConfirmed;
  },
};
