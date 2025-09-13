<template>
  <div class="table-container">
    <table class="equiment" border="1" cellspacing="0" cellpadding="10">
      <thead>
        <tr>
          <th>ID</th>
          <th>Tên Thiết bị</th>
          <th>Loại</th>
          <th>Thông số kỹ thuật</th>
          <th>Vị trí lưu trữ</th>
          <th>Tổng số lượng</th>
          <th>Số lượng khả dụng</th>
        </tr>
      </thead>
      <tbody>
        <tr v-for="item in equipmentList" :key="item.id">
          <td>{{ item.deviceId}}</td>
          <td>{{ item.deviceName }}</td>
          <td>{{ item.deviceType }}</td>
          <td>{{ item.specifications }}</td>
          <td>{{ item.storageLocation }}</td>
          <td>{{ item.totalQuantity}}</td>
          <td>{{ item.availableQuantity}}</td>
        </tr>
      </tbody>
    </table>
  </div>
</template>
<script setup>
import { ref, onMounted } from 'vue';
const equipmentList = ref([]);

onMounted(async () => {
  try {
    const response = await fetch('http://192.168.1.42:5129/api/device');
    if (!response.ok) {
      throw new Error('Kết nối thất bại');
    }
    equipmentList.value = await response.json();
  } catch (error) {
    console.error('Lỗi fetch dữ liệu', error);
  }
});
</script>
