/**
 * CẤU HÌNH API - QUẢN LÝ THIẾT BỊ PHÒNG HỌC
 */

export const API_CONFIG = {
  BASE_URL: 'http://192.168.103.78:5129/api',

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
  // ✅ SỬA LẠI: Truyền đúng cách vào apiCall
  getInstances: (modelId) => apiCall(`/models/${modelId}/instances`),
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

  getPendingRequests: (userId) =>
    apiCall(`${API_CONFIG.ENDPOINTS.USERS}/${userId}/borrow-requests/pending`),

  getApprovedRequests: (userId) =>
    apiCall(`${API_CONFIG.ENDPOINTS.USERS}/${userId}/borrow-requests/approved`),

  getRejectedRequests: (userId) =>
    apiCall(`${API_CONFIG.ENDPOINTS.USERS}/${userId}/borrow-requests/rejected`),
}

/**
 * ✨ API QUẢN LÝ INSTANCES (THIẾT BỊ CỤ THỂ) - MỚI
 */
export const instanceApi = {
  /**
   * Tạo instance mới
   * @param {Object} data - Dữ liệu instance
   * @param {number} data.modelId - ID của model
   * @param {string} data.instanceCode - Mã thiết bị (VD: CAM-001)
   * @param {string} data.currentLocation - Vị trí hiện tại
   * @param {number} data.statusId - ID trạng thái (1: Khả dụng, 2: Đang mượn, 3: Bảo trì, 4: Hỏng)
   * @param {string} data.notes - Ghi chú (optional)
   */
  create: (data) =>
    apiCall('/instances', {
      method: 'POST',
      body: JSON.stringify(data),
    }),

  /**
   * Lấy thông tin instance theo ID
   */
  getById: (instanceId) => apiCall(`/instances/${instanceId}`),

  /**
   * Cập nhật thông tin instance
   */
  update: (instanceId, data) =>
    apiCall(`/instances/${instanceId}`, {
      method: 'PUT',
      body: JSON.stringify(data),
    }),

  /**
   * Xóa instance
   */
  delete: (instanceId) =>
    apiCall(`/instances/${instanceId}`, {
      method: 'DELETE',
    }),

  /**
   * Cập nhật trạng thái instance
   */
  updateStatus: (instanceId, statusId) =>
    apiCall(`/instances/${instanceId}/status`, {
      method: 'PATCH',
      body: JSON.stringify({ statusId }),
    }),

  /**
   * Lấy danh sách instances theo trạng thái
   * @param {number} statusId - 1: Khả dụng, 2: Đang mượn, 3: Bảo trì, 4: Hỏng
   */
  getByStatus: (statusId) => apiCall(`/instances/status/${statusId}`),
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

  /** ❌ Xóa yêu cầu */
  delete: (requestId) =>
    apiCall(`${API_CONFIG.ENDPOINTS.BORROWS}/${requestId}`, {
      method: 'DELETE',
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

  /**
   * ⚠️ Tạo báo cáo thiết bị hỏng
   * @param {FormData} formData - FormData chứa UserId, InstanceId, Description, và image
   */
  createBrokenReport: async (formData) => {
    try {
      const response = await fetch(`${API_CONFIG.BASE_URL}${API_CONFIG.ENDPOINTS.REPORTS}`, {
        method: 'POST',
        body: formData,
        // Không set Content-Type header khi dùng FormData
      })

      const data = await response.json().catch(() => ({}))

      return {
        ok: response.ok,
        status: response.status,
        message: data.message || data || 'Không có phản hồi từ server.',
      }
    } catch (error) {
      console.error('❌ Lỗi khi gọi API createBrokenReport:', error)
      return {
        ok: false,
        status: 0,
        message: 'Không thể kết nối đến máy chủ.',
      }
    }
  },
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
