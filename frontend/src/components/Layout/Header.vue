<template>
  <header class="header" role="banner">
    <!-- Bên trái -->
    <div class="left">
      <button class="menu-btn" @click="$emit('toggle-sidebar')">
        <span class="material-icons">menu</span>
      </button>
      <h1 class="title">Quản lý Thiết Bị Phòng Học</h1>
    </div>

    <!-- Bên phải -->
    <div class="right">
      <div class="search-box">
        <input type="text" placeholder="Tìm kiếm..." />
        <span class="material-icons">search</span>
      </div>

      <button class="icon-btn">
        <span class="material-icons">notifications</span>
      </button>

      <div class="user-info">
        <img src="/logo.png" alt="user" />
        <span>Admin</span>
      </div>
    </div>
  </header>
</template>

<script setup>
defineOptions({ name: 'AppHeader' })
</script>

<style scoped>
.header {
  position: fixed;
  top: 0;
  left: 0;
  right: 0;
  height: 64px;
  display: flex;
  align-items: center;
  justify-content: space-between;
  background: #2563eb; /* xanh dương đơn giản */
  color: #fff;
  padding: 0 20px;
  z-index: 1000;
  box-shadow: 0 2px 6px rgba(0, 0, 0, 0.05);
}

.left {
  display: flex;
  align-items: center;
  gap: 12px;
}

.title {
  font-size: 20px;
  font-weight: 600;
  margin: 0;
  white-space: nowrap;
  overflow: hidden;
  text-overflow: ellipsis;
}

.menu-btn {
  display: flex;
  align-items: center;
  justify-content: center;
  width: 36px;
  height: 36px;
  border: none;
  border-radius: 6px;
  background: rgba(255, 255, 255, 0.15);
  color: #fff;
  cursor: pointer;
  transition:
    background 0.2s,
    transform 0.2s;
}

.menu-btn:hover {
  background: rgba(255, 255, 255, 0.25);
  transform: scale(1.05);
}

.menu-btn:focus {
  outline: 2px solid rgba(255, 255, 255, 0.5);
  outline-offset: 2px;
}

.right {
  display: flex;
  align-items: center;
  gap: 16px;
}

.search-box {
  display: flex;
  align-items: center;
  background: rgba(255, 255, 255, 0.15);
  border-radius: 8px;
  padding: 4px 8px;
}

.search-box input {
  background: transparent;
  border: none;
  outline: none;
  color: #fff;
  padding: 4px;
}

.search-box input::placeholder {
  color: rgba(255, 255, 255, 0.7);
}

.icon-btn {
  background: none;
  border: none;
  color: #fff;
  cursor: pointer;
  padding: 6px;
  border-radius: 50%;
  transition: background 0.2s;
}

.icon-btn:hover {
  background: rgba(255, 255, 255, 0.2);
}

.user-info {
  display: flex;
  align-items: center;
  gap: 8px;
  margin-right: 12px;
}

.user-info img {
  border-radius: 50%;
  width: 32px;
  height: 32px;
}

@media (max-width: 768px) {
  .title {
    font-size: 16px;
  }
}
</style>
