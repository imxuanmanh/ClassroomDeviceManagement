<template>
  <section class="users">
    <header class="page-header">
      <h2>Người dùng</h2>
      <div class="actions">
        <button v-if="isAdmin" @click="showForm = true">Thêm người dùng</button>
      </div>
    </header>

    <!-- Form thêm người dùng -->
    <UserFormModal v-if="showForm" @register="register" @cancel="cancel" />

    <!-- Thông báo thành công -->
    <transition name="toast">
      <div v-if="success" class="toast-success">
        <svg
          xmlns="http://www.w3.org/2000/svg"
          width="20"
          height="20"
          viewBox="0 0 24 24"
          fill="none"
          stroke="currentColor"
          stroke-width="2"
          stroke-linecap="round"
          stroke-linejoin="round"
          class="toast-icon"
        >
          <path d="M22 11.08V12a10 10 0 1 1-5.93-9.14"></path>
          <polyline points="22 4 12 14.01 9 11.01"></polyline>
        </svg>
        <span>{{ success }}</span>
        <button class="toast-close" @click="success = ''">✖</button>
      </div>
    </transition>

    <!-- Thông báo lỗi -->
    <div v-if="error" class="error">{{ error }}</div>

    <!-- Bảng người dùng -->
    <table class="table">
      <thead>
        <tr>
          <th>Tên đăng nhập</th>
          <th>Họ và tên</th>
          <th>Email</th>
          <th>Vai trò</th>
        </tr>
      </thead>
      <tbody>
        <tr v-for="u in users" :key="u.username">
          <td>{{ u.username }}</td>
          <td>{{ u.fullname }}</td>
          <td>{{ u.email }}</td>
          <td>
            {{ roleName(u.role) }}
          </td>
        </tr>
      </tbody>
    </table>
  </section>
</template>

<script setup>
import { ref, onMounted } from 'vue'
//import { userApi, authApi } from '@/config/api.js'
import { userApi, authApi } from '@/config/apiWrapper.js'
import UserFormModal from '@/components/UserFormModal.vue'
import { useAuthStore } from '@/stores/auth'

const auth = useAuthStore()
const isAdmin = auth.roleId === 1

const users = ref([])
const showForm = ref(false)
const error = ref('')
const success = ref('')

// Gọi API lấy danh sách người dùng
async function fetchUsers() {
  try {
    const res = await userApi.getAll()
    users.value = Array.isArray(res)
      ? res.map((u) => ({
          username: u.username,
          fullname: u.fullname,
          email: u.email,
          role: u.role,
        }))
      : []
  } catch (err) {
    console.error('Lỗi tải danh sách người dùng:', err)
    error.value = 'Không thể tải danh sách người dùng'
  }
}

// Đăng ký người dùng mới
async function register(form) {
  if (!form.username || !form.fullname || !form.email || !form.password || !form.roleId) return
  try {
    await authApi.register(form)
    await fetchUsers() // cập nhật lại danh sách sau khi thêm
    success.value = 'Thêm người dùng thành công!'
    error.value = ''
    cancel()

    // Tự động ẩn thông báo sau 3 giây
    setTimeout(() => {
      success.value = ''
    }, 3000)
  } catch (err) {
    console.error('Lỗi đăng ký người dùng:', err)
    error.value = 'Không thể đăng ký người dùng'
    success.value = ''
  }
}

function cancel() {
  showForm.value = false
}

// Hàm hiển thị tên vai trò
function roleName(role) {
  switch (role) {
    case 1:
      return 'Quản trị viên'
    case 2:
      return 'Giảng viên'
    case 3:
      return 'Sinh viên'
    default:
      return 'Không xác định'
  }
}

// Tự động tải khi component được mount
onMounted(fetchUsers)
</script>

<style scoped>
.users {
  padding: 16px 12px;
  color: #eeeeee;
}
.page-header {
  display: flex;
  align-items: center;
  justify-content: space-between;
  gap: 12px;
  margin-bottom: 16px;
}
.page-header h2 {
  margin: 0;
  color: #00adb5;
  text-shadow: 0 0 10px rgba(0, 173, 181, 0.5);
}
.actions {
  display: flex;
  gap: 8px;
}

/* Nút "Thêm người dùng" */
.actions button {
  background: #00adb5;
  color: #222831;
  border: none;
  border-radius: 8px;
  padding: 6px 12px;
  cursor: pointer;
  font-weight: 600;
  transition: 0.2s;
}
.actions button:hover {
  background: #eeeeee;
  color: #222831;
}

.table {
  width: 100%;
  border-collapse: collapse;
  background: #393e46;
  border-radius: 12px;
  overflow: hidden;
  border: 1px solid rgba(0, 173, 181, 0.2);
  box-shadow: 0 0 20px rgba(0, 173, 181, 0.15);
}
.table th,
.table td {
  border: 1px solid rgba(0, 173, 181, 0.15);
  padding: 10px 12px;
  text-align: left;
}
.table th {
  background: #222831;
  color: #00adb5;
  font-weight: 600;
  text-transform: uppercase;
  font-size: 12px;
}
.table tbody tr:hover {
  background: rgba(0, 173, 181, 0.05);
}

.table td:last-child {
  color: rgba(238, 238, 238, 0.9);
  font-weight: 500;
}

/* Thông báo thành công */
.success {
  color: #4ade80;
  margin-bottom: 8px;
  background: rgba(74, 222, 128, 0.1);
  border: 1px solid rgba(74, 222, 128, 0.3);
  padding: 8px 12px;
  border-radius: 8px;
  animation: slideDown 0.3s ease;
}

/* Thông báo lỗi */
.error {
  color: #ff8a80;
  margin-bottom: 8px;
  background: rgba(255, 138, 128, 0.1);
  border: 1px solid rgba(255, 138, 128, 0.3);
  padding: 8px 12px;
  border-radius: 8px;
  animation: slideDown 0.3s ease;
}

@keyframes slideDown {
  from {
    opacity: 0;
    transform: translateY(-10px);
  }
  to {
    opacity: 1;
    transform: translateY(0);
  }
}

/* --- TOAST NOTIFICATION CSS --- */
.toast-success {
  position: fixed; /* Ghim vào màn hình */
  top: 20px;
  right: 20px;
  z-index: 9999; /* Đảm bảo luôn nổi trên cùng */

  display: flex;
  align-items: center;
  gap: 12px;

  background: rgba(20, 30, 35, 0.95); /* Nền tối, hơi trong suốt */
  color: #4ade80; /* Màu chữ xanh lá */
  border-left: 4px solid #4ade80; /* Đường viền nhấn bên trái */

  padding: 16px 20px;
  border-radius: 8px;
  box-shadow: 0 10px 30px rgba(0, 0, 0, 0.5); /* Đổ bóng tạo chiều sâu */
  backdrop-filter: blur(5px); /* Hiệu ứng mờ nền phía sau (nếu trình duyệt hỗ trợ) */

  font-weight: 500;
  min-width: 300px;
}

.toast-icon {
  color: #4ade80;
  flex-shrink: 0;
}

/* Nút đóng */
.toast-close {
  background: transparent;
  border: none;
  color: #6b7280;
  font-size: 16px;
  cursor: pointer;
  padding: 4px;
  margin-left: auto; /* Đẩy nút đóng sang tận cùng bên phải */
  transition: color 0.2s;
}
.toast-close:hover {
  color: #eeeeee;
}

/* Vue Transition: Hiệu ứng trượt từ phải vào */
.toast-enter-active,
.toast-leave-active {
  transition: all 0.4s cubic-bezier(0.175, 0.885, 0.32, 1.275); /* Hiệu ứng nảy nhẹ */
}

.toast-enter-from,
.toast-leave-to {
  opacity: 0;
  transform: translateX(100%); /* Bắt đầu từ ngoài màn hình bên phải */
}
</style>
