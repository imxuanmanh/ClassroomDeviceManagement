<template>
  <section class="users">
    <header class="page-header">
      <h2>Người dùng</h2>
      <div class="actions">
        <button v-if="isAdmin" @click="showForm = true">Thêm người dùng</button>
      </div>
    </header>

    <UserFormModal v-if="showForm" @register="register" @cancel="cancel" />

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
import { userApi, authApi } from '@/config/apiWrapper.js'
import UserFormModal from '@/components/Device/UserFormModal.vue'
import { useAuthStore } from '@/stores/auth'
// 👇 Import Toast Utility
import { toast } from '@/utils/toast.js'

const auth = useAuthStore()
const isAdmin = auth.roleId === 1

const users = ref([])
const showForm = ref(false)

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
    // 🔥 Dùng Toast báo lỗi
    toast.error('Không thể tải danh sách người dùng')
  }
}

// Đăng ký người dùng mới
async function register(form) {
  if (!form.username || !form.fullname || !form.email || !form.password || !form.roleId) return
  try {
    await authApi.register(form)
    await fetchUsers() // cập nhật lại danh sách sau khi thêm

    // 🔥 Dùng Toast báo thành công
    toast.success('Thêm người dùng thành công!')

    cancel()
  } catch (err) {
    console.error('Lỗi đăng ký người dùng:', err)
    // 🔥 Dùng Toast báo lỗi
    toast.error('Không thể đăng ký người dùng. Có thể tên đăng nhập đã tồn tại.')
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


</style>
