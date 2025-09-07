<template>
  <table>
    <thead>
      <tr>
        <th>Mã TB</th><th>Tên thiết bị</th><th>Loại</th><th>Trạng thái</th>
      </tr>
    </thead>
    <tbody>
      <EquipmentCard code="LAP001" name="Laptop Dell" type="Laptop" status="Khả dụng" />
      <EquipmentCard code="PJ002" name="Máy chiếu Epson" type="Máy chiếu" status="Đang mượn" />
    </tbody>
  </table>
</template>

<script setup>
import EquipmentCard from './EquipmentCard.vue'
</script>
