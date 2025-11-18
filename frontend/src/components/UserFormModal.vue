<template>
  <div class="modal-overlay">
    <div class="modal-content">
      <h3>Thêm người dùng</h3>
      <input v-model="form.username" placeholder="Tên người dùng" />
      <input v-model="form.fullname" type="text" placeholder="Họ và tên" autocomplete="off" />
      <input
        v-model="form.password"
        type="password"
        placeholder="Mật khẩu"
        autocomplete="new-password"
      />
      <input v-model="form.email" type="email" placeholder="Email" />
      <select v-model="form.roleId">
        <option disabled value="">Chọn vai trò</option>
        <option :value="2">Giảng viên</option>
        <option :value="3">Sinh viên</option>
      </select>
      <div class="modal-actions">
        <button @click="onRegister">Đăng ký</button>
        <button @click="$emit('cancel')">Hủy</button>
      </div>
    </div>
  </div>
</template>

<script setup>
import { reactive } from 'vue'
const emit = defineEmits(['register', 'cancel'])

const form = reactive({ username: '', fullname: '', password: '', email: '', roleId: '' })

function onRegister() {
  emit('register', { ...form })
  form.username = ''
  form.fullname = ''
  form.password = ''
  form.email = ''
  form.roleId = ''
}
</script>

<style scoped>
.modal-overlay {
  position: fixed;
  top: 0;
  left: 0;
  right: 0;
  bottom: 0;
  background: rgba(0, 0, 0, 0.7); /* THAY ĐỔI: Nền tối hơn */
  display: flex;
  align-items: center;
  justify-content: center;
  z-index: 1000;
}
.modal-content {
  background: #393e46; /* THAY ĐỔI: Nền phụ */
  padding: 32px 24px;
  border-radius: 16px;
  min-width: 350px;
  /* THAY ĐỔI: Đổ bóng và viền màu nhấn */
  box-shadow: 0 0 30px rgba(0, 173, 181, 0.25);
  border: 1px solid rgba(0, 173, 181, 0.3);
  display: flex;
  flex-direction: column;
  gap: 16px;
}
.modal-content h3 {
  margin: 0 0 8px 0;
  text-align: center;
  font-size: 1.2rem;
  color: #00adb5; /* ✅ Chữ nhấn */
}
.modal-content input,
.modal-content select {
  padding: 8px 10px;
  /* THAY ĐỔI: Cập nhật input/select */
  border: 1px solid rgba(0, 173, 181, 0.3);
  background: #222831;
  color: #eeeeee; /* ✅ Chữ chính */
  border-radius: 8px;
  font-size: 1rem;
  outline: none;
  transition: all 0.2s ease;
}

/* Thêm placeholder */
.modal-content input::placeholder {
  color: rgba(238, 238, 238, 0.7); /* ✅ Chữ phụ */
}

/* Thêm focus */
.modal-content input:focus,
.modal-content select:focus {
  border-color: #00adb5;
  box-shadow: 0 0 10px rgba(0, 173, 181, 0.3);
}

.modal-actions {
  display: flex;
  gap: 12px;
  justify-content: flex-end;
  margin-top: 8px; /* Thêm chút khoảng cách */
}

/* Style cho các nút */
.modal-actions button {
  border: none;
  border-radius: 8px;
  padding: 8px 16px;
  cursor: pointer;
  font-weight: 600;
  transition: all 0.2s;
  font-size: 14px;
}

/* Nút Đăng ký (Nút chính) */
.modal-actions button:first-of-type {
  background: #00adb5; /* Nền nhấn */
  color: #222831; /* Chữ tối */
}
.modal-actions button:first-of-type:hover {
  background: #eeeeee;
  color: #222831;
}

/* Nút Hủy (Nút phụ) */
.modal-actions button:last-of-type {
  background: rgba(238, 238, 238, 0.1); /* Nền phụ */
  color: #eeeeee; /* Chữ chính */
}
.modal-actions button:last-of-type:hover {
  background: rgba(238, 238, 238, 0.2);
}
</style>
