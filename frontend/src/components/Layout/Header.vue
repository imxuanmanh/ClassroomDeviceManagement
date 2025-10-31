<template>
  <header class="header">
    <button class="logout-btn" @click="handleLogout">Đăng xuất</button>
  </header>
</template>

<script setup>
import { useAuthStore } from '@/stores/auth'
import { useRouter } from 'vue-router'

const auth = useAuthStore()
const router = useRouter()

function handleLogout() {
  auth.logout()
  router.replace('/login')
}
</script>

<style scoped>
.header {
  display: flex;
  align-items: center;
  justify-content: flex-end; /* ✅ Đẩy nút sang phải */
  background: #417c85; /* giữ màu đồng bộ Layout */
  height: 64px;
  padding: 0 24px;
  box-sizing: border-box;
}

.logout-btn {
  background: #ef4444;
  color: white;
  border: none;
  border-radius: 6px;
  padding: 8px 16px;
  cursor: pointer;
  font-weight: 500;
  font-size: 14px;
  transition: background 0.2s ease;
}

.logout-btn:hover {
  background: #dc2626;
}
</style>
