<template>
  <section class="reports">
    <header class="page-header">
      <h2>Báo cáo</h2>
      <div class="filters">
        <select v-model="range">
          <option value="7">7 ngày</option>
          <option value="30">30 ngày</option>
          <option value="90">90 ngày</option>
        </select>
      </div>
    </header>
    <div class="cards">
      <div class="card">
        <div class="label">Thiết bị</div>
        <div class="value">{{ totalDevices }}</div>
      </div>
      <div class="card">
        <div class="label">Đang mượn</div>
        <div class="value">{{ activeLoans }}</div>
      </div>
      <div class="card">
        <div class="label">Lượt mượn</div>
        <div class="value">{{ loansInRange }}</div>
      </div>
    </div>
    <table class="table">
      <thead>
        <tr><th>Ngày</th><th>Loại</th><th>Thiết bị</th><th>Người mượn</th></tr>
      </thead>
      <tbody>
        <tr v-for="(r,i) in recent" :key="i">
          <td>{{ r.date }}</td>
          <td>{{ r.type === 'borrow' ? 'Mượn' : 'Trả' }}</td>
          <td>{{ r.deviceName }}</td>
          <td>{{ r.borrowerName }}</td>
        </tr>
        <tr v-if="recent.length===0"><td colspan="4" style="text-align:center">Không có dữ liệu</td></tr>
      </tbody>
    </table>
  </section>
</template>

<script setup>
import { computed, ref } from 'vue'
import { useHistoryStore } from '@/stores/history.js'

const store = useHistoryStore()
const range = ref('30')

const recent = computed(() => {
  const days = parseInt(range.value, 10)
  const from = new Date(); from.setDate(from.getDate() - days)
  return store.records.filter(r => new Date(r.date) >= from).slice(0, 50)
})
const loansInRange = computed(() => recent.value.filter(r => r.type==='borrow').length)
const activeLoans = computed(() => store.records.filter(r => r.type==='borrow').length - store.records.filter(r => r.type==='return').length)
const totalDevices = computed(() =>  recent.value.reduce((set, r) => set.add(r.deviceId), new Set()).size )
</script>

<style scoped>
.reports { padding: 16px 12px; }
.page-header { display: flex; align-items: center; justify-content: space-between; margin-bottom: 12px; }
.filters select { padding: 6px 8px; border: 1px solid #e5e7eb; border-radius: 8px; }
.cards { display: grid; grid-template-columns: repeat(3, minmax(0,1fr)); gap: 12px; margin-bottom: 12px; }
.card { background: #fff; border-radius: 12px; box-shadow: 0 2px 8px rgba(0,0,0,0.06); padding: 16px; }
.card .label { color: #6b7280; margin-bottom: 4px; }
.card .value { font-size: 22px; font-weight: 700; color: #111827; }
.table { width: 100%; border-collapse: collapse; background: #fff; border-radius: 12px; overflow: hidden; }
.table th, .table td { border: 1px solid #eee; padding: 10px; text-align: left; }
.table th { background: #f8fafc; }
</style>