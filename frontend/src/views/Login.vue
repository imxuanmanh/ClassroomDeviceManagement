<template>
  <div class="login">
    <div class="card">
      <h2>Đăng nhập</h2>
      <form @submit.prevent="onSubmit">
        <label>
          Tên đăng nhập
          <input v-model="username" type="text" placeholder="Nhập tên đăng nhập" />
        </label>
        <label>
          Mật khẩu
          <input v-model="password" type="password" placeholder="••••••••" />
        </label>
        <button type="submit">Đăng nhập</button>
      </form>
    </div>
  </div>
</template>

<script setup>
import { ref } from 'vue'
import { useRouter, useRoute } from 'vue-router'
import { useAuthStore } from '@/stores/auth.js'
import { authApi } from '@/config/api'

const username = ref('')
const password = ref('')
const router = useRouter()
const route = useRoute()
const auth = useAuthStore()

async function onSubmit() {
  try {
    // Gửi request đăng nhập lên server với username và password
    const res = await authApi.login({ username: username.value, password: password.value })
    console.log('Kết quả trả về từ API:', res) // <-- Thêm dòng này để log kết quả
    // Server trả về { token, role, expires }
    if (res && res.token && res.role) {
      auth.login(res.token, res.role)
      const redirect = route.query.redirect
      if (redirect) {
        router.replace(String(redirect))
      } else if (res.role.toLowerCase() === 'admin') {
        router.replace('/')
      } else {
        router.replace('/user-borrow')
      }
    } else {
      alert('Sai tài khoản hoặc mật khẩu!')
    }
  } catch (err) {
    alert('Đăng nhập thất bại!')
  }
}
</script>

<style scoped>
.login {
  min-height: calc(90vh - 90px);
  display: flex;
  align-items: center;
  justify-content: center;
  padding: 24px;
}
.card {
  width: 100%;
  max-width: 25rem;
  /*background: linear-gradient(135deg, #66a8ea 0%, #996cc5 100%);*/
  border-radius: 12px;
  box-shadow: 0 10px 30px rgba(0, 0, 0, 0.08);
  padding: 24px;
}
h2 {
  margin: 0;
  padding-bottom: 2rem;
  text-align: center;
}
form {
  display: grid;
  gap: 12px;
}
label {
  display: grid;
  gap: 6px;
  font-weight: 600;
}
input {
  border: 1px solid #e5e7eb;
  border-radius: 8px;
  padding: 10px 12px;
}
button {
  margin-top: 8px;
  border: none;
  background: #3b82f6;
  color: #fff;
  padding: 10px 12px;
  border-radius: 8px;
  cursor: pointer;
}
button:hover {
  background: #2563eb;
}
</style>
