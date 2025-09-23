<template>
  <div class="login">
    <div class="card">
      <h2>Đăng nhập</h2>
      <form @submit.prevent="onSubmit">
        <label>
          Email
          <input v-model="email" type="email" placeholder="you@example.com" />
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

const email = ref('')
const password = ref('')
const router = useRouter()
const route = useRoute()
const auth = useAuthStore()

function onSubmit() {
  // Demo: chấp nhận bất kỳ thông tin, tạo token giả lập
  auth.login('demo-token')
  const redirect = route.query.redirect || '/'
  router.replace(String(redirect))
}
</script>

<style scoped>
.login {
  min-height: calc(100vh - 80px);
  display: flex;
  align-items: center;
  justify-content: center;
  padding: 24px;
}
.card {
  width: 100%;
  max-width: 360px;
  background: #fff;
  border-radius: 12px;
  box-shadow: 0 10px 30px rgba(0,0,0,0.08);
  padding: 24px;
}
form { display: grid; gap: 12px; }
label { display: grid; gap: 6px; font-weight: 600; }
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
button:hover { background: #2563eb; }
</style>


