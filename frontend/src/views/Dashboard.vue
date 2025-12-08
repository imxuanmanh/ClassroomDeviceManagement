<template>
  <section class="dashboard">
    <div class="dashboard-container">
      <header class="page-header">
        <div>
          <h2>Tổng quan & Báo cáo</h2>
        </div>
      </header>

      <div class="stats-grid">
        <div class="stat-card card-overview">
          <div class="overview-header">
            <div class="stat-icon icon-primary">
              <svg
                width="24"
                height="24"
                viewBox="0 0 24 24"
                fill="none"
                stroke="currentColor"
              >
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
            <div class="status-item status-available">
              <span class="dot available"></span>
              <span class="label">Khả dụng</span>
              <span class="count">{{ deviceStats.available }}</span>
            </div>
            <div class="status-item status-borrowed">
              <span class="dot borrowed"></span>
              <span class="label">Đang mượn</span>
              <span class="count">{{ deviceStats.borrowed }}</span>
            </div>
            <div class="status-item status-maintenance">
              <span class="dot maintenance"></span>
              <span class="label">Bảo trì</span>
              <span class="count">{{ deviceStats.maintenance }}</span>
            </div>
            <div class="status-item status-broken">
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
                <div
                  class="bar bar-borrow"
                  :style="{ height: day.borrow * 10 + '%' }"
                >
                  <span class="bar-value">{{ day.borrow }}</span>
                </div>
                <div
                  class="bar bar-return"
                  :style="{ height: day.return * 10 + '%' }"
                >
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
              <tr
                v-for="(item, index) in recentActivities"
                :key="index"
                class="table-row"
              >
                <td>{{ item.date }}</td>
                <td>
                  <span
                    :class="[
                      'badge',
                      item.type === 'borrow' ? 'badge-blue' : 'badge-green',
                    ]"
                  >
                    {{ item.type === "borrow" ? "Mượn" : "Trả" }}
                  </span>
                </td>
                <td class="device-name">{{ item.device }}</td>
                <td>
                  <div class="borrower-info">
                    <div class="borrower-avatar">
                      {{ item.user.substring(0, 2).toUpperCase() }}
                    </div>
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
import { ref, computed, onMounted, watch } from "vue";
import { useDeviceStore } from "@/stores/device";
import { useRoute } from "vue-router";

const deviceStore = useDeviceStore();
const isRefreshing = ref(false);
const route = useRoute();

// --- COMPUTED: Lấy stats từ store ---
const deviceStats = computed(() => {
  const stats = deviceStore.stats;

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
  };
});

// --- DỮ LIỆU GIẢ LẬP ---
const chartData = computed(() => {
  return [
    { label: "T2", borrow: 6, return: 4 },
    { label: "T3", borrow: 8, return: 5 },
    { label: "T4", borrow: 5, return: 6 },
    { label: "T5", borrow: 9, return: 7 },
    { label: "T6", borrow: 4, return: 3 },
    { label: "T7", borrow: 7, return: 5 },
    { label: "CN", borrow: 3, return: 2 },
  ];
});

const recentActivities = ref([
  {
    device: "MacBook Pro M1",
    user: "Nguyễn Văn A",
    date: "02/12 08:30",
    type: "borrow",
    status: "Đang mượn",
    statusClass: "warning",
  },
  {
    device: "Dell XPS 15",
    user: "Trần Thị B",
    date: "02/12 09:15",
    type: "return",
    status: "Hoàn thành",
    statusClass: "success",
  },
  {
    device: "iPad Pro 12.9",
    user: "Lê Văn C",
    date: "01/12 14:20",
    type: "borrow",
    status: "Đang mượn",
    statusClass: "warning",
  },
  {
    device: "Sony Alpha A7",
    user: "Phạm Thị D",
    date: "01/12 16:45",
    type: "borrow",
    status: "Quá hạn",
    statusClass: "danger",
  },
]);

// --- 🔥 FIX: HÀM REFRESH ĐÚNG CÁC BƯỚC ---
async function refreshData() {
  if (isRefreshing.value) return;

  isRefreshing.value = true;
  console.log("🔄 Dashboard refreshing...");

  try {
    // 🔥 BƯỚC 1: Fetch categories
    await deviceStore.fetchCategories();
    console.log(`✅ Đã fetch ${deviceStore.categories.length} categories`);

    // 🔥 BƯỚC 2: Fetch models + instances cho TỪNG category
    for (const category of deviceStore.categories) {
      await deviceStore.fetchModelsByCategory(category.id);
    }

    console.log(`✅ Tổng instances: ${deviceStore.allInstances.length}`);
    console.log("📊 Stats hiện tại:", deviceStore.stats);
  } catch (e) {
    console.error("❌ Lỗi refresh dashboard:", e);
  } finally {
    isRefreshing.value = false;
  }
}

// --- MOUNTED ---
onMounted(async () => {
  console.log("🎯 Dashboard mounted");
  await refreshData();
});

// --- WATCH ROUTE để refresh khi vào lại ---
watch(
  () => route.path,
  (newPath) => {
    if (newPath === "/dashboard" || newPath === "/") {
      console.log("🔄 Quay lại Dashboard");
      refreshData();
    }
  }
);
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
  background: linear-gradient(
    135deg,
    rgba(30, 41, 59, 0.95),
    rgba(15, 23, 42, 0.95)
  );
  border: 1px solid rgba(0, 173, 181, 0.2);
}

.overview-header {
  display: flex;
  align-items: center;
  gap: 16px;
}

.total-info {
  flex: 1;
}

.icon-primary {
  background: rgba(0, 173, 181, 0.15);
  color: #00adb5;
  width: 56px;
  height: 56px;
  border-radius: 14px;
  display: flex;
  align-items: center;
  justify-content: center;
  flex-shrink: 0;
}

.stat-label {
  color: #94a3b8;
  font-size: 13px;
  margin: 0 0 4px 0;
  text-transform: uppercase;
  font-weight: 600;
}

.stat-value {
  font-size: 32px;
  font-weight: 700;
  margin: 0;
  color: #fff;
}

/* === STATUS GRID WITH COLORS === */
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
  padding: 16px;
  border-radius: 12px;
  gap: 8px;
  border: 1px solid;
  transition: all 0.3s ease;
  position: relative;
  overflow: hidden;
}

.status-item::before {
  content: "";
  position: absolute;
  top: 0;
  left: 0;
  right: 0;
  height: 3px;
  transition: height 0.3s ease;
}

.status-item:hover {
  transform: translateY(-4px);
  box-shadow: 0 8px 20px rgba(0, 0, 0, 0.4);
}

.status-item:hover::before {
  height: 100%;
  opacity: 0.1;
}

/* === KHẢ DỤNG - GREEN === */
.status-available {
  background: linear-gradient(
    135deg,
    rgba(16, 185, 129, 0.12),
    rgba(16, 185, 129, 0.05)
  );
  border-color: rgba(16, 185, 129, 0.3);
}

.status-available::before {
  background: linear-gradient(135deg, #10b981, #059669);
}

.status-available:hover {
  border-color: rgba(16, 185, 129, 0.5);
}

/* === ĐANG MƯỢN - ORANGE === */
.status-borrowed {
  background: linear-gradient(
    135deg,
    rgba(245, 158, 11, 0.12),
    rgba(245, 158, 11, 0.05)
  );
  border-color: rgba(245, 158, 11, 0.3);
}

.status-borrowed::before {
  background: linear-gradient(135deg, #f59e0b, #d97706);
}

.status-borrowed:hover {
  border-color: rgba(245, 158, 11, 0.5);
}

/* === BẢO TRÌ - BLUE === */
.status-maintenance {
  background: linear-gradient(
    135deg,
    rgba(59, 130, 246, 0.12),
    rgba(59, 130, 246, 0.05)
  );
  border-color: rgba(59, 130, 246, 0.3);
}

.status-maintenance::before {
  background: linear-gradient(135deg, #3b82f6, #2563eb);
}

.status-maintenance:hover {
  border-color: rgba(59, 130, 246, 0.5);
}

/* === HỎNG - RED === */
.status-broken {
  background: linear-gradient(
    135deg,
    rgba(239, 68, 68, 0.12),
    rgba(239, 68, 68, 0.05)
  );
  border-color: rgba(239, 68, 68, 0.3);
}

.status-broken::before {
  background: linear-gradient(135deg, #ef4444, #dc2626);
}

.status-broken:hover {
  border-color: rgba(239, 68, 68, 0.5);
}

.status-item .label {
  color: #94a3b8;
  font-weight: 600;
  display: flex;
  align-items: center;
  gap: 8px;
  margin: 0;
  font-size: 13px;
  text-transform: uppercase;
  letter-spacing: 0.5px;
}

.status-item .count {
  color: #f1f5f9;
  font-weight: 700;
  font-size: 24px;
  margin-top: 4px;
}

.dot {
  width: 10px;
  height: 10px;
  border-radius: 50%;
  display: block;
}

.dot.available {
  background: #10b981;
  box-shadow: 0 0 8px rgba(16, 185, 129, 0.6);
}

.dot.borrowed {
  background: #f59e0b;
  box-shadow: 0 0 8px rgba(245, 158, 11, 0.6);
}

.dot.maintenance {
  background: #3b82f6;
  box-shadow: 0 0 8px rgba(59, 130, 246, 0.6);
}

.dot.broken {
  background: #ef4444;
  box-shadow: 0 0 8px rgba(239, 68, 68, 0.6);
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
