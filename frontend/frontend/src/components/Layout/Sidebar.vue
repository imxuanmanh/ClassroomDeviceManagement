<!--
  SIDEBAR ĐIỀU HƯỚNG
  - Hiển thị menu điều hướng chính của ứng dụng
  - Hỗ trợ thu gọn/mở rộng với tooltip
  - Nhóm các menu theo chức năng
-->
<template>
  <aside class="sidebar">
    <!-- Logo và link về trang chủ -->
    <div class="logo">
      <router-link to="/">
        <span class="material-icons">computer</span>
        <span class="label">Home</span>
      </router-link>
    </div>

    <!-- Menu điều hướng chính -->
    <nav class="nav">
      <!-- Lặp qua từng nhóm menu -->
      <template v-for="group in menu" :key="group.title">
        <div class="group">
          <div class="group-title">{{ group.title }}</div>
          <router-link
            v-for="item in group.items"
            :key="item.to"
            :to="item.to"
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

/**
 * Cấu hình menu điều hướng
 * Được tổ chức theo nhóm chức năng để dễ quản lý
 */
const menu = computed(() => [
  {
    title: 'Tổng quan',
    items: [
      { to: '/', icon: 'analytics', label: 'Dashboard' }
    ]
  },
  {
    title: 'Quản lý',
    items: [
      { to: '/devices', icon: 'devices', label: 'Thiết bị' },
      { to: '/users', icon: 'person', label: 'Người mượn' }
    ]
  },
  {
    title: 'Theo dõi',
    items: [
      { to: '/history', icon: 'history', label: 'Lịch sử' },
      { to: '/reports', icon: 'report', label: 'Báo cáo' }
    ]
  }
])
</script>

<style scoped>
.sidebar {
  height: 100%;
  background: #ffffff;
  color: #000000;
  padding: 16px 12px;
  box-sizing: border-box;
  border-right: 1px solid #eee;
}
.material-icons {
  color: black;
  margin-bottom: 0;
  vertical-align: middle;
  margin-right: 12px;
}
.nav {
  margin-top: 16px;
}
.group { margin-bottom: 12px; }
.group-title {
  font-size: 12px;
  color: #6b7280;
  padding: 0 12px;
  text-transform: uppercase;
  letter-spacing: .04em;
  margin-bottom: 6px;
  font-weight: 600;
}
.nav a {
  display: flex;
  align-items: center;
  gap: 12px;
  color: #000000;
  text-decoration: none;
  padding: 10px 12px;
  border-radius: 8px;
  font-size: 16px;
}
.nav a.router-link-active {
  font-weight: 600;
}
.logo a {
  display: flex;
  align-items: center;
  gap: 12px;
  color: #000000;
  text-decoration: none;
  font-weight: 600;
  padding: 8px 12px;
  border-radius: 8px;
}
.logo {
  padding: 0;
  margin-bottom: 8px;
}
.label {
  white-space: nowrap;
}
</style>
