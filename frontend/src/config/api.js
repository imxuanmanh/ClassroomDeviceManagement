/**
 * CẤU HÌNH API - QUẢN LÝ THIẾT BỊ PHÒNG HỌC (CÓ TOKEN ĐẦY ĐỦ)
 */

export const API_CONFIG = {
  BASE_URL: "http://10.10.49.23:5129/api",

  ENDPOINTS: {
    DEVICES: "/instances",
    USERS: "/users",
    BORROWS: "/borrow-requests",
    HISTORY: "/history",
    REPORTS: "/reports",
  },

  TIMEOUT: 10000,

  DEFAULT_HEADERS: {
    "Content-Type": "application/json",
    Accept: "application/json",
  },
};

/**
 * ✅ LẤY TOKEN TỪ LOCALSTORAGE
 */
function getAuthToken() {
  return localStorage.getItem("authToken") || "";
}

/**
 * ✅ TẠO HEADERS VỚI TOKEN TỰ ĐỘNG
 */
function getAuthHeaders(additionalHeaders = {}) {
  const token = getAuthToken();
  const headers = { ...API_CONFIG.DEFAULT_HEADERS, ...additionalHeaders };

  if (token) {
    headers["Authorization"] = `Bearer ${token}`;
  }

  return headers;
}

/**
 * Tạo URL đầy đủ từ endpoint
 */
export function buildApiUrl(endpoint) {
  return `${API_CONFIG.BASE_URL}${endpoint}`;
}

/**
 * ✅ GỌI API CHUNG (TỰ ĐỘNG THÊM TOKEN)
 */
export async function apiCall(endpoint, options = {}) {
  const url = buildApiUrl(endpoint);
  const config = {
    timeout: API_CONFIG.TIMEOUT,
    headers: getAuthHeaders(options.headers),
    ...options,
  };

  try {
    const response = await fetch(url, config);
    const data = await response.json().catch(() => ({}));

    // ✅ Xử lý 401 Unauthorized
    if (response.status === 401) {
      localStorage.removeItem("authToken");
      window.location.href = "/login";
      throw new Error("Phiên đăng nhập đã hết hạn. Vui lòng đăng nhập lại.");
    }

    if (!response.ok) {
      throw new Error(
        data.message || `HTTP ${response.status}: ${response.statusText}`
      );
    }
    return data;
  } catch (error) {
    console.error("API call failed:", error);
    throw error;
  }
}

/**
 * ✅ API XÁC THỰC
 */
export const authApi = {
  login: async (data) => {
    const response = await apiCall("/auth/login", {
      method: "POST",
      body: JSON.stringify(data),
    });

    if (response.token) {
      localStorage.setItem("authToken", response.token);
    }

    return response;
  },

  register: (data) =>
    apiCall("/auth/register", {
      method: "POST",
      body: JSON.stringify(data),
    }),

  logout: () => {
    localStorage.removeItem("authToken");
    window.location.href = "/login";
  },
};

/**
 * ✅ API QUẢN LÝ CATEGORY
 */
export const categoryApi = {
  getAll: () => apiCall("/categories"),
  getModelsByCategory: (id) => apiCall(`/categories/${id}/models`),
  create: (data) =>
    apiCall("/categories", {
      method: "POST",
      body: JSON.stringify(data),
    }),
};

/**
 * ✅ API QUẢN LÝ MODEL
 */
export const modelApi = {
  getAll: () => apiCall("/models"),
  getById: (id) => apiCall(`/models/${id}`),
  create: (data) =>
    apiCall("/models", {
      method: "POST",
      body: JSON.stringify(data),
    }),
  getInstances: (modelId) => apiCall(`/models/${modelId}/instances`),
};

/**
 * ✅ API QUẢN LÝ THIẾT BỊ (DEVICES)
 */
export const deviceApi = {
  getAll: () => apiCall(API_CONFIG.ENDPOINTS.DEVICES),
  getById: (id) => apiCall(`${API_CONFIG.ENDPOINTS.DEVICES}/${id}`),
  create: (data) =>
    apiCall(API_CONFIG.ENDPOINTS.DEVICES, {
      method: "POST",
      body: JSON.stringify(data),
    }),
  update: (id, data) =>
    apiCall(`${API_CONFIG.ENDPOINTS.DEVICES}/${id}`, {
      method: "PUT",
      body: JSON.stringify(data),
    }),
  delete: (id) =>
    apiCall(`${API_CONFIG.ENDPOINTS.DEVICES}/${id}`, {
      method: "DELETE",
    }),
};

/**
 * ✅ API QUẢN LÝ INSTANCES (THIẾT BỊ CỤ THỂ)
 */
export const instanceApi = {
  create: (data) =>
    apiCall("/instances", {
      method: "POST",
      body: JSON.stringify(data),
    }),

  getById: (instanceId) => apiCall(`/instances/${instanceId}`),

  update: (instanceId, data) =>
    apiCall(`/instances/${instanceId}`, {
      method: "PUT",
      body: JSON.stringify(data),
    }),

  delete: (instanceId) =>
    apiCall(`/instances/${instanceId}`, {
      method: "DELETE",
    }),

  updateStatus: (instanceId, statusId) =>
    apiCall(`/instances/${instanceId}/status`, {
      method: "PATCH",
      body: JSON.stringify({ statusId }),
    }),

  getByStatus: (statusId) => apiCall(`/instances/status/${statusId}`),
};

/**
 * ✅ API NGƯỜI DÙNG
 */
export const userApi = {
  getAll: () => apiCall(API_CONFIG.ENDPOINTS.USERS),

  create: (data) =>
    apiCall(API_CONFIG.ENDPOINTS.USERS, {
      method: "POST",
      body: JSON.stringify(data),
    }),

  update: (id, data) =>
    apiCall(`${API_CONFIG.ENDPOINTS.USERS}/${id}`, {
      method: "PUT",
      body: JSON.stringify(data),
    }),

  delete: (id) =>
    apiCall(`${API_CONFIG.ENDPOINTS.USERS}/${id}`, {
      method: "DELETE",
    }),

  getPendingRequests: (userId) =>
    apiCall(`${API_CONFIG.ENDPOINTS.USERS}/${userId}/borrow-requests/pending`),

  getApprovedRequests: (userId) =>
    apiCall(`${API_CONFIG.ENDPOINTS.USERS}/${userId}/borrow-requests/approved`),

  getRejectedRequests: (userId) =>
    apiCall(`${API_CONFIG.ENDPOINTS.USERS}/${userId}/borrow-requests/rejected`),

  getNotifications: (userId) =>
    apiCall(`${API_CONFIG.ENDPOINTS.USERS}/${userId}/notifications`),

  markAllSeen: (userId) =>
    apiCall(`${API_CONFIG.ENDPOINTS.USERS}/${userId}/notifications/seen`, {
      method: "PATCH",
    }),

  deleteAllNotifications: (userId) =>
    apiCall(`${API_CONFIG.ENDPOINTS.USERS}/${userId}/notifications/deleted`, {
      method: "PATCH",
    }),
};

/**
 * ✅ API MƯỢN/TRẢ
 */
export const borrowApi = {
  getByStatus: (status) => apiCall(`/borrow-requests/${status}`),

  approveRequest: async (requestId) => {
    try {
      const res = await fetch(
        buildApiUrl(`/borrow-requests/${requestId}/approve`),
        {
          method: "PATCH",
          headers: getAuthHeaders(),
        }
      );
      const data = await res.json().catch(() => ({}));
      return { ok: res.ok, status: res.status, message: data.message };
    } catch (error) {
      console.error("❌ Lỗi approveRequest:", error);
      return {
        ok: false,
        status: 0,
        message: "Không thể kết nối đến máy chủ.",
      };
    }
  },

  rejectRequest: async (requestId) => {
    try {
      const res = await fetch(
        buildApiUrl(`/borrow-requests/${requestId}/reject`),
        {
          method: "PATCH",
          headers: getAuthHeaders(),
        }
      );
      const data = await res.json().catch(() => ({}));
      return { ok: res.ok, status: res.status, message: data.message };
    } catch (error) {
      console.error("❌ Lỗi rejectRequest:", error);
      return {
        ok: false,
        status: 0,
        message: "Không thể kết nối đến máy chủ.",
      };
    }
  },

  return: async (requestId) => {
    try {
      const res = await fetch(
        buildApiUrl(`/borrow-requests/${requestId}/return`),
        {
          method: "PATCH",
          headers: getAuthHeaders(),
        }
      );
      const data = await res.json().catch(() => ({}));
      return { ok: res.ok, status: res.status, message: data.message };
    } catch (error) {
      console.error("❌ Lỗi return:", error);
      return {
        ok: false,
        status: 0,
        message: "Không thể kết nối đến máy chủ.",
      };
    }
  },

  create: (data) =>
    apiCall("/borrow-requests", {
      method: "POST",
      body: JSON.stringify(data),
    }),

  delete: (requestId) =>
    apiCall(`/borrow-requests/${requestId}`, {
      method: "DELETE",
    }),
};

/**
 * ✅ API LỊCH SỬ
 */
export const historyApi = {
  getAll: () => apiCall(API_CONFIG.ENDPOINTS.HISTORY),
  getByDevice: (deviceId) =>
    apiCall(`${API_CONFIG.ENDPOINTS.HISTORY}/device/${deviceId}`),
  getByUser: (userId) =>
    apiCall(`${API_CONFIG.ENDPOINTS.HISTORY}/user/${userId}`),
};

/**
 * ✅ API BÁO CÁO
 */
export const reportApi = {
  getStats: () => apiCall(`${API_CONFIG.ENDPOINTS.REPORTS}/stats`),

  getBorrowsByPeriod: (startDate, endDate) =>
    apiCall(
      `${API_CONFIG.ENDPOINTS.REPORTS}/borrows?start=${startDate}&end=${endDate}`
    ),

  getByStatus: (statusId) =>
    apiCall(`${API_CONFIG.ENDPOINTS.REPORTS}/status/${statusId}`),

  processReport: async (reportId) => {
    const res = await fetch(
      buildApiUrl(`${API_CONFIG.ENDPOINTS.REPORTS}/${reportId}/process`),
      {
        method: "PATCH",
        headers: getAuthHeaders(),
      }
    );
    return res.ok;
  },

  completeReport: async (reportId, isSuccess) => {
    const res = await fetch(
      buildApiUrl(
        `${API_CONFIG.ENDPOINTS.REPORTS}/${reportId}/complete?isSuccess=${isSuccess}`
      ),
      {
        method: "PATCH",
        headers: getAuthHeaders(),
      }
    );
    return res.ok;
  },

  cancelReport: async (reportId) => {
    const res = await fetch(
      buildApiUrl(`${API_CONFIG.ENDPOINTS.REPORTS}/${reportId}/cancel`),
      {
        method: "PATCH",
        headers: getAuthHeaders(),
      }
    );
    return res.ok;
  },

  createBrokenReport: async (formData) => {
    try {
      const token = getAuthToken();
      const headers = {};
      if (token) {
        headers["Authorization"] = `Bearer ${token}`;
      }

      const response = await fetch(buildApiUrl(API_CONFIG.ENDPOINTS.REPORTS), {
        method: "POST",
        headers: headers,
        body: formData,
      });

      const data = await response.json().catch(() => ({}));
      return {
        ok: response.ok,
        status: response.status,
        message: data.message || "Không có phản hồi từ server.",
      };
    } catch (error) {
      console.error("❌ Lỗi createBrokenReport:", error);
      return {
        ok: false,
        status: 0,
        message: "Không thể kết nối đến máy chủ.",
      };
    }
  },
};

/**
 * ✅ API THÔNG BÁO
 */
export const notificationApi = {
  getByUser: (userId) => apiCall(`/notifications/${userId}`),

  markAllAsRead: (userId) =>
    apiCall(`/notifications/${userId}/seen`, {
      method: "PATCH",
    }),

  // 👇 SỬA LẠI ĐÚNG NHƯ NÀY LÀ ĐƯỢC
  markAsRead: (notificationId) =>
    apiCall(`/notifications/${notificationId}/seen`, {
      method: "PATCH",
    }),

  getStreamUrl: (userId) => buildApiUrl(`/notifications/subscribe/${userId}`),
};
