<template>
  <div class="borrow-page">
    <h2>Quản lý thiết bị</h2>

    <!-- Form thêm/sửa -->
    <div class="form-box">
      <input v-model="form.id" placeholder="Mã thiết bị" />
      <input v-model="form.name" placeholder="Tên thiết bị" />
      <input v-model="form.type" placeholder="Loại thiết bị" />
      <button @click="saveDevice">
        {{ editingIndex !== null ? 'Cập nhật' : 'Thêm' }}
      </button>
      <button v-if="editingIndex !== null" @click="cancelEdit">Hủy</button>
    </div>

    <!-- Bảng danh sách thiết bị -->
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
            <button v-if="device.status === 'Khả dụng'" @click="borrowDevice(device)">Mượn</button>
            <button v-else @click="returnDevice(device)">Trả</button>
            <button @click="editDevice(device, index)">Sửa</button>
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
      form: { id: '', name: '', type: '' },
      editingIndex: null,
    }
  },
  methods: {
    // Thêm hoặc cập nhật thiết bị
    saveDevice() {
      if (this.form.id && this.form.name && this.form.type) {
        if (this.editingIndex !== null) {
          // Cập nhật
          Object.assign(this.devices[this.editingIndex], this.form)
          this.editingIndex = null
        } else {
          // Thêm mới
          this.devices.push({
            ...this.form,
            status: 'Khả dụng',
            borrower: '-',
            borrowDate: '-',
          })
        }
        this.form = { id: '', name: '', type: '' }
      }
    },
    // Hủy chế độ sửa
    cancelEdit() {
      this.form = { id: '', name: '', type: '' }
      this.editingIndex = null
    },
    // Bắt đầu sửa
    editDevice(device, index) {
      this.form = { ...device }
      this.editingIndex = index
    },
    // Xóa thiết bị
    deleteDevice(index) {
      this.devices.splice(index, 1)
    },
    // Mượn thiết bị
    borrowDevice(device) {
      device.status = 'Đang mượn'
      device.borrower = 'Nguyễn Văn B' // có thể thay bằng input
      device.borrowDate = new Date().toISOString().split('T')[0]
    },
    // Trả thiết bị
    returnDevice(device) {
      device.status = 'Khả dụng'
      device.borrower = '-'
      device.borrowDate = '-'
    },
  },
}
</script>

<style scoped>
.borrow-page {
  padding: 20px;
}

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
