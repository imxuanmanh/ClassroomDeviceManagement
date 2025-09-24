/**
 * STORE LỊCH SỬ MƯỢN/TRẢ THIẾT BỊ
 * 
 * Quản lý lịch sử tất cả các giao dịch mượn và trả thiết bị
 * Mỗi khi có hoạt động mượn/trả sẽ được ghi lại vào đây
 */
import { defineStore } from 'pinia'

// Key để lưu dữ liệu trong localStorage
const STORAGE_KEY = 'loan.history'

/**
 * Hàm tải dữ liệu lịch sử từ localStorage
 * @returns {Array} Danh sách các bản ghi lịch sử
 */
function loadInitial() {
  try {
    const raw = localStorage.getItem(STORAGE_KEY)
    if (raw) return JSON.parse(raw)
  } catch (_) {}
  return [] // Trả về mảng rỗng nếu chưa có dữ liệu
}

export const useHistoryStore = defineStore('history', {
  // Trạng thái ban đầu
  state: () => ({
    records: loadInitial(), // Danh sách các bản ghi lịch sử
  }),
  
  // Các action để thao tác với lịch sử
  actions: {
    /**
     * Lưu dữ liệu lịch sử vào localStorage
     */
    persist() {
      try { localStorage.setItem(STORAGE_KEY, JSON.stringify(this.records)) } catch (_) {}
    },
    
    /**
     * Ghi log khi có thiết bị được mượn
     * @param {Object} data - Thông tin giao dịch mượn
     * @param {string} data.deviceId - Mã thiết bị
     * @param {string} data.deviceName - Tên thiết bị
     * @param {string} data.borrowerId - Mã người mượn
     * @param {string} data.borrowerName - Tên người mượn
     * @param {string} data.date - Ngày mượn
     */
    logBorrow({ deviceId, deviceName, borrowerId, borrowerName, date }) {
      this.records.unshift({ 
        type: 'borrow', 
        deviceId, 
        deviceName, 
        borrowerId, 
        borrowerName, 
        date 
      })
      this.persist() // Lưu vào localStorage
    },
    
    /**
     * Ghi log khi có thiết bị được trả
     * @param {Object} data - Thông tin giao dịch trả
     * @param {string} data.deviceId - Mã thiết bị
     * @param {string} data.deviceName - Tên thiết bị
     * @param {string} data.borrowerId - Mã người mượn
     * @param {string} data.borrowerName - Tên người mượn
     * @param {string} data.date - Ngày trả
     */
    logReturn({ deviceId, deviceName, borrowerId, borrowerName, date }) {
      this.records.unshift({ 
        type: 'return', 
        deviceId, 
        deviceName, 
        borrowerId, 
        borrowerName, 
        date 
      })
      this.persist() // Lưu vào localStorage
    },
    
    /**
     * Xóa toàn bộ lịch sử
     */
    clear() { 
      this.records = []
      this.persist() // Lưu vào localStorage
    }
  },
})


