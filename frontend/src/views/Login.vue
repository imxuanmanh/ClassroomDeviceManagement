<template>
  <!-- Template không thay đổi -->
  <div class="login-page-container">
    <div class="warrper">
      <span class="bg-animate"></span>
      <div class="form-box login">
        <h2>Đăng nhập</h2>
        <form @submit.prevent="onSubmit">
          <div class="input-box">
            <input type="text" v-model="username" required />
            <label>Tên đăng nhập</label>
            <i class="bx bxs-user"></i>
          </div>
          <div class="input-box">
            <input type="password" v-model="password" required />
            <label>Mật khẩu</label>
            <i class="bx bxs-lock"></i>
          </div>
          <button type="submit" class="btn">Đăng nhập</button>
        </form>
      </div>
      <div class="info-text login">
        <h2>Hệ thống Quản lý Thiết bị</h2>
      </div>
    </div>
  </div>
</template>

<script setup>
/* PHẦN SCRIPT GIỮ NGUYÊN
  Không có gì thay đổi ở đây.
*/
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

<style>
/* PHẦN CSS ĐÃ ĐƯỢC ĐIỀU CHỈNH
*/
@import url('https://fonts.googleapis.com/css2?family=Poppins:wght@300;400;500;600;700;800;900&display=swap');
* {
  margin: 0;
  padding: 0;
  box-sizing: border-box;
  font-family: 'Poppins', sans-serif;
}

/* *** THAY ĐỔI QUAN TRỌNG ĐỂ SỬA LỖI LỆCH TRÁI ***

  Chúng ta sẽ dùng 'position: fixed' để buộc container
  này chiếm toàn bộ không gian của trình duyệt,
  bất chấp các style của component cha.
*/
.login-page-container {
  display: flex;
  justify-content: center;
  align-items: center;
  background: #081b29;

  /* Các thuộc tính mới được thêm vào: */
  position: fixed; /* Hoặc 'absolute' */
  top: 0;
  left: 0;
  width: 100vw; /* Chiếm 100% chiều rộng viewport */
  height: 100vh; /* Chiếm 100% chiều cao viewport */
  z-index: 9999; /* Đảm bảo nó nằm trên cùng */
}

.warrper {
  position: relative;
  width: 750px;
  height: 450px;
  background: transparent;
  border: 2px solid #0ef;
  box-shadow: 0 0 25px #0ef;
  overflow: hidden;
}

.warrper .form-box {
  position: absolute;
  top: 0;
  width: 50%;
  height: 100%;
  display: flex;
  flex-direction: column;
  justify-content: center;
}

.warrper .form-box.login {
  left: 0;
  padding: 0 60px 0 40px;
}

.form-box h2 {
  font-size: 32px;
  color: #fff;
  text-align: center;
}

.form-box .input-box {
  position: relative;
  width: 100%;
  height: 50px;
  margin: 25px 0;
}

.input-box input {
  width: 100%;
  height: 100%;
  background: transparent;
  border: none;
  outline: none;
  border-bottom: 2px solid #fff;
  padding-right: 23px;
  font-size: 16px;
  color: #fff;
  font-weight: 500;
  transition:
    border-bottom-color 0.5s,
    background-color 5000s ease-in-out 0s;
}

.input-box input:focus,
.input-box input:valid {
  border-bottom-color: #0ef;
}

/* Khối CSS để sửa lỗi autofill (Giữ nguyên) */
.input-box input:-webkit-autofill,
.input-box input:-webkit-autofill:hover,
.input-box input:-webkit-autofill:focus,
.input-box input:-webkit-autofill:active {
  -webkit-text-fill-color: #fff !important;
  caret-color: #fff;
  box-shadow: 0 0 0 30px #081b29 inset !important;
  border-bottom: 2px solid #fff;
}

.input-box input:-webkit-autofill:focus,
.input-box input:-webkit-autofill:valid {
  border-bottom-color: #0ef !important;
}

.input-box label {
  position: absolute;
  top: 50%;
  left: 0;
  transform: translateY(-50%);
  font-size: 16px;
  color: #fff;
  pointer-events: none;
  transition: 0.5s;
}

.input-box input:focus ~ label,
.input-box input:valid ~ label {
  top: -5px;
  color: #0ef;
}

.input-box i {
  position: absolute;
  top: 50%;
  right: 0;
  transform: translateY(-50%);
  font-size: 18px;
  color: #fff;
  transition: 0.5s;
}

.input-box input:focus ~ i,
.input-box input:valid ~ i {
  color: #0ef;
}

.btn {
  position: relative;
  width: 100%;
  height: 45px;
  background: transparent;
  border: 2px solid #0ef;
  outline: none;
  border-radius: 40px;
  cursor: pointer;
  font-size: 16px;
  color: #fff;
  font-weight: 600;
  z-index: 1;
  overflow: hidden;
}

.btn::before {
  content: '';
  position: absolute;
  top: -100%;
  left: 0;
  width: 100%;
  height: 300%;
  background: linear-gradient(#081b29, #0ef, #081b29, #0ef);
  z-index: -1;
  transition: 0.5s;
}

.btn:hover::before {
  top: 0;
}

.form-box .logreg-link {
  font-size: 14.5px;
  color: #fff;
  text-align: center;
  margin: 20px 0 10px;
}

.logreg-link p a {
  color: #0ef;
  text-decoration: none;
  font-weight: 600;
}

.logreg-link p a:hover {
  text-decoration: underline;
}

.warrper .info-text {
  position: absolute;
  top: 0;
  width: 50%;
  height: 100%;
  display: flex;
  flex-direction: column;
  justify-content: center;
}

.warrper .info-text.login {
  right: 0;
  text-align: right;
  padding: 0 40px 60px 150px;
}

.info-text h2 {
  font-size: 36px;
  color: #fff;
  line-height: 1.3;
  text-transform: uppercase;
}

.info-text p {
  font-size: 16px;
  color: #fff;
}

.warrper .bg-animate {
  position: absolute;
  top: -4px;
  right: 0;
  width: 850px;
  height: 600px;
  background: linear-gradient(45deg, #081b29, #0ef);
  border-bottom: 3px solid #0ef;
  transform: rotate(10deg) skewY(40deg);
  transform-origin: bottom right;
}
</style>

<!-- <template>
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
</style> -->
