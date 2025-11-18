<template>
  <div class="requests-page">
    <h1>📩 Lịch sử yêu cầu mượn thiết bị</h1>

    <!-- Tabs -->
    <div class="tabs">
      <button
        v-for="tab in tabs"
        :key="tab"
        @click="activeTab = tab"
        :class="['tab-btn', { active: activeTab === tab }]"
      >
        {{ tab }}
      </button>
    </div>

    <!-- Nội dung tab -->
    <div class="tab-content">
      <!-- TAB: Đang đợi -->
      <div v-if="activeTab === 'Đang đợi'">
        <table class="request-table" v-if="pendingRequests.length">
          <thead>
            <tr>
              <th>Tên thiết bị</th>
              <th>Thời gian</th>
              <th>Nơi sử dụng</th>
              <th>Mục đích</th>
              <th>Thao tác</th>
            </tr>
          </thead>
          <tbody>
            <tr v-for="(req, index) in pendingRequests" :key="index">
              <!-- ❌ Không hiển thị requestId -->
              <td>{{ req.deviceName }}</td>
              <td>Tiết {{ req.startPeriod }} - {{ req.endPeriod }}</td>
              <td>{{ req.location }}</td>
              <td>{{ req.purpose }}</td>
              <td>
                <button class="reject-btn" @click="deletePending(req.requestId)">Xóa</button>
              </td>
            </tr>
          </tbody>
        </table>

        <p v-else>Không có yêu cầu nào đang đợi.</p>
      </div>

      <!-- TAB: Đã chấp nhận -->
      <!-- TAB: Đã chấp nhận -->
      <div v-else-if="activeTab === 'Đã chấp nhận'">
        <table class="request-table" v-if="acceptedRequests.length">
          <thead>
            <tr>
              <th>Tên thiết bị</th>
              <th>Mã thiết bị</th>
              <th>Vị trí lưu trữ</th>
            </tr>
          </thead>
          <tbody>
            <tr v-for="(req, index) in acceptedRequests" :key="index">
              <td>{{ req.deviceName }}</td>
              <td>{{ req.deviceCode }}</td>
              <td>{{ req.storageLocation }}</td>
            </tr>
          </tbody>
        </table>
        <p v-else>Chưa có yêu cầu nào được chấp nhận.</p>
      </div>

      <!-- TAB: Bị từ chối -->
      <!-- TAB: Bị từ chối -->
      <div v-else-if="activeTab === 'Bị từ chối'">
        <table class="request-table" v-if="rejectedRequests.length">
          <thead>
            <tr>
              <th>Tên thiết bị</th>
              <th>Ngày yêu cầu</th>
              <th>Thời gian</th>
              <th>Nơi sử dụng</th>
              <th>Mục đích</th>
            </tr>
          </thead>
          <tbody>
            <tr v-for="(req, index) in rejectedRequests" :key="index">
              <td>{{ req.deviceName }}</td>
              <td>{{ formatDate(req.requestDate) }}</td>
              <td>Tiết {{ req.startPeriod }} - {{ req.endPeriod }}</td>
              <td>{{ req.location }}</td>
              <td>{{ req.purpose }}</td>
            </tr>
          </tbody>
        </table>
        <p v-else>Không có yêu cầu nào bị từ chối.</p>
      </div>
    </div>
  </div>
</template>

<script setup>
import { ref, onMounted, watch } from 'vue'
import { userApi, borrowApi } from '@/config/api.js'

// Tabs
const tabs = ['Đang đợi', 'Đã chấp nhận', 'Bị từ chối']
const activeTab = ref('Đang đợi')

// Data
const pendingRequests = ref([])
const acceptedRequests = ref([])
const rejectedRequests = ref([])

const loading = ref(false)
const error = ref(null)

/* ======================================================
   📌 HÀM LẤY DỮ LIỆU THEO TRẠNG THÁI
====================================================== */
async function fetchRequestsByStatus(status) {
  console.log('▶️ Fetch tab:', status)
  loading.value = true
  error.value = null

  try {
    const userId = localStorage.getItem('userId')

    // ---- TAB Đang đợi ----
    if (status === 'Đang đợi') {
      const data = await userApi.getPendingRequests(userId)
      console.log('📌 Pending API data:', data)
      pendingRequests.value = data.map((item) => ({
        requestId: item.requestId,
        deviceName: item.deviceName,
        startPeriod: item.startPeriod,
        endPeriod: item.endPeriod,
        location: item.usageLocation,
        purpose: item.purpose,
      }))
      return
    }

    // ---- TAB Đã chấp nhận ----
    if (status === 'Đã chấp nhận') {
      const data = await userApi.getApprovedRequests(userId)
      console.log('📌 Approved API data:', data)
      acceptedRequests.value = data.map((item) => ({
        requestId: item.requestId,
        deviceName: item.deviceName,
        deviceCode: item.instanceCode,
        storageLocation: item.storageLocation,
      }))
      return
    }

    // ---- TAB Bị từ chối ----
    if (status === 'Bị từ chối') {
      const data = await userApi.getRejectedRequests(userId)
      console.log('📌 Rejected API data:', data)
      rejectedRequests.value = data.map((item) => ({
        requestId: item.requestId,
        deviceName: item.deviceName,
        startPeriod: item.startPeriod,
        endPeriod: item.endPeriod,
        location: item.usageLocation,
        purpose: item.purpose,
        requestDate: item.requestDate,
      }))
      return
    }
  } catch (err) {
    console.error('❌ Lỗi:', err)
    error.value = 'Không thể tải dữ liệu.'
  } finally {
    loading.value = false
  }
}

/* ======================================================
   🧠 WATCH TAB CHANGE
====================================================== */
watch(activeTab, (tab) => fetchRequestsByStatus(tab))

onMounted(() => fetchRequestsByStatus(activeTab.value))

/* ======================================================
   ❌ HÀM XÓA YÊU CẦU ĐANG ĐỢI
====================================================== */

async function deletePending(requestId) {
  if (!confirm('Bạn có chắc muốn xóa yêu cầu này?')) return

  try {
    await borrowApi.delete(requestId)

    alert('🗑️ Đã xóa yêu cầu!')
    pendingRequests.value = pendingRequests.value.filter((req) => req.requestId !== requestId)
  } catch (error) {
    console.error('❌ Lỗi:', error)
    alert('❌ Không thể xoá yêu cầu!')
  }
}

/* ======================================================
   🪄 CÁC ACTION CŨ GIỮ NGUYÊN
====================================================== */
async function returnDevice(index) {
  const req = acceptedRequests.value[index]
  const result = await borrowApi.return(req.requestId)
  if (result.ok) {
    alert('↩️ Đã trả!')
    fetchRequestsByStatus(activeTab.value)
  }
}

/* ======================================================
   🗓 FORMAT NGÀY
====================================================== */
function formatDate(dateStr) {
  if (!dateStr) return ''
  const d = new Date(dateStr)
  return `${String(d.getDate()).padStart(2, '0')}/${String(d.getMonth() + 1).padStart(
    2,
    '0',
  )}/${d.getFullYear()}`
}
</script>

<style scoped>
.requests-page {
  /* background: #fff; */ /* Bỏ nền trắng */
  padding: 20px 12px; /* Đồng bộ padding */
  border-radius: 12px;
  /* box-shadow: 0 2px 6px rgba(0, 0, 0, 0.1); */ /* Bỏ bóng sáng */
  color: #eeeeee; /* ✅ Chữ chính */
}

/* Thêm style cho H1 */
h1 {
  color: #00adb5; /* ✅ Chữ nhấn */
  text-shadow: 0 0 10px rgba(0, 173, 181, 0.5);
  margin-top: 0;
  margin-bottom: 20px;
  font-size: 24px;
}

/* Tabs */
.tabs {
  display: flex;
  gap: 8px;
  border-bottom: 2px solid rgba(0, 173, 181, 0.3); /* Viền nhấn */
  margin-bottom: 20px;
}

.tab-btn {
  background: none;
  border: none;
  padding: 10px 16px;
  font-weight: 500;
  cursor: pointer;
  border-radius: 8px 8px 0 0;
  transition: all 0.2s ease;
  color: rgba(238, 238, 238, 0.7); /* ✅ Chữ phụ (cho tab không active) */
}

.tab-btn:hover {
  background: rgba(0, 173, 181, 0.1); /* Nền nhấn mờ */
  color: #00adb5; /* ✅ Chữ nhấn */
}

.tab-btn.active {
  background: #00adb5; /* ✅ Nền nhấn */
  color: #222831; /* Chữ tối để tương phản */
  font-weight: 600;
}

/* Nội dung tab */
.tab-content {
  background: #393e46; /* Nền phụ */
  padding: 20px;
  border-radius: 12px;
  border: 1px solid rgba(0, 173, 181, 0.2); /* Viền nhấn mờ */
}

/* Chữ khi không có dữ liệu */
.tab-content p {
  color: rgba(238, 238, 238, 0.7); /* ✅ Chữ phụ */
  text-align: center;
  padding: 20px 0;
}

/* Bảng */
.request-table {
  width: 100%;
  border-collapse: collapse;
  background: #393e46; /* Nền phụ */
}

.request-table th,
.request-table td {
  padding: 10px 12px;
  border-bottom: 1px solid rgba(0, 173, 181, 0.15); /* Viền nhấn mờ */
  text-align: left;
}

.request-table th {
  background: #222831; /* Nền chính (tối nhất) */
  color: #00adb5; /* ✅ Chữ nhấn */
  font-weight: 600;
  text-transform: uppercase;
  font-size: 12px;
}

/* Hover hàng */
.request-table tbody tr:hover {
  background: rgba(0, 173, 181, 0.05);
}

.actions {
  display: flex;
  gap: 8px;
}

/* Nút chung (Sử dụng lại từ file trước, dù ở đây chỉ có nút đỏ) */
.accept-btn,
.reject-btn,
.return-btn {
  border: none;
  padding: 6px 12px;
  border-radius: 6px;
  cursor: pointer;
  font-weight: 500;
  color: white;
  transition: background 0.2s ease;
}

.accept-btn {
  background: #00adb5;
  color: #222831;
}
.accept-btn:hover {
  background: #eeeeee;
}

/* Giữ màu đỏ cho nút Xóa/Từ chối */
.reject-btn {
  background: #ef4444;
}
.reject-btn:hover {
  background: #dc2626;
}

.return-btn {
  background: #00adb5;
  color: #222831;
}
.return-btn:hover {
  background: #eeeeee;
}
</style>
