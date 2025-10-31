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
import { deviceApi, categoryApi } from '@/config/api'
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
  showBorrowForm.value = true
}

function closeBorrowForm() {
  showBorrowForm.value = false
}
function confirmBorrow() {
  console.log('Tạm thời chưa xử lý — sẽ gửi request mượn sau')
  showBorrowForm.value = false
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
  width: 360px;
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
</style>
