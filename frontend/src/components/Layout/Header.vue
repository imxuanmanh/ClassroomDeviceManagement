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
          <button @click="goProfile">
            <span class="material-icons">person</span>
            Hồ sơ
          </button>
          <button @click="handleLogout">
            <span class="material-icons">logout</span>
            Đăng xuất
          </button>
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
  height: 70px;
  background: linear-gradient(135deg, #081b29 0%, #0a2332 100%);
  color: white;
  padding: 0 32px;
  box-shadow: 0 4px 20px rgba(0, 239, 255, 0.15);
  border-bottom: 2px solid rgba(0, 239, 255, 0.3);
  position: relative;
  z-index: 5;
}

/* Decorative glow */
.header::after {
  content: '';
  position: absolute;
  bottom: -2px;
  left: 0;
  width: 100%;
  height: 2px;
  background: linear-gradient(90deg, transparent 0%, #0ef 20%, #0ef 80%, transparent 100%);
  box-shadow: 0 0 10px rgba(0, 239, 255, 0.5);
}

.header-left {
  display: flex;
  align-items: center;
  gap: 16px;
}

.menu-toggle {
  background: rgba(0, 239, 255, 0.1);
  border: 1px solid rgba(0, 239, 255, 0.3);
  color: #0ef;
  font-size: 24px;
  cursor: pointer;
  width: 42px;
  height: 42px;
  border-radius: 12px;
  display: flex;
  align-items: center;
  justify-content: center;
  transition: all 0.3s ease;
}

.menu-toggle:hover {
  background: rgba(0, 239, 255, 0.2);
  border-color: #0ef;
  box-shadow: 0 0 15px rgba(0, 239, 255, 0.4);
  transform: scale(1.05);
}

.app-title {
  font-size: 20px;
  font-weight: 700;
  letter-spacing: 0.5px;
  margin: 0;
  color: #0ef;
  text-shadow: 0 0 10px rgba(0, 239, 255, 0.5);
}

.header-right {
  display: flex;
  align-items: center;
  gap: 16px;
}

.profile {
  display: flex;
  align-items: center;
  gap: 10px;
  position: relative;
  cursor: pointer;
  padding: 8px 16px;
  border-radius: 12px;
  transition: all 0.3s ease;
  background: rgba(0, 239, 255, 0.05);
  border: 1px solid rgba(0, 239, 255, 0.2);
}

.profile:hover {
  background: rgba(0, 239, 255, 0.1);
  border-color: rgba(0, 239, 255, 0.4);
  box-shadow: 0 0 15px rgba(0, 239, 255, 0.2);
}

.avatar {
  width: 38px;
  height: 38px;
  border-radius: 50%;
  border: 2px solid #0ef;
  box-shadow: 0 0 10px rgba(0, 239, 255, 0.5);
  transition: all 0.3s ease;
}

.profile:hover .avatar {
  box-shadow: 0 0 15px rgba(0, 239, 255, 0.8);
  transform: scale(1.05);
}

.profile span:not(.material-icons) {
  font-weight: 500;
  color: #fff;
}

.profile .material-icons {
  color: #0ef;
  transition: transform 0.3s ease;
}

.profile:hover .material-icons {
  transform: rotate(180deg);
}

/* Dropdown menu */
.dropdown {
  position: absolute;
  right: 0;
  top: 60px;
  background: linear-gradient(135deg, #081b29 0%, #0a2332 100%);
  color: #fff;
  border-radius: 12px;
  box-shadow:
    0 8px 25px rgba(0, 0, 0, 0.5),
    0 0 20px rgba(0, 239, 255, 0.2);
  display: flex;
  flex-direction: column;
  overflow: hidden;
  min-width: 180px;
  z-index: 10;
  border: 2px solid rgba(0, 239, 255, 0.3);
  animation: dropdownSlide 0.3s ease;
}

@keyframes dropdownSlide {
  from {
    opacity: 0;
    transform: translateY(-10px);
  }
  to {
    opacity: 1;
    transform: translateY(0);
  }
}

.dropdown button {
  background: none;
  border: none;
  padding: 14px 16px;
  text-align: left;
  width: 100%;
  cursor: pointer;
  transition: all 0.2s ease;
  color: #fff;
  display: flex;
  align-items: center;
  gap: 10px;
  font-size: 14px;
  font-weight: 500;
}

.dropdown button .material-icons {
  font-size: 20px;
  color: #0ef;
}

.dropdown button:hover {
  background: rgba(0, 239, 255, 0.15);
  color: #0ef;
}

.dropdown button:hover .material-icons {
  transform: scale(1.1);
}

.dropdown button:not(:last-child) {
  border-bottom: 1px solid rgba(0, 239, 255, 0.1);
}
</style>

<!-- <template>
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
</style> -->
