<template>
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

          <!-- Thay đổi: Thêm container để đặt nút và loading cạnh nhau -->
          <div class="button-container">
            <button type="submit" class="btn" :disabled="isLoading">Đăng nhập</button>
            <div v-if="isLoading" class="ring-loader"></div>
          </div>
        </form>
      </div>
      <div class="info-text login">
        <h2>Hệ thống Quản lý Thiết bị</h2>
      </div>
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
const isLoading = ref(false)

async function onSubmit() {
  isLoading.value = true

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
  } finally {
    isLoading.value = false
  }
}
</script>

<style>
@import url('https://fonts.googleapis.com/css2?family=Poppins:wght@300;400;500;600;700;800;900&display=swap');
* {
  margin: 0;
  padding: 0;
  box-sizing: border-box;
  font-family: 'Poppins', sans-serif;
}

.login-page-container {
  display: flex;
  justify-content: center;
  align-items: center;
  background: #081b29;
  position: fixed;
  top: 0;
  left: 0;
  width: 100vw;
  height: 100vh;
  z-index: 9999;
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

/* Container mới để đặt button và loader cạnh nhau */
.button-container {
  display: flex;
  align-items: center;
  gap: 15px;
}

.btn {
  position: relative;
  flex: 1; /* Cho nút chiếm phần còn lại của container */
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

.btn:disabled {
  opacity: 0.7;
  cursor: not-allowed;
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

.btn:hover:not(:disabled)::before {
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

/* Loader nằm bên cạnh nút */
.ring-loader {
  width: 30px;
  height: 30px;
  border-radius: 50%;
  border: 4px solid #46dff0;
  border-top-color: #081b29;
  animation: spin 1s linear infinite;
  flex-shrink: 0; /* Không cho loader bị co lại */
}

@keyframes spin {
  100% {
    transform: rotate(360deg);
  }
}
</style>
