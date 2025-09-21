<template>
  <section class="history">
    <header class="page-header">
      <h2>Lịch sử mượn/trả</h2>
      <div class="filters">
        <input v-model="q" placeholder="Tìm theo thiết bị/người mượn" />
        <select v-model="type">
          <option value="">Tất cả</option>
          <option value="borrow">Mượn</option>
          <option value="return">Trả</option>
        </select>
      </div>
    </header>
    <table class="table">
      <thead>
        <tr>
          <th>Thời gian</th>
          <th>Loại</th>
          <th>Thiết bị</th>
          <th>Người mượn</th>
        </tr>
      </thead>
      <tbody>
        <tr v-for="(r, i) in filtered" :key="i">
          <td>{{ r.date }}</td>
          <td>{{ r.type === 'borrow' ? 'Mượn' : 'Trả' }}</td>
          <td>{{ r.deviceId }} - {{ r.deviceName }}</td>
          <td>{{ r.borrowerId }} - {{ r.borrowerName }}</td>
        </tr>
        <tr v-if="filtered.length === 0">
          <td colspan="4" style="text-align: center">Không có dữ liệu</td>
        </tr>
      </tbody>
    </table>
  </section>
</template>

<script setup>
defineOptions({ name: 'BorrowHistory' })
import { computed, ref, onMounted } from 'vue'
import { historyApi } from '@/config/api.js'

const q = ref('')
const type = ref('')
const records = ref([])
const loading = ref(false)
const error = ref('')

async function fetchHistory() {
  loading.value = true
  error.value = ''
  try {
    const data = await historyApi.getAll()
    records.value = Array.isArray(data) ? data : []
  } catch (err) {
    console.error(err)
    error.value = 'Không thể tải lịch sử'
  } finally {
    loading.value = false
  }
}

onMounted(fetchHistory)

const filtered = computed(() => {
  const query = q.value.toLowerCase()
  return records.value.filter((r) => {
    const matchesType = !type.value || r.type === type.value
    const hay = `${r.deviceId} ${r.deviceName} ${r.borrowerId} ${r.borrowerName}`.toLowerCase()
    return matchesType && hay.includes(query)
  })
})
</script>

<style scoped>
.history {
  padding: 16px 12px;
}
.page-header {
  display: flex;
  align-items: center;
  justify-content: space-between;
  gap: 12px;
  margin-bottom: 16px;
}
.page-header h2 {
  margin: 0;
  color: #111827;
}
.filters {
  display: flex;
  gap: 8px;
}
.filters input,
.filters select {
  padding: 6px 8px;
  border: 1px solid #e5e7eb;
  border-radius: 8px;
}
.table {
  width: 100%;
  border-collapse: collapse;
  background: #fff;
  border-radius: 12px;
  overflow: hidden;
}
.table th,
.table td {
  border: 1px solid #eee;
  padding: 10px;
  text-align: left;
}
.table th {
  background: #f8fafc;
}
</style>
