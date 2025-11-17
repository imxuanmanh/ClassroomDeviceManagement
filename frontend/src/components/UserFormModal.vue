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
  background: rgba(0, 0, 0, 0.3);
  display: flex;
  align-items: center;
  justify-content: center;
  z-index: 1000;
}
.modal-content {
  background: #fff;
  padding: 32px 24px;
  border-radius: 16px;
  min-width: 350px;
  box-shadow: 0 8px 32px rgba(0, 0, 0, 0.18);
  display: flex;
  flex-direction: column;
  gap: 16px;
}
.modal-content h3 {
  margin: 0 0 8px 0;
  text-align: center;
  font-size: 1.2rem;
  color: #2d3748;
}
.modal-content input,
.modal-content select {
  padding: 8px 10px;
  border: 1px solid #e5e7eb;
  border-radius: 8px;
  font-size: 1rem;
}
.modal-actions {
  display: flex;
  gap: 12px;
  justify-content: flex-end;
}
</style>
