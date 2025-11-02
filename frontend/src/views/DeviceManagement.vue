<template>
  <section class="device">
    <!-- Header -->
    <header class="page-header">
      <header class="page-header">
        <h2>Danh Sách Thiết bị</h2>
      </header>
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
        <!-- <div class="models-header">
          <button class="back-btn" @click="backToCategories">← Quay lại</button>
          <h3>{{ selectedCategory.name }}</h3>
        </div> -->
        <div class="models-header">
          <div class="left-controls">
            <button class="back-btn" @click="backToCategories">⮌</button>
            <h3>{{ selectedCategory.name }}</h3>
          </div>

          <!-- Chỉ admin mới thấy nút thêm -->
          <button v-if="isAdmin" class="add-btn" @click="openCreate">➕ Thêm</button>
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
              </tr>
              <tr v-if="(modelsByCategory[selectedCategory.id] || []).length === 0">
                <td colspan="6" style="text-align: center">Không có dữ liệu</td>
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
import { deviceApi, categoryApi, modelApi } from '@/config/api' // 🔹 Thêm modelApi
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

// Form CRUD
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

// Fetch all devices (nếu cần cho admin)
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
    categoryId: selectedCategory.value?.id || null, // 🔹 Gán sẵn id category đang mở
  }
}

function closeForm() {
  showForm.value = false
}

import { ElMessage } from 'element-plus'

async function save(payload) {
  loading.value = true
  try {
    if (editingIndex.value !== null) {
      // 🟦 Nếu đang chỉnh sửa — cập nhật thiết bị
      const id = items.value[editingIndex.value]?.deviceId
      await deviceApi.update(id, payload)
      ElMessage.success('Cập nhật thiết bị thành công!')
    } else {
      // 🟩 Nếu đang thêm mới model
      await modelApi.create({
        modelName: payload.deviceName,
        categoryId: selectedCategory.value.id,
        specifications: payload.specification,
        storageLocation: payload.storageLocation,
      })
      ElMessage.success('Thêm thiết bị thành công!')
    }

    // 🟢 Sau khi thêm/cập nhật, load lại danh sách models từ server
    if (selectedCategory.value) {
      modelsByCategory.value[selectedCategory.value.id] = await categoryApi.getModelsByCategory(
        selectedCategory.value.id,
      )
    }

    // ✅ Đóng form sau khi lưu xong
    closeForm()
  } catch (err) {
    console.error(err)
    error.value = 'Không thể lưu thiết bị'
    ElMessage.error('Lưu thiết bị thất bại!')
  } finally {
    loading.value = false
  }
}

// Lifecycle
onMounted(() => {
  fetchCategories()
})
onMounted(() => {
  if (isAdmin) {
    fetchDevices()
  }
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

/* View models */
.models-view {
  animation: fadeIn 0.3s ease;
}
/* .models-header {
  display: flex;
  align-items: center;
  gap: 12px;
  margin-bottom: 16px;
} */

.models-header {
  display: flex;
  justify-content: space-between; /* 🔹 Đẩy 2 bên xa nhau */
  align-items: center;
  margin-bottom: 16px;
}

.left-controls {
  display: flex;
  align-items: center;
  gap: 12px;
}

.add-btn {
  background: #2563eb;
  color: white;
  border: none;
  border-radius: 8px;
  padding: 6px 12px;
  cursor: pointer;
  font-weight: 600;
  transition: 0.2s;
}

.add-btn:hover {
  background: #1e40af;
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
