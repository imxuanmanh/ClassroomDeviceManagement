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
      <div v-else-if="activeTab === 'Đã chấp nhận'">
        <table class="request-table" v-if="acceptedRequests.length">
          <thead>
            <tr>
              <th>Tên thiết bị</th>
              <th>Mã thiết bị</th>
              <th>Vị trí lưu trữ</th>
              <th>Hành động</th>
            </tr>
          </thead>
          <tbody>
            <tr v-for="(req, index) in acceptedRequests" :key="index">
              <td>{{ req.deviceName }}</td>
              <td>{{ req.deviceCode }}</td>
              <td>{{ req.storageLocation }}</td>
              <td>
                <div class="actions">
                  <button
                    class="broken-btn"
                    @click="openReportModal(req)"
                    title="Báo thiết bị hỏng"
                  >
                    ⚠️ Báo hỏng
                  </button>
                </div>
              </td>
            </tr>
          </tbody>
        </table>
        <p v-else>Chưa có yêu cầu nào được chấp nhận.</p>
      </div>

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

    <!-- Modal báo hỏng -->
    <ReportBrokenModal
      :is-open="showReportModal"
      :device-data="selectedDevice"
      @close="closeReportModal"
      @submit="handleReportSubmit"
    />
  </div>
</template>

<script setup>
import { ref, onMounted, watch } from 'vue'
import { userApi, borrowApi, reportApi } from '@/config/api.js'
import ReportBrokenModal from '@/components/Device/ReportBrokenModal.vue'

// Tabs
const tabs = ['Đang đợi', 'Đã chấp nhận', 'Bị từ chối']
const activeTab = ref('Đang đợi')

// Data
const pendingRequests = ref([])
const acceptedRequests = ref([])
const rejectedRequests = ref([])

const loading = ref(false)
const error = ref(null)

// Modal state
const showReportModal = ref(false)
const selectedDevice = ref({
  requestId: null,
  deviceName: '',
  deviceCode: '',
})

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
   🪄 XỬ LÝ MODAL BÁO HỎNG
====================================================== */
function openReportModal(device) {
  selectedDevice.value = {
    requestId: device.requestId,
    deviceName: device.deviceName,
    deviceCode: device.deviceCode,
  }
  showReportModal.value = true
}

function closeReportModal() {
  showReportModal.value = false
}

async function handleReportSubmit(data) {
  console.log('📤 Gửi báo cáo:', data)

  try {
    // Lấy userId từ localStorage
    const userId = localStorage.getItem('userId')
    if (!userId) {
      alert('❌ Không tìm thấy thông tin người dùng!')
      return
    }

    // Tạo FormData để gửi cả text và file
    const formData = new FormData()
    formData.append('UserId', userId)
    formData.append('InstanceId', data.requestId) // requestId chính là instanceId
    formData.append('Description', data.description)
    formData.append('image', data.image) // Tên field phải là "image" theo API

    // Gọi API thông qua reportApi
    const result = await reportApi.createBrokenReport(formData)

    if (result.ok) {
      alert('✅ ' + result.message)
      fetchRequestsByStatus(activeTab.value)
    } else {
      alert('❌ ' + result.message)
    }
  } catch (error) {
    console.error('❌ Lỗi:', error)
    alert('❌ Không thể gửi báo cáo thiết bị hỏng! Vui lòng kiểm tra kết nối.')
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
  padding: 20px 12px;
  border-radius: 12px;
  color: #eeeeee;
}

/* Thêm style cho H1 */
h1 {
  color: #00adb5;
  text-shadow: 0 0 10px rgba(0, 173, 181, 0.5);
  margin-top: 0;
  margin-bottom: 20px;
  font-size: 24px;
}

/* Tabs */
.tabs {
  display: flex;
  gap: 8px;
  border-bottom: 2px solid rgba(0, 173, 181, 0.3);
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
  color: rgba(238, 238, 238, 0.7);
}

.tab-btn:hover {
  background: rgba(0, 173, 181, 0.1);
  color: #00adb5;
}

.tab-btn.active {
  background: #00adb5;
  color: #222831;
  font-weight: 600;
}

/* Nội dung tab */
.tab-content {
  background: #393e46;
  padding: 20px;
  border-radius: 12px;
  border: 1px solid rgba(0, 173, 181, 0.2);
}

/* Chữ khi không có dữ liệu */
.tab-content p {
  color: rgba(238, 238, 238, 0.7);
  text-align: center;
  padding: 20px 0;
}

/* Bảng */
.request-table {
  width: 100%;
  border-collapse: collapse;
  background: #393e46;
}

.request-table th,
.request-table td {
  padding: 10px 12px;
  border-bottom: 1px solid rgba(0, 173, 181, 0.15);
}

.request-table th {
  background: #222831;
  color: #00adb5;
  font-weight: 600;
  text-transform: uppercase;
  font-size: 12px;
  text-align: center;
}

.request-table th:first-child {
  text-align: left;
}

.request-table td {
  text-align: center;
}

.request-table td:first-child {
  text-align: left;
}

/* Hover hàng */
.request-table tbody tr:hover {
  background: rgba(0, 173, 181, 0.05);
}

.actions {
  display: flex;
  gap: 10px;
  justify-content: center;
  align-items: center;
}

/* ✨ NÚT MỚI - ĐÃ CẬP NHẬT CSS */
.accept-btn,
.reject-btn,
.return-btn,
.broken-btn {
  border: none;
  padding: 8px 16px;
  border-radius: 6px;
  cursor: pointer;
  font-weight: 500;
  font-size: 14px;
  transition: all 0.3s ease;
  box-shadow: 0 2px 4px rgba(0, 0, 0, 0.2);
  min-width: 120px;
  text-align: center;
}

.accept-btn {
  background: #00adb5;
  color: #222831;
}
.accept-btn:hover {
  background: #eeeeee;
  transform: translateY(-2px);
  box-shadow: 0 4px 8px rgba(0, 173, 181, 0.3);
}

/* Nút Xóa/Từ chối */
.reject-btn {
  background: #ef4444;
  color: white;
}
.reject-btn:hover {
  background: #dc2626;
  transform: translateY(-2px);
  box-shadow: 0 4px 8px rgba(239, 68, 68, 0.3);
}

/* Nút Trả lại - với gradient xanh cyan đẹp */
.return-btn {
  background: linear-gradient(135deg, #00adb5 0%, #009fa7 100%);
  color: #222831;
  font-weight: 600;
}
.return-btn:hover {
  background: linear-gradient(135deg, #009fa7 0%, #008a91 100%);
  transform: translateY(-2px);
  box-shadow: 0 4px 12px rgba(0, 173, 181, 0.4);
}

/* Nút Báo hỏng - với gradient cam nổi bật */
.broken-btn {
  background: linear-gradient(135deg, #f59e0b 0%, #d97706 100%);
  color: #222831;
  font-weight: 600;
}
.broken-btn:hover {
  background: linear-gradient(135deg, #d97706 0%, #b45309 100%);
  transform: translateY(-2px);
  box-shadow: 0 4px 12px rgba(245, 158, 11, 0.4);
}
</style>
