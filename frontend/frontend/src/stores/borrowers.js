/**
 * STORE QUẢN LÝ NGƯỜI MƯỢN
 * 
 * Quản lý danh sách người mượn thiết bị
 * Dữ liệu được lưu trong localStorage để duy trì khi reload trang
 */
import { defineStore } from 'pinia'

// Key để lưu dữ liệu trong localStorage
const STORAGE_KEY = 'borrowers.list'

/**
 * Hàm tải dữ liệu ban đầu từ localStorage
 * @returns {Array} Danh sách người mượn
 */
function loadInitial() {
  try {
    const raw = localStorage.getItem(STORAGE_KEY)
    if (raw) return JSON.parse(raw)
  } catch (_) {}
  // Dữ liệu mẫu nếu chưa có
  return [
    { id: 'SV001', name: 'Nguyễn Văn A' },
    { id: 'SV002', name: 'Trần Thị B' },
  ]
}

export const useBorrowersStore = defineStore('borrowers', {
  // Trạng thái ban đầu
  state: () => ({
    borrowers: loadInitial(), // Danh sách người mượn
  }),
  
  // Các action để thao tác với dữ liệu
  actions: {
    /**
     * Lưu dữ liệu vào localStorage
     */
    persist() {
      try { localStorage.setItem(STORAGE_KEY, JSON.stringify(this.borrowers)) } catch (_) {}
    },
    
    /**
     * Thêm người mượn mới
     * @param {Object} borrower - Thông tin người mượn {id, name}
     */
    addBorrower(borrower) {
      this.borrowers.push(borrower)
      this.persist() // Lưu vào localStorage
    },
    
    /**
     * Xóa người mượn theo index
     * @param {number} index - Vị trí trong mảng
     */
    removeBorrower(index) {
      this.borrowers.splice(index, 1)
      this.persist() // Lưu vào localStorage
    },
    
    /**
     * Cập nhật thông tin người mượn
     * @param {number} index - Vị trí trong mảng
     * @param {Object} borrower - Thông tin mới
     */
    updateBorrower(index, borrower) {
      this.borrowers[index] = { ...borrower }
      this.persist() // Lưu vào localStorage
    }
  },
})


