<template>
  <header class="header">
    <div class="header-left">
      <button class="menu-toggle" @click="$emit('toggle-sidebar')">
        <span class="material-icons">menu</span>
      </button>

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
// Script không thay đổi
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
/* === STYLE ĐÃ ĐƯỢC CẬP NHẬT CHO DARK MODE === */
.header {
  display: flex;
  justify-content: space-between;
  align-items: center;
  height: 64px;
  background: #081b29; /* Nền tối chính */
  color: white;
  padding: 0 24px;
  box-shadow: 0 2px 4px rgba(14, 239, 239, 0.2); /* Đổ bóng cyan */
  border-bottom: 1px solid #0ef; /* Viền cyan */
  position: relative; /* Thêm để z-index hoạt động */
  z-index: 5; /* Đảm bảo header nằm trên sidebar 1 chút */
}

.header-left {
  display: flex;
  align-items: center;
  gap: 12px;
}

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

/* Style hover này vẫn đẹp, giữ nguyên */
.menu-toggle:hover {
  background: rgba(255, 255, 255, 0.15);
}

.app-title {
  font-size: 18px;
  font-weight: 600;
  letter-spacing: 0.5px;
  margin: 0;
  color: #0ef; /* Đổi tiêu đề sang màu cyan cho nổi bật */
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
  border: 2px solid #0ef; /* Viền avatar cyan */
}

/* Dropdown menu */
.dropdown {
  position: absolute;
  right: 0;
  top: 48px;
  background: #081b29; /* Nền tối */
  color: #fff; /* Chữ trắng */
  border-radius: 8px;
  box-shadow: 0 4px 10px rgba(14, 239, 239, 0.2); /* Đổ bóng cyan */
  display: flex;
  flex-direction: column;
  overflow: hidden;
  min-width: 140px;
  z-index: 10;
  border: 1px solid #0ef; /* Viền cyan */
}

.dropdown button {
  background: none;
  border: none;
  padding: 10px 12px;
  text-align: left;
  width: 100%;
  cursor: pointer;
  transition: background 0.2s;
  color: #fff; /* Chữ trắng */
}

.dropdown button:hover {
  background: rgba(14, 239, 239, 0.1); /* Nền cyan mờ */
  color: #0ef; /* Chữ cyan */
}
</style>
