/**
 * CẤU HÌNH API - QUẢN LÝ THIẾT BỊ PHÒNG HỌC
 */

export const API_CONFIG = {
  BASE_URL: 'http://192.168.1.75:5129/api',

  ENDPOINTS: {
    DEVICES: '/device',
    USERS: '/users',
    BORROWS: '/borrow-requests',
    HISTORY: '/history',
    REPORTS: '/reports',
  },

  TIMEOUT: 10000,

  DEFAULT_HEADERS: {
    'Content-Type': 'application/json',
    Accept: 'application/json',
  },
}

/**
 * Tạo URL đầy đủ từ endpoint
 */
export function buildApiUrl(endpoint) {
  return `${API_CONFIG.BASE_URL}${endpoint}`
}

/**
 * Gọi API chung
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
    const data = await response.json().catch(() => ({}))
    if (!response.ok) {
      throw new Error(data.message || `HTTP ${response.status}: ${response.statusText}`)
    }
    return data
  } catch (error) {
    console.error('API call failed:', error)
    throw error
  }
}

/**
 * API QUẢN LÝ THIẾT BỊ
 */
export const categoryApi = {
  getAll: () => apiCall('/categories'),
  getModelsByCategory: (id) => apiCall(`/categories/${id}/models`),
  create: (data) =>
    apiCall('/categories', {
      method: 'POST',
      body: JSON.stringify(data),
    }),
}

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
  getAll: () => apiCall(API_CONFIG.ENDPOINTS.DEVICES),
  getById: (id) => apiCall(`${API_CONFIG.ENDPOINTS.DEVICES}/${id}`),
  create: (data) =>
    apiCall(API_CONFIG.ENDPOINTS.DEVICES, {
      method: 'POST',
      body: JSON.stringify(data),
    }),
  update: (id, data) =>
    apiCall(`${API_CONFIG.ENDPOINTS.DEVICES}/${id}`, {
      method: 'PUT',
      body: JSON.stringify(data),
    }),
  delete: (id) =>
    apiCall(`${API_CONFIG.ENDPOINTS.DEVICES}/${id}`, {
      method: 'DELETE',
    }),
}

/**
 * API NGƯỜI DÙNG
 */
export const userApi = {
  getAll: () => apiCall(API_CONFIG.ENDPOINTS.USERS),
  create: (data) =>
    apiCall(API_CONFIG.ENDPOINTS.USERS, {
      method: 'POST',
      body: JSON.stringify(data),
    }),
  update: (id, data) =>
    apiCall(`${API_CONFIG.ENDPOINTS.USERS}/${id}`, {
      method: 'PUT',
      body: JSON.stringify(data),
    }),
  delete: (id) =>
    apiCall(`${API_CONFIG.ENDPOINTS.USERS}/${id}`, {
      method: 'DELETE',
    }),
}

/**
 * API MƯỢN/TRẢ
 */
export const borrowApi = {
  /**
   * ✅ Lấy danh sách yêu cầu theo trạng thái
   * @param {'pending' | 'approved' | 'rejected' | 'returned'} status - Trạng thái cần lấy
   */
  getByStatus: (status) => {
    const endpoint = `${API_CONFIG.ENDPOINTS.BORROWS}/${status}`
    return apiCall(endpoint)
  },

  /**
   * ✅ Chấp nhận yêu cầu
   */
  approveRequest: async (requestId) => {
    try {
      const res = await fetch(
        `${API_CONFIG.BASE_URL}${API_CONFIG.ENDPOINTS.BORROWS}/${requestId}/approve`,
        {
          method: 'PATCH',
          headers: API_CONFIG.DEFAULT_HEADERS,
        },
      )
      const data = await res.json().catch(() => ({}))
      return {
        ok: res.ok,
        status: res.status,
        message: data.message || 'Không có phản hồi từ server.',
      }
    } catch (error) {
      console.error('❌ Lỗi khi gọi API approveRequest:', error)
      return {
        ok: false,
        status: 0,
        message: 'Không thể kết nối đến máy chủ.',
      }
    }
  },

  /**
   * ❌ Từ chối yêu cầu
   */
  rejectRequest: async (requestId) => {
    try {
      const res = await fetch(
        `${API_CONFIG.BASE_URL}${API_CONFIG.ENDPOINTS.BORROWS}/${requestId}/reject`,
        {
          method: 'PATCH',
          headers: API_CONFIG.DEFAULT_HEADERS,
        },
      )
      const data = await res.json().catch(() => ({}))
      return {
        ok: res.ok,
        status: res.status,
        message: data.message || 'Không có phản hồi từ server.',
      }
    } catch (error) {
      console.error('❌ Lỗi khi gọi API rejectRequest:', error)
      return {
        ok: false,
        status: 0,
        message: 'Không thể kết nối đến máy chủ.',
      }
    }
  },

  /**
   * ➕ Tạo yêu cầu mượn thiết bị mới
   */
  create: (data) =>
    apiCall(API_CONFIG.ENDPOINTS.BORROWS, {
      method: 'POST',
      body: JSON.stringify(data),
    }),

  /**
   * 🔁 Xác nhận trả thiết bị
   */
  return: (id) =>
    apiCall(`${API_CONFIG.ENDPOINTS.BORROWS}/${id}/return`, {
      method: 'POST',
    }),
}

/**
 * API LỊCH SỬ
 */
export const historyApi = {
  getAll: () => apiCall(API_CONFIG.ENDPOINTS.HISTORY),
  getByDevice: (deviceId) => apiCall(`${API_CONFIG.ENDPOINTS.HISTORY}/device/${deviceId}`),
  getByUser: (userId) => apiCall(`${API_CONFIG.ENDPOINTS.HISTORY}/user/${userId}`),
}

/**
 * API BÁO CÁO
 */
export const reportApi = {
  getStats: () => apiCall(`${API_CONFIG.ENDPOINTS.REPORTS}/stats`),
  getBorrowsByPeriod: (startDate, endDate) =>
    apiCall(`${API_CONFIG.ENDPOINTS.REPORTS}/borrows?start=${startDate}&end=${endDate}`),
}

/**
 * API XÁC THỰC
 */
export const authApi = {
  login: (data) =>
    apiCall('/auth/login', {
      method: 'POST',
      body: JSON.stringify(data),
    }),

  register: (data) =>
    apiCall('/auth/register', {
      method: 'POST',
      body: JSON.stringify(data),
    }),
}

// /**
//  * CẤU HÌNH API - QUẢN LÝ THIẾT BỊ PHÒNG HỌC
//  *
//  * File này chứa tất cả cấu hình API và các hàm gọi API cho hệ thống quản lý thiết bị.
//  * Tập trung hóa việc quản lý endpoint, giúp dễ dàng thay đổi và bảo trì.
//  */

// // Cấu hình chung cho API
// export const API_CONFIG = {
//   // Địa chỉ cơ sở của server API
//   BASE_URL: 'http://192.168.1.75:5129/api',

//   // Danh sách các endpoint API
//   ENDPOINTS: {
//     DEVICES: '/device', // Quản lý thiết bị
//     USERS: '/users', // Quản lý người dùng
//     BORROWS: '/borrow-requests', // Quản lý mượn/trả
//     HISTORY: '/history', // Lịch sử hoạt động
//     REPORTS: '/reports', // Báo cáo thống kê
//   },

//   // Thời gian chờ tối đa cho mỗi request (10 giây)
//   TIMEOUT: 10000,

//   // Headers mặc định cho tất cả request
//   DEFAULT_HEADERS: {
//     'Content-Type': 'application/json',
//     Accept: 'application/json',
//   },
// }

// /**
//  * Hàm tạo URL đầy đủ từ endpoint
//  * @param {string} endpoint - Đường dẫn endpoint (ví dụ: '/device')
//  * @returns {string} URL đầy đủ
//  */
// export function buildApiUrl(endpoint) {
//   return `${API_CONFIG.BASE_URL}${endpoint}`
// }

// /**
//  * Hàm gọi API chung với xử lý lỗi thống nhất
//  * @param {string} endpoint - Đường dẫn API
//  * @param {object} options - Tùy chọn request (method, body, headers...)
//  * @returns {Promise} Kết quả từ API
//  */
// export async function apiCall(endpoint, options = {}) {
//   const url = buildApiUrl(endpoint)
//   const config = {
//     timeout: API_CONFIG.TIMEOUT,
//     headers: { ...API_CONFIG.DEFAULT_HEADERS, ...options.headers },
//     ...options,
//   }

//   try {
//     const response = await fetch(url, config)
//     if (!response.ok) {
//       throw new Error(`HTTP ${response.status}: ${response.statusText}`)
//     }
//     return await response.json()
//   } catch (error) {
//     console.error('API call failed:', error)
//     throw error
//   }
// }

// /**
//  * API QUẢN LÝ THIẾT BỊ
//  * Các hàm CRUD cho thiết bị phòng học
//  */
// // API cho loại thiết bị
// export const categoryApi = {
//   getAll: () => apiCall('/categories'),
//   getModelsByCategory: (id) => apiCall(`/categories/${id}/models`),
//   create: (data) =>
//     apiCall('/categories', {
//       method: 'POST',
//       body: JSON.stringify(data),
//     }),
// }
// // API cho mẫu thiết bị
// export const modelApi = {
//   getAll: () => apiCall('/models'),
//   getById: (id) => apiCall(`/models/${id}`),
//   create: (data) =>
//     apiCall('/models', {
//       method: 'POST',
//       body: JSON.stringify(data),
//     }),
// }
// // API cho thiết bị
// export const deviceApi = {
//   // Lấy danh sách tất cả thiết bị
//   getAll: () => apiCall(API_CONFIG.ENDPOINTS.DEVICES),

//   // Lấy thông tin thiết bị theo ID
//   getById: (id) => apiCall(`${API_CONFIG.ENDPOINTS.DEVICES}/${id}`),

//   // Tạo thiết bị mới
//   create: (data) =>
//     apiCall(API_CONFIG.ENDPOINTS.DEVICES, {
//       method: 'POST',
//       body: JSON.stringify(data),
//     }),

//   // Cập nhật thông tin thiết bị
//   update: (id, data) =>
//     apiCall(`${API_CONFIG.ENDPOINTS.DEVICES}/${id}`, {
//       method: 'PUT',
//       body: JSON.stringify(data),
//     }),

//   // Xóa thiết bị
//   delete: (id) =>
//     apiCall(`${API_CONFIG.ENDPOINTS.DEVICES}/${id}`, {
//       method: 'DELETE',
//     }),
// }

// /**
//  * API QUẢN LÝ NGƯỜI DÙNG
//  * Các hàm CRUD cho người mượn thiết bị
//  */
// export const userApi = {
//   // Lấy danh sách tất cả người dùng
//   getAll: () => apiCall(API_CONFIG.ENDPOINTS.USERS),

//   // Tạo người dùng mới
//   create: (data) =>
//     apiCall(API_CONFIG.ENDPOINTS.USERS, {
//       method: 'POST',
//       body: JSON.stringify(data),
//     }),

//   // Cập nhật thông tin người dùng
//   update: (id, data) =>
//     apiCall(`${API_CONFIG.ENDPOINTS.USERS}/${id}`, {
//       method: 'PUT',
//       body: JSON.stringify(data),
//     }),

//   // Xóa người dùng
//   delete: (id) =>
//     apiCall(`${API_CONFIG.ENDPOINTS.USERS}/${id}`, {
//       method: 'DELETE',
//     }),
// }

// /**
//  * API QUẢN LÝ MƯỢN/TRẢ
//  * Các hàm xử lý việc mượn và trả thiết bị
//  */
// export const borrowApi = {
//   // Lấy danh sách tất cả giao dịch mượn/trả
//   getAll: () => apiCall(`${API_CONFIG.ENDPOINTS.BORROWS}/pending`),

//   // ✅ Chấp nhận yêu cầu mượn
//   approveRequest: (requestId) =>
//     apiCall(`${API_CONFIG.ENDPOINTS.BORROWS}/${requestId}/approve`, 'PATCH'),

//   rejectRequest: (requestId) =>
//     apiCall(`${API_CONFIG.ENDPOINTS.BORROWS}/${requestId}/reject`, 'PATCH'),

//   // Tạo giao dịch mượn mới
//   create: (data) =>
//     apiCall(API_CONFIG.ENDPOINTS.BORROWS, {
//       method: 'POST',
//       body: JSON.stringify(data),
//     }),

//   // Xử lý trả thiết bị
//   return: (id) =>
//     apiCall(`${API_CONFIG.ENDPOINTS.BORROWS}/${id}/return`, {
//       method: 'POST',
//     }),
// }

// /**
//  * API LỊCH SỬ HOẠT ĐỘNG
//  * Các hàm truy vấn lịch sử mượn/trả thiết bị
//  */
// export const historyApi = {
//   // Lấy toàn bộ lịch sử
//   getAll: () => apiCall(API_CONFIG.ENDPOINTS.HISTORY),

//   // Lấy lịch sử theo thiết bị
//   getByDevice: (deviceId) => apiCall(`${API_CONFIG.ENDPOINTS.HISTORY}/device/${deviceId}`),

//   // Lấy lịch sử theo người dùng
//   getByUser: (userId) => apiCall(`${API_CONFIG.ENDPOINTS.HISTORY}/user/${userId}`),
// }

// /**
//  * API BÁO CÁO THỐNG KÊ
//  * Các hàm tạo báo cáo và thống kê
//  */
// export const reportApi = {
//   // Lấy thống kê tổng quan
//   getStats: () => apiCall(`${API_CONFIG.ENDPOINTS.REPORTS}/stats`),

//   // Lấy báo cáo mượn theo khoảng thời gian
//   getBorrowsByPeriod: (startDate, endDate) =>
//     apiCall(`${API_CONFIG.ENDPOINTS.REPORTS}/borrows?start=${startDate}&end=${endDate}`),
// }

// /**
//  * API XÁC THỰC
//  * Các hàm liên quan đến đăng nhập, đăng xuất
//  */
// export const authApi = {
//   login: (data) =>
//     apiCall('/auth/login', {
//       method: 'POST',
//       body: JSON.stringify(data),
//     }),

//   // Đăng ký
//   register: (data) =>
//     apiCall('/auth/register', {
//       method: 'POST',
//       body: JSON.stringify(data),
//     }),
// }
