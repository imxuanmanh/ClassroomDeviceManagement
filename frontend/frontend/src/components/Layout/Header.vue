<!-- 
  HEADER ỨNG DỤNG
  - Chứa nút menu toggle, tiêu đề, tìm kiếm, thông báo
  - Hiển thị thông tin người dùng và nút đăng nhập/đăng xuất
  - Hỗ trợ slots để tùy chỉnh nội dung
-->
<template>
  <header class="header" role="banner">
    <!-- Nút menu để bật/tắt sidebar -->
    <button class="menu-btn" @click="$emit('toggle-sidebar')" :aria-expanded="!collapsed" aria-label="Toggle menu">
      <span class="material-icons">menu</span>
    </button>
    
    <!-- Link đến trang profile quản trị viên -->
    <router-link to="/profile" class="user link left" aria-label="Thông tin quản trị">
      <img class="avatar" src="https://i.pravatar.cc/40?img=3" alt="Avatar" />
      <span class="name">Admin</span>
    </router-link>
    
    <!-- Tiêu đề chính của ứng dụng -->
    <h1 class="title">Quản lý Thiết Bị Phòng Học</h1>
    
    <!-- Ô tìm kiếm với slot để tùy chỉnh -->
    <div class="search">
      <slot name="search">
        <span class="material-icons">search</span>
        <input type="text" placeholder="Tìm kiếm..." />
      </slot>
    </div>
    
    <!-- Các nút hành động bên phải -->
    <div class="right">
      <slot name="actions">
        <!-- Nút thông báo -->
        <button class="icon-btn" aria-label="Thông báo">
          <span class="material-icons">notifications</span>
        </button>
        
        <!-- Nút đăng xuất (chỉ hiện khi đã đăng nhập) -->
        <button class="icon-btn" @click="onLogout" aria-label="Đăng xuất" v-if="isAuthenticated">
          <span class="material-icons">logout</span>
        </button>
        
        <!-- Link đăng nhập (chỉ hiện khi chưa đăng nhập) -->
        <router-link v-else to="/login" class="login-btn" aria-label="Đăng nhập">Đăng nhập</router-link>
      </slot>
    </div>
  </header>
  
</template>

<script setup>
import { computed } from 'vue'
import { useAuthStore } from '@/stores/auth.js'

// Props nhận từ component cha
defineProps({
  collapsed: {
    type: Boolean,
    default: false
  }
})

// Sử dụng store xác thực để quản lý trạng thái đăng nhập
const auth = useAuthStore()

// Computed property kiểm tra người dùng đã đăng nhập chưa
const isAuthenticated = computed(() => auth.isAuthenticated)

/**
 * Xử lý đăng xuất người dùng
 * Gọi action logout từ auth store
 */
function onLogout() {
  auth.logout()
}
</script>

<style scoped>
.header {
  display: flex;
  align-items: center;
  gap: 20px;
  padding: 12px 16px; /* giảm khoảng cách hai bên, đặc biệt bên trái */
}
.menu-btn {
  display: inline-flex;
  align-items: center;
  justify-content: center;
  width: 40px;
  height: 40px;
  border-radius: 8px;
  border: none;
  background: rgba(255,255,255,0.2);
  color: #fff;
  cursor: pointer;
}
.menu-btn:focus {
  outline: 2px solid rgba(255,255,255,0.6);
  outline-offset: 2px;
}
.title {
  font-size: 20px;
  font-weight: 600;
  margin: 0;
  white-space: nowrap;
  overflow: hidden;
  text-overflow: ellipsis;
}
.search {
  display: flex;
  align-items: center;
  gap: 8px;
  margin-left: 16px;
  padding: 6px 10px;
  background: rgba(255,255,255,0.2);
  border-radius: 10px;
}
.search input {
  border: none;
  background: transparent;
  color: #fff;
  outline: none;
}
.search input::placeholder { color: rgba(255,255,255,0.8); }
.right {
  /* đặt cạnh khối search thay vì đẩy sang mép phải */
  display: flex;
  align-items: center;
  gap: 8px;
}
.icon-btn {
  display: inline-flex;
  align-items: center;
  justify-content: center;
  width: 36px;
  height: 36px;
  border-radius: 8px;
  border: none;
  background: rgba(255,255,255,0.2);
  color: #fff;
  cursor: pointer;
}
.login-btn { color: #fff; text-decoration: none; padding: 6px 10px; border-radius: 8px; background: rgba(255,255,255,0.2); }
.user { display: flex; align-items: center; gap: 8px; }
.user.link { text-decoration: none; color: #fff; }
.user.left { margin-left: 8px; }
.avatar { width: 32px; height: 32px; border-radius: 50%; }
.name { font-weight: 500; }
@media (max-width: 768px) {
  .title { font-size: 16px; }
  .search { display: none; }
}
</style>
