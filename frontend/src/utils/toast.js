// src/utils/toast.js
import Swal from 'sweetalert2'

// Cấu hình Toast chung
const ToastMixin = Swal.mixin({
  toast: true,
  position: 'top-end', // Hiện ở góc trên bên phải
  showConfirmButton: false, // Ẩn nút OK
  timer: 3000, // Tự tắt sau 3 giây
  timerProgressBar: true,
  background: '#222831', // Màu nền tối (hợp với theme của bạn)
  color: '#eeeeee', // Màu chữ trắng
  iconColor: '#00adb5', // Màu icon xanh theme
  didOpen: (toast) => {
    toast.addEventListener('mouseenter', Swal.stopTimer)
    toast.addEventListener('mouseleave', Swal.resumeTimer)
  },
})

// Xuất các hàm dùng nhanh
export const toast = {
  success: (message) => {
    ToastMixin.fire({
      icon: 'success',
      title: message,
    })
  },
  error: (message) => {
    ToastMixin.fire({
      icon: 'error',
      title: message,
      iconColor: '#ef4444', // Màu đỏ cho lỗi
    })
  },
  warning: (message) => {
    ToastMixin.fire({
      icon: 'warning',
      title: message,
      iconColor: '#f59e0b', // Màu vàng cam
    })
  },
  info: (message) => {
    ToastMixin.fire({
      icon: 'info',
      title: message,
    })
  },
  // Hàm hiển thị hộp thoại xác nhận (Confirm Dialog)
  confirm: async (title, text, confirmText = 'Đồng ý') => {
    const result = await Swal.fire({
      title: title,
      text: text,
      icon: 'warning',
      showCancelButton: true,
      confirmButtonColor: '#ef4444',
      cancelButtonColor: '#393e46',
      confirmButtonText: confirmText,
      cancelButtonText: 'Hủy bỏ',
      background: '#222831',
      color: '#eeeeee',
    })
    return result.isConfirmed
  },
}
