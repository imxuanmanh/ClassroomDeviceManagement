<!-- 
  TRANG QUẢN LÝ THIẾT BỊ
  - CRUD đầy đủ: thêm, sửa, xóa, tìm kiếm thiết bị
  - Phân trang client-side
  - Lưu dữ liệu trong localStorage
-->
<template>
  <section class="equipment">
    <!-- Header trang với tiêu đề và các nút hành động -->
    <header class="page-header">
      <h2>Danh Sách Thiết Bị</h2>
      <div class="actions">
        <!-- Ô tìm kiếm theo tên hoặc loại thiết bị -->
        <input v-model="q" placeholder="Tìm theo tên/loại" />
        <!-- Nút thêm thiết bị mới -->
        <button @click="openCreate">Thêm</button>
      </div>
    </header>
    
    <div class="content">
      <!-- Form thêm/sửa thiết bị -->
      <div class="tools">
        <EquipmentForm 
          v-if="showForm" 
          :value="form" 
          :submit-text="editingIndex !== null ? 'Cập nhật' : 'Thêm'" 
          :show-cancel="true" 
          @submit="save" 
          @cancel="closeForm" 
        />
      </div>
      
      <!-- Bảng danh sách thiết bị với phân trang -->
      <div class="table-wrap">
        <table class="table">
          <thead>
            <tr>
              <th>Mã</th><th>Tên</th><th>Loại</th><th>Vị trí</th><th>Tổng SL</th><th>Khả dụng</th><th></th>
            </tr>
          </thead>
          <tbody>
            <!-- Hiển thị thiết bị theo trang hiện tại -->
            <tr v-for="(d, i) in paged" :key="d.deviceId">
              <td>{{ d.deviceId }}</td>
              <td>{{ d.deviceName }}</td>
              <td>{{ d.deviceType }}</td>
              <td>{{ d.storageLocation }}</td>
              <td>{{ d.totalQuantity }}</td>
              <td>{{ d.availableQuantity }}</td>
              <td>
                <!-- Nút sửa thiết bị -->
                <button @click="startEdit(d, i + (page-1)*pageSize)">Sửa</button>
                <!-- Nút xóa thiết bị -->
                <button @click="remove(i + (page-1)*pageSize)">Xóa</button>
              </td>
            </tr>
            <!-- Thông báo khi không có dữ liệu -->
            <tr v-if="paged.length === 0"><td colspan="7" style="text-align:center">Không có dữ liệu</td></tr>
          </tbody>
        </table>
        
        <!-- Thanh phân trang -->
        <div class="pager">
          <button :disabled="page===1" @click="page--">Trước</button>
          <span>Trang {{ page }} / {{ totalPages }}</span>
          <button :disabled="page===totalPages" @click="page++">Sau</button>
        </div>
      </div>
    </div>
  </section>
</template>
<script setup>
  import { ref, computed, watch } from 'vue'
  import EquipmentForm from '@/components/Equipment/EquipmentForm.vue'
  
  // Key để lưu dữ liệu trong localStorage
  const STORAGE_KEY = 'equipment.items'
  
  /**
   * Tải dữ liệu thiết bị từ localStorage
   * @returns {Array} Danh sách thiết bị
   */
  function load() {
    try { 
      const raw = localStorage.getItem(STORAGE_KEY)
      if (raw) return JSON.parse(raw) 
    } catch(_){}
    return []
  }
  
  // Dữ liệu và trạng thái
  const items = ref(load()) // Danh sách thiết bị
  const q = ref('') // Từ khóa tìm kiếm
  const page = ref(1) // Trang hiện tại
  const pageSize = 10 // Số item mỗi trang
  const form = ref({ deviceId:'', deviceName:'', deviceType:'', storageLocation:'', totalQuantity:0, availableQuantity:0 }) // Form dữ liệu
  const editingIndex = ref(null) // Index thiết bị đang sửa
  const showForm = ref(false) // Hiển thị form

  // Computed properties
  /**
   * Lọc thiết bị theo từ khóa tìm kiếm
   */
  const filtered = computed(() => {
    const query = q.value.toLowerCase()
    return items.value.filter(d => `${d.deviceId} ${d.deviceName} ${d.deviceType}`.toLowerCase().includes(query))
  })
  
  /**
   * Tính tổng số trang
   */
  const totalPages = computed(() => Math.max(1, Math.ceil(filtered.value.length / pageSize)))
  
  /**
   * Lấy dữ liệu cho trang hiện tại
   */
  const paged = computed(() => filtered.value.slice((page.value-1)*pageSize, page.value*pageSize))
  
  // Watch để điều chỉnh trang khi dữ liệu thay đổi
  watch([items, page, q], () => { 
    if (page.value > totalPages.value) page.value = totalPages.value 
  })

  /**
   * Lưu dữ liệu vào localStorage
   */
  function persist(){ 
    try{ localStorage.setItem(STORAGE_KEY, JSON.stringify(items.value)) }catch(_){}
  }
  
  /**
   * Mở form thêm thiết bị mới
   */
  function openCreate(){ 
    showForm.value = true
    editingIndex.value = null
    form.value = { deviceId:'', deviceName:'', deviceType:'', storageLocation:'', totalQuantity:0, availableQuantity:0 }
  }
  
  /**
   * Mở form sửa thiết bị
   * @param {Object} d - Thiết bị cần sửa
   * @param {number} absoluteIndex - Vị trí trong mảng gốc
   */
  function startEdit(d, absoluteIndex){ 
    showForm.value = true
    editingIndex.value = absoluteIndex
    form.value = { ...d }
  }
  
  /**
   * Đóng form
   */
  function closeForm(){ 
    showForm.value = false 
  }
  
  /**
   * Lưu thiết bị (thêm mới hoặc cập nhật)
   * @param {Object} payload - Dữ liệu thiết bị
   */
  function save(payload){
    if (editingIndex.value !== null) { 
      items.value[editingIndex.value] = payload 
    } else { 
      items.value.unshift(payload) 
    }
    persist()
    closeForm()
  }
  
  /**
   * Xóa thiết bị
   * @param {number} absoluteIndex - Vị trí trong mảng gốc
   */
  function remove(absoluteIndex){ 
    items.value.splice(absoluteIndex,1)
    persist() 
  }
</script>
<style scoped>
.equipment { padding: 16px 12px; }
.page-header { display: flex; align-items: center; justify-content: space-between; gap: 12px; margin-bottom: 16px; }
.page-header h2 { margin: 0; color: #111827; }
.actions { display: flex; gap: 8px; }
.actions input { padding: 6px 8px; border: 1px solid #e5e7eb; border-radius: 8px; }
.content { background: #fff; border-radius: 12px; box-shadow: 0 2px 8px rgba(0,0,0,0.06); padding: 16px; }
.table { width: 100%; border-collapse: collapse; }
.table th, .table td { border: 1px solid #eee; padding: 10px; text-align: left; }
.table th { background: #f8fafc; }
.pager { display: flex; align-items: center; gap: 8px; justify-content: flex-end; padding-top: 10px; }
</style>
