<template>
  <div>
    <h2>Dashboard</h2>
    <div class="cards">
      <StatCard title="thiết bị" value="24" />
      <StatCard title="Đang mượn" value="4" />
      <StatCard title="Khả dụng" value="18" />
      <StatCard title="Lượt mượn(30 ngày)" value="2" />
      <!-- <StatCard title="Bảo trì" value="2" /> -->
    </div>
  </div>
</template>

<script setup>
import StatCard from '@/components/Dashboard/StatCard.vue'
</script>

<style scoped>
.cards {
  display: flex;
  gap: 1rem;
}
</style>
