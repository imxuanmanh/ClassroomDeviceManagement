<template>
  <section class="dashboard">
    <div class="home">
      <header class="page-header">
        <h2>Tổng quan</h2>
      </header>

      <!-- Thẻ thống kê tổng quan -->
      <div class="stats">
        <StatCard title="Thiết bị" :value="stats.total.toString()" />
        <StatCard title="Đang mượn" :value="stats.borrowed.toString()" />
        <StatCard title="Khả dụng" :value="stats.available.toString()" />
        <StatCard title="Lượt mượn (30 ngày)" :value="stats.recentBorrows.toString()" />
      </div>
    </div>
  </section>
</template>

<script>
import { useHistoryStore } from '@/stores/history.js'
import { deviceApi } from '@/config/api.js'

export default {
  name: 'BorrowPage',
  data() {
    return {
      devices: [],
      loading: false,
      error: '',
    }
  },
  async created() {
    await this.fetchDevices()
  },
  methods: {
    async fetchDevices() {
      this.loading = true
      this.error = ''
      try {
        const list = await deviceApi.getAll()
        this.devices = (Array.isArray(list) ? list : []).map((d) => ({
          id: d.deviceId || d.id || '',
          name: d.deviceName || d.name || '',
          type: d.deviceType || d.type || '',
          status: d.status || 'Khả dụng',
          borrower: d.borrower || '-',
          borrowDate: d.borrowDate || '-',
        }))
      } catch (err) {
        console.error(err)
        this.error = 'Không thể tải danh sách thiết bị'
      } finally {
        this.loading = false
      }
    },
  },
  computed: {
    stats() {
      const total = this.devices.length
      const borrowed = this.devices.filter((d) => d.status !== 'Khả dụng').length
      const available = total - borrowed
      let recentBorrows = 0

      try {
        const store = useHistoryStore()
        const from = new Date()
        from.setDate(from.getDate() - 30)
        recentBorrows = store.records.filter(
          (r) => r.type === 'borrow' && new Date(r.date) >= from,
        ).length
      } catch (err) {
        console.warn('Tính lượt mượn gần đây thất bại', err)
      }

      return { total, borrowed, available, recentBorrows }
    },
  },
}
</script>

<script setup>
import StatCard from '@/components/Dashboard/StatCard.vue'
</script>

<style scoped>
.home {
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
  color: #00adb5;
}
.stats {
  display: grid;
  grid-template-columns: repeat(4, minmax(0, 1fr));
  gap: 12px;
  margin-bottom: 16px;
}
</style>
