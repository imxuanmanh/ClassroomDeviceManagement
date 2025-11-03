<template>
  <aside class="sidebar">
    <div class="user-info">
      <span class="material-icons user-icon">account_circle</span>
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
    ]
  }
})
</script>

<style scoped>
.sidebar {
  background: #ffffff;
  color: #333;
  height: 100vh;
  width: 250px;
  display: flex;
  flex-direction: column;
  border-right: 1px solid #e5e7eb;
  font-family: 'Inter', sans-serif;
  box-shadow: 2px 0 6px rgba(0, 0, 0, 0.05);
}

.user-info {
  text-align: center;
  padding: 24px 0 16px;
  border-bottom: 1px solid #eee;
  background: #f9fbfb;
}

.user-icon {
  font-size: 56px;
  color: #417c85;
}

.username {
  display: block;
  font-weight: 600;
  margin-top: 4px;
}

.role {
  font-size: 12px;
  color: #6b7280;
}

.menu {
  flex: 1;
  padding: 16px;
}

.group-title {
  font-size: 12px;
  font-weight: 600;
  color: #6b7280;
  text-transform: uppercase;
  margin: 16px 0 4px;
}

.menu-item {
  display: flex;
  align-items: center;
  gap: 10px;
  color: #333;
  text-decoration: none;
  padding: 10px 12px;
  border-radius: 10px;
  transition: all 0.2s ease;
}

.menu-item:hover {
  background: #eaf4f5;
  color: #2a5d65;
  transform: translateX(2px);
}

.router-link-active {
  background: #d0e7e9;
  color: #2a5d65;
  font-weight: 600;
  border-left: 4px solid #2a5d65;
  padding-left: 8px;
}
</style>
