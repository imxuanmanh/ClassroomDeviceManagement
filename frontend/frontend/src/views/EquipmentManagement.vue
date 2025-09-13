<template>
  <div class="equipment-List">
    <h2>Danh Sách Thiết Bị</h2>
    <EquipmentList />
  </div>
</template>
<script setup>
  import EquipmentList from '@/components/Equipment/EquipmentList.vue';
</script>
<style scoped>
.equipment-List {
  margin: 50px;
  margin-top: 0;
  font-family: Arial, Helvetica, sans-serif;
  font-size: 16px;
  color: #0419ff;
}
.equipment-List h2 {
  margin-bottom: 20px;
  margin-left: 400px;
}
</style>
