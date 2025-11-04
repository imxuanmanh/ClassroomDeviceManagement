<template>
  <header class="header">
    <div class="header-left">
      <!-- Nút toggle sidebar -->
      <button class="menu-toggle" @click="$emit('toggle-sidebar')">
        <span class="material-icons">menu</span>
      </button>

      <!-- Tiêu đề -->
      <h1 class="app-title">Quản lý Thiết bị</h1>
    </div>

    <div class="header-right">
      <div class="profile" @click="toggleDropdown">
        <img src="/logo.png" alt="avatar" class="avatar" />
        <span>{{ auth.fullname || 'Admin' }}</span>
        <span class="material-icons">expand_more</span>

        <div v-if="dropdownOpen" class="dropdown">
          <button @click="goProfile">Hồ sơ</button>
          <button @click="handleLogout">Đăng xuất</button>
        </div>
      </div>
    </div>
  </header>
</template>

<script setup>
import { ref } from 'vue'
import { useRouter } from 'vue-router'
import { useAuthStore } from '@/stores/auth'

const auth = useAuthStore()
const router = useRouter()
const dropdownOpen = ref(false)

function toggleDropdown() {
  dropdownOpen.value = !dropdownOpen.value
}

function goProfile() {
  router.push('/profile')
  dropdownOpen.value = false
}

function handleLogout() {
  auth.logout()
  router.replace('/login')
}
</script>

<style scoped>
.header {
  display: flex;
  justify-content: space-between;
  align-items: center;
  height: 64px;
  background: linear-gradient(90deg, #417c85, #2a5d65);
  color: white;
  padding: 0 24px;
  box-shadow: 0 2px 4px rgba(0, 0, 0, 0.1);
}

/* ✅ Gom nút menu và tiêu đề thành một nhóm riêng */
.header-left {
  display: flex;
  align-items: center;
  gap: 12px; /* tạo khoảng cách giữa nút menu và chữ */
}

/* Nút menu */
.menu-toggle {
  background: none;
  border: none;
  color: white;
  font-size: 24px;
  cursor: pointer;
  width: 36px;
  height: 36px;
  border-radius: 8px;
  display: flex;
  align-items: center;
  justify-content: center;
  transition: background 0.2s;
}

.menu-toggle:hover {
  background: rgba(255, 255, 255, 0.15);
}

/* Tiêu đề */
.app-title {
  font-size: 18px;
  font-weight: 600;
  letter-spacing: 0.5px;
  margin: 0;
}

.profile {
  display: flex;
  align-items: center;
  gap: 8px;
  position: relative;
  cursor: pointer;
}

.avatar {
  width: 36px;
  height: 36px;
  border-radius: 50%;
  border: 2px solid #fff;
}

.dropdown {
  position: absolute;
  right: 0;
  top: 48px;
  background: white;
  color: #333;
  border-radius: 8px;
  box-shadow: 0 4px 10px rgba(0, 0, 0, 0.15);
  display: flex;
  flex-direction: column;
  overflow: hidden;
  min-width: 140px;
  z-index: 10;
}

.dropdown button {
  background: none;
  border: none;
  padding: 10px 12px;
  text-align: left;
  width: 100%;
  cursor: pointer;
  transition: background 0.2s;
}

.dropdown button:hover {
  background: #f2f8f8;
  color: #417c85;
}
</style>
