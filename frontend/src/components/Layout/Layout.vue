<template>
  <div class="layout">
    <!-- ✅ Sidebar -->
    <div v-if="!isSidebarCollapsed" class="sidebar">
      <Sidebar />
    </div>

    <!-- ✅ Khu vực chính -->
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
  background: #fff;
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
  color: #000;
  padding: 24px;
  background: #f6f9ff;
  overflow-y: auto;
}
</style>
