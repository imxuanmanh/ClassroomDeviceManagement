<template>
  <header class="header">
    <div class="header-left">
      <button class="menu-toggle" @click="$emit('toggle-sidebar')">
        <span class="material-icons">menu</span>
      </button>
      <h1 class="app-title">Quản lý Thiết bị</h1>
    </div>

    <div class="header-right">
      <div class="notification-wrapper">
        <button class="notification-btn" aria-label="Notifications" @click="toggleNotification">
          <img src="@/components/icons/notification-bell.png" alt="Notifications" />
        </button>

        <!-- Notification Dropdown -->
        <div v-if="notificationOpen" class="notification-dropdown">
          <div class="notification-header">
            <h3>Thông báo</h3>
            <button @click="markAllAsRead" class="mark-read-btn" title="Đánh dấu đã đọc tất cả">
              <span class="material-icons">done</span>
            </button>
          </div>
          <div class="notification-content">
            <div v-if="notifications.length === 0" class="no-notifications">
              Bạn không có thông báo
            </div>
            <div v-else class="notification-list">
              <div v-for="(notif, index) in notifications" :key="index" class="notification-item">
                <span class="material-icons notif-icon">info</span>
                <div class="notif-text">
                  <p>{{ notif.message }}</p>
                  <small>{{ notif.time }}</small>
                </div>
              </div>
            </div>
          </div>
        </div>
      </div>

      <div class="profile" @click="toggleDropdown">
        <img src="/src/components/icons/logo.png" alt="avatar" class="avatar" />
        <span>{{ auth.fullname || 'Admin' }}</span>
        <span class="material-icons">expand_more</span>

        <div v-if="dropdownOpen" class="dropdown">
          <button @click="goProfile">
            <span class="material-icons">person</span>
            Hồ sơ
          </button>
          <button @click="handleLogout">
            <img src="@/components/icons/logout-static.png" alt="Logout" class="dropdown-icon" />
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
const notificationOpen = ref(false)

// Dữ liệu thông báo mẫu - bạn có thể thay thế bằng dữ liệu thực từ API
const notifications = ref([
  // Để trống hoặc thêm thông báo mẫu
])

function toggleDropdown() {
  dropdownOpen.value = !dropdownOpen.value
  notificationOpen.value = false
}

function toggleNotification() {
  notificationOpen.value = !notificationOpen.value
  dropdownOpen.value = false
}

function markAllAsRead() {
  // Xử lý đánh dấu đã đọc tất cả thông báo
  // Không đóng dropdown, giữ nguyên bảng thông báo
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

/* Style cho nút notification */
.notification-btn {
  background: rgba(0, 239, 255, 0.1);
  border: 1px solid rgba(0, 239, 255, 0.3);
  cursor: pointer;
  width: 42px;
  height: 42px;
  border-radius: 12px;
  display: flex;
  align-items: center;
  justify-content: center;
  transition: all 0.3s ease;
  padding: 0;
}

.notification-btn:hover {
  background: rgba(0, 239, 255, 0.2);
  border-color: #0ef;
  box-shadow: 0 0 15px rgba(0, 239, 255, 0.4);
}

.notification-btn img {
  width: 24px;
  height: 24px;
  object-fit: contain;
}

/* Notification Wrapper */
.notification-wrapper {
  position: relative;
}

/* Notification Dropdown */
.notification-dropdown {
  position: absolute;
  right: 0;
  top: 60px;
  background: linear-gradient(135deg, #081b29 0%, #0a2332 100%);
  border-radius: 12px;
  box-shadow:
    0 8px 25px rgba(0, 0, 0, 0.5),
    0 0 20px rgba(0, 239, 255, 0.2);
  min-width: 320px;
  max-width: 400px;
  z-index: 10;
  border: 2px solid rgba(0, 239, 255, 0.3);
  animation: dropdownSlide 0.3s ease;
  overflow: hidden;
}

.notification-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
  padding: 16px 20px;
  border-bottom: 1px solid rgba(0, 239, 255, 0.2);
  background: rgba(0, 239, 255, 0.05);
}

.notification-header h3 {
  margin: 0;
  font-size: 16px;
  font-weight: 600;
  color: #0ef;
}

.mark-read-btn {
  background: rgba(0, 239, 255, 0.1);
  border: 1px solid rgba(0, 239, 255, 0.3);
  border-radius: 8px;
  padding: 6px 8px;
  cursor: pointer;
  transition: all 0.3s ease;
  display: flex;
  align-items: center;
  justify-content: center;
}

.mark-read-btn .material-icons {
  color: #0ef;
  font-size: 20px;
}

.mark-read-btn:hover {
  background: rgba(0, 239, 255, 0.2);
  border-color: #0ef;
  box-shadow: 0 0 10px rgba(0, 239, 255, 0.3);
}

.notification-content {
  max-height: 400px;
  overflow-y: auto;
}

.no-notifications {
  padding: 40px 20px;
  text-align: center;
  color: rgba(255, 255, 255, 0.6);
  font-size: 14px;
}

.notification-list {
  padding: 8px 0;
}

.notification-item {
  display: flex;
  align-items: flex-start;
  gap: 12px;
  padding: 14px 20px;
  border-bottom: 1px solid rgba(0, 239, 255, 0.1);
  transition: all 0.2s ease;
}

.notification-item:hover {
  background: rgba(0, 239, 255, 0.08);
}

.notification-item:last-child {
  border-bottom: none;
}

.notif-icon {
  color: #0ef;
  font-size: 22px;
  margin-top: 2px;
}

.notif-text {
  flex: 1;
}

.notif-text p {
  margin: 0 0 6px 0;
  color: #fff;
  font-size: 14px;
  line-height: 1.4;
}

.notif-text small {
  color: rgba(255, 255, 255, 0.5);
  font-size: 12px;
}

/* Custom scrollbar for notification content */
.notification-content::-webkit-scrollbar {
  width: 6px;
}

.notification-content::-webkit-scrollbar-track {
  background: rgba(0, 239, 255, 0.05);
}

.notification-content::-webkit-scrollbar-thumb {
  background: rgba(0, 239, 255, 0.3);
  border-radius: 3px;
}

.notification-content::-webkit-scrollbar-thumb:hover {
  background: rgba(0, 239, 255, 0.5);
}

/* Animation lắc chuông */
@keyframes ring-animation {
  0%,
  100% {
    transform: rotate(0);
  }
  10%,
  50% {
    transform: rotate(-15deg);
  }
  30%,
  70% {
    transform: rotate(15deg);
  }
  90% {
    transform: rotate(0);
  }
}

.notification-btn:hover img {
  animation: ring-animation 0.7s ease-in-out;
  animation-iteration-count: 1;
  transform-origin: top center;
}

.dropdown-icon {
  width: 20px;
  height: 20px;
  object-fit: contain;
  transition: transform 0.2s ease;
}

@keyframes logout-shake {
  0%,
  100% {
    transform: rotate(0) scale(1.1);
  }
  30% {
    transform: rotate(-8deg) scale(1.1);
  }
  60% {
    transform: rotate(8deg) scale(1.1);
  }
}

.dropdown button:hover .dropdown-icon {
  animation: logout-shake 0.5s ease-in-out;
  animation-iteration-count: 1;
}
</style>
