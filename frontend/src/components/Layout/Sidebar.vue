<template>
  <aside class="sidebar">
    <div class="user-info">
      <div class="avatar-wrapper">
        <i class="bx bxs-user-circle user-icon"></i>
        <div class="avatar-glow"></div>
      </div>
      <span class="username">{{ loginLabel }}</span>
      <span class="role" v-if="isAdmin">Admin</span>
    </div>

    <nav class="menu">
      <ul>
        <template v-for="group in menu" :key="group.title">
          <p class="group-title" v-if="group.title">{{ group.title }}</p>
          <li
            v-for="item in group.items"
            :key="item.to"
            class="item"
            :class="{ active: isActive(item.to) }"
            @click="handleItemClick(item.to)"
          >
            <div class="item-content">
              <span class="material-icons">{{ item.icon }}</span>
              <span class="item-label">{{ item.label }}</span>
            </div>
            <div class="item-shine"></div>
          </li>
        </template>
      </ul>
    </nav>
  </aside>
</template>

<script setup>
import { computed, onMounted, watch } from 'vue'
import { useAuthStore } from '@/stores/auth'
import { useRouter, useRoute } from 'vue-router'

const auth = useAuthStore()
const router = useRouter()
const route = useRoute()

const isAdmin = computed(() => auth.roleId === 1)

const loginLabel = computed(() =>
  auth.fullname && auth.fullname.trim() !== '' ? auth.fullname : 'Người dùng',
)

const menu = computed(() => {
  if (isAdmin.value) {
    return [
      {
        title: 'TỔNG QUAN',
        items: [{ to: '/dashboard', icon: 'analytics', label: 'Bảng điều khiển' }],
      },
      {
        title: 'QUẢN LÝ',
        items: [
          { to: '/devices', icon: 'devices', label: 'Thiết bị' },
          { to: '/users', icon: 'person', label: 'Người dùng' },
        ],
      },
      {
        title: 'THEO DÕI',
        items: [
          { to: '/history', icon: 'history', label: 'Lịch sử' },
          { to: '/reports', icon: 'bar_chart', label: 'Thống kê' },
          { to: '/requests', icon: 'assignment', label: 'Yêu cầu' },
        ],
      },
    ]
  } else {
    return [
      {
        title: 'THIẾT BỊ CỦA TÔI',
        items: [{ to: '/devices', icon: 'devices', label: 'Thiết bị' }],
      },
      {
        title: '',
        items: [{ to: '/history-user', icon: 'history', label: 'Lịch sử' }],
      },
    ]
  }
})

function isActive(path) {
  return route.path === path
}

function handleItemClick(path) {
  router.push(path)
}

watch(
  () => route.path,
  () => {},
)

onMounted(() => {})
</script>

<style scoped>
.sidebar {
  background: linear-gradient(180deg, #0a1f2e 0%, #081b29 100%);
  color: #fff;
  height: 100vh;
  width: 280px;
  display: flex;
  flex-direction: column;
  font-family: 'Poppins', sans-serif;
  box-shadow: 4px 0 30px rgba(0, 239, 255, 0.2);
  position: relative;
  overflow: hidden;
  border-right: 1px solid rgba(0, 239, 255, 0.3);
}

/* Animated background effect */
.sidebar::before {
  content: '';
  position: absolute;
  top: 0;
  left: 0;
  width: 100%;
  height: 100%;
  background:
    radial-gradient(circle at 10% 20%, rgba(0, 239, 255, 0.05) 0%, transparent 50%),
    radial-gradient(circle at 90% 80%, rgba(0, 239, 255, 0.05) 0%, transparent 50%);
  pointer-events: none;
  animation: bg-pulse 8s ease-in-out infinite;
}

@keyframes bg-pulse {
  0%,
  100% {
    opacity: 0.3;
  }
  50% {
    opacity: 0.6;
  }
}

/* User Info Section */
.user-info {
  text-align: center;
  padding: 36px 0 28px;
  border-bottom: 1px solid rgba(0, 239, 255, 0.15);
  background: rgba(0, 239, 255, 0.03);
  position: relative;
  z-index: 1;
}

.avatar-wrapper {
  position: relative;
  display: inline-block;
  margin-bottom: 12px;
}

.user-icon {
  font-size: 72px;
  color: #0ef;
  filter: drop-shadow(0 0 20px rgba(0, 239, 255, 0.6));
  transition: all 0.4s ease;
  position: relative;
  z-index: 2;
}

.avatar-glow {
  position: absolute;
  top: 50%;
  left: 50%;
  transform: translate(-50%, -50%);
  width: 90px;
  height: 90px;
  background: radial-gradient(circle, rgba(0, 239, 255, 0.3), transparent 70%);
  border-radius: 50%;
  animation: glow-pulse 3s ease-in-out infinite;
  z-index: 1;
}

@keyframes glow-pulse {
  0%,
  100% {
    transform: translate(-50%, -50%) scale(0.9);
    opacity: 0.4;
  }
  50% {
    transform: translate(-50%, -50%) scale(1.1);
    opacity: 0.8;
  }
}

.user-info:hover .user-icon {
  transform: scale(1.08);
  filter: drop-shadow(0 0 25px rgba(0, 239, 255, 0.9));
}

.username {
  display: block;
  font-weight: 600;
  margin-top: 8px;
  color: #fff;
  font-size: 17px;
  letter-spacing: 0.3px;
}

.role {
  font-size: 11px;
  color: #0ef;
  font-weight: 600;
  margin-top: 8px;
  display: inline-block;
  padding: 4px 14px;
  background: rgba(0, 239, 255, 0.12);
  border-radius: 14px;
  border: 1px solid rgba(0, 239, 255, 0.3);
  text-transform: uppercase;
  letter-spacing: 0.5px;
}

/* Menu Section */
.menu {
  flex: 1;
  padding: 24px 16px;
  position: relative;
  z-index: 1;
  overflow-y: auto;
}

.menu::-webkit-scrollbar {
  width: 6px;
}

.menu::-webkit-scrollbar-track {
  background: rgba(8, 27, 41, 0.3);
}

.menu::-webkit-scrollbar-thumb {
  background: rgba(0, 239, 255, 0.3);
  border-radius: 10px;
}

.menu::-webkit-scrollbar-thumb:hover {
  background: rgba(0, 239, 255, 0.5);
}

.menu ul {
  list-style: none;
  padding: 0;
  margin: 0;
}

.group-title {
  font-size: 10px;
  font-weight: 700;
  color: rgba(0, 239, 255, 0.5);
  text-transform: uppercase;
  margin: 24px 0 12px;
  padding-left: 16px;
  letter-spacing: 1.8px;
}

/* Menu Items */
.item {
  margin-bottom: 4px;
  position: relative;
  cursor: pointer;
  border-radius: 14px;
  overflow: hidden;
  transition: all 0.3s cubic-bezier(0.4, 0, 0.2, 1);
}

.item-content {
  display: flex;
  align-items: center;
  padding: 14px 16px;
  color: rgba(255, 255, 255, 0.65);
  position: relative;
  z-index: 2;
  transition: all 0.3s ease;
}

.item .material-icons {
  font-size: 22px;
  margin-right: 14px;
  transition: all 0.3s ease;
  width: 24px;
  text-align: center;
}

.item-label {
  font-size: 14.5px;
  font-weight: 500;
  letter-spacing: 0.3px;
}

/* Shine effect */
.item-shine {
  position: absolute;
  top: 0;
  left: -100%;
  width: 100%;
  height: 100%;
  background: linear-gradient(90deg, transparent 0%, rgba(0, 239, 255, 0.1) 50%, transparent 100%);
  transition: left 0.5s ease;
  z-index: 1;
}

/* Hover state */
.item:hover {
  background: rgba(0, 239, 255, 0.08);
  transform: translateX(4px);
}

.item:hover .item-content {
  color: #0ef;
}

.item:hover .material-icons {
  transform: scale(1.15) rotate(5deg);
  color: #0ef;
}

.item:hover .item-shine {
  left: 100%;
}

/* Active state */
.item.active {
  background: linear-gradient(135deg, rgba(0, 239, 255, 0.15) 0%, rgba(0, 239, 255, 0.08) 100%);
  border-left: 3px solid #0ef;
  box-shadow:
    inset 0 0 20px rgba(0, 239, 255, 0.1),
    0 0 15px rgba(0, 239, 255, 0.2);
}

.item.active .item-content {
  color: #fff;
  font-weight: 600;
  padding-left: 13px;
}

.item.active .material-icons {
  color: #0ef;
  transform: scale(1.1);
  filter: drop-shadow(0 0 8px rgba(0, 239, 255, 0.6));
}

/* Active item animation */
@keyframes active-glow {
  0%,
  100% {
    box-shadow:
      inset 0 0 20px rgba(0, 239, 255, 0.1),
      0 0 15px rgba(0, 239, 255, 0.2);
  }
  50% {
    box-shadow:
      inset 0 0 25px rgba(0, 239, 255, 0.15),
      0 0 20px rgba(0, 239, 255, 0.3);
  }
}

.item.active {
  animation: active-glow 2s ease-in-out infinite;
}
</style>

<!-- <template>
  <aside class="sidebar">
    <div class="user-info">
      <i class="bx bxs-user-circle user-icon"></i>
      <span class="username">{{ loginLabel }}</span>
      <span class="role" v-if="isAdmin">Admin</span>
    </div>

    <nav class="menu">
      <template v-for="group in menu" :key="group.title">
        <p class="group-title">{{ group.title }}</p>
        <router-link v-for="item in group.items" :key="item.to" :to="item.to" class="menu-item">
          <span class="material-icons">{{ item.icon }}</span>
          <span>{{ item.label }}</span>
        </router-link>
      </template>
    </nav>
  </aside>
</template>

<script setup>
// Script không thay đổi
import { computed } from 'vue'
import { useAuthStore } from '@/stores/auth'

const auth = useAuthStore()
const isAdmin = computed(() => auth.roleId === 1)

const loginLabel = computed(() =>
  auth.fullname && auth.fullname.trim() !== '' ? auth.fullname : 'Người dùng',
)

const menu = computed(() => {
  if (isAdmin.value) {
    return [
      {
        title: 'Tổng quan',
        items: [{ to: '/dashboard', icon: 'analytics', label: 'Bảng điều khiển' }],
      },
      {
        title: 'Quản lý',
        items: [
          { to: '/devices', icon: 'devices', label: 'Thiết bị' },
          { to: '/users', icon: 'person', label: 'Người dùng' },
        ],
      },
      {
        title: 'Theo dõi',
        items: [
          { to: '/history', icon: 'history', label: 'Lịch sử' },
          { to: '/reports', icon: 'bar_chart', label: 'Thống kê' },
          { to: '/requests', icon: 'assignment', label: 'Yêu cầu' },
        ],
      },
    ]
  } else {
    return [
      {
        title: 'Thiết bị của tôi',
        items: [{ to: '/devices', icon: 'devices', label: 'Thiết bị' }],
      },
      {
        items: [{ to: '/history-user', icon: 'history', label: 'Lịch sử' }],
      },
    ]
  }
})
</script>

<style scoped>
/* === TOÀN BỘ STYLE ĐÃ ĐƯỢC CẬP NHẬT CHO DARK MODE === */
.sidebar {
  background: white; /* Nền tối chính */
  color: #fff; /* Chữ trắng */
  height: 100vh;
  width: 250px;
  display: flex;
  flex-direction: column;
  border-right: 1px solid #0ef; /* Viền cyan */
  font-family: 'Poppins', sans-serif; /* Đổi font cho nhất quán */
  box-shadow: 2px 0 6px rgba(14, 239, 239, 0.2); /* Đổ bóng cyan */
}

.user-info {
  text-align: center;
  padding: 24px 0 16px;
  border-bottom: 1px solid #0ef; /* Viền cyan */
  background: transparent; /* Bỏ nền cũ */
}

.user-icon {
  font-size: 56px;
  color: #0ef; /* Màu cyan */
}

.username {
  display: block;
  font-weight: 600;
  margin-top: 4px;
  color: black; /* Đảm bảo chữ trắng */
}

.role {
  font-size: 12px;
  color: #aaa; /* Màu xám nhạt */
}

.menu {
  flex: 1;
  padding: 16px;
}

.group-title {
  font-size: 12px;
  font-weight: 600;
  color: #aaa; /* Màu xám nhạt */
  text-transform: uppercase;
  margin: 16px 0 4px;
}

.menu-item {
  display: flex;
  align-items: center;
  gap: 10px;
  color: black; /* Chữ trắng */
  text-decoration: none;
  padding: 10px 12px;
  border-radius: 10px;
  transition: all 0.2s ease;
  border-left: 4px solid transparent; /* Thêm để giữ layout khi active */
}

.menu-item:hover {
  background: rgba(14, 239, 239, 0.1); /* Nền cyan mờ */
  color: #0ef; /* Chữ cyan */
  transform: translateX(2px);
}

/* Kiểu cho mục đang active */
.router-link-active {
  background: rgba(14, 239, 239, 0.15); /* Nền cyan mờ (đậm hơn hover) */
  color: #0ef; /* Chữ cyan */
  font-weight: 600;
  border-left: 4px solid #0ef; /* Viền trái cyan */
  padding-left: 8px; /* Giữ nguyên từ code cũ */
}
</style> -->
