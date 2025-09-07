<template>
  <div id="app">
    <!-- Header -->
    <Header />

    <div class="main">
      <!-- Sidebar -->
      <Sidebar />

      <!-- Nội dung trang sẽ hiển thị ở đây -->
      <router-view />
    </div>
  </div>
</template>

<script>
import Header from '@/components/Layout/Header.vue'
import Sidebar from '@/components/Layout/Sidebar.vue'
export default {
  name: 'App',
  components: {
    Header,
    Sidebar,
  },
}

</script>

<style>
#app {
  font-family: Arial, sans-serif;
}

.main {
  display: flex;
}
</style>
