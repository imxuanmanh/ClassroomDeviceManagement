<template>
  <section class="device">
    <!-- Header -->
    <header class="page-header">
      <h2>Danh Sách Thiết bị</h2>
      <div class="actions">
        <input v-model="q" placeholder="Tìm theo tên/loại" />
        <button v-if="isAdmin" @click="openCreate">Thêm</button>
      </div>
    </header>

    <div class="content">
      <!-- Modal thêm/sửa thiết bị -->
      <DeviceModal
        v-if="showForm"
        :value="form"
        :title="editingIndex !== null ? 'Cập nhật thiết bị' : 'Thêm thiết bị mới'"
        :submit-text="editingIndex !== null ? 'Cập nhật' : 'Thêm'"
        @submit="save"
        @close="closeForm"
      />

      <!-- Modal mượn thiết bị -->
      <div v-if="showBorrowForm" class="borrow-overlay">
        <div class="borrow-form">
          <h3>Mượn thiết bị</h3>
          <p><strong>Model:</strong> {{ selectedModel?.modelName }}</p>

          <div class="field">
            <label>Vị trí sử dụng</label>
            <input v-model="usageLocation" placeholder="Nhập vị trí sử dụng" />
          </div>

          <div class="field">
            <label>Mục đích</label>
            <input v-model="usagePurpose" placeholder="Nhập mục đích sử dụng" />
          </div>

          <!-- Thanh trượt chọn tiết -->
          <div class="field">
            <label>Chọn tiết sử dụng</label>

            <div class="period-slider-container">
              <div class="labels">
                <span
                  v-for="(period, index) in validPeriods"
                  :key="period"
                  class="period-label"
                  :style="{ left: `calc(${(index / (validPeriods.length - 1)) * 100}% + 7px)` }"
                >
                  {{ period }}
                </span>
              </div>

              <div class="slider-wrap">
                <div class="slider-track-bg"></div>
                <div
                  class="range-highlight"
                  :style="{
                    left: getPeriodPosition(startPeriodIndex) + '%',
                    width:
                      getPeriodPosition(endPeriodIndex) - getPeriodPosition(startPeriodIndex) + '%',
                  }"
                ></div>

                <input
                  type="range"
                  min="0"
                  :max="validPeriods.length - 1"
                  v-model.number="startPeriodIndex"
                  @input="fixPeriod(true)"
                  class="slider slider-start"
                />

                <input
                  type="range"
                  min="0"
                  :max="validPeriods.length - 1"
                  v-model.number="endPeriodIndex"
                  @input="fixPeriod(false)"
                  class="slider slider-end"
                />
              </div>

              <div class="result">
                Tiết bắt đầu: {{ validPeriods[startPeriodIndex] }} – Tiết kết thúc:
                {{ validPeriods[endPeriodIndex] }}
              </div>
            </div>
          </div>

          <div class="actions">
            <button class="cancel-btn" @click="closeBorrowForm">Hủy</button>
            <button class="submit-btn" @click="confirmBorrow">Xác nhận</button>
          </div>
        </div>
      </div>

      <!-- Danh sách category -->
      <div v-if="!selectedCategory" class="categories">
        <div
          v-for="category in categories"
          :key="category.id"
          class="category-card"
          @click="openCategory(category)"
        >
          <h3>{{ category.name }}</h3>
        </div>

        <div v-if="categories.length === 0" class="empty">Không có dữ liệu</div>
      </div>

      <!-- Chi tiết model -->
      <div v-else class="models-view">
        <div class="models-header">
          <button class="back-btn" @click="backToCategories">← Quay lại</button>
          <h3>{{ selectedCategory.name }}</h3>
        </div>

        <div class="models-table">
          <table>
            <thead>
              <tr>
                <th>ID</th>
                <th>Tên model</th>
                <th>Thông số kỹ thuật</th>
                <th>Vị trí lưu trữ</th>
                <th>Tổng</th>
                <th>Khả dụng</th>
                <th>Hành động</th>
              </tr>
            </thead>
            <tbody>
              <tr v-for="m in modelsByCategory[selectedCategory.id] || []" :key="m.modelId">
                <td>{{ m.modelId }}</td>
                <td>{{ m.modelName }}</td>
                <td>{{ m.specifications }}</td>
                <td>{{ m.storageLocation }}</td>
                <td>{{ m.totalQuantity }}</td>
                <td>{{ m.availableQuantity }}</td>
                <td>
                  <button class="borrow-btn" @click="openBorrowForm(m)">Mượn</button>
                </td>
              </tr>
              <tr v-if="(modelsByCategory[selectedCategory.id] || []).length === 0">
                <td colspan="7" style="text-align: center">Không có dữ liệu</td>
              </tr>
            </tbody>
          </table>
        </div>
      </div>
    </div>
  </section>
</template>

<script setup>
import { ref, onMounted } from 'vue'
import DeviceModal from '@/components/Device/DeviceModal.vue'
import { deviceApi, categoryApi, borrowApi } from '@/config/api'
import { useAuthStore } from '@/stores/auth'

const auth = useAuthStore()
const isAdmin = auth.roleId === 1

// State
const categories = ref([])
const modelsByCategory = ref({})
const selectedCategory = ref(null)
const loading = ref(false)
const error = ref('')
const q = ref('')
const items = ref([])

// Danh sách tiết hợp lệ (bỏ tiết 5 và 15)
const validPeriods = [1, 2, 3, 4, 6, 7, 8, 9, 10, 11, 12, 13, 14]

// Index trong mảng validPeriods
const startPeriodIndex = ref(0) // tương ứng tiết 1
const endPeriodIndex = ref(2) // tương ứng tiết 3

// CRUD form
const form = ref({
  deviceId: '',
  deviceName: '',
  deviceType: '',
  specification: '',
  storageLocation: '',
  totalQuantity: 0,
  availableQuantity: 0,
})
const editingIndex = ref(null)
const showForm = ref(false)

// Borrow form
const showBorrowForm = ref(false)
const selectedModel = ref(null)
const usageLocation = ref('')
const usagePurpose = ref('')

// Fetch categories
async function fetchCategories() {
  loading.value = true
  try {
    categories.value = await categoryApi.getAll()
  } catch {
    error.value = 'Không thể tải danh sách loại thiết bị'
  } finally {
    loading.value = false
  }
}

// Fetch all devices
async function fetchDevices() {
  loading.value = true
  try {
    const data = await deviceApi.getAll()
    items.value = Array.isArray(data) ? data : []
  } catch {
    error.value = 'Không thể tải danh sách thiết bị'
  } finally {
    loading.value = false
  }
}

// Khi nhấn 1 category
async function openCategory(category) {
  selectedCategory.value = category
  if (!modelsByCategory.value[category.id]) {
    try {
      modelsByCategory.value[category.id] = await categoryApi.getModelsByCategory(category.id)
    } catch {
      modelsByCategory.value[category.id] = []
    }
  }
}

function backToCategories() {
  selectedCategory.value = null
}

// Hàm tính vị trí % dựa vào index
function getPeriodPosition(index) {
  return (index / (validPeriods.length - 1)) * 100
}

// Sửa hàm fixPeriod để làm việc với index
function fixPeriod(isStart) {
  if (isStart) {
    if (startPeriodIndex.value > endPeriodIndex.value) {
      startPeriodIndex.value = endPeriodIndex.value
    }
  } else {
    if (endPeriodIndex.value < startPeriodIndex.value) {
      endPeriodIndex.value = startPeriodIndex.value
    }
  }
}

// CRUD
function openCreate() {
  showForm.value = true
  editingIndex.value = null
  form.value = {
    deviceId: '',
    deviceName: '',
    deviceType: '',
    specification: '',
    storageLocation: '',
    totalQuantity: 0,
    availableQuantity: 0,
  }
}

function closeForm() {
  showForm.value = false
}

async function save(payload) {
  loading.value = true
  try {
    if (editingIndex.value !== null) {
      const id = items.value[editingIndex.value]?.deviceId
      await deviceApi.update(id, payload)
    } else {
      await deviceApi.create(payload)
    }
    await fetchDevices()
    closeForm()
  } catch {
    error.value = 'Không thể lưu thiết bị'
  } finally {
    loading.value = false
  }
}

function openBorrowForm(model) {
  selectedModel.value = model
  usageLocation.value = ''
  usagePurpose.value = ''
  startPeriodIndex.value = 0
  endPeriodIndex.value = 2
  showBorrowForm.value = true
}

function closeBorrowForm() {
  showBorrowForm.value = false
}

async function confirmBorrow() {
  if (!selectedModel.value) return

  // Kiểm tra dữ liệu nhập
  if (!usageLocation.value.trim() || !usagePurpose.value.trim()) {
    alert('⚠️ Vui lòng nhập đầy đủ vị trí và mục đích sử dụng.')
    return
  }

  // Lấy tiết thực tế từ validPeriods
  const startPeriodValue = validPeriods[startPeriodIndex.value]
  const endPeriodValue = validPeriods[endPeriodIndex.value]

  // Chuẩn bị payload theo đúng format API
  const payload = {
    userId: auth.userId,
    modelId: selectedModel.value.modelId,
    usageLocation: usageLocation.value.trim(),
    startPeriod: startPeriodValue,
    endPeriod: endPeriodValue,
    purpose: usagePurpose.value.trim(),
  }

  console.log('📦 Payload gửi đi:', payload)
  console.log(`🕐 Tiết: ${startPeriodValue} → ${endPeriodValue}`)

  try {
    const result = await borrowApi.create(payload)
    console.log('✅ Borrow request sent:', result)
    alert(
      `✅ Yêu cầu mượn thiết bị đã được gửi thành công!\n📍 Tiết ${startPeriodValue} đến tiết ${endPeriodValue}`,
    )
    showBorrowForm.value = false
  } catch (err) {
    console.error('❌ Lỗi khi gửi yêu cầu mượn:', err)
    alert('❌ Không thể gửi yêu cầu mượn thiết bị. Vui lòng thử lại sau.')
  }
}

onMounted(() => {
  fetchDevices()
  fetchCategories()
})
</script>

<style scoped>
.device {
  padding: 16px 12px;
}
.page-header {
  display: flex;
  align-items: center;
  justify-content: space-between;
  margin-bottom: 16px;
}
.page-header h2 {
  margin: 0;
  color: #111827;
}
.actions {
  display: flex;
  gap: 8px;
}
.actions input {
  padding: 6px 8px;
  border: 1px solid #e5e7eb;
  border-radius: 8px;
}
.content {
  background: #fff;
  border-radius: 12px;
  box-shadow: 0 2px 8px rgba(0, 0, 0, 0.06);
  padding: 16px;
}

/* Category cards */
.categories {
  display: grid;
  grid-template-columns: repeat(auto-fill, minmax(260px, 1fr));
  gap: 16px;
  margin-top: 16px;
}
.category-card {
  background: #ffffff;
  border: 1px solid #e5e7eb;
  border-radius: 12px;
  padding: 16px;
  cursor: pointer;
  box-shadow: 0 2px 6px rgba(0, 0, 0, 0.04);
  transition: all 0.2s;
}
.category-card:hover {
  transform: translateY(-3px);
  box-shadow: 0 4px 12px rgba(0, 0, 0, 0.1);
}
.category-card h3 {
  margin: 0;
  color: #111827;
}

/* Models table */
.models-view {
  animation: fadeIn 0.3s ease;
}
.models-header {
  display: flex;
  align-items: center;
  gap: 12px;
  margin-bottom: 16px;
}
.back-btn {
  background: none;
  border: none;
  color: #2563eb;
  font-weight: 600;
  cursor: pointer;
  font-size: 15px;
  padding: 4px 8px;
  border-radius: 6px;
}
.back-btn:hover {
  background: #f3f4f6;
}
.models-table {
  overflow-x: auto;
}
.models-table table {
  width: 100%;
  border-collapse: collapse;
  font-size: 14px;
}
.models-table th,
.models-table td {
  border: 1px solid #e5e7eb;
  padding: 8px 10px;
  text-align: left;
}
.models-table th {
  background: #f9fafb;
  font-weight: 600;
}
.borrow-btn {
  background-color: #2563eb;
  color: white;
  border: none;
  padding: 6px 10px;
  border-radius: 6px;
  cursor: pointer;
  transition: background 0.2s;
}
.borrow-btn:hover {
  background-color: #1d4ed8;
}

/* Borrow form modal */
.borrow-overlay {
  position: fixed;
  inset: 0;
  background: rgba(0, 0, 0, 0.45);
  display: flex;
  align-items: center;
  justify-content: center;
  z-index: 50;
}
.borrow-form {
  background: white;
  padding: 20px;
  border-radius: 10px;
  width: 400px;
  max-width: 90vw;
  box-shadow: 0 6px 20px rgba(0, 0, 0, 0.15);
  animation: fadeIn 0.3s ease;
}
.borrow-form h3 {
  margin-top: 0;
  color: #111827;
}
.field {
  display: flex;
  flex-direction: column;
  margin-top: 10px;
}
.field label {
  font-size: 14px;
  color: #374151;
  margin-bottom: 4px;
  font-weight: 500;
}
.field input {
  padding: 6px 8px;
  border: 1px solid #d1d5db;
  border-radius: 6px;
}
.actions {
  display: flex;
  justify-content: flex-end;
  gap: 8px;
  margin-top: 16px;
}
.cancel-btn {
  background: #e5e7eb;
  border: none;
  padding: 6px 10px;
  border-radius: 6px;
  cursor: pointer;
}
.submit-btn {
  background: #2563eb;
  color: white;
  border: none;
  padding: 6px 10px;
  border-radius: 6px;
  cursor: pointer;
}
.empty {
  text-align: center;
  color: #6b7280;
  padding: 24px 0;
}

@keyframes fadeIn {
  from {
    opacity: 0;
    transform: translateY(8px);
  }
  to {
    opacity: 1;
    transform: translateY(0);
  }
}

/* Period Slider */
.period-slider-container {
  width: 100%;
  padding-top: 10px;
}

/* Labels - chỉ hiển thị các tiết hợp lệ */
.labels {
  display: flex;
  position: relative;
  margin-bottom: 12px;
  padding: 0;
  height: 24px;
}

.period-label {
  position: absolute;
  font-size: 13px;
  color: #374151;
  font-weight: 600;
  transform: translateX(-50%);
  user-select: none;
}

/* Vùng chứa slider */
.slider-wrap {
  position: relative;
  width: 100%;
  height: 40px;
  margin-top: 0;
}

.slider-track-bg {
  position: absolute;
  height: 6px;
  background: #e5e7eb;
  border-radius: 3px;
  top: 17px;
  width: 100%;
  z-index: 1;
}

/* Thanh highlight */
.range-highlight {
  position: absolute;
  height: 6px;
  background: linear-gradient(90deg, #10b981 0%, #34d399 100%);
  border-radius: 3px;
  top: 17px;
  pointer-events: none;
  transition:
    left 0.2s ease,
    width 0.2s ease;
  z-index: 2;
  box-shadow: 0 2px 4px rgba(16, 185, 129, 0.3);
}

/* Slider base */
.slider {
  position: absolute;
  top: 0;
  width: 100%;
  height: 40px;
  -webkit-appearance: none;
  background: none;
  cursor: pointer;
  margin: 0;
  padding: 0;
}

/* Track */
.slider::-webkit-slider-runnable-track {
  height: 40px;
  background: transparent;
}

.slider::-moz-range-track {
  height: 40px;
  background: transparent;
}

/* Thumb được thiết kế đẹp hơn */
.slider::-webkit-slider-thumb {
  pointer-events: all;
  -webkit-appearance: none;
  width: 18px;
  height: 18px;
  background: #ffffff;
  border: 3px solid #10b981;
  border-radius: 50%;
  cursor: grab;
  box-shadow: 0 2px 8px rgba(0, 0, 0, 0.25);
  transition: transform 0.15s ease;
  margin-top: 11px;
}

.slider::-webkit-slider-thumb:hover {
  transform: scale(1.2);
  box-shadow: 0 3px 10px rgba(16, 185, 129, 0.4);
}

.slider::-webkit-slider-thumb:active {
  cursor: grabbing;
  transform: scale(1.15);
}

/* Firefox */
.slider::-moz-range-thumb {
  width: 18px;
  height: 18px;
  background: #ffffff;
  border: 3px solid #10b981;
  border-radius: 50%;
  cursor: grab;
  box-shadow: 0 2px 8px rgba(0, 0, 0, 0.25);
  border: none;
}

.slider-start {
  z-index: 4;
}

.slider-end {
  z-index: 3;
}

.result {
  margin-top: 12px;
  font-size: 13px;
  font-weight: 600;
  color: #10b981;
  text-align: center;
  padding: 8px;
  background: #f0fdf4;
  border-radius: 6px;
  border: 1px solid #d1fae5;
}
</style>
