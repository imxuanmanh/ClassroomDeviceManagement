<template>
  <section class="dashboard">
    <div class="dashboard-container">
      <header class="page-header">
        <div>
          <h2>Tổng quan & Báo cáo</h2>
        </div>
        <!-- Nút refresh thủ công -->
        <button class="refresh-btn" @click="refreshData" :disabled="isRefreshing">
          <span class="refresh-icon" :class="{ spinning: isRefreshing }">🔄</span>
          <span>{{ isRefreshing ? 'Đang tải...' : 'Làm mới' }}</span>
        </button>
      </header>

      <div class="stats-grid">
        <div class="stat-card card-overview">
          <div class="overview-header">
            <div class="stat-icon icon-primary">
              <svg width="24" height="24" viewBox="0 0 24 24" fill="none" stroke="currentColor">
                <rect x="2" y="7" width="20" height="14" rx="2" ry="2" />
                <path d="M16 21V5a2 2 0 0 0-2-2h-4a2 2 0 0 0-2 2v16" />
              </svg>
            </div>
            <div class="total-info">
              <p class="stat-label">Tổng thiết bị</p>
              <h3 class="stat-value">{{ deviceStats.total }}</h3>
            </div>
          </div>

          <div class="status-grid">
            <div class="status-item">
              <span class="dot available"></span>
              <span class="label">Khả dụng</span>
              <span class="count">{{ deviceStats.available }}</span>
            </div>
            <div class="status-item">
              <span class="dot borrowed"></span>
              <span class="label">Đang mượn</span>
              <span class="count">{{ deviceStats.borrowed }}</span>
            </div>
            <div class="status-item">
              <span class="dot maintenance"></span>
              <span class="label">Bảo trì</span>
              <span class="count">{{ deviceStats.maintenance }}</span>
            </div>
            <div class="status-item">
              <span class="dot broken"></span>
              <span class="label">Hỏng</span>
              <span class="count">{{ deviceStats.broken }}</span>
            </div>
          </div>
        </div>
      </div>

      <div class="chart-section">
        <div class="chart-header">
          <h3>Biểu đồ hoạt động</h3>
          <div class="chart-legend">
            <div class="legend-item">
              <span class="legend-color borrow"></span>
              <span>Mượn</span>
            </div>
            <div class="legend-item">
              <span class="legend-color return"></span>
              <span>Trả</span>
            </div>
          </div>
        </div>
        <div class="chart-container">
          <div class="chart-bars">
            <div v-for="(day, i) in chartData" :key="i" class="bar-group">
              <div class="bars">
                <div class="bar bar-borrow" :style="{ height: day.borrow * 10 + '%' }">
                  <span class="bar-value">{{ day.borrow }}</span>
                </div>
                <div class="bar bar-return" :style="{ height: day.return * 10 + '%' }">
                  <span class="bar-value">{{ day.return }}</span>
                </div>
              </div>
              <span class="bar-label">{{ day.label }}</span>
            </div>
          </div>
        </div>
      </div>

      <div class="table-section">
        <div class="table-header">
          <h3>Hoạt động gần đây</h3>
        </div>
        <div class="table-container">
          <table class="modern-table">
            <thead>
              <tr>
                <th>Ngày giờ</th>
                <th>Loại</th>
                <th>Thiết bị</th>
                <th>Người dùng</th>
                <th>Trạng thái</th>
              </tr>
            </thead>
            <tbody>
              <tr v-for="(item, index) in recentActivities" :key="index" class="table-row">
                <td>{{ item.date }}</td>
                <td>
                  <span :class="['badge', item.type === 'borrow' ? 'badge-blue' : 'badge-green']">
                    {{ item.type === 'borrow' ? 'Mượn' : 'Trả' }}
                  </span>
                </td>
                <td class="device-name">{{ item.device }}</td>
                <td>
                  <div class="borrower-info">
                    <div class="borrower-avatar">{{ item.user.substring(0, 2).toUpperCase() }}</div>
                    <span>{{ item.user }}</span>
                  </div>
                </td>
                <td>
                  <span class="status-badge" :class="item.statusClass">
                    {{ item.status }}
                  </span>
                </td>
              </tr>
            </tbody>
          </table>
        </div>
      </div>
    </div>
  </section>
</template>

<script setup>
import { ref, computed, onMounted, onUnmounted, watch } from 'vue'
import { useDeviceStore } from '@/stores/device'
import { useRoute } from 'vue-router'

const deviceStore = useDeviceStore()
const isRefreshing = ref(false)
const route = useRoute()

// --- 1. LOGIC TÍNH TOÁN 4 TRẠNG THÁI ---
const deviceStats = computed(() => {
  const stats = deviceStore.stats

  return {
    total: stats.total,
    available: stats.available,
    borrowed: stats.borrowed,
    maintenance: stats.maintenance,
    broken: stats.broken,
    percent: {
      available: stats.total ? (stats.available / stats.total) * 100 : 0,
      borrowed: stats.total ? (stats.borrowed / stats.total) * 100 : 0,
      maintenance: stats.total ? (stats.maintenance / stats.total) * 100 : 0,
      broken: stats.total ? (stats.broken / stats.total) * 100 : 0,
    },
  }
})

// --- 2. DỮ LIỆU GIẢ LẬP ---
const chartData = computed(() => {
  return [
    { label: 'T2', borrow: 6, return: 4 },
    { label: 'T3', borrow: 8, return: 5 },
    { label: 'T4', borrow: 5, return: 6 },
    { label: 'T5', borrow: 9, return: 7 },
    { label: 'T6', borrow: 4, return: 3 },
    { label: 'T7', borrow: 7, return: 5 },
    { label: 'CN', borrow: 3, return: 2 },
  ]
})

const recentActivities = ref([
  {
    device: 'MacBook Pro M1',
    user: 'Nguyễn Văn A',
    date: '02/12 08:30',
    type: 'borrow',
    status: 'Đang mượn',
    statusClass: 'warning',
  },
  {
    device: 'Dell XPS 15',
    user: 'Trần Thị B',
    date: '02/12 09:15',
    type: 'return',
    status: 'Hoàn thành',
    statusClass: 'success',
  },
  {
    device: 'iPad Pro 12.9',
    user: 'Lê Văn C',
    date: '01/12 14:20',
    type: 'borrow',
    status: 'Đang mượn',
    statusClass: 'warning',
  },
  {
    device: 'Sony Alpha A7',
    user: 'Phạm Thị D',
    date: '01/12 16:45',
    type: 'borrow',
    status: 'Quá hạn',
    statusClass: 'danger',
  },
])

// --- 3. HÀM REFRESH DỮ LIỆU ---
async function refreshData() {
  if (isRefreshing.value) return // Tránh gọi nhiều lần

  isRefreshing.value = true
  console.log('🔄 Đang refresh dashboard...')

  try {
    // 🔥 RESET toàn bộ allInstances trước khi fetch mới
    deviceStore.allInstances = []
    console.log('🧹 Đã reset allInstances')

    // Tải lại categories
    await deviceStore.fetchCategories()

    // Tải lại models và instances cho từng category
    for (const category of deviceStore.categories) {
      if (deviceStore.fetchModelsByCategory) {
        await deviceStore.fetchModelsByCategory(category.id)
      }
    }

    console.log('✅ Dashboard đã refresh!')
  } catch (e) {
    console.error('❌ Lỗi khi refresh dashboard:', e)
  } finally {
    isRefreshing.value = false
  }
}

// --- 4. MOUNTED ---
onMounted(async () => {
  // Load dữ liệu lần đầu
  console.log('🎯 Dashboard mounted - loading initial data')
  await refreshData()
})

// --- 5. WATCH ROUTE - REFRESH KHI VÀO LẠI DASHBOARD ---
watch(
  () => route.path,
  (newPath) => {
    if (newPath === '/dashboard' || newPath === '/') {
      console.log('🔄 Quay lại Dashboard - refreshing data')
      refreshData()
    }
  },
)

// --- 6. CLEANUP KHI UNMOUNT ---
onUnmounted(() => {
  console.log('👋 Dashboard unmounted')
})
</script>

<style scoped>
.dashboard {
  padding: 24px;
  background: linear-gradient(135deg, #0f172a 0%, #1e293b 100%);
  min-height: 100vh;
  color: #f1f5f9;
}

.dashboard-container {
  max-width: 1400px;
  margin: 0 auto;
}

.page-header {
  margin-bottom: 32px;
  display: flex;
  justify-content: space-between;
  align-items: center;
}

.page-header h2 {
  font-size: 28px;
  font-weight: 700;
  margin: 0;
  color: #00adb5;
}

/* === NÚT REFRESH === */
.refresh-btn {
  display: flex;
  align-items: center;
  gap: 8px;
  padding: 10px 20px;
  background: linear-gradient(135deg, #00adb5 0%, #007b82 100%);
  border: none;
  border-radius: 8px;
  color: white;
  font-weight: 600;
  cursor: pointer;
  transition: all 0.3s ease;
  box-shadow: 0 2px 8px rgba(0, 173, 181, 0.3);
}

.refresh-btn:hover:not(:disabled) {
  background: linear-gradient(135deg, #00c9d4 0%, #00adb5 100%);
  transform: translateY(-2px);
  box-shadow: 0 4px 12px rgba(0, 173, 181, 0.5);
}

.refresh-btn:disabled {
  opacity: 0.6;
  cursor: not-allowed;
}

.refresh-icon {
  font-size: 18px;
  transition: transform 0.3s ease;
}

.refresh-icon.spinning {
  animation: spin 1s linear infinite;
}

@keyframes spin {
  from {
    transform: rotate(0deg);
  }
  to {
    transform: rotate(360deg);
  }
}

.subtitle {
  color: #94a3b8;
  margin-top: 4px;
  font-size: 14px;
}

/* --- STATS GRID --- */
.stats-grid {
  display: grid;
  grid-template-columns: 1fr;
  gap: 20px;
  margin-bottom: 32px;
}

.stat-card {
  background: rgba(255, 255, 255, 0.03);
  backdrop-filter: blur(10px);
  border-radius: 16px;
  padding: 24px;
  border: 1px solid rgba(255, 255, 255, 0.05);
  transition: transform 0.2s;
}

.stat-card:hover {
  transform: translateY(-4px);
  box-shadow: 0 10px 25px -5px rgba(0, 0, 0, 0.3);
}

/* === CARD OVERVIEW STYLE === */
.card-overview {
  display: flex;
  flex-direction: column;
  gap: 20px;
  background: linear-gradient(135deg, rgba(30, 41, 59, 0.95), rgba(15, 23, 42, 0.95));
  border: 1px solid rgba(0, 173, 181, 0.2);
}

.overview-header {
  display: flex;
  align-items: center;
  gap: 16px;
}

.icon-primary {
  background: rgba(0, 173, 181, 0.15);
  color: #00adb5;
}

.progress-stacked {
  display: flex;
  width: 100%;
  height: 10px;
  background: rgba(255, 255, 255, 0.1);
  border-radius: 6px;
  overflow: hidden;
}

.progress-segment {
  height: 100%;
  transition: width 0.5s ease;
}

.seg-available {
  background: #10b981;
}
.seg-borrowed {
  background: #f59e0b;
}
.seg-maintenance {
  background: #3b82f6;
}
.seg-broken {
  background: #ef4444;
}

.status-grid {
  display: grid;
  grid-template-columns: repeat(4, 1fr);
  gap: 16px;
  margin-top: 4px;
}

.status-item {
  display: flex;
  flex-direction: column;
  align-items: flex-start;
  justify-content: center;
  font-size: 14px;
  padding: 12px 16px;
  background: rgba(255, 255, 255, 0.03);
  border-radius: 10px;
  gap: 6px;
}

.status-item .label {
  color: #94a3b8;
  font-weight: 500;
  display: flex;
  align-items: center;
  gap: 8px;
  margin: 0;
}

.status-item .count {
  color: #f1f5f9;
  font-weight: 700;
  font-size: 20px;
}

.dot {
  width: 8px;
  height: 8px;
  border-radius: 50%;
  display: block;
}
.dot.available {
  background: #10b981;
  box-shadow: 0 0 6px rgba(16, 185, 129, 0.4);
}
.dot.borrowed {
  background: #f59e0b;
  box-shadow: 0 0 6px rgba(245, 158, 11, 0.4);
}
.dot.maintenance {
  background: #3b82f6;
  box-shadow: 0 0 6px rgba(59, 130, 246, 0.4);
}
.dot.broken {
  background: #ef4444;
  box-shadow: 0 0 6px rgba(239, 68, 68, 0.4);
}

.stat-icon {
  width: 56px;
  height: 56px;
  border-radius: 14px;
  display: flex;
  align-items: center;
  justify-content: center;
  flex-shrink: 0;
}

.stat-content {
  flex: 1;
}
.stat-label {
  color: #94a3b8;
  font-size: 13px;
  margin: 0 0 4px 0;
  text-transform: uppercase;
  font-weight: 600;
}
.stat-value {
  font-size: 28px;
  font-weight: 700;
  margin: 0;
  color: #fff;
}

/* === BIỂU ĐỒ === */
.chart-section {
  background: rgba(255, 255, 255, 0.03);
  backdrop-filter: blur(10px);
  border-radius: 20px;
  padding: 24px;
  border: 1px solid rgba(255, 255, 255, 0.05);
  margin-bottom: 32px;
}

.chart-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
  margin-bottom: 24px;
}
.chart-header h3 {
  margin: 0;
  color: #00adb5;
  font-size: 18px;
}
.chart-legend {
  display: flex;
  gap: 16px;
}
.legend-item {
  display: flex;
  align-items: center;
  gap: 8px;
  font-size: 13px;
  color: #cbd5e1;
}
.legend-color {
  width: 12px;
  height: 12px;
  border-radius: 3px;
}
.legend-color.borrow {
  background: #00adb5;
}
.legend-color.return {
  background: #10b981;
}

.chart-container {
  height: 220px;
  padding: 10px 0;
}
.chart-bars {
  display: flex;
  align-items: flex-end;
  justify-content: space-around;
  height: 100%;
}
.bar-group {
  display: flex;
  flex-direction: column;
  align-items: center;
  gap: 8px;
  height: 100%;
  flex: 1;
}
.bars {
  display: flex;
  align-items: flex-end;
  gap: 6px;
  height: 100%;
  width: 100%;
  justify-content: center;
}

.bar {
  width: 24px;
  border-radius: 4px 4px 0 0;
  position: relative;
  display: flex;
  justify-content: center;
  transition: height 0.5s ease;
}
.bar-borrow {
  background: linear-gradient(180deg, #00adb5 0%, #0891b2 100%);
}
.bar-return {
  background: linear-gradient(180deg, #10b981 0%, #059669 100%);
}
.bar-value {
  position: absolute;
  top: -20px;
  font-size: 10px;
  color: #cbd5e1;
  opacity: 0;
  transition: opacity 0.2s;
}
.bar:hover .bar-value {
  opacity: 1;
}
.bar-label {
  font-size: 12px;
  color: #64748b;
  font-weight: 600;
}

/* === BẢNG === */
.table-section {
  background: rgba(255, 255, 255, 0.03);
  border-radius: 20px;
  padding: 24px;
  border: 1px solid rgba(255, 255, 255, 0.05);
}
.table-header {
  margin-bottom: 20px;
}
.table-header h3 {
  margin: 0;
  color: #00adb5;
  font-size: 18px;
}

.table-container {
  overflow-x: auto;
}
.modern-table {
  width: 100%;
  border-collapse: collapse;
}
.modern-table th {
  text-align: left;
  padding: 16px;
  color: #00adb5;
  font-size: 13px;
  font-weight: 700;
  text-transform: uppercase;
  border-bottom: 2px solid rgba(0, 173, 181, 0.2);
}
.modern-table td {
  padding: 16px;
  color: #cbd5e1;
  font-size: 14px;
  border-bottom: 1px solid rgba(255, 255, 255, 0.05);
}

.badge {
  padding: 4px 10px;
  border-radius: 6px;
  font-size: 12px;
  font-weight: 600;
}
.badge-blue {
  background: rgba(0, 173, 181, 0.15);
  color: #00adb5;
}
.badge-green {
  background: rgba(16, 185, 129, 0.15);
  color: #10b981;
}

.device-name {
  font-weight: 600;
  color: #f1f5f9;
}
.borrower-info {
  display: flex;
  align-items: center;
  gap: 10px;
}
.borrower-avatar {
  width: 28px;
  height: 28px;
  border-radius: 6px;
  background: #00adb5;
  display: flex;
  align-items: center;
  justify-content: center;
  color: white;
  font-size: 10px;
  font-weight: 700;
}

.status-badge {
  font-size: 12px;
  font-weight: 600;
}
.status-badge.warning {
  color: #f59e0b;
}
.status-badge.success {
  color: #10b981;
}
.status-badge.danger {
  color: #ef4444;
}

@media (max-width: 768px) {
  .status-grid {
    grid-template-columns: 1fr 1fr;
  }
  .bars {
    gap: 4px;
  }
  .bar {
    width: 16px;
  }
}
</style>
