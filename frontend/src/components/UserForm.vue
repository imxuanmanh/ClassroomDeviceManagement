<template>
  <div class="form-box">
    <input v-model="form.username" placeholder="Tên người dùng" />
    <input v-model="form.fullname" type="text" placeholder="Họ và tên" />
    <input v-model="form.password" type="password" placeholder="Mật khẩu" />
    <input v-model="form.email" type="email" placeholder="Email" />
    <select v-model="form.roleId">
      <option disabled value="">Chọn vai trò</option>

      <option :value="2">Giảng viên</option>
      <option :value="3">Sinh viên</option>
    </select>
    <button @click="onSave">Lưu</button>
    <button @click="$emit('cancel')">Hủy</button>
  </div>
</template>

<script setup>
import { reactive } from 'vue'
const emit = defineEmits(['save', 'cancel'])

const form = reactive({ username: '', fullname: '', password: '', email: '', roleId: '' })

function onSave() {
  emit('save', { ...form })
  form.username = ''
  form.fullname = ''
  form.password = ''
  form.email = ''
  form.roleId = ''
}
</script>

<style scoped>
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
</style>
