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
          <!-- Thanh trượt chọn tiết (Multi Range Slider) -->
          <div class="field">
            <label>Chọn tiết sử dụng</label>
            <div class="multi-range-slider">
              <input
                type="range"
                id="input-left"
                :min="0"
                :max="validPeriods.length - 1"
                v-model.number="startPeriodIndex"
              />
              <input
                type="range"
                id="input-right"
                :min="0"
                :max="validPeriods.length - 1"
                v-model.number="endPeriodIndex"
              />
              <div class="slider">
                <div class="track"></div>
                <div class="range" ref="rangeElement"></div>
              </div>
            </div>
            <div class="price__wrapper">
              <span class="price-from">Tiết: {{ validPeriods[startPeriodIndex] }}</span>
              <span class="price-to">Tiết: {{ validPeriods[endPeriodIndex] }}</span>
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

// async function fetchCategories() {
//   loading.value = true
//   try {
//     categories.value = await categoryApi.getAll()
//   } catch {
//     error.value = 'Không thể tải danh sách loại thiết bị'
//   } finally {
//     loading.value = false
//   }
// }

// async function fetchDevices() {
//   loading.value = true
//   try {
//     const data = await deviceApi.getAll()
//     items.value = Array.isArray(data) ? data : []
//   } catch {
//     error.value = 'Không thể tải danh sách thiết bị'
//   } finally {
//     loading.value = false
//   }
// }

// Thay thế fetchCategories và fetchDevices bằng dữ liệu giả lập
function fetchCategories() {
  loading.value = true
  try {
    categories.value = [
      { id: 1, name: 'Laptop' },
      { id: 2, name: 'Máy chiếu' },
    ]
    // Gán luôn dữ liệu models cho mỗi category
    modelsByCategory.value = {
      1: [
        {
          modelId: 101,
          modelName: 'Dell XPS 13',
          specifications: 'i7, 16GB RAM, 512GB SSD',
          storageLocation: 'A1',
          totalQuantity: 5,
          availableQuantity: 3,
        },
        {
          modelId: 102,
          modelName: 'MacBook Pro 14',
          specifications: 'M1 Pro, 16GB RAM, 1TB SSD',
          storageLocation: 'A2',
          totalQuantity: 2,
          availableQuantity: 2,
        },
      ],
      2: [
        {
          modelId: 201,
          modelName: 'Epson EB-X41',
          specifications: '3000 lumens, XGA',
          storageLocation: 'B1',
          totalQuantity: 1,
          availableQuantity: 1,
        },
      ],
    }
  } catch {
    error.value = 'Không thể tải danh sách loại thiết bị'
  } finally {
    loading.value = false
  }
}

function fetchDevices() {
  // Không cần fetch thực, chỉ để items rỗng hoặc bạn có thể thêm vài device demo
  items.value = []
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

import { watch } from 'vue'

const rangeElement = ref(null) // Tham chiếu .range

// Cập nhật vị trí range khi startPeriodIndex thay đổi
watch(startPeriodIndex, () => {
  const min = 0
  const max = validPeriods.length - 1
  const leftPercent = ((startPeriodIndex.value - min) / (max - min)) * 100
  if (rangeElement.value) {
    rangeElement.value.style.left = `${leftPercent}%`
  }
})

// Cập nhật khi endPeriodIndex thay đổi
watch(endPeriodIndex, () => {
  const min = 0
  const max = validPeriods.length - 1
  const rightPercent = ((endPeriodIndex.value - min) / (max - min)) * 100
  if (rangeElement.value) {
    rangeElement.value.style.right = `${100 - rightPercent}%`
  }
})
</script>

<style scoped>
.device {
  padding: 16px 12px;
  color: #eeeeee; /* ✅ Chữ chính */
}
.page-header {
  display: flex;
  align-items: center;
  justify-content: space-between;
  margin-bottom: 16px;
}
.page-header h2 {
  margin: 0;
  color: #00adb5; /* ✅ Chữ nhấn */
  text-shadow: 0 0 10px rgba(0, 173, 181, 0.5);
}
.actions {
  display: flex;
  gap: 8px;
}
/* Cập nhật ô tìm kiếm */
.actions input {
  padding: 6px 10px;
  border: 1px solid rgba(0, 173, 181, 0.3);
  background: #222831;
  color: #eeeeee;
  border-radius: 8px;
  transition: all 0.2s;
}
.actions input:focus {
  outline: none;
  border-color: #00adb5;
  box-shadow: 0 0 10px rgba(0, 173, 181, 0.3);
}
.actions input::placeholder {
  color: rgba(238, 238, 238, 0.7); /* ✅ Chữ phụ */
}
/* Nút "Thêm" của Admin */
.actions button {
  background: #00adb5;
  color: #222831;
  border: none;
  border-radius: 8px;
  padding: 6px 12px;
  cursor: pointer;
  font-weight: 600;
  transition: 0.2s;
}
.actions button:hover {
  background: #eeeeee;
}

.content {
  background: #393e46; /* Nền phụ */
  border-radius: 12px;
  box-shadow: 0 0 20px rgba(0, 173, 181, 0.15);
  border: 1px solid rgba(0, 173, 181, 0.2);
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
  background: #222831; /* Nền chính */
  border: 1px solid rgba(0, 173, 181, 0.2);
  border-radius: 12px;
  padding: 16px;
  cursor: pointer;
  box-shadow: 0 2px 6px rgba(0, 0, 0, 0.04);
  transition: all 0.2s;
}
.category-card:hover {
  transform: translateY(-3px);
  border-color: #00adb5;
  background: rgba(0, 173, 181, 0.1);
  box-shadow: 0 4px 12px rgba(0, 173, 181, 0.1);
}
.category-card h3 {
  margin: 0;
  color: #eeeeee; /* ✅ Chữ chính */
  transition: color 0.2s;
}
.category-card:hover h3 {
  color: #00adb5; /* ✅ Chữ nhấn */
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
  color: #00adb5; /* ✅ Chữ nhấn */
  font-weight: 600;
  cursor: pointer;
  font-size: 15px;
  padding: 4px 8px;
  border-radius: 6px;
  transition: background 0.2s;
}
.back-btn:hover {
  background: rgba(0, 173, 181, 0.1);
}
.models-header h3 {
  color: #eeeeee; /* ✅ Chữ chính */
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
  border: 1px solid rgba(0, 173, 181, 0.15);
  padding: 8px 10px;
  text-align: left;
}
.models-table th {
  background: #222831;
  color: #00adb5; /* ✅ Chữ nhấn */
  font-weight: 600;
}
.borrow-btn {
  background-color: #00adb5; /* ✅ Chữ nhấn */
  color: #222831; /* Chữ tối để tương phản */
  border: none;
  padding: 6px 10px;
  border-radius: 6px;
  cursor: pointer;
  transition: background 0.2s;
  font-weight: 600;
}
.borrow-btn:hover {
  background-color: #eeeeee;
}

/* Borrow form modal */
.borrow-overlay {
  position: fixed;
  inset: 0;
  background: rgba(0, 0, 0, 0.7); /* Tối hơn */
  display: flex;
  align-items: center;
  justify-content: center;
  z-index: 50;
}
.borrow-form {
  background: #393e46; /* Nền phụ */
  padding: 20px;
  border-radius: 10px;
  width: 400px;
  max-width: 90vw;
  box-shadow: 0 0 30px rgba(0, 173, 181, 0.25);
  border: 1px solid rgba(0, 173, 181, 0.3);
  animation: fadeIn 0.3s ease;
  color: #eeeeee; /* ✅ Chữ chính */
}
.borrow-form h3 {
  margin-top: 0;
  color: #00adb5; /* ✅ Chữ nhấn */
}
.field {
  display: flex;
  flex-direction: column;
  margin-top: 10px;
}
.field label {
  font-size: 14px;
  color: rgba(238, 238, 238, 0.7); /* ✅ Chữ phụ */
  margin-bottom: 4px;
  font-weight: 500;
}
.field input {
  padding: 6px 8px;
  border: 1px solid rgba(0, 173, 181, 0.3);
  background: #222831;
  color: #eeeeee;
  border-radius: 6px;
}
.field input:focus {
  outline: none;
  border-color: #00adb5;
  box-shadow: 0 0 10px rgba(0, 173, 181, 0.3);
}
.actions {
  display: flex;
  justify-content: flex-end;
  gap: 8px;
  margin-top: 16px;
}
.cancel-btn {
  background: rgba(238, 238, 238, 0.1);
  color: #eeeeee;
  border: none;
  padding: 6px 10px;
  border-radius: 6px;
  cursor: pointer;
  transition: background 0.2s;
}
.cancel-btn:hover {
  background: rgba(238, 238, 238, 0.2);
}
.submit-btn {
  background: #00adb5;
  color: #222831;
  border: none;
  padding: 6px 10px;
  border-radius: 6px;
  cursor: pointer;
  font-weight: 600;
  transition: background 0.2s;
}
.submit-btn:hover {
  background: #eeeeee;
}
.empty,
td[colspan='7'] {
  text-align: center;
  color: rgba(238, 238, 238, 0.7); /* ✅ Chữ phụ */
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
/* ====================== Thanh trượt chọn tiết ====================== */
.multi-range-slider {
  position: relative;
  width: 100%;
  height: 2rem;
}

input[type='range'] {
  -webkit-appearance: none;
  -moz-appearance: none;
  width: 100%;
  background: transparent;
  appearance: none;
  position: absolute;
  z-index: 10;
  height: 100%;
  pointer-events: all;
}

input[type='range']::-webkit-slider-thumb {
  -webkit-appearance: none;
  cursor: pointer;
  border: 0 none;
  width: 1.8rem;
  height: 1.8rem;
  background: #efedfc;
  border: 0.35rem solid #8e7eeb;
  border-radius: 50%;
}

.slider {
  position: absolute;
  width: 100%;
  top: 0.6rem;
  z-index: 1;
  height: 0.8rem;
}

.slider > .track {
  position: absolute;
  z-index: 1;
  left: 0;
  right: 0;
  bottom: 0;
  top: 0;
  border-radius: 5rem;
  background-color: #eeeeee;
}

.slider > .range {
  position: absolute;
  z-index: 2;
  top: 0;
  bottom: 0;
  border-radius: 5rem;
  background: #cec8f6;
}

.price__wrapper {
  width: 100%;
  color: #efedfc;
  display: flex;
  justify-content: space-between;
  font-size: 1.6rem;
  margin-top: 1rem;
  font-weight: bold;
}
</style>
