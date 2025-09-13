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
  height: 100vh;   /* phủ toàn màn hình */
  width: 100vw;
}

.layout > *:first-child {
  flex-shrink: 0;
  width: 250px;   /* sidebar cố định */
  height: 100vh;
}

.main {
  flex: 1;
  display: flex;
  flex-direction: column;
}
.sidebar {
  flex: 1;
  position: fixed;
  height: 100vh;
}
.header {
  flex: 0 0 80px; /* chiều cao cố định */
  height: 80px;
  margin-left: 300px;
  background: linear-gradient(90deg, #d451ad 0%, #0bdee6 100%);
  display: flex;
  align-items: center;
  padding: 20px;
  color: #fff;
}

.content {
  flex: 1;
  padding: 20px;
  margin-left: 300px;
  background: linear-gradient(90deg, #f3f9ff 0%, #c1e6ff 100%);
  overflow-y: auto; /* cuộn nội dung khi quá dài */
}
</style>
