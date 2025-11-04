<template>
  <div class="login-page">
    <div class="login-card">
      <div class="login-header">
        <img src="/logo.png" alt="Logo" class="login-logo" />
        <h2>Hệ thống Quản lý Thiết bị</h2>
      </div>

      <form @submit.prevent="onSubmit" class="login-form">
        <div class="form-group">
          <label for="username">Tên đăng nhập</label>
          <input
            id="username"
            v-model="username"
            type="text"
            placeholder="Nhập tên đăng nhập"
            required
          />
        </div>

        <div class="form-group">
          <label for="password">Mật khẩu</label>
          <input id="password" v-model="password" type="password" placeholder="••••••••" required />
        </div>

        <button type="submit" class="btn-login">Đăng nhập</button>
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
    const res = await authApi.login({
      username: username.value,
      password: password.value,
    })

    if (res && res.token && res.user) {
      auth.login(res.token, res.user)
      const redirect = route.query.redirect
      if (redirect) router.replace(String(redirect))
      else if (res.user.roleId === 1) router.replace('/devices')
      else router.replace('/user-borrow')
    } else {
      alert('Sai tài khoản hoặc mật khẩu!')
    }
  } catch (err) {
    console.error(err)
    alert('Đăng nhập thất bại!')
  }
}
</script>

<style scoped>
.login-page {
  display: flex;
  align-items: center; /* Căn giữa theo chiều dọc */
  justify-content: center; /* Căn giữa theo chiều ngang */
  height: 100vh;
  width: 100vw;
  background: linear-gradient(135deg, #3b82f6, #06b6d4);
}

.login-container {
  background: white;
  padding: 40px;
  border-radius: 16px;
  box-shadow: 0 8px 20px rgba(0, 0, 0, 0.2);
  width: 380px;
}

.login-card {
  background: white;
  padding: 2.5rem;
  border-radius: 20px;
  box-shadow: 0 15px 35px rgba(0, 0, 0, 0.15);
  width: 100%;
  max-width: 400px;
  text-align: center;
  animation: fadeIn 0.6s ease;
}

.login-header {
  margin-bottom: 1.5rem;
}

.login-logo {
  width: 60px;
  height: 60px;
  object-fit: contain;
  margin-bottom: 1rem;
}

.login-header h2 {
  font-size: 1.5rem;
  color: #1e3a8a;
  margin-bottom: 0.3rem;
}

.login-header p {
  color: #6b7280;
  font-size: 0.9rem;
}

/* Form */
.login-form {
  display: flex;
  flex-direction: column;
  gap: 1rem;
}

.form-group {
  text-align: left;
}

label {
  font-weight: 600;
  color: #374151;
  margin-bottom: 4px;
  display: block;
}

input {
  width: 100%;
  border: 1px solid #d1d5db;
  border-radius: 10px;
  padding: 10px 12px;
  font-size: 0.95rem;
  transition: all 0.2s ease;
}

input:focus {
  border-color: #3b82f6;
  box-shadow: 0 0 0 3px rgba(59, 130, 246, 0.25);
  outline: none;
}

/* Nút đăng nhập */
.btn-login {
  background: linear-gradient(90deg, #3b82f6, #06b6d4);
  color: white;
  border: none;
  border-radius: 10px;
  padding: 12px;
  font-weight: 600;
  cursor: pointer;
  font-size: 1rem;
  transition:
    transform 0.2s,
    box-shadow 0.2s;
}

.btn-login:hover {
  transform: translateY(-1px);
  box-shadow: 0 6px 14px rgba(59, 130, 246, 0.3);
}

/* Hiệu ứng fade */
@keyframes fadeIn {
  from {
    opacity: 0;
    transform: translateY(-20px);
  }
  to {
    opacity: 1;
    transform: translateY(0);
  }
}
</style>
