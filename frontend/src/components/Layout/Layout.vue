<template>
  <div class="layout">
    <Sidebar />
    <div class="main">
      <Header />
      <div class="content">
        <router-view />
      </div>
    </div>
  </div>
</template>

<script setup>
import Sidebar from './Sidebar.vue'
import Header from './Header.vue'
</script>

<style scoped>
.layout {
  display: flex;
  min-height: 100vh;
}
.layout > *:first-child {
  flex-shrink: 0;
  width: 200px;
}
.main {
  flex: 1;
  display: flex;
  flex-direction: column;
}
.content {
  flex: 1;
  padding: 20px;
  background: #f4f4f4;
  min-height: calc(100vh - 60px);
}
</style>
