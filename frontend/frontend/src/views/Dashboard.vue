<!-- 
  TRANG TỔNG QUAN (DASHBOARD)
  - Hiển thị thống kê tổng quan về thiết bị
  - Thẻ số liệu: tổng thiết bị, đang mượn, khả dụng, lượt mượn
  - Bảng demo thao tác mượn/trả thiết bị
-->
<template>
  <div class="home">
    <h2>Tổng quan</h2>
    
    <!-- Thẻ thống kê tổng quan -->
    <div class="stats">
      <StatCard title="Thiết bị" :value="stats.total.toString()" />
      <StatCard title="Đang mượn" :value="stats.borrowed.toString()" />
      <StatCard title="Khả dụng" :value="stats.available.toString()" />
      <StatCard title="Lượt mượn (30 ngày)" :value="stats.recentBorrows.toString()" />
    </div>

    <!-- Form thêm/sửa thiết bị demo -->
    <div class="form-box">
      <input v-model="form.id" placeholder="Mã thiết bị" />
      <input v-model="form.name" placeholder="Tên thiết bị" />
      <input v-model="form.type" placeholder="Loại thiết bị" />
      <button @click="saveDevice">
        {{ editingIndex !== null ? 'Cập nhật' : 'Thêm' }}
      </button>
      <button v-if="editingIndex !== null" @click="cancelEdit">Hủy</button>
    </div>

    <!-- Bảng danh sách thiết bị mẫu với thao tác mượn/trả -->
    <table class="device-table">
      <thead>
        <tr>
          <th>Mã</th>
          <th>Tên</th>
          <th>Loại</th>
          <th>Trạng thái</th>
          <th>Người mượn</th>
          <th>Ngày mượn</th>
          <th>Hành động</th>
        </tr>
      </thead>
      <tbody>
        <tr v-for="(device, index) in devices" :key="device.id">
          <td>{{ device.id }}</td>
          <td>{{ device.name }}</td>
          <td>{{ device.type }}</td>
          <td>{{ device.status }}</td>
          <td>{{ device.borrower }}</td>
          <td>{{ device.borrowDate }}</td>
          <td>
            <!-- Nút mượn (chỉ hiện khi thiết bị khả dụng) -->
            <button v-if="device.status === 'Khả dụng'" @click="borrowDevice(device)">Mượn</button>
            <!-- Nút trả (chỉ hiện khi thiết bị đang được mượn) -->
            <button v-else @click="returnDevice(device)">Trả</button>
            <!-- Nút sửa thông tin thiết bị -->
            <button @click="editDevice(device, index)">Sửa</button>
            <!-- Nút xóa thiết bị -->
            <button @click="deleteDevice(index)">Xóa</button>
          </td>
        </tr>
      </tbody>
    </table>
  </div>
</template>

<script>
export default {
  name: 'BorrowPage',
  data() {
    return {
      // Danh sách thiết bị mẫu để demo
      devices: [
        {
          id: 'LAP001',
          name: 'Laptop Dell Inspiron 15',
          type: 'Laptop',
          status: 'Khả dụng',
          borrower: '-',
          borrowDate: '-',
        },
        {
          id: 'PJ002',
          name: 'Máy chiếu Epson X05',
          type: 'Máy chiếu',
          status: 'Đang mượn',
          borrower: 'Nguyễn Văn A',
          borrowDate: '2025-09-10',
        },
      ],
      // Form dữ liệu để thêm/sửa thiết bị
      form: { id: '', name: '', type: '' },
      // Index của thiết bị đang được sửa (null nếu đang thêm mới)
      editingIndex: null,
    }
  },
  methods: {
    /**
     * Lưu thiết bị (thêm mới hoặc cập nhật)
     * Kiểm tra dữ liệu đầu vào trước khi lưu
     */
    saveDevice() {
      if (this.form.id && this.form.name && this.form.type) {
        if (this.editingIndex !== null) {
          // Cập nhật thiết bị hiện có
          Object.assign(this.devices[this.editingIndex], this.form)
          this.editingIndex = null
        } else {
          // Thêm thiết bị mới với trạng thái mặc định
          this.devices.push({
            ...this.form,
            status: 'Khả dụng',
            borrower: '-',
            borrowDate: '-',
          })
        }
        // Reset form sau khi lưu
        this.form = { id: '', name: '', type: '' }
      }
    },
    
    /**
     * Hủy chế độ sửa và reset form
     */
    cancelEdit() {
      this.form = { id: '', name: '', type: '' }
      this.editingIndex = null
    },
    
    /**
     * Bắt đầu sửa thiết bị
     * @param {Object} device - Thiết bị cần sửa
     * @param {number} index - Vị trí trong mảng
     */
    editDevice(device, index) {
      this.form = { ...device }
      this.editingIndex = index
    },
    
    /**
     * Xóa thiết bị khỏi danh sách
     * @param {number} index - Vị trí trong mảng
     */
    deleteDevice(index) {
      this.devices.splice(index, 1)
    },
    
    /**
     * Xử lý mượn thiết bị
     * Cập nhật trạng thái và ghi log vào lịch sử
     * @param {Object} device - Thiết bị được mượn
     */
    borrowDevice(device) {
      const borrowerName = 'Nguyễn Văn B'
      device.status = 'Đang mượn'
      device.borrower = borrowerName
      device.borrowDate = new Date().toISOString().split('T')[0]
      
      // Ghi log vào lịch sử
      try {
        const { useHistoryStore } = require('@/stores/history.js')
        const store = useHistoryStore()
        store.logBorrow({
          deviceId: device.id,
          deviceName: device.name,
          borrowerId: '-',
          borrowerName,
          date: device.borrowDate,
        })
      } catch (_) {}
    },
    
    /**
     * Xử lý trả thiết bị
     * Cập nhật trạng thái và ghi log vào lịch sử
     * @param {Object} device - Thiết bị được trả
     */
    returnDevice(device) {
      const date = new Date().toISOString().split('T')[0]
      
      // Ghi log vào lịch sử
      try {
        const { useHistoryStore } = require('@/stores/history.js')
        const store = useHistoryStore()
        store.logReturn({
          deviceId: device.id,
          deviceName: device.name,
          borrowerId: '-',
          borrowerName: device.borrower || '-',
          date,
        })
      } catch (_) {}
      
      // Reset trạng thái thiết bị
      device.status = 'Khả dụng'
      device.borrower = '-'
      device.borrowDate = '-'
    },
  },
  computed: {
    /**
     * Tính toán thống kê tổng quan
     * @returns {Object} Các số liệu thống kê
     */
    stats() {
      const total = this.devices.length
      const borrowed = this.devices.filter(d => d.status !== 'Khả dụng').length
      const available = total - borrowed
      let recentBorrows = 0
      
      // Lấy số lượt mượn trong 30 ngày gần nhất
      try {
        const { useHistoryStore } = require('@/stores/history.js')
        const store = useHistoryStore()
        const from = new Date(); from.setDate(from.getDate() - 30)
        recentBorrows = store.records.filter(r => r.type==='borrow' && new Date(r.date) >= from).length
      } catch (_) {}
      
      return { total, borrowed, available, recentBorrows }
    }
  },
}
</script>

<script setup>
import StatCard from '@/components/Dashboard/StatCard.vue'
</script>

<style scoped>
.home { padding: 16px 12px; }
.stats { display: grid; grid-template-columns: repeat(4, minmax(0,1fr)); gap: 12px; margin-bottom: 16px; }

.form-box {
  margin-bottom: 15px;
}

.form-box input {
  margin-right: 10px;
  padding: 5px;
}

.device-table {
  width: 100%;
  border-collapse: collapse;
}

.device-table th,
.device-table td {
  border: 1px solid #ddd;
  padding: 8px;
  text-align: center;
}

.device-table th {
  background-color: #f4f4f4;
}

.device-table tr:nth-child(even) {
  background-color: #f9f9f9;
}

.device-table button {
  margin: 2px;
  padding: 5px 10px;
  border: none;
  background-color: #2196f3;
  color: white;
  cursor: pointer;
  border-radius: 4px;
}

.device-table button:hover {
  background-color: #1976d2;
}
</style>
