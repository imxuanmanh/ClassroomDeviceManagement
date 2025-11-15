<template>
  <div class="layout">
    <div v-if="!isSidebarCollapsed" class="sidebar">
      <Sidebar />
    </div>

    <div class="main" :style="isSidebarCollapsed ? { paddingLeft: 0 } : {}">
      <Header @toggle-sidebar="toggleSidebar" />

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

const isSidebarCollapsed = ref(false)

function toggleSidebar() {
  isSidebarCollapsed.value = !isSidebarCollapsed.value
}

function onKeyDown(e) {
  const isMeta = e.ctrlKey || e.metaKey
  if (isMeta && (e.key === 'b' || e.key === 'B')) {
    e.preventDefault()
    toggleSidebar()
  }
  if (e.key === 'Escape') {
    isSidebarCollapsed.value = true
  }
}

onMounted(() => window.addEventListener('keydown', onKeyDown))
onBeforeUnmount(() => window.removeEventListener('keydown', onKeyDown))
</script>

<style scoped>
.layout {
  display: flex;
  height: 100vh;
  width: 100vw;
  --sidebar-width: 250px;
  --header-height: 64px;
}

.sidebar {
  width: var(--sidebar-width);
  flex-shrink: 0;
  height: 100vh;
  position: fixed;
  left: 0;
  top: 0;
  z-index: 10;
  /* Thuộc tính background sẽ được kiểm soát bởi file Sidebar.vue */
  transition: all 0.3s ease;
}

.main {
  flex: 1;
  display: flex;
  flex-direction: column;
  height: 100vh;
  transition: padding-left 0.3s ease;
  padding-left: var(--sidebar-width);
}

.content {
  flex: 1;
  /* === THAY ĐỔI MÀU NỀN VÀ CHỮ === */
  color: #fff; /* Chữ mặc định là màu trắng */
  padding: 24px;
  background: linear-gradient(135deg, #e8fff8, #d2fff0, #b8f5e1);
}
</style>
