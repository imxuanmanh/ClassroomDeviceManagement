/**
 * CẤU HÌNH API - QUẢN LÝ THIẾT BỊ PHÒNG HỌC
 *
 * File này chứa tất cả cấu hình API và các hàm gọi API cho hệ thống quản lý thiết bị.
 * Tập trung hóa việc quản lý endpoint, giúp dễ dàng thay đổi và bảo trì.
 */

// Cấu hình chung cho API
export const API_CONFIG = {
  // Địa chỉ cơ sở của server API
  BASE_URL: 'http://172.16.0.155:5129/api',

  // Danh sách các endpoint API
  ENDPOINTS: {
    DEVICES: '/device', // Quản lý thiết bị
    USERS: '/users', // Quản lý người dùng
    BORROWS: '/borrows', // Quản lý mượn/trả
    HISTORY: '/history', // Lịch sử hoạt động
    REPORTS: '/reports', // Báo cáo thống kê
  },

  // Thời gian chờ tối đa cho mỗi request (10 giây)
  TIMEOUT: 10000,

  // Headers mặc định cho tất cả request
  DEFAULT_HEADERS: {
    'Content-Type': 'application/json',
    Accept: 'application/json',
  },
}

/**
 * Hàm tạo URL đầy đủ từ endpoint
 * @param {string} endpoint - Đường dẫn endpoint (ví dụ: '/device')
 * @returns {string} URL đầy đủ
 */
export function buildApiUrl(endpoint) {
  return `${API_CONFIG.BASE_URL}${endpoint}`
}

/**
 * Hàm gọi API chung với xử lý lỗi thống nhất
 * @param {string} endpoint - Đường dẫn API
 * @param {object} options - Tùy chọn request (method, body, headers...)
 * @returns {Promise} Kết quả từ API
 */
export async function apiCall(endpoint, options = {}) {
  const url = buildApiUrl(endpoint)
  const config = {
    timeout: API_CONFIG.TIMEOUT,
    headers: { ...API_CONFIG.DEFAULT_HEADERS, ...options.headers },
    ...options,
  }

  try {
    const response = await fetch(url, config)
    if (!response.ok) {
      throw new Error(`HTTP ${response.status}: ${response.statusText}`)
    }
    return await response.json()
  } catch (error) {
    console.error('API call failed:', error)
    throw error
  }
}

/**
 * API QUẢN LÝ THIẾT BỊ
 * Các hàm CRUD cho thiết bị phòng học
 */
// API cho loại thiết bị
export const categoryApi = {
  getAll: () => apiCall('/categories'),
  getModelsByCategory: (id) => apiCall(`/models/${id}`),
}

// API cho mẫu thiết bị
export const modelApi = {
  getAll: () => apiCall('/models'),
  getById: (id) => apiCall(`/models/${id}`),
  create: (data) =>
    apiCall('/models', {
      method: 'POST',
      body: JSON.stringify(data),
    }),
}

export const deviceApi = {
  // Lấy danh sách tất cả thiết bị
  getAll: () => apiCall(API_CONFIG.ENDPOINTS.DEVICES),

  // Lấy thông tin thiết bị theo ID
  getById: (id) => apiCall(`${API_CONFIG.ENDPOINTS.DEVICES}/${id}`),

  // Tạo thiết bị mới
  create: (data) =>
    apiCall(API_CONFIG.ENDPOINTS.DEVICES, {
      method: 'POST',
      body: JSON.stringify(data),
    }),

  // Cập nhật thông tin thiết bị
  update: (id, data) =>
    apiCall(`${API_CONFIG.ENDPOINTS.DEVICES}/${id}`, {
      method: 'PUT',
      body: JSON.stringify(data),
    }),

  // Xóa thiết bị
  delete: (id) =>
    apiCall(`${API_CONFIG.ENDPOINTS.DEVICES}/${id}`, {
      method: 'DELETE',
    }),
}

/**
 * API QUẢN LÝ NGƯỜI DÙNG
 * Các hàm CRUD cho người mượn thiết bị
 */
export const userApi = {
  // Lấy danh sách tất cả người dùng
  getAll: () => apiCall(API_CONFIG.ENDPOINTS.USERS),

  // Tạo người dùng mới
  create: (data) =>
    apiCall(API_CONFIG.ENDPOINTS.USERS, {
      method: 'POST',
      body: JSON.stringify(data),
    }),

  // Cập nhật thông tin người dùng
  update: (id, data) =>
    apiCall(`${API_CONFIG.ENDPOINTS.USERS}/${id}`, {
      method: 'PUT',
      body: JSON.stringify(data),
    }),

  // Xóa người dùng
  delete: (id) =>
    apiCall(`${API_CONFIG.ENDPOINTS.USERS}/${id}`, {
      method: 'DELETE',
    }),
}

/**
 * API QUẢN LÝ MƯỢN/TRẢ
 * Các hàm xử lý việc mượn và trả thiết bị
 */
export const borrowApi = {
  // Lấy danh sách tất cả giao dịch mượn/trả
  getAll: () => apiCall(API_CONFIG.ENDPOINTS.BORROWS),

  // Tạo giao dịch mượn mới
  create: (data) =>
    apiCall(API_CONFIG.ENDPOINTS.BORROWS, {
      method: 'POST',
      body: JSON.stringify(data),
    }),

  // Xử lý trả thiết bị
  return: (id) =>
    apiCall(`${API_CONFIG.ENDPOINTS.BORROWS}/${id}/return`, {
      method: 'POST',
    }),
}

/**
 * API LỊCH SỬ HOẠT ĐỘNG
 * Các hàm truy vấn lịch sử mượn/trả thiết bị
 */
export const historyApi = {
  // Lấy toàn bộ lịch sử
  getAll: () => apiCall(API_CONFIG.ENDPOINTS.HISTORY),

  // Lấy lịch sử theo thiết bị
  getByDevice: (deviceId) => apiCall(`${API_CONFIG.ENDPOINTS.HISTORY}/device/${deviceId}`),

  // Lấy lịch sử theo người dùng
  getByUser: (userId) => apiCall(`${API_CONFIG.ENDPOINTS.HISTORY}/user/${userId}`),
}

/**
 * API BÁO CÁO THỐNG KÊ
 * Các hàm tạo báo cáo và thống kê
 */
export const reportApi = {
  // Lấy thống kê tổng quan
  getStats: () => apiCall(`${API_CONFIG.ENDPOINTS.REPORTS}/stats`),

  // Lấy báo cáo mượn theo khoảng thời gian
  getBorrowsByPeriod: (startDate, endDate) =>
    apiCall(`${API_CONFIG.ENDPOINTS.REPORTS}/borrows?start=${startDate}&end=${endDate}`),
}
