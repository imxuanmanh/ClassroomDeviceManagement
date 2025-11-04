<template>
  <Layout v-if="auth.isAuthenticated">
    <router-view />
  </Layout>
  <router-view v-else />
</template>

<script setup>
import Layout from '@/components/Layout/Layout.vue'
import { useAuthStore } from '@/stores/auth'

const auth = useAuthStore()
</script>
