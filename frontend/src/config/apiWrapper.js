/**
 * API Wrapper - Cho phép switch giữa Real API và Mock API
 * Logic: Ưu tiên LocalStorage -> Sau đó đến file .env
 */

import * as realApi from './api'
import * as mockApi from '../mock/apimock'

// 🔥 HÀM KIỂM TRA CHẾ ĐỘ (SMART CHECK)
const getUseMock = () => {
  // 1. Kiểm tra trong LocalStorage trình duyệt trước (để switch nóng)
  const localSetting = localStorage.getItem('USE_MOCK')
  if (localSetting !== null) {
    return localSetting === 'true'
  }

  // 2. Nếu không có, lấy từ file môi trường .env (Vite dùng import.meta.env)
  return import.meta.env.VITE_USE_MOCK === 'true'
}

// Gọi hàm để xác định chế độ hiện tại
const USE_MOCK = getUseMock()

// Log ra console để bạn biết đang chạy chế độ nào
console.log(
  `%c 📡 API MODE: ${USE_MOCK ? 'MOCK DATA (Dữ liệu giả)' : 'REAL API (Server thật)'} `,
  `background: ${USE_MOCK ? '#faad14' : '#52c41a'}; color: white; font-weight: bold; padding: 4px; border-radius: 4px;`,
)

/**
 * Category API Wrapper
 */
export const categoryApi = {
  getAll: async () => {
    if (USE_MOCK) return mockApi.mockGetCategories()
    return realApi.categoryApi.getAll()
  },
  getModelsByCategory: async (id) => {
    if (USE_MOCK) return mockApi.mockGetModelsByCategory(id)
    return realApi.categoryApi.getModelsByCategory(id)
  },
  create: async (data) => {
    if (USE_MOCK) return mockApi.mockCreateCategory(data)
    return realApi.categoryApi.create(data)
  },
}

/**
 * Model API Wrapper
 */
export const modelApi = {
  getAll: async () => {
    if (USE_MOCK) return [] // Mock chưa cần hàm này
    return realApi.modelApi.getAll()
  },
  getById: async (id) => {
    if (USE_MOCK) return null
    return realApi.modelApi.getById(id)
  },
  create: async (data) => {
    if (USE_MOCK) return mockApi.mockCreateModel(data)
    return realApi.modelApi.create(data)
  },
  getInstances: async (modelId) => {
    if (USE_MOCK) return mockApi.mockGetInstances(modelId)
    return realApi.modelApi.getInstances(modelId)
  },
}

/**
 * Instance API Wrapper
 */
export const instanceApi = {
  create: async (data) => {
    if (USE_MOCK) return mockApi.mockCreateInstance(data)
    return realApi.instanceApi.create(data)
  },
  getById: async (instanceId) => {
    if (USE_MOCK) return null
    return realApi.instanceApi.getById(instanceId)
  },
  getByModelId: async (modelId) => {
    if (USE_MOCK) return mockApi.mockGetInstances(modelId)
    return realApi.instanceApi.getByModelId(modelId)
  },
  update: async (instanceId, data) => {
    if (USE_MOCK) return data
    return realApi.instanceApi.update(instanceId, data)
  },
  delete: async (instanceId) => {
    if (USE_MOCK) return { success: true }
    return realApi.instanceApi.delete(instanceId)
  },
  updateStatus: async (instanceId, statusId) => {
    if (USE_MOCK) return { success: true }
    return realApi.instanceApi.updateStatus(instanceId, statusId)
  },
  getByStatus: async (statusId) => {
    if (USE_MOCK) return []
    return realApi.instanceApi.getByStatus(statusId)
  },
}

/**
 * Device API Wrapper
 */
export const deviceApi = {
  getAll: async () => {
    if (USE_MOCK) return []
    return realApi.deviceApi.getAll()
  },
  getById: async (id) => {
    if (USE_MOCK) return null
    return realApi.deviceApi.getById(id)
  },
  create: async (data) => {
    if (USE_MOCK) return data
    return realApi.deviceApi.create(data)
  },
  update: async (id, data) => {
    if (USE_MOCK) return data
    return realApi.deviceApi.update(id, data)
  },
  delete: async (id) => {
    if (USE_MOCK) return { success: true }
    return realApi.deviceApi.delete(id)
  },
}

/**
 * Report API Wrapper
 */
export const reportApi = {
  getByStatus: async (statusId) => {
    if (USE_MOCK) return mockApi.mockGetReportsByStatus(statusId)
    return realApi.reportApi.getByStatus(statusId) // Đảm bảo api.js có hàm này
  },
  processReport: async (reportId) => {
    if (USE_MOCK) return mockApi.mockProcessReport(reportId)
    return realApi.reportApi.process(reportId)
  },
  completeReport: async (reportId, isSuccess) => {
    if (USE_MOCK) return mockApi.mockCompleteReport(reportId, isSuccess)
    return realApi.reportApi.complete(reportId, isSuccess)
  },
  cancelReport: async (reportId) => {
    if (USE_MOCK) return mockApi.mockCancelReport(reportId)
    return realApi.reportApi.cancel(reportId)
  },
}

/**
 * User API Wrapper
 */
export const userApi = {
  getAll: async () => {
    if (USE_MOCK) return mockApi.mockGetUsers()
    return realApi.userApi.getAll()
  },
}

/**
 * Auth API Wrapper
 */
export const authApi = {
  register: async (form) => {
    if (USE_MOCK) return mockApi.mockRegister(form)
    return realApi.authApi.register(form)
  },
  login: async (credentials) => {
    if (USE_MOCK) {
      // Mock login trả về token giả
      return {
        token: 'mock-token-123456',
        user: { username: credentials.username, role: 1 },
      }
    }
    return realApi.authApi.login(credentials)
  },
}
