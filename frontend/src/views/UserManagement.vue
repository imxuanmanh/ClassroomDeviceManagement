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
import { userApi, authApi } from '@/config/api.js'
import UserFormModal from '@/components/UserFormModal.vue'
import { useAuthStore } from '@/stores/auth'

const auth = useAuthStore()
const isAdmin = auth.roleId === 1

const users = ref([])
const showForm = ref(false)
const error = ref('')

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
    cancel()
  } catch (err) {
    console.error('Lỗi đăng ký người dùng:', err)
    error.value = 'Không thể đăng ký người dùng'
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
  color: #eeeeee; /* ✅ Chữ chính */
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
  color: #00adb5; /* ✅ Chữ nhấn */
  text-shadow: 0 0 10px rgba(0, 173, 181, 0.5);
}
.actions {
  display: flex;
  gap: 8px;
}

/* Nút "Thêm người dùng" */
.actions button {
  background: #00adb5; /* Nền nhấn */
  color: #222831; /* Chữ tối để tương phản */
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

/* Lớp .form-box không được dùng trong template nên tôi đã bỏ qua */

.table {
  width: 100%;
  border-collapse: collapse;
  background: #393e46; /* Nền phụ */
  border-radius: 12px;
  overflow: hidden;
  /* Thêm viền và bóng cho nhất quán */
  border: 1px solid rgba(0, 173, 181, 0.2);
  box-shadow: 0 0 20px rgba(0, 173, 181, 0.15);
}
.table th,
.table td {
  border: 1px solid rgba(0, 173, 181, 0.15); /* Viền nhấn mờ */
  padding: 10px 12px;
  text-align: left;
}
.table th {
  background: #222831; /* Nền chính */
  color: #00adb5; /* ✅ Chữ nhấn */
  font-weight: 600;
  text-transform: uppercase;
  font-size: 12px;
}
/* Thêm hover cho hàng */
.table tbody tr:hover {
  background: rgba(0, 173, 181, 0.05);
}

/* Cột vai trò */
.table td:last-child {
  color: rgba(238, 238, 238, 0.9);
  font-weight: 500;
}

.error {
  color: #ff8a80; /* Màu đỏ sáng dễ đọc */
  margin-bottom: 8px;
  background: rgba(255, 138, 128, 0.1);
  border: 1px solid rgba(255, 138, 128, 0.3);
  padding: 8px 12px;
  border-radius: 8px;
}
</style>

<!-- <style scoped>
.users {
  padding: 16px 12px;
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
  color: #111827;
}
.actions {
  display: flex;
  gap: 8px;
}
.form-box {
  display: flex;
  gap: 8px;
  margin-bottom: 12px;
}
.form-box input,
.form-box select {
  padding: 6px 8px;
  border: 1px solid #e5e7eb;
  border-radius: 8px;
}
.table {
  width: 100%;
  border-collapse: collapse;
  background: #fff;
  border-radius: 12px;
  overflow: hidden;
}
.table th,
.table td {
  border: 1px solid #eee;
  padding: 10px;
  text-align: left;
}
.table th {
  background: #f8fafc;
}
.error {
  color: #b91c1c;
  margin-bottom: 8px;
}
</style> -->
