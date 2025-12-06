<template>
  <section class="device">
    <header class="page-header">
      <h2>Danh Sách Thiết bị</h2>
    </header>

    <div class="content">
      <DeviceModal
        v-if="showModelForm"
        :value="form"
        :title="editingIndex !== null ? 'Cập nhật Model' : 'Thêm Model mới'"
        :submit-text="editingIndex !== null ? 'Cập nhật' : 'Thêm'"
        @submit="saveModel"
        @close="closeModelForm"
      />

      <InstanceModal
        v-if="showInstanceForm"
        :title="`Thêm thiết bị vào ${selectedModelName}`"
        :model-id="selectedModelId"
        :model-name="selectedModelName"
        :initial-location="selectedModelLocation"
        submit-text="Thêm thiết bị"
        @submit="saveInstance"
        @close="closeInstanceForm"
      />

      <div v-if="!selectedCategory" class="categories">
        <div
          v-for="category in categories"
          :key="category.id"
          class="category-card"
          @click="openCategory(category)"
        >
          <h3>{{ category.name }}</h3>
        </div>

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

      <div v-if="selectedCategory" class="models-view">
        <div class="models-header">
          <div class="left-controls">
            <button class="back-btn" @click="backToCategories">⮌</button>
            <h3>{{ selectedCategory.name }}</h3>
          </div>

          <button v-if="isAdmin" class="add-btn" @click="openCreate">➕ Thêm Model</button>
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

                  <td class="action-cell">
                    <button
                      class="toggle-btn"
                      :class="{ 'is-expanded': expandedModelIds.includes(m.modelId) }"
                      @click="handleToggleDetails(m.modelId)"
                      :disabled="loadingInstances[m.modelId]"
                    >
                      <span v-if="loadingInstances[m.modelId]" class="loading-icon">⏳</span>
                      <template v-else>
                        <span class="btn-icon">{{
                          expandedModelIds.includes(m.modelId) ? '▲' : '▼'
                        }}</span>
                        <span class="btn-text">{{
                          expandedModelIds.includes(m.modelId) ? 'Thu gọn' : 'Chi tiết'
                        }}</span>
                      </template>
                    </button>
                  </td>
                </tr>

                <tr v-if="expandedModelIds.includes(m.modelId)" class="model-details">
                  <td colspan="7">
                    <table class="sub-table">
                      <thead>
                        <tr>
                          <th>Mã thiết bị</th>
                          <th>Trạng thái</th>
                          <th>Vị trí hiện tại</th>
                          <th>Người mượn</th>
                          <th>Thời gian sử dụng</th>
                        </tr>
                      </thead>
                      <tbody>
                        <tr
                          v-for="instance in instancesByModel[m.modelId] || []"
                          :key="instance.instanceId"
                        >
                          <td>{{ instance.instanceCode }}</td>
                          <td>
                            <span :class="['status-badge', getStatusClass(instance.statusId)]">
                              {{ getStatusText(instance.statusId) }}
                            </span>
                          </td>
                          <td>{{ instance.currentLocation }}</td>
                          <td>{{ instance.borrower || '---' }}</td>
                          <td>{{ instance.usageDuration || '---' }}</td>
                        </tr>

                        <tr v-if="isAdmin" class="add-instance-row">
                          <td colspan="5">
                            <button
                              class="add-instance-btn"
                              @click="openAddInstance(m.modelId, m.modelName)"
                            >
                              ➕ Thêm thiết bị mới
                            </button>
                          </td>
                        </tr>

                        <tr v-if="(instancesByModel[m.modelId] || []).length === 0 && !isAdmin">
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
import InstanceModal from '@/components/Device/InstanceModal.vue'
import { deviceApi, categoryApi, modelApi, instanceApi } from '@/config/apiWrapper'
import { useDeviceStore } from '@/stores/device'
import { ref, onMounted } from 'vue'
// 👇 IMPORT TOAST (Thay thế ElMessage)
import { toast } from '@/utils/toast.js'

const isAdmin = true
const deviceStore = useDeviceStore()

// State
const selectedCategory = ref(null)
const expandedModelIds = ref([])
const instancesByModel = ref({})
const loadingInstances = ref({})
const loading = ref(false)
const error = ref('')
const showCategoryForm = ref(false)
const newCategoryName = ref('')
const showModelForm = ref(false)
const editingIndex = ref(null)
const showInstanceForm = ref(false)
const selectedModelId = ref(null)
const selectedModelName = ref('')
const selectedModelLocation = ref('')

const form = ref({
  deviceId: '',
  deviceName: '',
  specification: '',
  storageLocation: '',
})

const categories = ref([])
const modelsByCategory = ref({})

// ✅ Fetch categories
async function fetchCategories() {
  loading.value = true
  try {
    categories.value = await categoryApi.getAll()
    await deviceStore.fetchCategories()
  } catch {
    error.value = 'Không thể tải danh sách loại thiết bị'
    // 🔥 Toast error
    toast.error('Không thể tải danh sách loại thiết bị')
  } finally {
    loading.value = false
  }
}

// ✅ Open category và refresh store
async function openCategory(category) {
  selectedCategory.value = category
  loading.value = true
  try {
    const models = await categoryApi.getModelsByCategory(category.id)
    modelsByCategory.value[category.id] = models

    // 🔄 Cập nhật Store để Dashboard nhận được update
    await deviceStore.fetchModelsByCategory(category.id)
  } catch {
    // 🔥 Toast error
    toast.error('Không thể tải danh sách model')
    modelsByCategory.value[category.id] = []
  } finally {
    loading.value = false
  }
}

function backToCategories() {
  selectedCategory.value = null
  expandedModelIds.value = []
  instancesByModel.value = {}
}

// ✅ CRUD Model
function openCreate() {
  showModelForm.value = true
  editingIndex.value = null
  form.value = {
    deviceId: '',
    deviceName: '',
    specification: '',
    storageLocation: '',
  }
}

function saveModel(payload) {
  save(payload)
}

function closeModelForm() {
  showModelForm.value = false
}

async function save(payload) {
  loading.value = true
  try {
    if (editingIndex.value !== null) {
      await deviceApi.update(payload.deviceId, payload)
      // 🔥 Toast success
      toast.success('Cập nhật model thành công!')
    } else {
      await modelApi.create({
        modelName: payload.deviceName,
        categoryId: selectedCategory.value.id,
        specifications: payload.specification,
        storageLocation: payload.storageLocation,
      })
      // 🔥 Toast success
      toast.success('Thêm model thành công!')
    }

    // Reload models cho category hiện tại
    if (selectedCategory.value) {
      const models = await categoryApi.getModelsByCategory(selectedCategory.value.id)
      modelsByCategory.value[selectedCategory.value.id] = models

      await deviceStore.fetchModelsByCategory(selectedCategory.value.id)
    }

    closeModelForm()
  } catch {
    // 🔥 Toast error
    toast.error('Lưu thiết bị thất bại!')
  } finally {
    loading.value = false
  }
}

// ✅ Thêm category
function openAddCategory() {
  showCategoryForm.value = true
}

function closeCategoryForm() {
  showCategoryForm.value = false
  newCategoryName.value = ''
}

async function addCategory(payload) {
  if (!payload.name.trim()) {
    // 🔥 Toast warning
    toast.warning('Vui lòng nhập tên loại thiết bị')
    return
  }

  try {
    await categoryApi.create({ name: payload.name })
    await fetchCategories()

    // 🔄 Refresh store để Dashboard update
    await deviceStore.fetchCategories()

    // 🔥 Toast success
    toast.success('Thêm loại thiết bị thành công!')
    closeCategoryForm()
  } catch {
    // 🔥 Toast error
    toast.error('Không thể thêm loại thiết bị')
  }
}

// ✅ Toggle chi tiết instances
async function handleToggleDetails(modelId) {
  const index = expandedModelIds.value.indexOf(modelId)

  if (index !== -1) {
    expandedModelIds.value.splice(index, 1)
    return
  }

  expandedModelIds.value = []
  expandedModelIds.value.push(modelId)

  loadingInstances.value[modelId] = true
  try {
    const data = await modelApi.getInstances(modelId)
    instancesByModel.value[modelId] = data

    // 🔄 Cập nhật Store để Dashboard tự động update stats
    await deviceStore.fetchModelsByCategory(selectedCategory.value.id)
  } catch (err) {
    console.error('❌ Lỗi tải instances của model:', err)
    // 🔥 Toast error
    toast.error('Không thể tải danh sách thiết bị chi tiết')
    instancesByModel.value[modelId] = []
  } finally {
    loadingInstances.value[modelId] = false
  }
}

function openAddInstance(modelId, modelName) {
  editingIndex.value = null

  const models = modelsByCategory.value[selectedCategory.value.id] || []
  const model = models.find((m) => m.modelId === modelId)

  selectedModelId.value = modelId
  selectedModelName.value = modelName
  selectedModelLocation.value = model?.storageLocation || 'Kho'

  showInstanceForm.value = true
}

function closeInstanceForm() {
  showInstanceForm.value = false
  selectedModelId.value = null
  selectedModelName.value = ''
  selectedModelLocation.value = ''
}

async function saveInstance(payload) {
  loading.value = true
  try {
    await instanceApi.create(payload)
    // 🔥 Toast success
    toast.success('Thêm thiết bị thành công!')

    // Refresh instances nếu đang mở chi tiết
    if (expandedModelIds.value && expandedModelIds.value.includes(payload.modelId)) {
      loadingInstances.value[payload.modelId] = true
      const data = await modelApi.getInstances(payload.modelId)
      instancesByModel.value[payload.modelId] = data
      loadingInstances.value[payload.modelId] = false
    }

    // 🔄 Cập nhật models cho category để Dashboard nhận update
    if (selectedCategory.value) {
      const models = await categoryApi.getModelsByCategory(selectedCategory.value.id)
      modelsByCategory.value[selectedCategory.value.id] = models

      // 🔄 Cập nhật Store để Dashboard tự động update
      await deviceStore.fetchModelsByCategory(selectedCategory.value.id)
    }

    closeInstanceForm()
  } catch (err) {
    console.error('Save instance error:', err)
    // 🔥 Toast error
    toast.error('Lỗi khi thêm thiết bị: ' + (err.message || 'Không rõ'))
  } finally {
    loading.value = false
  }
}

// Helper functions
function getStatusText(statusId) {
  const statusMap = {
    1: 'Khả dụng',
    2: 'Đang mượn',
    3: 'Bảo trì',
    4: 'Hỏng',
  }
  return statusMap[statusId] || 'Không rõ'
}

function getStatusClass(statusId) {
  const classMap = {
    1: 'status-available',
    2: 'status-borrowed',
    3: 'status-maintenance',
    4: 'status-broken',
  }
  return classMap[statusId] || ''
}

onMounted(() => {
  fetchCategories()
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

.content {
  background: #393e46;
  border-radius: 12px;
  box-shadow: 0 0 20px rgba(0, 173, 181, 0.15);
  border: 1px solid rgba(0, 173, 181, 0.2);
  padding: 16px;
}

/* ===== CATEGORY CARDS ===== */
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
  transition: all 0.3s ease;
  color: #eeeeee;
}

.category-card:hover {
  background: rgba(0, 173, 181, 0.1);
  border-color: #00adb5;
  transform: translateY(-3px);
  color: #00adb5;
}

.add-category-card {
  border: 2px dashed rgba(0, 173, 181, 0.5);
  color: #00adb5;
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
  color: rgba(238, 238, 238, 0.7);
  text-align: center;
  padding: 20px;
}

/* ===== MODELS VIEW ===== */
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
  margin: 0;
  color: #eeeeee;
}

.add-btn {
  background: #00adb5;
  color: #222831;
  border: none;
  border-radius: 8px;
  padding: 8px 16px;
  cursor: pointer;
  font-weight: 600;
  transition: all 0.3s ease;
}

.add-btn:hover {
  background: #00c9d4;
  transform: translateY(-2px);
  box-shadow: 0 4px 12px rgba(0, 173, 181, 0.4);
}

.back-btn {
  background: none;
  border: none;
  color: #00adb5;
  font-weight: 600;
  cursor: pointer;
  font-size: 20px;
  padding: 4px 8px;
  transition: all 0.3s ease;
}

.back-btn:hover {
  transform: scale(1.2);
}

/* ===== MAIN TABLE ===== */
.models-table table {
  width: 100%;
  border-collapse: separate;
  border-spacing: 0 10px;
  margin-top: -10px;
}

.models-table td {
  border: none;
  padding: 20px 24px;
  color: #eeeeee;
  vertical-align: middle;
}

.models-table tbody tr td:last-child {
  border-top-right-radius: 12px;
  border-bottom-right-radius: 12px;
}

.models-table tbody tr td:first-child {
  border-top-left-radius: 12px;
  border-bottom-left-radius: 12px;
}

.models-table th,
.models-table td {
  border: 1px solid rgba(0, 173, 181, 0.15);
  padding: 12px 16px;
  text-align: left;
}

.models-table th {
  background: transparent;
  color: #94a3b8;
  font-weight: 700;
  text-transform: uppercase;
  font-size: 12px;
  letter-spacing: 1px;
  padding: 0 24px 12px 24px;
  border: none;
}

.models-table tbody tr:not(.model-details) {
  background: #2b323b;
  box-shadow:
    0 4px 6px -1px rgba(0, 0, 0, 0.1),
    0 2px 4px -1px rgba(0, 0, 0, 0.06);
  transition: all 0.2s ease;
}

.models-table td[colspan='7'],
.sub-table td[colspan='5'] {
  color: rgba(238, 238, 238, 0.7);
  text-align: center;
}

/* ===== TOGGLE BUTTON ===== */
.action-cell {
  text-align: center;
}

.toggle-btn {
  display: inline-flex;
  align-items: center;
  gap: 6px;
  padding: 8px 16px;
  background: linear-gradient(135deg, #00adb5 0%, #007b82 100%);
  border: none;
  border-radius: 8px;
  color: #ffffff;
  font-weight: 600;
  font-size: 13px;
  cursor: pointer;
  transition: all 0.3s ease;
  box-shadow: 0 2px 8px rgba(0, 173, 181, 0.3);
  white-space: nowrap;
}

.toggle-btn:hover:not(:disabled) {
  background: linear-gradient(135deg, #00c9d4 0%, #00adb5 100%);
  transform: translateY(-2px);
  box-shadow: 0 4px 12px rgba(0, 173, 181, 0.5);
}

.toggle-btn:active:not(:disabled) {
  transform: translateY(0);
  box-shadow: 0 2px 6px rgba(0, 173, 181, 0.4);
}

.toggle-btn.is-expanded {
  background: linear-gradient(135deg, #ef4444 0%, #dc2626 100%);
  box-shadow: 0 2px 8px rgba(239, 68, 68, 0.3);
}

.toggle-btn.is-expanded:hover:not(:disabled) {
  background: linear-gradient(135deg, #f87171 0%, #ef4444 100%);
  box-shadow: 0 4px 12px rgba(239, 68, 68, 0.5);
}

.toggle-btn:disabled {
  opacity: 0.6;
  cursor: not-allowed;
  background: #393e46;
}

.toggle-btn .btn-icon {
  font-size: 12px;
  transition: transform 0.3s ease;
}

.toggle-btn:hover:not(:disabled) .btn-icon {
  transform: scale(1.2);
}

.toggle-btn .loading-icon {
  animation: spin 1s linear infinite;
}

@keyframes spin {
  from {
    transform: rotate(0deg);
  }
  to {
    transform: rotate(360deg);
  }
}

/* ===== SUB TABLE (INSTANCES) ===== */
.model-details > td {
  padding: 20px;
  background: #222831 !important;
  cursor: default;
  border-bottom: none;
}

.models-table tr.model-details:hover td {
  background: #222831 !important;
}

.sub-table {
  width: 100%;
  border-collapse: separate;
  border-spacing: 0;
  margin-top: 0;
  background: #1b1f24;
  border: 1px solid #00adb5;
  border-radius: 12px;
  overflow: hidden;
  box-shadow: 0 8px 20px rgba(0, 0, 0, 0.4);
}

.sub-table th {
  background: #2c3440;
  color: #00adb5;
  font-weight: 700;
  padding: 12px 16px;
  text-transform: uppercase;
  font-size: 12px;
  border-bottom: 1px solid rgba(0, 173, 181, 0.2);
  border-right: 1px solid rgba(255, 255, 255, 0.05);
}

.sub-table th:last-child {
  border-right: none;
}

.sub-table td {
  padding: 12px 16px;
  font-size: 13px;
  border-bottom: 1px solid rgba(255, 255, 255, 0.05);
  border-right: 1px solid rgba(255, 255, 255, 0.05);
  color: #eeeeee;
}

.sub-table td:last-child {
  border-right: none;
}

.sub-table tbody tr:not(.add-instance-row):last-child td {
  border-bottom: none;
}

.sub-table tbody tr:not(.add-instance-row) {
  transition: background 0.2s ease;
}

.sub-table tbody tr:not(.add-instance-row):hover {
  background: rgba(0, 173, 181, 0.15);
}

.sub-table th:nth-child(4),
.sub-table th:nth-child(5),
.sub-table td:nth-child(4),
.sub-table td:nth-child(5) {
  text-align: center;
}

/* ===== STATUS BADGES ===== */
.status-badge {
  display: inline-flex;
  align-items: center;
  gap: 8px;
  padding: 6px 12px;
  border-radius: 99px;
  font-size: 13px;
  font-weight: 600;
  line-height: 1;
  transition: all 0.3s ease;
}

.status-badge::before {
  content: '';
  display: block;
  width: 8px;
  height: 8px;
  border-radius: 50%;
  background-color: currentColor;
  box-shadow: 0 0 8px currentColor;
}

.status-available {
  background: rgba(34, 197, 94, 0.1);
  color: #4ade80;
  border: 1px solid rgba(34, 197, 94, 0.2);
}

.status-borrowed {
  background: rgba(250, 204, 21, 0.1);
  color: #facc15;
  border: 1px solid rgba(250, 204, 21, 0.2);
}

.status-maintenance {
  background: rgba(96, 165, 250, 0.1);
  color: #60a5fa;
  border: 1px solid rgba(96, 165, 250, 0.2);
}

.status-broken {
  background: rgba(248, 113, 113, 0.1);
  color: #f87171;
  border: 1px solid rgba(248, 113, 113, 0.2);
}

/* ===== ADD INSTANCE BUTTON ===== */
.add-instance-row {
  background: transparent !important;
}

.add-instance-row:hover {
  background: transparent !important;
}

.add-instance-btn {
  width: 100%;
  padding: 12px;
  background: transparent;
  border: 2px dashed rgba(0, 173, 181, 0.4);
  border-radius: 8px;
  color: #00adb5;
  font-weight: 600;
  cursor: pointer;
  transition: all 0.3s ease;
  font-size: 14px;
}

.add-instance-btn:hover {
  background: rgba(0, 173, 181, 0.1);
  border-color: #00adb5;
  transform: translateY(-2px);
  color: #00adb5;
  box-shadow: 0 4px 12px rgba(0, 173, 181, 0.2);
}
</style>
