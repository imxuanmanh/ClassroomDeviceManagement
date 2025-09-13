<template>
  <aside class="sidebar">
    <div class="logo">
      <router-link to="/"><span class="material-icons">computer</span>Home</router-link> <br>
    </div>
    <nav class="nav">
      <router-link to="/"><span class="material-icons">analytics</span>Dashboard</router-link> <br>
      <router-link to="/equipment"><span class="material-icons">devices</span>Thiết bị</router-link> <br>
      <router-link to="/users"><span class="material-icons">person</span>Người mượn</router-link> <br>
      <router-link to="/history"><span class="material-icons">history</span>Lịch sử</router-link><br>
      <router-link to="/reports"><span class="material-icons">report</span>Báo cáo</router-link>
    </nav>
  </aside>
</template>

<style scoped>
.sidebar {
  flex: 1;
  position: fixed;
  margin-right: 300px;
  background: #ffffff;
  color: rgb(97, 0, 241);
  padding: 20px;
}
.material-icons {
  color: black;
  margin-bottom: 10px;
  vertical-align: middle;
  margin-right: 10px;
}
.router-link {
  display: block;
  color: #000000;
  text-decoration: none;
  margin: 15px 0;
  font-size: 20px;
}
.nav {
  margin-top: 30px;
  color: #000000;
}
.logo {
  font-size: 20px;
  margin-top:0px;
  margin-left: 50px;
  margin-bottom: 20px;
}

</style>
