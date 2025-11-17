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
  --sidebar-width: 280px;
  --header-height: 70px;
  background: #0a1520;
}

.sidebar {
  width: var(--sidebar-width);
  flex-shrink: 0;
  height: 100vh;
  position: fixed;
  left: 0;
  top: 0;
  z-index: 10;
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
  color: #fff;
  padding: 24px;
  overflow-y: auto;
  background: linear-gradient(135deg, #0a1f2e 0%, #081b29 100%);
  position: relative;
}

/* Decorative background pattern */
.content::before {
  content: '';
  position: absolute;
  top: 0;
  left: 0;
  width: 100%;
  height: 100%;
  background-image:
    radial-gradient(circle at 20% 50%, rgba(0, 239, 255, 0.03) 0%, transparent 50%),
    radial-gradient(circle at 80% 80%, rgba(0, 239, 255, 0.03) 0%, transparent 50%);
  pointer-events: none;
}

/* Custom scrollbar cho content */
.content::-webkit-scrollbar {
  width: 10px;
}

.content::-webkit-scrollbar-track {
  background: rgba(8, 27, 41, 0.5);
}

.content::-webkit-scrollbar-thumb {
  background: rgba(0, 239, 255, 0.3);
  border-radius: 10px;
}

.content::-webkit-scrollbar-thumb:hover {
  background: rgba(0, 239, 255, 0.5);
}
</style>

<!-- <template>
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
  color: black; /* Chữ mặc định là màu trắng */
  padding: 24px;
  background: linear-gradient(135deg, #e8fff8, #d2fff0, #b8f5e1);
}
</style> -->
