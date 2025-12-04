<template>
  <div class="requests-page">
    <div class="page-header">
      <h1>📩 Danh sách yêu cầu mượn thiết bị</h1>
    </div>

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

    <div class="tab-content">
      <div v-if="activeTab === 'Đang đợi'">
        <table class="request-table" v-if="pendingRequests.length">
          <thead>
            <tr>
              <th>Người mượn</th>
              <th>Tên thiết bị</th>
              <th>Mã thiết bị</th>
              <th>Ngày yêu cầu</th>
              <th>Thời gian</th>
              <th>Nơi sử dụng</th>
              <th>Mục đích</th>
              <th>Thao tác</th>
            </tr>
          </thead>
          <tbody>
            <tr v-for="(req, index) in pendingRequests" :key="index">
              <td>{{ req.user }}</td>
              <td>{{ req.deviceName }}</td>
              <td>{{ req.deviceCode }}</td>
              <td>{{ formatDate(req.requestDate) }}</td>
              <td>Tiết {{ req.startPeriod }} - {{ req.endPeriod }}</td>
              <td>{{ req.location }}</td>
              <td>{{ req.purpose }}</td>
              <td class="actions">
                <button class="accept-btn" @click="acceptRequest(req)">Chấp nhận</button>
                <button class="reject-btn" @click="rejectRequest(req)">Từ chối</button>
              </td>
            </tr>
          </tbody>
        </table>
        <p v-else>Không có yêu cầu nào đang đợi.</p>
      </div>

      <div v-else-if="activeTab === 'Đã chấp nhận'">
        <table class="request-table" v-if="acceptedRequests.length">
          <thead>
            <tr>
              <th>Người mượn</th>
              <th>Tên thiết bị</th>
              <th>Mã thiết bị</th>
              <th>Ngày yêu cầu</th>
              <th>Ngày chấp nhận</th>
              <th>Thời gian</th>
              <th>Thao tác</th>
            </tr>
          </thead>
          <tbody>
            <tr v-for="(req, index) in acceptedRequests" :key="index">
              <td>{{ req.user }}</td>
              <td>{{ req.deviceName }}</td>
              <td>{{ req.deviceCode }}</td>
              <td>{{ formatDate(req.requestDate) }}</td>
              <td>{{ formatDate(req.acceptedDate) }}</td>
              <td>Tiết {{ req.startPeriod }} - {{ req.endPeriod }}</td>
              <td class="actions">
                <button class="return-btn" @click="returnDevice(req)">Trả lại</button>
              </td>
            </tr>
          </tbody>
        </table>
        <p v-else>Chưa có yêu cầu nào được chấp nhận.</p>
      </div>

      <div v-else-if="activeTab === 'Đã từ chối'">
        <table class="request-table" v-if="rejectedRequests.length">
          <thead>
            <tr>
              <th>Người mượn</th>
              <th>Tên thiết bị</th>
              <th>Mã thiết bị</th>
              <th>Ngày yêu cầu</th>
            </tr>
          </thead>
          <tbody>
            <tr v-for="(req, index) in rejectedRequests" :key="index">
              <td>{{ req.user }}</td>
              <td>{{ req.deviceName }}</td>
              <td>{{ req.deviceCode }}</td>
              <td>{{ formatDate(req.requestDate) }}</td>
            </tr>
          </tbody>
        </table>
        <p v-else>Không có yêu cầu nào bị từ chối.</p>
      </div>

      <div v-else-if="activeTab === 'Đã trả lại'">
        <table class="request-table" v-if="returnedRequests.length">
          <thead>
            <tr>
              <th>Người mượn</th>
              <th>Tên thiết bị</th>
              <th>Mã thiết bị</th>
              <th>Ngày yêu cầu</th>
              <th>Ngày chấp nhận</th>
              <th>Ngày trả</th>
              <th>Nơi sử dụng</th>
              <th>Mục đích</th>
            </tr>
          </thead>
          <tbody>
            <tr v-for="(req, index) in returnedRequests" :key="index">
              <td>{{ req.user }}</td>
              <td>{{ req.deviceName }}</td>
              <td>{{ req.deviceCode }}</td>
              <td>{{ formatDate(req.requestDate) }}</td>
              <td>{{ formatDate(req.acceptedDate) }}</td>
              <td>{{ formatDate(req.returnDate) }}</td>
              <td>{{ req.location }}</td>
              <td>{{ req.purpose }}</td>
            </tr>
          </tbody>
        </table>
        <p v-else>Chưa có thiết bị nào được trả lại.</p>
      </div>
    </div>
  </div>
</template>

<script setup>
import { ref, onMounted, watch } from 'vue'
import { borrowApi } from '@/config/api.js'
// 👇 IMPORT TOAST
import { toast } from '@/utils/toast.js'

// Tabs
const tabs = ['Đang đợi', 'Đã chấp nhận', 'Đã từ chối', 'Đã trả lại']
const activeTab = ref('Đang đợi')

// Dữ liệu theo trạng thái
const pendingRequests = ref([])
const acceptedRequests = ref([])
const rejectedRequests = ref([])
const returnedRequests = ref([])

// Trạng thái tải
const loading = ref(false)

/* =========================
   📦 HÀM GỌI API THEO TRẠNG THÁI
========================= */
async function fetchRequestsByStatus(status) {
  loading.value = true

  try {
    const mapStatus = {
      'Đang đợi': 'pending',
      'Đã chấp nhận': 'approved',
      'Đã từ chối': 'rejected',
      'Đã trả lại': 'returned',
    }

    const apiStatus = mapStatus[status]
    const data = await borrowApi.getByStatus(apiStatus)

    const mapped = data.map((item) => ({
      requestId: item.requestId,
      user: item.borrower || 'Không rõ',
      deviceName: item.deviceName,
      deviceCode: item.instanceCode,
      requestDate: item.requestDate,
      startPeriod: item.startPeriod,
      endPeriod: item.endPeriod,
      acceptedDate: item.approvedDate,
      returnDate: item.returnDate,
      location: item.location || item.usageLocation || '—',
      purpose: item.purpose || '—',
      status: item.status || status,
    }))

    if (status === 'Đang đợi') pendingRequests.value = mapped
    if (status === 'Đã chấp nhận') acceptedRequests.value = mapped
    if (status === 'Đã từ chối') rejectedRequests.value = mapped
    if (status === 'Đã trả lại') returnedRequests.value = mapped
  } catch (err) {
    console.error(`❌ Lỗi tải dữ liệu:`, err)
    // 🔥 Báo lỗi nhẹ nhàng bằng toast thay vì console đỏ lòm
    toast.error('Không thể tải dữ liệu yêu cầu.')
  } finally {
    loading.value = false
  }
}

watch(activeTab, (tab) => {
  fetchRequestsByStatus(tab)
})

onMounted(() => {
  fetchRequestsByStatus(activeTab.value)
})

/* =========================
   ⚙️ CÁC HÀNH ĐỘNG (Đã nâng cấp Toast & Confirm)
========================= */

// 1. CHẤP NHẬN YÊU CẦU
async function acceptRequest(req) {
  try {
    const result = await borrowApi.approveRequest(req.requestId)
    if (result.ok) {
      toast.success('Đã chấp nhận yêu cầu mượn thiết bị!')
      fetchRequestsByStatus(activeTab.value)
    } else {
      toast.error(`Lỗi: ${result.message}`)
    }
  } catch (error) {
    toast.error('Lỗi kết nối đến máy chủ.')
  }
}

// 2. TỪ CHỐI YÊU CẦU (Có xác nhận)
async function rejectRequest(req) {
  // 🔥 Hộp thoại xác nhận trước khi từ chối
  const confirmed = await toast.confirm(
    'Từ chối yêu cầu?',
    'Bạn có chắc chắn muốn từ chối yêu cầu này không?',
    'Từ chối',
  )

  if (!confirmed) return

  try {
    const result = await borrowApi.rejectRequest(req.requestId)
    if (result.ok) {
      toast.success('Đã từ chối yêu cầu.')
      fetchRequestsByStatus(activeTab.value)
    } else {
      toast.error(`Lỗi: ${result.message}`)
    }
  } catch (error) {
    toast.error('Lỗi kết nối đến máy chủ.')
  }
}

// 3. TRẢ THIẾT BỊ (Có xác nhận)
async function returnDevice(req) {
  // 🔥 Hộp thoại xác nhận trả
  const confirmed = await toast.confirm(
    'Xác nhận trả thiết bị?',
    'Xác nhận thiết bị đã được kiểm tra và trả về kho.',
    'Xác nhận trả',
  )

  if (!confirmed) return

  try {
    const result = await borrowApi.return(req.requestId)
    if (result.ok) {
      toast.success('Đã cập nhật trạng thái trả thiết bị!')
      fetchRequestsByStatus(activeTab.value)
    } else {
      toast.error(`Lỗi: ${result.message}`)
    }
  } catch (error) {
    toast.error('Lỗi kết nối đến máy chủ.')
  }
}

/* =========================
   🗓️ FORMAT NGÀY
========================= */
function formatDate(dateStr) {
  if (!dateStr) return ''
  const date = new Date(dateStr)
  return `${String(date.getDate()).padStart(2, '0')}/${String(date.getMonth() + 1).padStart(
    2,
    '0',
  )}/${date.getFullYear()}`
}
</script>

<style scoped>
.requests-page {
  padding: 24px;
  background: linear-gradient(135deg, #0f172a 0%, #1e293b 100%);
  min-height: 100vh;
  color: #eeeeee;
}

.page-header h1 {
  font-size: 28px;
  font-weight: 700;
  color: #eeeeee;
  margin: 0 0 24px 0;
}

/* Tabs */
.tabs {
  display: flex;
  gap: 8px;
  border-bottom: 2px solid rgba(238, 238, 238, 0.1);
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
  color: rgba(238, 238, 238, 0.6);
  font-size: 14px;
}

.tab-btn:hover {
  background: rgba(0, 173, 181, 0.1);
  color: #00adb5;
}

.tab-btn.active {
  background: rgba(0, 173, 181, 0.1);
  color: #00adb5;
  border-bottom: 2px solid #00adb5;
  margin-bottom: -2px;
}

.tab-content {
  background: #393e46;
  padding: 20px;
  border-radius: 12px;
  border: 1px solid rgba(0, 173, 181, 0.2);
  box-shadow: 0 4px 6px -1px rgba(0, 0, 0, 0.1);
}

.tab-content p {
  color: rgba(238, 238, 238, 0.6);
  text-align: center;
  padding: 40px 0;
  font-style: italic;
}

/* Bảng */
.request-table {
  width: 100%;
  border-collapse: collapse;
}

.request-table th,
.request-table td {
  padding: 14px 16px;
  border-bottom: 1px solid rgba(238, 238, 238, 0.1);
  text-align: left;
}

.request-table th {
  background: #222831;
  color: #00adb5;
  font-weight: 600;
  text-transform: uppercase;
  font-size: 12px;
  letter-spacing: 0.5px;
}

.request-table tbody tr:hover {
  background-color: rgba(255, 255, 255, 0.02);
}

.actions {
  display: flex;
  gap: 8px;
}

/* Nút chung */
.accept-btn,
.reject-btn,
.return-btn {
  border: none;
  padding: 6px 12px;
  border-radius: 6px;
  cursor: pointer;
  font-weight: 500;
  color: white;
  font-size: 13px;
  transition: all 0.2s ease;
}

.accept-btn {
  background: rgba(0, 173, 181, 0.2);
  color: #00adb5;
  border: 1px solid rgba(0, 173, 181, 0.3);
}

.accept-btn:hover {
  background: rgba(0, 173, 181, 0.3);
}

.reject-btn {
  background: rgba(239, 68, 68, 0.2);
  color: #ef4444;
  border: 1px solid rgba(239, 68, 68, 0.3);
}

.reject-btn:hover {
  background: rgba(239, 68, 68, 0.3);
}

.return-btn {
  background: rgba(16, 185, 129, 0.2); /* Màu xanh lá */
  color: #10b981;
  border: 1px solid rgba(16, 185, 129, 0.3);
}

.return-btn:hover {
  background: rgba(16, 185, 129, 0.3);
}
</style>
