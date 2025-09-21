<!--
  HEADER ỨNG DỤNG
  - Chứa nút menu toggle, tiêu đề, tìm kiếm, thông báo
  - Hiển thị thông tin người dùng và nút đăng nhập/đăng xuất
  - Hỗ trợ slots để tùy chỉnh nội dung
-->
<template>
  <header class="header" role="banner">
    <h1 class="title">Quản lý Thiết Bị Phòng Học</h1>
  </header>
</template>

<script setup>
defineOptions({ name: 'AppHeader' })
</script>

<style scoped>
.header {
  display: grid;
  grid-template-columns: 1fr auto;
  align-items: center;
  padding: 12px 16px;
  gap: 12px;
}
.title {
  margin: 0;
  font-size: 20px;
  text-align: center;
  justify-self: center;
}
.menu-btn {
  display: inline-flex;
  align-items: center;
  justify-content: center;
  width: 40px;
  height: 40px;
  border-radius: 8px;
  border: none;
  background: rgba(255, 255, 255, 0.2);
  color: #fff;
  cursor: pointer;
}
.menu-btn:focus {
  outline: 2px solid rgba(255, 255, 255, 0.6);
  outline-offset: 2px;
}
.title {
  font-size: 20px;
  font-weight: 600;
  margin: 0;
  white-space: nowrap;
  overflow: hidden;
  text-overflow: ellipsis;
}
.right {
  display: none;
}
@media (max-width: 768px) {
  .title {
    font-size: 16px;
  }
}
</style>
