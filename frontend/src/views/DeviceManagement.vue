<template>
  <section class="device">
    <!-- Header -->
    <header class="page-header">
      <h2>Danh Sách Thiết bị</h2>
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

        <!-- ✅ Nút thêm category -->
        <div v-if="isAdmin" class="category-card add-category-card" @click="openAddCategory">
          <span class="plus">+</span>
          <p>Thêm mới</p>
        </div>

        <div v-if="categories.length === 0" class="empty">Không có dữ liệu</div>
      </div>

      <CategoryModal
        v-if="showCategoryForm"
        :value="{ name: newCategoryName }"
        title="Thêm loại thiết bị mới"
        submit-text="Thêm"
        @submit="addCategory"
        @close="closeCategoryForm"
      />

      <!-- Chi tiết model -->
      <div v-if="selectedCategory" class="models-view">
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
                <th>Hành động</th>
              </tr>
            </thead>

            <tbody>
              <template v-for="m in modelsByCategory[selectedCategory.id] || []" :key="m.modelId">
                <tr>
                  <td>{{ m.modelId }}</td>
                  <td>{{ m.modelName }}</td>
                  <td>{{ m.specifications }}</td>
                  <td>{{ m.storageLocation }}</td>
                  <td>{{ m.totalQuantity }}</td>
                  <td>{{ m.availableQuantity }}</td>

                  <!-- ✅ cột dấu cộng -->
                  <td class="action-cell">
                    <button class="icon-btn" @click="handleAdd(m)">
                      {{ expandedModelIds.includes(m.modelId) ? '⮌' : '➕' }}
                    </button>
                  </td>
                </tr>

                <!-- ✅ Bảng con hiển thị danh sách thiết bị -->
                <tr v-if="expandedModelIds.includes(m.modelId)" class="model-details">
                  <td colspan="7">
                    <table class="sub-table">
                      <thead>
                        <tr>
                          <th>Mã thiết bị</th>
                          <th>Trạng thái</th>
                          <th>Vị trí hiện tại</th>
                          <th>Người mượn</th>
                          <th>Ngày mượn</th>
                        </tr>
                      </thead>
                      <tbody>
                        <tr v-for="d in devicesByModel[m.modelId] || []" :key="d.deviceId">
                          <td>{{ d.deviceId }}</td>
                          <td>{{ d.status }}</td>
                          <td>{{ d.currentLocation }}</td>
                          <td>{{ d.borrower || '—' }}</td>
                          <td>{{ formatDate(d.borrowedDate) }}</td>
                        </tr>

                        <tr v-if="(devicesByModel[m.modelId] || []).length === 0">
                          <td colspan="5" style="text-align: center">Không có thiết bị</td>
                        </tr>
                      </tbody>
                    </table>
                  </td>
                </tr>
              </template>

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
import CategoryModal from '@/components/Device/CategoryModal.vue'
import DeviceModal from '@/components/Device/DeviceModal.vue'
import { deviceApi, categoryApi, modelApi } from '@/config/api'
import { useAuthStore } from '@/stores/auth'
import { ref, onMounted } from 'vue'
import { ElMessage } from 'element-plus'

const auth = useAuthStore()
const isAdmin = auth.roleId === 1

// State
const categories = ref([])
const modelsByCategory = ref({})
const selectedCategory = ref(null)
const expandedModelIds = ref([])
const devicesByModel = ref({})
const loading = ref(false)
const error = ref('')
const showCategoryForm = ref(false)
const newCategoryName = ref('')
const showForm = ref(false)
const editingIndex = ref(null)
const items = ref([])
const q = ref('')

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
    categoryId: selectedCategory.value?.id || null,
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
      ElMessage.success('Cập nhật thiết bị thành công!')
    } else {
      await modelApi.create({
        modelName: payload.deviceName,
        categoryId: selectedCategory.value.id,
        specifications: payload.specification,
        storageLocation: payload.storageLocation,
      })
      ElMessage.success('Thêm thiết bị thành công!')
    }

    if (selectedCategory.value) {
      modelsByCategory.value[selectedCategory.value.id] = await categoryApi.getModelsByCategory(
        selectedCategory.value.id,
      )
    }

    closeForm()
  } catch (err) {
    console.error(err)
    ElMessage.error('Lưu thiết bị thất bại!')
  } finally {
    loading.value = false
  }
}

onMounted(() => {
  fetchCategories()
  if (isAdmin) fetchDevices()
})

// Thêm category
function openAddCategory() {
  showCategoryForm.value = true
}
function closeCategoryForm() {
  showCategoryForm.value = false
  newCategoryName.value = ''
}
async function addCategory(payload) {
  if (!payload.name.trim()) {
    ElMessage.warning('Vui lòng nhập tên loại thiết bị')
    return
  }

  try {
    const newCategory = await categoryApi.create({ name: payload.name })
    categories.value.push(newCategory)
    ElMessage.success('Thêm loại thiết bị thành công!')
    closeCategoryForm()
  } catch {
    ElMessage.error('Không thể thêm loại thiết bị')
  }
}

// ✅ Xử lý mở/đóng model & tải danh sách thiết bị
async function handleAdd(model) {
  const id = model.modelId
  const index = expandedModelIds.value.indexOf(id)

  if (index !== -1) {
    // Nếu đang mở thì đóng lại
    expandedModelIds.value.splice(index, 1)
    return
  }

  // Nếu chưa mở thì thêm vào danh sách mở
  expandedModelIds.value.push(id)

  // ✅ Chỉ tải dữ liệu nếu chưa có
  if (!devicesByModel.value[id]) {
    try {
      const data = await deviceApi.getByModel(id)
      devicesByModel.value[id] = data
    } catch (err) {
      console.error('Lỗi tải thiết bị của model', err)
      devicesByModel.value[id] = []
    }
  }
}

function formatDate(dateStr) {
  if (!dateStr) return '—'
  const date = new Date(dateStr)
  return date.toLocaleDateString('vi-VN')
}
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
.content {
  background: #393e46;
  border-radius: 12px;
  box-shadow: 0 0 20px rgba(0, 173, 181, 0.15);
  border: 1px solid rgba(0, 173, 181, 0.2);
  padding: 16px;
}

/* Category cards */
.categories {
  display: grid;
  grid-template-columns: repeat(auto-fill, minmax(180px, 1fr));
  gap: 20px;
  margin-top: 16px;
}
.category-card {
  display: flex;
  align-items: center;
  justify-content: center;
  flex-direction: column;
  background: #222831;
  border: 1px solid rgba(0, 173, 181, 0.2);
  border-radius: 12px;
  height: 120px;
  cursor: pointer;
  transition: 0.2s;
  color: #eeeeee; /* ✅ Chữ chính */
}
.category-card:hover {
  background: rgba(0, 173, 181, 0.1);
  border-color: #00adb5;
  transform: translateY(-3px);
  color: #00adb5; /* ✅ Chữ nhấn */
}
.add-category-card {
  border: 2px dashed rgba(0, 173, 181, 0.5);
  color: #00adb5; /* ✅ Chữ nhấn */
  font-weight: 600;
  background: transparent;
}
.add-category-card:hover {
  background: rgba(0, 173, 181, 0.1);
  border-color: #00adb5;
}
.add-category-card .plus {
  font-size: 36px;
}
.empty {
  color: rgba(238, 238, 238, 0.7); /* ✅ Chữ phụ */
  text-align: center;
  padding: 20px;
}

/* Models View */
.models-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
  margin-bottom: 16px;
}
.left-controls {
  display: flex;
  align-items: center;
  gap: 12px;
}
.left-controls h3 {
  color: #eeeeee; /* ✅ Chữ chính */
}
.add-btn {
  background: #00adb5;
  color: #222831; /* THAY ĐỔI: Chữ tối để tăng tương phản */
  border: none;
  border-radius: 8px;
  padding: 6px 12px;
  cursor: pointer;
  font-weight: 600;
  transition: 0.2s;
}
.add-btn:hover {
  background: #eeeeee;
  color: #222831; /* THÊM: Đảm bảo chữ rõ trên nền hover */
}
.back-btn {
  background: none;
  border: none;
  color: #00adb5; /* ✅ Chữ nhấn */
  font-weight: 600;
  cursor: pointer;
  font-size: 20px;
  padding: 4px 8px;
  transition: 0.2s;
}
.back-btn:hover {
  transform: scale(1.1);
}

/* Tables */
.models-table table {
  width: 100%;
  border-collapse: collapse;
  font-size: 14px;
}
.models-table th,
.models-table td {
  border: 1px solid rgba(0, 173, 181, 0.15);
  padding: 10px 12px;
  text-align: left;
}
/* Hàng <td> trong <tbody> sẽ kế thừa .device -> #eeeeee (Chữ chính) */

.models-table th {
  background: #222831;
  color: #00adb5; /* ✅ Chữ nhấn */
  font-weight: 600;
  text-transform: uppercase;
  font-size: 12px;
}
.models-table tr:hover td {
  background: rgba(0, 173, 181, 0.05);
}

/* THÊM: Áp dụng chữ phụ cho dòng "Không có dữ liệu" */
.models-table td[colspan='7'],
.sub-table td[colspan='5'] {
  color: rgba(238, 238, 238, 0.7); /* ✅ Chữ phụ */
  text-align: center;
}

/* Nút hành động trong bảng */
.action-cell {
  text-align: center;
}
.icon-btn {
  background: none;
  border: none;
  color: #00adb5; /* ✅ Chữ nhấn */
  cursor: pointer;
  font-size: 18px;
  transition: all 0.2s ease;
}
.icon-btn:hover {
  transform: scale(1.2);
}

/* Bảng con (sub-table) */
.model-details > td {
  padding: 12px;
  background: #222831;
}
.sub-table {
  width: 100%;
  border-collapse: collapse;
  margin-top: 6px;
  background: #222831;
}
.sub-table th,
.sub-table td {
  border: 1px solid rgba(0, 173, 181, 0.1);
  padding: 8px 10px;
  font-size: 13px;
}
/* Hàng <td> trong <tbody> sẽ kế thừa .device -> #eeeeee (Chữ chính) */

.sub-table th {
  background: #393e46;
  color: #00adb5; /* ✅ Chữ nhấn */
  font-weight: 600;
  font-size: 12px;
}
.sub-table th:nth-child(4),
.sub-table th:nth-child(5),
.sub-table td:nth-child(4),
.sub-table td:nth-child(5) {
  text-align: center;
  white-space: nowrap;
}
</style>
