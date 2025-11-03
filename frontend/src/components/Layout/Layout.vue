<!--
  LAYOUT CHÍNH CỦA ỨNG DỤNG
  - Quản lý bố cục tổng thể: sidebar, header, content
  - Hỗ trợ thu gọn/mở rộng sidebar
  - Responsive cho mobile với backdrop
-->

<template>
  <div class="layout" :style="hideLayout ? { background: '#f6f9ff', paddingLeft: 0 } : {}">
    <!-- Sidebar điều hướng -->
    <div class="sidebar" v-if="!hideLayout">
      <Sidebar />
    </div>

    <!-- Phần nội dung chính -->
    <div class="main" :style="hideLayout ? { paddingLeft: 0 } : {}">
      <!-- Header -->
      <div class="header" v-if="!hideLayout">
        <Header :collapsed="false" @toggle-sidebar="toggleSidebar" />
      </div>

      <!-- Nội dung trang hiện tại -->
      <main
        class="content"
        role="main"
        :style="hideLayout ? { padding: 0, background: '#f6f9ff' } : {}"
      >
        <router-view />
      </main>
    </div>
  </div>
</template>

<script setup>
defineOptions({ name: 'AppLayout' })
import { ref, onMounted, onBeforeUnmount, computed } from 'vue'
import { useRoute } from 'vue-router'
import Sidebar from './Sidebar.vue'
import Header from './Header.vue'

const route = useRoute()

// ✅ Tự động ẩn layout ở các trang cụ thể
// const hideLayout = computed(() => ['/login', '/user-borrow'].includes(route.path))
const hideLayout = computed(() => ['/login'].includes(route.path))

// Trạng thái thu gọn/mở rộng sidebar
const isSidebarCollapsed = ref(false)

function loadInitialSidebarState() {
  try {
    const saved = localStorage.getItem('layout.sidebarCollapsed')
    if (saved === 'true') {
      isSidebarCollapsed.value = true
    }
  } catch {}
}

function toggleSidebar() {
  isSidebarCollapsed.value = !isSidebarCollapsed.value
  try {
    localStorage.setItem('layout.sidebarCollapsed', String(isSidebarCollapsed.value))
  } catch {}
}

function closeSidebarOnMobile() {
  if (!isSidebarCollapsed.value) {
    isSidebarCollapsed.value = true
    try {
      localStorage.setItem('layout.sidebarCollapsed', 'true')
    } catch {}
  }
}

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

onMounted(() => {
  loadInitialSidebarState()
  window.addEventListener('keydown', onKeyDown)
})

onBeforeUnmount(() => {
  window.removeEventListener('keydown', onKeyDown)
})
</script>

<style scoped>
.layout {
  display: flex;
  height: 100vh;
  width: 100vw;
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

/* .header {
  flex: 0 0 var(--header-height);
  height: var(--header-height);
  background: #417c85;
  display: flex;
  align-items: center;
  padding: 0;
  color: #fff;
} */

.content {
  flex: 1;
  color: #000000;
  padding: 20px;
  background: #f6f9ff;
  overflow-y: auto;
  min-height: calc(100vh - var(--header-height));
  box-sizing: border-box;
}

@media (max-width: 768px) {
  .layout {
    padding-left: 0;
  }
  .sidebar {
    display: none;
  }
}

.content {
  flex: 1;
  color: #000;
  padding: 24px;
  background: #f6f9ff;
  overflow-y: auto;
  min-height: calc(100vh - var(--header-height));
  box-sizing: border-box;
  box-shadow: inset 0 1px 3px rgba(0, 0, 0, 0.03);
  transition: all 0.3s ease;
}
</style>
