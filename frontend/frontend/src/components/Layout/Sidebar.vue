<!-- 
  SIDEBAR ĐIỀU HƯỚNG
  - Hiển thị menu điều hướng chính của ứng dụng
  - Hỗ trợ thu gọn/mở rộng với tooltip
  - Nhóm các menu theo chức năng
-->
<template>
  <aside class="sidebar" :class="{ 'is-collapsed': collapsed }">
    <!-- Logo và link về trang chủ -->
    <div class="logo" :title="collapsed ? 'Trang chủ' : ''">
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
          <!-- Tiêu đề nhóm (ẩn khi thu gọn) -->
          <div class="group-title" v-if="!collapsed">{{ group.title }}</div>
          
          <!-- Các item menu trong nhóm -->
          <router-link 
            v-for="item in group.items" 
            :key="item.to" 
            :to="item.to" 
            :title="collapsed ? item.label : ''"
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

// Props nhận từ component cha
defineProps({
  collapsed: {
    type: Boolean,
    default: false
  }
})

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
      { to: '/equipment', icon: 'devices', label: 'Thiết bị' },
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
  box-shadow: 0 2px 8px rgba(0,0,0,0.06);
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
  transition: all 0.2s ease;
}
.nav a.router-link-active {
  background: #f0f7ff;
  color: #1d4ed8;
  font-weight: 600;
}
.nav a:hover {
  background: #f8fafc;
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
  transition: all 0.2s ease;
}
.logo a:hover {
  background: #f8fafc;
}
.logo {
  padding: 0;
  margin-bottom: 8px;
}
.label {
  white-space: nowrap;
}

/* Collapsed styles */
.is-collapsed .label { display: none; }
.is-collapsed .nav a { justify-content: center; }
.is-collapsed .material-icons { margin-right: 0; }
.is-collapsed .logo a { justify-content: center; }

</style>
