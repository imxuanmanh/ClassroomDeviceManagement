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

/* ===== TRANSITION LOGIN → MAIN ===== */
/* Login fade out + scale down khi rời đi */
.login-leave-active {
  transition: all 0.6s cubic-bezier(0.68, -0.55, 0.265, 1.55);
}

.login-leave-to {
  opacity: 0;
  transform: scale(0.9) translateY(-20px);
}

/* Trang chính fade in + slide từ phải */
.login-enter-active {
  transition: all 0.7s cubic-bezier(0.22, 1, 0.36, 1);
}

.login-enter-from {
  opacity: 0;
  transform: translateX(50px);
}

.login-enter-to {
  opacity: 1;
  transform: translateX(0);
}


</style>

<!-- <template>
  <Layout v-if="auth.isAuthenticated">
    <router-view />
  </Layout>
  <router-view v-else />
</template>

<script setup>
import Layout from '@/components/Layout/Layout.vue'
import { useAuthStore } from '@/stores/auth'

const auth = useAuthStore()
</script> -->
