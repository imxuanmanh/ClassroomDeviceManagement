<template>
  <aside class="sidebar">
    <h2 class="logo">QUẢN LÝ THIẾT BỊ PHÒNG HỌC</h2>
    <nav>
      <li><router-link to="/">Dashboard</router-link></li>
      <li><router-link to="/equipment">Thiết bị</router-link></li>
      <li><router-link to="/users">Người mượn</router-link></li>
      <li><router-link to="/history">Lịch sử</router-link></li>
      <li><router-link to="/reports">Báo cáo</router-link></li>
    </nav>
  </aside>
</template>

<style scoped>
.sidebar {
  position: fixed;
  left: 0;
  top: 0;
  bottom: 0;
  width: 200px;
  background: #ffffff;
  color: rgb(97, 0, 241);
  min-height: 100vh;
  padding: 20px;
}
.logo {
  font-size: 10px;
  font-weight: bold;
  margin-bottom: 30px;
}

</style>
