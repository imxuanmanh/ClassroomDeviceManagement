<!-- 
  LAYOUT CHÍNH CỦA ỨNG DỤNG
  - Quản lý bố cục tổng thể: sidebar, header, content
  - Hỗ trợ thu gọn/mở rộng sidebar
  - Responsive cho mobile với backdrop
-->
<template>
  <div class="layout" :class="{ 'is-collapsed': isSidebarCollapsed }">
    <!-- Sidebar điều hướng -->
    <div class="sidebar">
      <Sidebar :collapsed="isSidebarCollapsed" />
    </div>
    
    <!-- Backdrop cho mobile khi sidebar mở -->
    <div class="backdrop" @click="closeSidebarOnMobile" />
    
    <!-- Phần nội dung chính -->
    <div class="main">
      <!-- Header với menu toggle và thông tin người dùng -->
      <div class="header">
        <Header :collapsed="isSidebarCollapsed" @toggle-sidebar="toggleSidebar" />
      </div>
      
      <!-- Nội dung trang hiện tại -->
      <main class="content" role="main">
        <router-view />
      </main>
    </div>
  </div>
</template>

<script setup>
import { ref, onMounted, onBeforeUnmount } from 'vue'
import Sidebar from './Sidebar.vue'
import Header from './Header.vue'

// Trạng thái thu gọn/mở rộng sidebar
const isSidebarCollapsed = ref(false)

/**
 * Tải trạng thái sidebar từ localStorage khi khởi động
 * Nhớ trạng thái người dùng đã chọn trước đó
 */
function loadInitialSidebarState() {
  try {
    const saved = localStorage.getItem('layout.sidebarCollapsed')
    if (saved === 'true') {
      isSidebarCollapsed.value = true
    }
  } catch (_) {
    // Bỏ qua lỗi nếu localStorage không khả dụng
  }
}

/**
 * Bật/tắt trạng thái thu gọn sidebar
 * Lưu trạng thái vào localStorage
 */
function toggleSidebar() {
  isSidebarCollapsed.value = !isSidebarCollapsed.value
  try {
    localStorage.setItem('layout.sidebarCollapsed', String(isSidebarCollapsed.value))
  } catch (_) {
    // Bỏ qua lỗi nếu localStorage không khả dụng
  }
}

/**
 * Đóng sidebar trên mobile khi click backdrop
 * Chỉ có ý nghĩa trên màn hình nhỏ khi sidebar đang mở
 */
function closeSidebarOnMobile() {
  if (!isSidebarCollapsed.value) {
    isSidebarCollapsed.value = true
    try { localStorage.setItem('layout.sidebarCollapsed', 'true') } catch (_) {}
  }
}

/**
 * Xử lý phím tắt
 * - Ctrl+B hoặc Cmd+B: bật/tắt sidebar
 * - Escape: đóng sidebar trên mobile
 */
function onKeyDown(e) {
  const isMeta = e.ctrlKey || e.metaKey
  if (isMeta && (e.key === 'b' || e.key === 'B')) {
    e.preventDefault()
    toggleSidebar()
    return
  }
  if (e.key === 'Escape') {
    closeSidebarOnMobile()
  }
}

// Khởi tạo khi component được mount
onMounted(() => {
  loadInitialSidebarState()
  window.addEventListener('keydown', onKeyDown)
})

// Dọn dẹp khi component bị unmount
onBeforeUnmount(() => {
  window.removeEventListener('keydown', onKeyDown)
})
</script>

<style scoped>
.layout {
  display: flex;
  height: 100vh;
  width: 100vw;
  /* CSS Variables for easy theming */
  --sidebar-width: 250px;
  --sidebar-width-collapsed: 72px;
  --header-height: 80px;
  --header-gradient: linear-gradient(90deg, #d451ad 0%, #0bdee6 100%);
  --content-gradient: linear-gradient(90deg, #f3f9ff 0%, #c1e6ff 100%);
  padding-left: var(--sidebar-width);
  box-sizing: border-box;
}

.sidebar {
  width: var(--sidebar-width);
  flex-shrink: 0;
  height: 100vh;
  position: fixed;
  left: 0;
  top: 0;
  z-index: 10;
  background: #fff;
}

.main {
  flex: 1;
  display: flex;
  flex-direction: column;
  height: 100vh;
}

.header {
  flex: 0 0 var(--header-height);
  height: var(--header-height);
  background: var(--header-gradient);
  display: flex;
  align-items: center;
  padding: 0; /* padding do Header.vue tự quản lý để canh sát sidebar hơn */
  color: #fff;
}
.content {
  flex: 1;
  color: #000000;
  padding: 20px;
  background: var(--content-gradient);
  overflow-y: auto;
  /* content nằm dưới header theo luồng, không cần margin-top */
  min-height: calc(100vh - var(--header-height));
  box-sizing: border-box;
  border-radius: 12px 0 0 0;
  /* Thêm shadow nếu muốn */
  box-shadow: 0 2px 8px rgba(0,0,0,0.03);
}

/* Collapsed state */
.layout.is-collapsed .sidebar {
  width: var(--sidebar-width-collapsed);
}
.layout.is-collapsed { padding-left: var(--sidebar-width-collapsed); }

/* Responsive adjustments */
@media (max-width: 768px) {
  .layout { padding-left: 0; }
  .sidebar {
    transform: translateX(0);
    transition: width 0.2s ease, transform 0.2s ease;
  }
  .layout.is-collapsed .sidebar {
    transform: translateX(-100%);
  }
  /* Backdrop only visible when sidebar is open on mobile (not collapsed) */
  .backdrop {
    position: fixed;
    inset: 0;
    background: rgba(0,0,0,0.35);
    z-index: 5;
    opacity: 1;
    pointer-events: auto;
  }
  .layout.is-collapsed .backdrop {
    opacity: 0;
    pointer-events: none;
  }
  /* Ensure stacking: sidebar above backdrop, header/main above backdrop when collapsed */
  .sidebar { z-index: 10; }
}
</style>
