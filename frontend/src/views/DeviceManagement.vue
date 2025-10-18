<!--
  TRANG QUẢN LÝ THIẾT BỊ
  - CRUD đầy đủ: thêm, sửa, xóa, tìm kiếm thiết bị
  - Phân trang client-side
  - Lưu dữ liệu trong localStorage
-->
<template>
  <section class="device">
    <!-- Header trang với tiêu đề và các nút hành động -->
    <header class="page-header">
      <h2>Danh Sách Thiết bị</h2>
      <div class="actions">
        <!-- Ô tìm kiếm theo tên hoặc loại -->
        <input v-model="q" placeholder="Tìm theo tên/loại" />
        <!-- Nút thêm device mới -->
        <button @click="openCreate">Thêm</button>
      </div>
    </header>

    <div class="content">
      <!-- Form thêm/sửa thiết bị -->
      <div class="tools">
        <DeviceForm
          v-if="showForm"
          :value="form"
          :submit-text="editingIndex !== null ? 'Cập nhật' : 'Thêm'"
          :show-cancel="true"
          @submit="save"
          @cancel="closeForm"
        />
      </div>

      <!-- Danh sách category (ID, Name). Bấm để xem danh sách model -->
      <div class="category">
  <div class="category-card" v-for="category in categories" :key="category.id">
    <div class="category-header">
      <div class="info">
        <div class="type">{{ category.name || 'Không xác định' }}</div>
      </div>
      <button class="toggle" @click="toggleType(category.id)">
        {{ isExpanded(category.id) ? 'Ẩn' : 'Xem tất cả' }}
      </button>
    </div>
    <div class="category-body" v-if="isExpanded(category.id)">
      <table class="table">
        <thead>
          <tr>
            <th>ID</th>
            <th>Tên thiết bị</th>
            <th>Thông số kỹ thuật</th>
            <th>Vị trí lưu trữ</th>
            <th>Tổng số lượng</th>
            <th>Số lượng khả dụng</th>
          </tr>
        </thead>
        <tbody>
          <tr v-for="m in modelsByCategory[category.id] || []" :key="m.modelId">
            <td>{{ m.modelId }}</td>
            <td>{{ m.modelName }}</td>
            <td>{{ m.specifications }}</td>
            <td>{{ m.storageLocation }}</td>
            <td>{{ m.totalQuantity }}</td>
            <td>{{ m.availableQuantity }}</td>
          </tr>
          <tr v-if="(modelsByCategory[category.id] || []).length === 0">
            <td colspan="6" style="text-align: center">Không có dữ liệu</td>
          </tr>
        </tbody>
      </table>
    </div>
  </div>
  <div v-if="categories.length === 0" class="empty">Không có dữ liệu</div>
</div>

      <!-- Phân trang tổng thể (áp dụng khi xem dạng danh sách, hiện tắt trong chế độ nhóm) -->
      <!--
      <div class="pager">
        <button :disabled="page===1" @click="page--">Trước</button>
        <span>Trang {{ page }} / {{ totalPages }}</span>
        <button :disabled="page===totalPages" @click="page++">Sau</button>
      </div>
      -->
    </div>
  </section>
</template>
<script setup>
import { ref, onMounted } from 'vue'
import DeviceForm from '@/components/Device/DeviceForm.vue'
import { deviceApi, categoryApi } from '@/config/api'

// Trạng thái dữ liệu
const loading = ref(false)
const error = ref('')

// Danh sách loại thiết bị và model theo loại
const categories = ref([])
const modelsByCategory = ref({})
const expanded = ref(new Set())

// Danh sách device (nếu cần cho form thêm/sửa)
const items = ref([])

// Tìm kiếm
const q = ref('')

// Form và trạng thái form
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

// Lấy danh sách loại thiết bị
async function fetchCategories() {
  loading.value = true
  error.value = ''
  try {
    categories.value = await categoryApi.getAll()
  } catch (err) {
    error.value = 'Không thể tải danh sách loại thiết bị'
  } finally {
    loading.value = false
  }
}

// Lấy danh sách device (nếu cần cho CRUD)
async function fetchDevices() {
  loading.value = true
  error.value = ''
  try {
    const data = await deviceApi.getAll()
    items.value = Array.isArray(data) ? data : []
  } catch (err) {
    error.value = 'Không thể tải danh sách thiết bị'
  } finally {
    loading.value = false
  }
}

// Khi bấm vào loại thiết bị, lấy model theo id
async function toggleType(categoryId) {
  if (expanded.value.has(categoryId)) {
    expanded.value.delete(categoryId)
  } else {
    expanded.value.add(categoryId)
    if (!modelsByCategory.value[categoryId]) {
      try {
        modelsByCategory.value[categoryId] = await categoryApi.getModelsByCategory(categoryId)
      } catch {
        modelsByCategory.value[categoryId] = []
      }
    }
  }
  expanded.value = new Set(expanded.value)
}

function isExpanded(id) {
  return expanded.value.has(id)
}

// CRUD Device (nếu bạn vẫn muốn giữ chức năng này)
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

function startEdit(d, absoluteIndex) {
  showForm.value = true
  editingIndex.value = absoluteIndex
  form.value = { ...d }
}

function closeForm() {
  showForm.value = false
}

async function save(payload) {
  loading.value = true
  error.value = ''
  try {
    if (editingIndex.value !== null) {
      const id = items.value[editingIndex.value]?.deviceId
      await deviceApi.update(id, payload)
    } else {
      await deviceApi.create(payload)
    }
    await fetchDevices()
    closeForm()
  } catch (err) {
    error.value = 'Không thể lưu thiết bị'
  } finally {
    loading.value = false
  }
}

async function remove(absoluteIndex) {
  loading.value = true
  error.value = ''
  try {
    const id = items.value[absoluteIndex]?.deviceId
    await deviceApi.delete(id)
    await fetchDevices()
  } catch (err) {
    error.value = 'Không thể xóa thiết bị'
  } finally {
    loading.value = false
  }
}

// Khi component mount, nạp dữ liệu
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
  gap: 12px;
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
.page-header {
  display: flex;
  align-items: center;
  justify-content: space-between;
  gap: 12px;
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
.table {
  width: 100%;
  border-collapse: collapse;
}
.table th,
.table td {
  border: 1px solid #eee;
  padding: 10px;
  text-align: left;
}
.table th {
  background: #f8fafc;
}
.pager {
  display: flex;
  align-items: center;
  gap: 8px;
  justify-content: flex-end;
  padding-top: 10px;
}

/* Nhóm theo loại */
.categories {
  display: grid;
  gap: 12px;
}
.category-card {
  border: 1px solid #eee;
  border-radius: 12px;
  overflow: hidden;
}
.category-header {
  display: flex;
  align-items: center;
  justify-content: space-between;
  padding: 12px 14px;
  background: #f8fafc;
}
.category-header .info {
  display: grid;
}
.category-header .type {
  font-weight: 700;
  color: #111827;
}
.category-header .meta {
  color: #6b7280;
  font-size: 12px;
}
.category-header .toggle {
  padding: 6px 10px;
  border: none;
  border-radius: 8px;
  background: #3b82f6;
  color: #fff;
  cursor: pointer;
}
.category-body {
  padding: 0 12px 12px;
  background: #fff;
}
.empty {
  text-align: center;
  color: #6b7280;
  padding: 24px 0;
}
</style>
