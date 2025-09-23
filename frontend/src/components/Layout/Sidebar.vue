<!--
  SIDEBAR ĐIỀU HƯỚNG
  - Hiển thị menu điều hướng chính của ứng dụng
  - Hỗ trợ thu gọn/mở rộng với tooltip
  - Nhóm các menu theo chức năng
-->
<template>
  <aside class="sidebar">
    <!-- Logo -->
    <div class="logo">
      <router-link to="/">
        <span class="material-icons">computer</span>
        <span class="label">Quản lý Thiết Bị</span>
      </router-link>
    </div>

    <!-- Menu -->
    <nav class="nav">
      <template v-for="group in menu" :key="group.title">
        <div class="group">
          <div class="group-title">{{ group.title }}</div>
          <router-link
            v-for="item in group.items"
            :key="item.to"
            :to="item.to"
            active-class="active"
          >
            <span class="material-icons">{{ item.icon }}</span>
            <span class="label">{{ item.label }}</span>
          </router-link>
        </div>
      </template>
    </nav>
  </aside>
</template>

<script setup>
import { computed } from 'vue'

const menu = computed(() => [
  {
    title: 'Tổng quan',
    items: [{ to: '/', icon: 'analytics', label: 'Dashboard' }],
  },
  {
    title: 'Quản lý',
    items: [
      { to: '/devices', icon: 'devices', label: 'Device' },
      { to: '/users', icon: 'person', label: 'Người mượn' },
    ],
  },
  {
    title: 'Theo dõi',
    items: [
      { to: '/history', icon: 'history', label: 'Lịch sử' },
      { to: '/reports', icon: 'report', label: 'Báo cáo' },
    ],
  },
])
</script>

<style scoped>
.sidebar {
  height: 100%;
  background: #0d1b2a;
  color: #e0e0e0;
  padding: 16px 12px;
  box-sizing: border-box;
}
.material-icons {
  color: inherit;
}
.nav {
  margin-top: 16px;
}
.group {
  margin-bottom: 12px;
}
.group-title {
  font-size: 12px;
  color: #4a4a4a;
  padding: 0 12px;
  text-transform: uppercase;
  letter-spacing: 0.05em;
  margin-bottom: 6px;
  font-weight: 600;
}
.nav a {
  display: flex;
  align-items: center;
  gap: 12px;
  color: #334155; /*Chỉnh màu tất cả chữ menu*/
  text-decoration: none;
  padding: 10px 12px;
  border-radius: 8px;
  font-size: 15px;
  transition:
    background 0.2s,
    color 0.2s;
}

.nav a:hover {
  background: #dbeafe;
  color: #2563eb;
}

.nav a.active {
  background: #2563eb;
  color: #fff;
  font-weight: 600;
}

.nav a.router-link-active {
  font-weight: 600;
}
.logo a {
  display: flex;
  align-items: center;
  gap: 12px;
  color: #fff;
  text-decoration: none;
  font-weight: 700;
  padding: 8px 12px;
  border-radius: 8px;
  font-size: 18px;
}
.logo {
  margin-bottom: 16px;
}
.label {
  white-space: nowrap;
}
</style>
