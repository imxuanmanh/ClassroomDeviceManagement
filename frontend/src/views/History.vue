<template>
  <section class="history">
    <div class="history-container">
      <header class="page-header">
        <div>
          <h2>Lịch sử mượn/trả</h2>
          <p class="subtitle">Theo dõi toàn bộ hoạt động mượn trả thiết bị</p>
        </div>
        <div class="header-actions">
          <button class="export-btn" @click="exportData">
            <svg width="18" height="18" viewBox="0 0 24 24" fill="none" stroke="currentColor">
              <path d="M21 15v4a2 2 0 0 1-2 2H5a2 2 0 0 1-2-2v-4" />
              <polyline points="7 10 12 15 17 10" />
              <line x1="12" y1="15" x2="12" y2="3" />
            </svg>
            Xuất dữ liệu
          </button>
        </div>
      </header>

      <!-- Filters -->
      <div class="filters-section">
        <div class="search-box">
          <svg width="20" height="20" viewBox="0 0 24 24" fill="none" stroke="currentColor">
            <circle cx="11" cy="11" r="8" />
            <path d="m21 21-4.35-4.35" />
          </svg>
          <input
            v-model="q"
            type="text"
            placeholder="Tìm theo thiết bị hoặc người mượn..."
            class="search-input"
          />
        </div>

        <div class="filter-group">
          <select v-model="type" class="filter-select">
            <option value="">Tất cả loại</option>
            <option value="borrow">Chỉ mượn</option>
            <option value="return">Chỉ trả</option>
          </select>

          <select class="filter-select">
            <option value="">30 ngày qua</option>
            <option value="7">7 ngày qua</option>
            <option value="90">90 ngày qua</option>
            <option value="365">1 năm qua</option>
          </select>
        </div>
      </div>

      <!-- Stats Summary -->
      <div class="stats-summary">
        <div class="summary-card">
          <div class="summary-icon icon-blue">
            <svg width="20" height="20" viewBox="0 0 24 24" fill="none" stroke="currentColor">
              <rect x="2" y="7" width="20" height="14" rx="2" ry="2" />
              <path d="M16 21V5a2 2 0 0 0-2-2h-4a2 2 0 0 0-2 2v16" />
            </svg>
          </div>
          <div>
            <p class="summary-label">Tổng giao dịch</p>
            <p class="summary-value">{{ records.length }}</p>
          </div>
        </div>

        <div class="summary-card">
          <div class="summary-icon icon-green">
            <svg width="20" height="20" viewBox="0 0 24 24" fill="none" stroke="currentColor">
              <path d="M22 11.08V12a10 10 0 1 1-5.93-9.14" />
              <polyline points="22 4 12 14.01 9 11.01" />
            </svg>
          </div>
          <div>
            <p class="summary-label">Lượt mượn</p>
            <p class="summary-value">{{ borrowCount }}</p>
          </div>
        </div>

        <div class="summary-card">
          <div class="summary-icon icon-purple">
            <svg width="20" height="20" viewBox="0 0 24 24" fill="none" stroke="currentColor">
              <polyline points="9 11 12 14 22 4" />
              <path d="M21 12v7a2 2 0 0 1-2 2H5a2 2 0 0 1-2-2V5a2 2 0 0 1 2-2h11" />
            </svg>
          </div>
          <div>
            <p class="summary-label">Đã trả</p>
            <p class="summary-value">{{ returnCount }}</p>
          </div>
        </div>

        <div class="summary-card">
          <div class="summary-icon icon-orange">
            <svg width="20" height="20" viewBox="0 0 24 24" fill="none" stroke="currentColor">
              <circle cx="12" cy="12" r="10" />
              <polyline points="12 6 12 12 16 14" />
            </svg>
          </div>
          <div>
            <p class="summary-label">Đang mượn</p>
            <p class="summary-value">{{ activeLoans }}</p>
          </div>
        </div>
      </div>

      <!-- Loading State -->
      <div v-if="loading" class="loading-state">
        <div class="spinner"></div>
        <p>Đang tải dữ liệu...</p>
      </div>

      <!-- Error State -->
      <div v-else-if="error" class="error-state">
        <svg width="48" height="48" viewBox="0 0 24 24" fill="none" stroke="currentColor">
          <circle cx="12" cy="12" r="10" />
          <line x1="12" y1="8" x2="12" y2="12" />
          <line x1="12" y1="16" x2="12.01" y2="16" />
        </svg>
        <p>{{ error }}</p>
        <button @click="fetchHistory" class="retry-btn">Thử lại</button>
      </div>

      <!-- Table -->
      <div v-else class="table-section">
        <div class="table-container">
          <table class="modern-table">
            <thead>
              <tr>
                <th>
                  <div class="th-content">
                    <svg
                      width="16"
                      height="16"
                      viewBox="0 0 24 24"
                      fill="none"
                      stroke="currentColor"
                    >
                      <rect x="3" y="4" width="18" height="18" rx="2" ry="2" />
                      <line x1="16" y1="2" x2="16" y2="6" />
                      <line x1="8" y1="2" x2="8" y2="6" />
                      <line x1="3" y1="10" x2="21" y2="10" />
                    </svg>
                    Thời gian
                  </div>
                </th>
                <th>
                  <div class="th-content">
                    <svg
                      width="16"
                      height="16"
                      viewBox="0 0 24 24"
                      fill="none"
                      stroke="currentColor"
                    >
                      <circle cx="12" cy="12" r="10" />
                      <polyline points="12 6 12 12 16 14" />
                    </svg>
                    Loại
                  </div>
                </th>
                <th>
                  <div class="th-content">
                    <svg
                      width="16"
                      height="16"
                      viewBox="0 0 24 24"
                      fill="none"
                      stroke="currentColor"
                    >
                      <rect x="2" y="7" width="20" height="14" rx="2" ry="2" />
                      <path d="M16 21V5a2 2 0 0 0-2-2h-4a2 2 0 0 0-2 2v16" />
                    </svg>
                    Thiết bị
                  </div>
                </th>
                <th>
                  <div class="th-content">
                    <svg
                      width="16"
                      height="16"
                      viewBox="0 0 24 24"
                      fill="none"
                      stroke="currentColor"
                    >
                      <path d="M20 21v-2a4 4 0 0 0-4-4H8a4 4 0 0 0-4 4v2" />
                      <circle cx="12" cy="7" r="4" />
                    </svg>
                    Người mượn
                  </div>
                </th>
                <th>Trạng thái</th>
              </tr>
            </thead>
            <tbody>
              <tr v-for="(r, i) in paginatedRecords" :key="i" class="table-row">
                <td>
                  <div class="date-cell">
                    <span class="date-badge">{{ formatDate(r.date) }}</span>
                  </div>
                </td>
                <td>
                  <span :class="['type-badge', r.type === 'borrow' ? 'badge-blue' : 'badge-green']">
                    <svg
                      width="14"
                      height="14"
                      viewBox="0 0 24 24"
                      fill="none"
                      stroke="currentColor"
                    >
                      <path
                        v-if="r.type === 'borrow'"
                        d="M21 15v4a2 2 0 0 1-2 2H5a2 2 0 0 1-2-2v-4M7 10l5 5 5-5M12 15V3"
                      />
                      <path
                        v-else
                        d="M21 15v4a2 2 0 0 1-2 2H5a2 2 0 0 1-2-2v-4M17 8l-5-5-5 5M12 3v12"
                      />
                    </svg>
                    {{ r.type === 'borrow' ? 'Mượn' : 'Trả' }}
                  </span>
                </td>
                <td>
                  <div class="device-cell">
                    <span class="device-id">#{{ r.deviceId }}</span>
                    <span class="device-name">{{ r.deviceName }}</span>
                  </div>
                </td>
                <td>
                  <div class="borrower-cell">
                    <div class="avatar">{{ getInitials(r.borrowerName) }}</div>
                    <div>
                      <p class="borrower-name">{{ r.borrowerName }}</p>
                      <p class="borrower-id">ID: {{ r.borrowerId }}</p>
                    </div>
                  </div>
                </td>
                <td>
                  <span class="status-badge status-success">
                    <svg
                      width="12"
                      height="12"
                      viewBox="0 0 24 24"
                      fill="none"
                      stroke="currentColor"
                    >
                      <polyline points="20 6 9 17 4 12" />
                    </svg>
                    Hoàn thành
                  </span>
                </td>
              </tr>
            </tbody>
          </table>
        </div>

        <!-- Empty State -->
        <div v-if="filtered.length === 0" class="empty-state">
          <svg width="64" height="64" viewBox="0 0 24 24" fill="none" stroke="currentColor">
            <circle cx="12" cy="12" r="10" />
            <line x1="12" y1="8" x2="12" y2="12" />
            <line x1="12" y1="16" x2="12.01" y2="16" />
          </svg>
          <h3>Không tìm thấy dữ liệu</h3>
          <p>Không có giao dịch nào phù hợp với bộ lọc của bạn</p>
        </div>

        <!-- Pagination -->
        <div v-if="filtered.length > 0" class="pagination">
          <button class="page-btn" :disabled="currentPage === 1" @click="currentPage--">
            <svg width="16" height="16" viewBox="0 0 24 24" fill="none" stroke="currentColor">
              <polyline points="15 18 9 12 15 6" />
            </svg>
            Trước
          </button>

          <div class="page-numbers">
            <button
              v-for="page in totalPages"
              :key="page"
              :class="['page-num', { active: page === currentPage }]"
              @click="currentPage = page"
            >
              {{ page }}
            </button>
          </div>

          <button class="page-btn" :disabled="currentPage === totalPages" @click="currentPage++">
            Sau
            <svg width="16" height="16" viewBox="0 0 24 24" fill="none" stroke="currentColor">
              <polyline points="9 18 15 12 9 6" />
            </svg>
          </button>
        </div>

        <!-- Results Info -->
        <div v-if="filtered.length > 0" class="results-info">
          Hiển thị {{ startIndex + 1 }}-{{ endIndex }} trong tổng số {{ filtered.length }} kết quả
        </div>
      </div>
    </div>
  </section>
</template>

<script setup>
import { computed, ref, onMounted } from 'vue'
import { historyApi } from '@/config/api.js'

const q = ref('')
const type = ref('')
const records = ref([])
const loading = ref(false)
const error = ref('')
const currentPage = ref(1)
const itemsPerPage = 10

async function fetchHistory() {
  loading.value = true
  error.value = ''
  try {
    const data = await historyApi.getAll()
    records.value = Array.isArray(data) ? data : []
  } catch (err) {
    console.error(err)
    error.value = 'Không thể tải lịch sử. Vui lòng thử lại.'
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

const borrowCount = computed(() => records.value.filter((r) => r.type === 'borrow').length)

const returnCount = computed(() => records.value.filter((r) => r.type === 'return').length)

const activeLoans = computed(() => borrowCount.value - returnCount.value)

const totalPages = computed(() => Math.ceil(filtered.value.length / itemsPerPage))

const startIndex = computed(() => (currentPage.value - 1) * itemsPerPage)

const endIndex = computed(() => Math.min(startIndex.value + itemsPerPage, filtered.value.length))

const paginatedRecords = computed(() => filtered.value.slice(startIndex.value, endIndex.value))

function formatDate(dateStr) {
  const date = new Date(dateStr)
  return date.toLocaleDateString('vi-VN', {
    day: '2-digit',
    month: '2-digit',
    year: 'numeric',
    hour: '2-digit',
    minute: '2-digit',
  })
}

function getInitials(name) {
  return name
    .split(' ')
    .map((word) => word[0])
    .join('')
    .toUpperCase()
    .slice(0, 2)
}

function exportData() {
  console.log('Exporting data...')
  // Implement export functionality
}
</script>

<style scoped>
.history {
  min-height: 100vh;
  background: linear-gradient(135deg, #0f172a 0%, #1e293b 100%);
  padding: 24px;
}

.history-container {
  max-width: 1400px;
  margin: 0 auto;
}

.page-header {
  display: flex;
  align-items: center;
  justify-content: space-between;
  margin-bottom: 32px;
}

.page-header h2 {
  margin: 0 0 4px 0;
  color: #00adb5;
  font-size: 32px;
  font-weight: 700;
}

.subtitle {
  margin: 0;
  color: #94a3b8;
  font-size: 14px;
}

.header-actions {
  display: flex;
  gap: 12px;
}

.export-btn {
  display: flex;
  align-items: center;
  gap: 8px;
  padding: 12px 24px;
  background: #00adb5;
  border: none;
  border-radius: 12px;
  color: white;
  font-size: 14px;
  font-weight: 600;
  cursor: pointer;
  transition: all 0.3s ease;
}

.export-btn:hover {
  background: #008c94;
  transform: translateY(-2px);
  box-shadow: 0 4px 12px rgba(0, 173, 181, 0.4);
}

.filters-section {
  display: flex;
  gap: 16px;
  margin-bottom: 24px;
  flex-wrap: wrap;
}

.search-box {
  flex: 1;
  min-width: 300px;
  position: relative;
  display: flex;
  align-items: center;
}

.search-box svg {
  position: absolute;
  left: 16px;
  color: #64748b;
}

.search-input {
  width: 100%;
  padding: 12px 16px 12px 48px;
  background: rgba(255, 255, 255, 0.05);
  border: 1px solid rgba(255, 255, 255, 0.1);
  border-radius: 12px;
  color: #f1f5f9;
  font-size: 14px;
  transition: all 0.3s ease;
}

.search-input::placeholder {
  color: #64748b;
}

.search-input:focus {
  outline: none;
  background: rgba(255, 255, 255, 0.08);
  border-color: #00adb5;
}

.filter-group {
  display: flex;
  gap: 12px;
}

.filter-select {
  padding: 12px 16px;
  background: rgba(255, 255, 255, 0.05);
  border: 1px solid rgba(255, 255, 255, 0.1);
  border-radius: 12px;
  color: #f1f5f9;
  font-size: 14px;
  cursor: pointer;
  transition: all 0.3s ease;
}

.filter-select:hover {
  background: rgba(255, 255, 255, 0.08);
  border-color: #00adb5;
}

.filter-select option {
  background: #1e293b;
  color: #f1f5f9;
}

.stats-summary {
  display: grid;
  grid-template-columns: repeat(auto-fit, minmax(240px, 1fr));
  gap: 16px;
  margin-bottom: 24px;
}

.summary-card {
  display: flex;
  align-items: center;
  gap: 16px;
  padding: 20px;
  background: linear-gradient(135deg, rgba(255, 255, 255, 0.1), rgba(255, 255, 255, 0.05));
  backdrop-filter: blur(10px);
  border-radius: 16px;
  border: 1px solid rgba(255, 255, 255, 0.1);
  transition: all 0.3s ease;
}

.summary-card:hover {
  transform: translateY(-4px);
  box-shadow: 0 8px 24px rgba(0, 0, 0, 0.3);
}

.summary-icon {
  width: 48px;
  height: 48px;
  border-radius: 12px;
  display: flex;
  align-items: center;
  justify-content: center;
}

.icon-blue {
  background: rgba(0, 173, 181, 0.15);
  color: #00adb5;
}

.icon-green {
  background: rgba(16, 185, 129, 0.15);
  color: #10b981;
}

.icon-purple {
  background: rgba(139, 92, 246, 0.15);
  color: #8b5cf6;
}

.icon-orange {
  background: rgba(245, 158, 11, 0.15);
  color: #f59e0b;
}

.summary-label {
  margin: 0 0 4px 0;
  color: #94a3b8;
  font-size: 13px;
  font-weight: 500;
}

.summary-value {
  margin: 0;
  color: #f1f5f9;
  font-size: 24px;
  font-weight: 800;
}

.loading-state,
.error-state {
  display: flex;
  flex-direction: column;
  align-items: center;
  justify-content: center;
  padding: 80px 20px;
  background: linear-gradient(135deg, rgba(255, 255, 255, 0.1), rgba(255, 255, 255, 0.05));
  backdrop-filter: blur(10px);
  border-radius: 20px;
  border: 1px solid rgba(255, 255, 255, 0.1);
  color: #94a3b8;
}

.spinner {
  width: 48px;
  height: 48px;
  border: 4px solid rgba(0, 173, 181, 0.2);
  border-top-color: #00adb5;
  border-radius: 50%;
  animation: spin 1s linear infinite;
  margin-bottom: 16px;
}

@keyframes spin {
  to {
    transform: rotate(360deg);
  }
}

.error-state svg {
  color: #ef4444;
  margin-bottom: 16px;
}

.retry-btn {
  margin-top: 16px;
  padding: 10px 24px;
  background: #00adb5;
  border: none;
  border-radius: 8px;
  color: white;
  font-weight: 600;
  cursor: pointer;
  transition: all 0.3s ease;
}

.retry-btn:hover {
  background: #008c94;
  transform: translateY(-2px);
}

.table-section {
  background: linear-gradient(135deg, rgba(255, 255, 255, 0.1), rgba(255, 255, 255, 0.05));
  backdrop-filter: blur(10px);
  border-radius: 20px;
  padding: 28px;
  border: 1px solid rgba(255, 255, 255, 0.1);
}

.table-container {
  overflow-x: auto;
  margin-bottom: 24px;
}

.modern-table {
  width: 100%;
  border-collapse: collapse;
}

.modern-table thead {
  background: rgba(0, 173, 181, 0.1);
}

.modern-table th {
  padding: 16px;
  text-align: left;
  font-size: 13px;
  font-weight: 700;
  text-transform: uppercase;
  letter-spacing: 0.5px;
  border-bottom: 2px solid rgba(0, 173, 181, 0.3);
}

.th-content {
  display: flex;
  align-items: center;
  gap: 8px;
  color: #00adb5;
}

.modern-table td {
  padding: 18px 16px;
  border-bottom: 1px solid rgba(255, 255, 255, 0.05);
}

.table-row {
  transition: all 0.2s ease;
}

.table-row:hover {
  background: rgba(0, 173, 181, 0.05);
}

.date-cell {
  display: flex;
  align-items: center;
}

.date-badge {
  padding: 6px 12px;
  background: rgba(100, 116, 139, 0.2);
  border-radius: 8px;
  color: #cbd5e1;
  font-size: 13px;
  font-weight: 500;
}

.type-badge {
  display: inline-flex;
  align-items: center;
  gap: 6px;
  padding: 8px 14px;
  border-radius: 10px;
  font-size: 13px;
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

.device-cell {
  display: flex;
  flex-direction: column;
  gap: 4px;
}

.device-id {
  color: #64748b;
  font-size: 12px;
  font-weight: 600;
}

.device-name {
  color: #f1f5f9;
  font-weight: 600;
  font-size: 14px;
}

.borrower-cell {
  display: flex;
  align-items: center;
  gap: 12px;
}

.avatar {
  width: 40px;
  height: 40px;
  border-radius: 10px;
  background: linear-gradient(135deg, #00adb5, #0891b2);
  display: flex;
  align-items: center;
  justify-content: center;
  color: white;
  font-size: 13px;
  font-weight: 700;
}

.borrower-name {
  margin: 0 0 2px 0;
  color: #f1f5f9;
  font-weight: 600;
  font-size: 14px;
}

.borrower-id {
  margin: 0;
  color: #64748b;
  font-size: 12px;
}

.status-badge {
  display: inline-flex;
  align-items: center;
  gap: 6px;
  padding: 8px 14px;
  border-radius: 10px;
  font-size: 13px;
  font-weight: 600;
}

.status-success {
  background: rgba(16, 185, 129, 0.15);
  color: #10b981;
}

.empty-state {
  display: flex;
  flex-direction: column;
  align-items: center;
  justify-content: center;
  padding: 80px 20px;
  color: #64748b;
  text-align: center;
}

.empty-state svg {
  margin-bottom: 20px;
  opacity: 0.5;
}

.empty-state h3 {
  margin: 0 0 8px 0;
  color: #94a3b8;
  font-size: 20px;
  font-weight: 600;
}

.empty-state p {
  margin: 0;
  font-size: 14px;
}

.pagination {
  display: flex;
  align-items: center;
  justify-content: center;
  gap: 8px;
  padding: 20px 0;
  border-top: 1px solid rgba(255, 255, 255, 0.05);
}

.page-btn,
.page-num {
  padding: 10px 16px;
  background: rgba(255, 255, 255, 0.05);
  border: 1px solid rgba(255, 255, 255, 0.1);
  border-radius: 10px;
  color: #94a3b8;
  font-size: 14px;
  font-weight: 600;
  cursor: pointer;
  transition: all 0.2s ease;
  display: flex;
  align-items: center;
  gap: 6px;
}

.page-btn:hover:not(:disabled),
.page-num:hover {
  background: rgba(0, 173, 181, 0.15);
  border-color: #00adb5;
  color: #00adb5;
}

.page-btn:disabled {
  opacity: 0.3;
  cursor: not-allowed;
}

.page-num.active {
  background: #00adb5;
  border-color: #00adb5;
  color: white;
}

.page-numbers {
  display: flex;
  gap: 6px;
}

.results-info {
  text-align: center;
  color: #64748b;
  font-size: 13px;
  margin-top: 12px;
}

/* Responsive */
@media (max-width: 768px) {
  .history {
    padding: 16px;
  }

  .page-header {
    flex-direction: column;
    align-items: flex-start;
    gap: 16px;
  }

  .page-header h2 {
    font-size: 24px;
  }

  .filters-section {
    flex-direction: column;
  }

  .search-box {
    min-width: 100%;
  }

  .filter-group {
    flex-direction: column;
  }

  .filter-select {
    width: 100%;
  }

  .stats-summary {
    grid-template-columns: 1fr;
  }

  .pagination {
    flex-wrap: wrap;
  }

  .table-container {
    font-size: 13px;
  }
}
</style>
