<template>
  <section class="device">
    <header class="page-header">
      <h2>Danh Sách Thiết bị</h2>
      <div class="actions">
        <input v-model="q" placeholder="Tìm theo tên/loại" />
        <button v-if="isAdmin" @click="openCreate">Thêm</button>
      </div>
    </header>

    <div class="content">
      <DeviceModal
        v-if="showForm"
        :value="form"
        :title="editingIndex !== null ? 'Cập nhật thiết bị' : 'Thêm thiết bị mới'"
        :submit-text="editingIndex !== null ? 'Cập nhật' : 'Thêm'"
        @submit="save"
        @close="closeForm"
      />

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

          <div class="field">
            <label>Chọn tiết sử dụng</label>

            <div class="period-numbers">
              <span v-for="period in validPeriods" :key="period" class="period-num">
                {{ period }}
              </span>
            </div>

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

          <div class="actions" style="justify-content: flex-end">
            <button class="cancel-btn" @click="closeBorrowForm">Hủy</button>
            <button class="submit-btn" @click="confirmBorrow">Xác nhận</button>
          </div>
        </div>
      </div>

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
                  <button
                    class="borrow-btn"
                    :disabled="m.availableQuantity === 0"
                    @click="openBorrowForm(m)"
                  >
                    Mượn
                  </button>
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
import { ref, onMounted, watch } from 'vue'
import DeviceModal from '@/components/Device/DeviceModal.vue'
import { deviceApi, categoryApi, borrowApi } from '@/config/api'
import { useAuthStore } from '@/stores/auth'
// 👇 IMPORT TOAST
import { toast } from '@/utils/toast.js'

const auth = useAuthStore()
const isAdmin = auth.roleId === 1

// State
const categories = ref([])
const modelsByCategory = ref({})
const selectedCategory = ref(null)
const loading = ref(false)
const q = ref('')
const items = ref([])

// ✅ Danh sách tiết theo yêu cầu: 1→4, 7→10, 11→14
const validPeriods = [1, 2, 3, 4, 7, 8, 9, 10, 11, 12, 13, 14]

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
const rangeElement = ref(null)

async function fetchCategories() {
  loading.value = true
  try {
    categories.value = await categoryApi.getAll()
  } catch {
    toast.error('Không thể tải danh sách loại thiết bị')
  } finally {
    loading.value = false
  }
}

async function fetchDevices() {
  loading.value = true
  try {
    const data = await deviceApi.getAll()
    items.value = Array.isArray(data) ? data : []
  } catch {
    toast.error('Không thể tải danh sách thiết bị')
  } finally {
    loading.value = false
  }
}

async function openCategory(category) {
  selectedCategory.value = category
  if (!modelsByCategory.value[category.id]) {
    try {
      modelsByCategory.value[category.id] = await categoryApi.getModelsByCategory(category.id)
    } catch {
      modelsByCategory.value[category.id] = []
      toast.error('Không thể tải danh sách model')
    }
  }
}

function backToCategories() {
  selectedCategory.value = null
}

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

// Hàm lưu (Admin thêm/sửa)
async function save(payload) {
  loading.value = true
  try {
    if (editingIndex.value !== null) {
      const id = items.value[editingIndex.value]?.deviceId
      await deviceApi.update(id, payload)
      toast.success('Cập nhật thiết bị thành công!')
    } else {
      await deviceApi.create(payload)
      toast.success('Thêm thiết bị mới thành công!')
    }
    await fetchDevices()
    closeForm()
  } catch {
    toast.error('Không thể lưu thiết bị. Vui lòng thử lại.')
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

  // ✅ Cập nhật range ngay khi mở form
  setTimeout(() => {
    if (rangeElement.value) {
      const min = 0
      const max = validPeriods.length - 1
      const leftPercent = ((startPeriodIndex.value - min) / (max - min)) * 100
      const rightPercent = ((endPeriodIndex.value - min) / (max - min)) * 100
      rangeElement.value.style.left = `${leftPercent}%`
      rangeElement.value.style.right = `${100 - rightPercent}%`
    }
  }, 0)
}

function closeBorrowForm() {
  showBorrowForm.value = false
}

// Xác nhận mượn
async function confirmBorrow() {
  if (!selectedModel.value) return

  if (!usageLocation.value.trim() || !usagePurpose.value.trim()) {
    // 🔥 Thay alert bằng toast warning
    toast.warning('Vui lòng nhập đầy đủ vị trí và mục đích sử dụng.')
    return
  }

  const startPeriodValue = validPeriods[startPeriodIndex.value]
  const endPeriodValue = validPeriods[endPeriodIndex.value]

  const payload = {
    userId: auth.userId,
    modelId: selectedModel.value.modelId,
    usageLocation: usageLocation.value.trim(),
    startPeriod: startPeriodValue,
    endPeriod: endPeriodValue,
    purpose: usagePurpose.value.trim(),
  }

  try {
    await borrowApi.create(payload)

    // 🔥 Thay alert bằng toast success
    toast.success(`Gửi yêu cầu thành công! (Tiết ${startPeriodValue} - ${endPeriodValue})`)

    showBorrowForm.value = false
  } catch (err) {
    console.error('❌ Lỗi khi gửi yêu cầu mượn:', err)
    // 🔥 Thay alert bằng toast error
    toast.error('Không thể gửi yêu cầu mượn. Vui lòng thử lại.')
  }
}

onMounted(() => {
  fetchDevices()
  fetchCategories()

  // ✅ Khởi tạo vị trí range ban đầu
  if (rangeElement.value) {
    const min = 0
    const max = validPeriods.length - 1
    const leftPercent = ((startPeriodIndex.value - min) / (max - min)) * 100
    const rightPercent = ((endPeriodIndex.value - min) / (max - min)) * 100
    rangeElement.value.style.left = `${leftPercent}%`
    rangeElement.value.style.right = `${100 - rightPercent}%`
  }
})

// ✅ Cập nhật vị trí range khi startPeriodIndex thay đổi + CHẶN vượt qua
watch(startPeriodIndex, () => {
  if (startPeriodIndex.value > endPeriodIndex.value) {
    startPeriodIndex.value = endPeriodIndex.value
    return
  }
  const min = 0
  const max = validPeriods.length - 1
  const leftPercent = ((startPeriodIndex.value - min) / (max - min)) * 100
  if (rangeElement.value) {
    rangeElement.value.style.left = `${leftPercent}%`
  }
})

// ✅ Cập nhật khi endPeriodIndex thay đổi + CHẶN về trước
watch(endPeriodIndex, () => {
  if (endPeriodIndex.value < startPeriodIndex.value) {
    endPeriodIndex.value = startPeriodIndex.value
    return
  }
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
  color: #eeeeee;
}
.page-header {
  display: flex;
  align-items: center;
  justify-content: space-between;
  margin-bottom: 16px;
}
.page-header h2 {
  margin: 0;
  color: #00adb5;
  text-shadow: 0 0 10px rgba(0, 173, 181, 0.5);
}
.actions {
  display: flex;
  justify-content: flex-end;
  gap: 8px;
  margin-top: 16px;
}
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
  color: rgba(238, 238, 238, 0.7);
}
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
  background: #393e46;
  border-radius: 12px;
  box-shadow: 0 0 20px rgba(0, 173, 181, 0.15);
  border: 1px solid rgba(0, 173, 181, 0.2);
  padding: 16px;
}

.categories {
  display: grid;
  grid-template-columns: repeat(auto-fill, minmax(260px, 1fr));
  gap: 16px;
  margin-top: 16px;
}
.category-card {
  background: #222831;
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
  color: #eeeeee;
  transition: color 0.2s;
}
.category-card:hover h3 {
  color: #00adb5;
}

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
  color: #00adb5;
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
  color: #eeeeee;
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
  color: #00adb5;
  font-weight: 600;
}
.borrow-btn {
  background-color: #00adb5;
  color: #222831;
  border: none;
  padding: 6px 10px;
  border-radius: 6px;
  cursor: pointer;
  transition: all 0.2s;
  font-weight: 600;
}
.borrow-btn:hover:not(:disabled) {
  background-color: #eeeeee;
}
.borrow-btn:disabled {
  background-color: rgba(0, 173, 181, 0.3);
  color: rgba(34, 40, 49, 0.5);
  cursor: not-allowed;
  opacity: 0.5;
}

.borrow-overlay {
  position: fixed;
  inset: 0;
  background: rgba(0, 0, 0, 0.7);
  display: flex;
  align-items: center;
  justify-content: center;
  z-index: 50;
}
.borrow-form {
  background: #393e46;
  padding: 20px;
  border-radius: 10px;
  width: 400px;
  max-width: 90vw;
  box-shadow: 0 0 30px rgba(0, 173, 181, 0.25);
  border: 1px solid rgba(0, 173, 181, 0.3);
  animation: fadeIn 0.3s ease;
  color: #eeeeee;
}
.borrow-form h3 {
  margin-top: 0;
  color: #00adb5;
}
.field {
  display: flex;
  flex-direction: column;
  margin-top: 10px;
}
.field label {
  font-size: 14px;
  color: rgba(238, 238, 238, 0.7);
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
  color: rgba(238, 238, 238, 0.7);
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

/* ====================== Thanh trượt chọn tiết ====================== */
.period-numbers {
  display: flex;
  justify-content: space-between;
  padding: 0 4px;
  margin-bottom: 4px;
}

.period-num {
  font-size: 12px;
  color: #00adb5;
  font-weight: 600;
  min-width: 20px;
  text-align: center;
}

.multi-range-slider {
  position: relative;
  width: 100%;
  height: 2rem;
  margin-top: 10px;
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
  pointer-events: none;
}

input[type='range']:focus {
  outline: none;
}

input[type='range']::-webkit-slider-thumb {
  -webkit-appearance: none;
  pointer-events: auto;
  cursor: pointer;
  border: 0 none;
  width: 1.5rem;
  height: 1.5rem;
  background: #eeeeee;
  border: 0.3rem solid #00adb5;
  border-radius: 50%;
  margin-top: -0.35rem;
}

input[type='range']::-moz-range-thumb {
  -moz-appearance: none;
  pointer-events: auto;
  cursor: pointer;
  border: 0 none;
  width: 1.5rem;
  height: 1.5rem;
  background: #eeeeee;
  border: 0.3rem solid #00adb5;
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
  background-color: #222831;
}

.slider > .range {
  position: absolute;
  z-index: 2;
  top: 0;
  bottom: 0;
  border-radius: 5rem;
  background: #00adb5;
}

.price__wrapper {
  width: 100%;
  color: #eeeeee;
  display: flex;
  justify-content: space-between;
  font-size: 13px;
  margin-top: 10px;
  font-weight: 500;
}
</style>
