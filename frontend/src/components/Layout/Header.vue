<template>
  <header class="header">
    <div class="header-left">
      <h1 class="app-title">Quản lý Thiết bị</h1>
    </div>

    <div class="header-right">
      <div class="profile" @click="toggleDropdown">
        <img src="https://i.pravatar.cc/40" alt="avatar" class="avatar" />
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

.app-title {
  font-size: 18px;
  font-weight: 600;
  letter-spacing: 0.5px;
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

<!-- <template>
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
  justify-content: flex-end; /* ✅ đẩy nút sang phải */
  background: #417c85;
  height: 64px;
  padding: 0 24px; /* khoảng cách với mép phải */
  box-sizing: border-box;
}

.logout-btn {
  background: blue;
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
  background: blue;
}
</style> -->
