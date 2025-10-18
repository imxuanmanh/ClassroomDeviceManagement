<!-- <template>
  <Layout v-if="auth.isAuthenticated" />
  <router-view v-else />
</template>

<script setup>
import Layout from '@/components/Layout/Layout.vue'
import { useAuthStore } from '@/stores/auth.js'
const auth = useAuthStore()
</script> -->

<template>
  <Layout>
    <router-view />
  </Layout>
</template>

<script setup>
import Layout from '@/components/Layout/Layout.vue'
</script>
