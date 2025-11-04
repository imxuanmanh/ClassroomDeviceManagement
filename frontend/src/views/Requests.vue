<template>
  <div class="requests-page">
    <h1>📩 Danh sách yêu cầu mượn thiết bị</h1>

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
              <th>Người mượn</th>
              <th>Tên thiết bị</th>
              <th>Mã thiết bị</th>
              <th>Ngày yêu cầu</th>
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
              <td>{{ req.location }}</td>
              <td>{{ req.purpose }}</td>
              <td class="actions">
                <button class="accept-btn" @click="acceptRequest(index)">Chấp nhận</button>
                <button class="reject-btn" @click="rejectRequest(index)">Từ chối</button>
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
              <th>Người mượn</th>
              <th>Tên thiết bị</th>
              <th>Mã thiết bị</th>
              <th>Ngày yêu cầu</th>
              <th>Ngày chấp nhận</th>
              <!-- ❌ Bỏ 2 cột này: Nơi sử dụng, Mục đích -->
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
              <!-- ❌ Không hiển thị req.location và req.purpose -->
              <td class="actions">
                <button class="return-btn" @click="returnDevice(index)">Trả lại</button>
              </td>
            </tr>
          </tbody>
        </table>
        <p v-else>Chưa có yêu cầu nào được chấp nhận.</p>
      </div>

      <!-- TAB: Đã từ chối -->
      <div v-else-if="activeTab === 'Đã từ chối'">
        <table class="request-table" v-if="rejectedRequests.length">
          <thead>
            <tr>
              <th>Người mượn</th>
              <th>Tên thiết bị</th>
              <th>Mã thiết bị</th>
              <th>Ngày yêu cầu</th>
              <!-- ❌ Bỏ Nơi sử dụng và Mục đích -->
            </tr>
          </thead>
          <tbody>
            <tr v-for="(req, index) in rejectedRequests" :key="index">
              <td>{{ req.user }}</td>
              <td>{{ req.deviceName }}</td>
              <td>{{ req.deviceCode }}</td>
              <td>{{ formatDate(req.requestDate) }}</td>
              <!-- ❌ Không hiển thị req.location và req.purpose -->
            </tr>
          </tbody>
        </table>
        <p v-else>Không có yêu cầu nào bị từ chối.</p>
      </div>

      <!-- TAB: Đã trả lại -->
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
const error = ref(null)

/* =========================
   📦 HÀM GỌI API THEO TRẠNG THÁI
========================= */
async function fetchRequestsByStatus(status) {
  loading.value = true
  error.value = null

  try {
    // Map tên tab → endpoint backend
    const mapStatus = {
      'Đang đợi': 'pending',
      'Đã chấp nhận': 'approved',
      'Đã từ chối': 'rejected',
      'Đã trả lại': 'returned',
    }

    const apiStatus = mapStatus[status]
    const data = await borrowApi.getByStatus(apiStatus)
    console.log(`📦 Dữ liệu [${status}]:`, data)

    const mapped = data.map((item) => ({
      requestId: item.requestId,
      user: item.borrower || 'Không rõ',
      deviceName: item.deviceName,
      deviceCode: item.instanceCode,
      requestDate: item.requestDate,
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
    console.error(`❌ Lỗi khi tải dữ liệu [${status}]:`, err)
    error.value = `Không thể tải dữ liệu trạng thái "${status}".`
  } finally {
    loading.value = false
  }
}

/* =========================
   🧠 CHUYỂN TAB TỰ ĐỘNG GỌI API
========================= */
watch(activeTab, (tab) => {
  fetchRequestsByStatus(tab)
})

// Lần đầu mở trang → tải tab đầu tiên
onMounted(() => {
  fetchRequestsByStatus(activeTab.value)
})

/* =========================
   ⚙️ CÁC HÀNH ĐỘNG: DUYỆT / TỪ CHỐI / TRẢ
========================= */
async function acceptRequest(index) {
  const req = pendingRequests.value[index]
  try {
    const result = await borrowApi.approveRequest(req.requestId)
    if (result.ok) {
      alert(`✅ Thành công (${result.status}): ${result.message}`)
      fetchRequestsByStatus(activeTab.value)
    } else {
      alert(`❌ Lỗi (${result.status}): ${result.message}`)
    }
  } catch (error) {
    alert('❌ Lỗi kết nối đến máy chủ.')
  }
}

async function rejectRequest(index) {
  const req = pendingRequests.value[index]
  try {
    const result = await borrowApi.rejectRequest(req.requestId)
    if (result.ok) {
      alert(`🚫 Từ chối (${result.status}): ${result.message}`)
      fetchRequestsByStatus(activeTab.value)
    } else {
      alert(`❌ Lỗi (${result.status}): ${result.message}`)
    }
  } catch (error) {
    alert('❌ Lỗi kết nối đến máy chủ.')
  }
}

async function returnDevice(index) {
  const req = acceptedRequests.value[index]
  try {
    const result = await borrowApi.return(req.requestId)
    if (result.ok) {
      alert(`↩️ Đã trả (${result.status}): ${result.message}`)
      fetchRequestsByStatus(activeTab.value)
    } else {
      alert(`❌ Lỗi (${result.status}): ${result.message}`)
    }
  } catch (error) {
    alert('❌ Lỗi kết nối đến máy chủ.')
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

<!-- <script setup>
import { ref, onMounted } from 'vue'
import { borrowApi } from '@/config/api.js' // 👈 đường dẫn đúng tới file bạn gửi

// Tabs
const tabs = ['Đang đợi', 'Đã chấp nhận', 'Đã từ chối', 'Đã trả lại']
const activeTab = ref('Đang đợi')

// Dữ liệu
const pendingRequests = ref([])
const acceptedRequests = ref([])
const rejectedRequests = ref([])
const returnedRequests = ref([])

// Hàm gọi API

async function fetchPendingRequests() {
  try {
    const data = await borrowApi.getAll()
    console.log('📦 Dữ liệu từ API:', data)

    // Dữ liệu API đã là mảng
    pendingRequests.value = data.map((item) => ({
      requestId: item.requestId,
      user: item.borrower || 'Không rõ',
      deviceName: item.deviceName,
      deviceCode: item.instanceCode,
      requestDate: new Date(item.requestDate).toLocaleString(),
      location: item.location || item.usageLocation || '—',
      purpose: item.purpose || '—',
      status: item.status || 'Pending',
    }))
  } catch (error) {
    console.error('❌ Lỗi khi tải danh sách yêu cầu:', error)
  }
}

onMounted(() => {
  fetchPendingRequests()
})

// 👉 Hàm format ngày dd/mm/yyyy
function formatDate(dateStr) {
  if (!dateStr) return ''
  const date = new Date(dateStr)
  const day = String(date.getDate()).padStart(2, '0')
  const month = String(date.getMonth() + 1).padStart(2, '0')
  const year = date.getFullYear()
  return `${day}/${month}/${year}`
}

// Xử lý yêu cầu
async function acceptRequest(index) {
  const req = pendingRequests.value[index]
  try {
    const result = await borrowApi.approveRequest(req.requestId)

    if (result.ok) {
      alert(`✅ Thành công (${result.status}): ${result.message}`)
      const accepted = { ...req, acceptedDate: new Date().toISOString() }
      acceptedRequests.value.push(accepted)
      pendingRequests.value.splice(index, 1)
    } else {
      alert(`❌ Lỗi (${result.status}): ${result.message}`)
    }
  } catch (error) {
    console.error('❌ Lỗi khi chấp nhận yêu cầu:', error)
    alert('❌ Đã xảy ra lỗi kết nối đến máy chủ.')
  }
}

async function rejectRequest(index) {
  const req = pendingRequests.value[index]
  try {
    await borrowApi.rejectRequest(req.requestId)
    console.log(`❌ Đã từ chối yêu cầu ID ${req.requestId}`)
    rejectedRequests.value.push(req)
    pendingRequests.value.splice(index, 1)
  } catch (error) {
    console.error('Lỗi khi từ chối yêu cầu:', error)
    alert('Không thể từ chối yêu cầu.')
  }
}

function returnDevice(index) {
  const req = acceptedRequests.value[index]
  const returned = { ...req, returnDate: new Date().toISOString() }
  console.log('↩️ Thiết bị đã được trả lại:', returned)
  returnedRequests.value.push(returned)
  acceptedRequests.value.splice(index, 1)
}
</script> -->

<style scoped>
.requests-page {
  background: #fff;
  padding: 20px;
  border-radius: 12px;
  box-shadow: 0 2px 6px rgba(0, 0, 0, 0.1);
}

/* Tabs */
.tabs {
  display: flex;
  gap: 8px;
  border-bottom: 2px solid #ddd;
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
  color: #444;
}

.tab-btn:hover {
  background: #e7f3f5;
  color: #000;
}

.tab-btn.active {
  background: #417c85;
  color: white;
  font-weight: 600;
}

/* Nội dung tab */
.tab-content {
  background: #f9fafb;
  padding: 20px;
  border-radius: 8px;
}

/* Bảng */
.request-table {
  width: 100%;
  border-collapse: collapse;
  background: white;
}

.request-table th,
.request-table td {
  padding: 10px 12px;
  border-bottom: 1px solid #e5e7eb;
  text-align: left;
}

.request-table th {
  background: #417c85;
  color: white;
  font-weight: 600;
}

.actions {
  display: flex;
  gap: 8px;
}

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
  background: #16a34a;
}

.accept-btn:hover {
  background: #15803d;
}

.reject-btn {
  background: #ef4444;
}

.reject-btn:hover {
  background: #dc2626;
}

.return-btn {
  background: #2563eb;
}

.return-btn:hover {
  background: #1d4ed8;
}
</style>
