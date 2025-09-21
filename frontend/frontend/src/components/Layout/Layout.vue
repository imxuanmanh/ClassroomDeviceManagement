<!--
  LAYOUT CHÍNH CỦA ỨNG DỤNG
  - Quản lý bố cục tổng thể: sidebar, header, content
  - Hỗ trợ thu gọn/mở rộng sidebar
  - Responsive cho mobile với backdrop
-->
<template>
  <div class="layout">
    <!-- Sidebar điều hướng -->
    <div class="sidebar">
      <Sidebar />
    </div>

    <!-- Backdrop cho mobile khi sidebar mở -->
    <!-- <div class="backdrop" @click="closeSidebarOnMobile" /> -->

    <!-- Phần nội dung chính -->
    <div class="main">
      <!-- Header với menu toggle và thông tin người dùng -->
      <div class="header">
        <Header :collapsed="false" @toggle-sidebar="toggleSidebar" />
      </div>

      <!-- Nội dung trang hiện tại -->
      <main class="content" role="main">
        <router-view />
      </main>
    </div>
  </div>
</template>

<script setup>
defineOptions({ name: 'AppLayout' })
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
  } catch {
    /* ignore */
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
  } catch {
    /* ignore */
  }
}

/**
 * Đóng sidebar trên mobile khi click backdrop
 * Chỉ có ý nghĩa trên màn hình nhỏ khi sidebar đang mở
 */
function closeSidebarOnMobile() {
  if (!isSidebarCollapsed.value) {
    isSidebarCollapsed.value = true
    try {
      localStorage.setItem('layout.sidebarCollapsed', 'true')
    } catch {
      /* ignore */
    }
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
  --header-height: 64px;
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
  background: #417c85;
  display: flex;
  align-items: center;
  padding: 0;
  color: #fff;
}
.content {
  flex: 1;
  color: #000000;
  padding: 20px;
  background: #f6f9ff;
  overflow-y: auto;
  /* content nằm dưới header theo luồng, không cần margin-top */
  min-height: calc(100vh - var(--header-height));
  box-sizing: border-box;
  border-radius: 0;
}

/* Collapsed state */
/* no collapsed state */

/* Responsive adjustments */
@media (max-width: 768px) {
  .layout {
    padding-left: 0;
  }
  .sidebar {
    display: none;
  }
}
</style>
