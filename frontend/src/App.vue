<template>
  <router-view v-slot="{ Component, route }">
    <transition :name="route.meta.transition || 'page'" mode="out-in">
      <component :is="Component" :key="route.path" />
    </transition>
  </router-view>
</template>

<script setup>
// Không cần logic phức tạp, dùng meta từ router
</script>

<style>
/* Reset cơ bản */
* {
  margin: 0;
  padding: 0;
  box-sizing: border-box;
}

body {
  font-family: 'Poppins', 'Inter', sans-serif;
  overflow-x: hidden;
}

/* ===== TRANSITION LOGIN → DASHBOARD (Fade + Scale) ===== */

.login-leave-to {
  opacity: 0;
  transform: scale(0.9) rotate(-2deg);
}

.login-enter-from {
  opacity: 0;
  transform: scale(1.1) rotate(2deg);
}
.login-enter-to {
  opacity: 1;
  transform: scale(1) rotate(0);
}

.login-enter-active {
  transition: all 0.6s cubic-bezier(0.25, 0.46, 0.45, 0.94);
  transition-delay: 0.2s;
}

.login-enter-from {
  opacity: 0;
  transform: scale(1.03);
}
.login-enter-to {
  opacity: 1;
  transform: scale(1);
}

/* ===== TRANSITION GIỮA CÁC TRANG THÔNG THƯỜNG ===== */
.page-enter-active,
.page-leave-active {
  transition: all 0.3s ease-in-out;
}

.page-enter-from {
  opacity: 0;
  transform: translateX(20px);
}

.page-leave-to {
  opacity: 0;
  transform: translateX(-20px);
}
</style>
