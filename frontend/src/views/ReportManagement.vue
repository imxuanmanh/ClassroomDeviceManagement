<template>
  <div class="report-management">
    <div class="page-header">
      <h1>Báo Cáo Hư Hỏng</h1>
    </div>

    <!-- Tab Navigation -->
    <div class="tabs-container">
      <button
        v-for="(tab, index) in tabs"
        :key="index"
        :class="['tab-button', { active: activeTab === tab.status }]"
        @click="handleTabChange(tab.statusId)"
      >
        <span class="tab-label">{{ tab.label }}</span>
        <span class="tab-badge">{{ getReportCount(tab.status) }}</span>
      </button>
    </div>

    <!-- Reports List -->
    <div class="reports-section">
      <div v-if="loading" class="loading-state">
        <p>Đang tải dữ liệu...</p>
      </div>
      <div v-else-if="filteredReports.length === 0" class="empty-state">
        <div class="empty-icon">📋</div>
        <p>Không có báo cáo nào đang xử lý</p>
      </div>

      <div v-else class="reports-list">
        <div
          v-for="report in filteredReports"
          :key="report.reportId"
          class="report-card"
        >
          <div class="report-body">
            <div class="report-date">{{ formatDate(report.createdDate) }}</div>
            <div class="report-info">
              <p><strong>Người báo cáo:</strong> {{ report.reporterName }}</p>
              <p><strong>Thiết bị:</strong> {{ report.deviceName }}</p>
              <p><strong>Vấn đề:</strong></p>
              <p class="description">{{ report.description }}</p>

              <div v-if="report.image" class="image-section">
                <p><strong>Hình ảnh:</strong></p>
                <div class="image-container">
                  <button
                    class="btn btn-view-detail"
                    @click="viewImageDetail(report)"
                  >
                    Xem ảnh
                  </button>
                </div>
              </div>
            </div>
          </div>

          <div class="report-footer">
            <div class="action-buttons">
              <button
                v-if="activeTab === 'pending'"
                class="btn btn-primary"
                @click="updateReportStatus(report.reportId, 'processing')"
              >
                Xử lý
              </button>
              <button
                v-else-if="activeTab === 'processing'"
                class="btn btn-success"
                @click="updateReportStatus(report.reportId, 'completed')"
              >
                Đã xử lý
              </button>
              <button
                v-if="activeTab === 'pending'"
                class="btn btn-danger"
                @click="rejectReport(report.reportId)"
              >
                Từ chối
              </button>
              <button
                v-else-if="activeTab === 'processing'"
                class="btn btn-warning"
                @click="updateReportStatus(report.reportId, 'failed')"
              >
                Không thể xử lý
              </button>
            </div>
          </div>
        </div>
      </div>
    </div>

    <!-- Image Modal -->
    <div v-if="showImageModal" class="modal-overlay" @click="closeImageModal">
      <div class="modal-content" @click.stop>
        <div class="modal-header">
          <h3>Chi tiết hình ảnh</h3>
          <button class="modal-close" @click="closeImageModal">✕</button>
        </div>
        <div class="modal-body" v-if="selectedReport">
          <img
            :src="selectedReport.image"
            alt="Report image"
            class="modal-image"
          />
        </div>
      </div>
    </div>
  </div>
</template>

<script>
import { ref, computed, onMounted } from "vue";
//import { reportApi } from '@/config/apiWrapper.js'
import { reportApi, API_CONFIG } from "@/config/api.js";
// 👇 Import bộ Toast vừa tạo
import { toast } from "@/utils/toast.js";

export default {
  name: "ReportManagement",
  setup() {
    const activeTab = ref("pending");
    const showImageModal = ref(false);
    const selectedReport = ref(null);
    const loading = ref(false);

    const tabs = [
      { status: "pending", label: "Đang chờ xử lý", statusId: 1 },
      { status: "processing", label: "Đang xử lý", statusId: 2 },
      { status: "completed", label: "Đã hoàn thành", statusId: 3 },
    ];

    const reports = ref([]);
    const reportsByStatus = ref({ 1: [], 2: [], 3: [] });

    // ... (Giữ nguyên logic mapStatusIdToStatus, formatDate, getReportCount, helper...)
    const mapStatusIdToStatus = (id) =>
      ({ 1: "pending", 2: "processing", 3: "completed" })[id] || "pending";

    const getReportCount = (status) => {
      const map = { pending: 1, processing: 2, completed: 3 };
      return reportsByStatus.value[map[status]]?.length || 0;
    };

    const formatDate = (date) =>
      date ? new Date(date).toLocaleDateString("vi-VN") : "";

    // Hàm remove local helper
    const removeReportFromLocal = (statusId, reportId) => {
      const idx = reportsByStatus.value[statusId].findIndex(
        (r) => r.reportId === reportId
      );
      if (idx !== -1) reportsByStatus.value[statusId].splice(idx, 1);

      if (activeTab.value === mapStatusIdToStatus(statusId)) {
        const vIdx = reports.value.findIndex((r) => r.reportId === reportId);
        if (vIdx !== -1) reports.value.splice(vIdx, 1);
      }
    };

    // Fetch Data
    const fetchReportsForTab = async (statusId) => {
      try {
        const data = await reportApi.getByStatus(statusId);
        const baseUrlWithoutApi = API_CONFIG.BASE_URL.replace("/api", "");

        const transformedData = data.map((item) => ({
          reportId: item.reportId,
          status: mapStatusIdToStatus(item.status),
          reporterName: item.userFullName,
          deviceName: item.deviceName,
          description: item.description,
          image: item.imagePath
            ? item.imagePath.startsWith("http")
              ? item.imagePath
              : `${baseUrlWithoutApi}${item.imagePath}`
            : null,
          createdDate: item.reportDate
            ? new Date(item.reportDate).toISOString().split("T")[0]
            : "",
        }));

        reportsByStatus.value[statusId] = transformedData;

        const currentMap = { pending: 1, processing: 2, completed: 3 };
        if (currentMap[activeTab.value] === statusId) {
          reports.value = transformedData;
        }
      } catch (error) {
        console.error(error);
        reportsByStatus.value[statusId] = [];
      }
    };

    // --- CẬP NHẬT TRẠNG THÁI (Dùng Toast) ---
    const updateReportStatus = async (reportId, newStatus) => {
      try {
        if (activeTab.value === "pending") {
          await reportApi.processReport(reportId);
          removeReportFromLocal(1, reportId);
          fetchReportsForTab(2);

          // 🔥 Toast Thành công
          toast.success("Đã chuyển sang trạng thái đang xử lý!");
        } else if (activeTab.value === "processing") {
          const isSuccess = newStatus !== "failed";
          await reportApi.completeReport(reportId, isSuccess);
          removeReportFromLocal(2, reportId);
          if (isSuccess) fetchReportsForTab(3);

          // 🔥 Toast tùy biến
          if (isSuccess) {
            toast.success("Báo cáo đã hoàn thành!");
          } else {
            toast.warning("Đã xác nhận không thể xử lý!");
          }
        }
      } catch (error) {
        console.error(error);
        // 🔥 Toast Lỗi
        toast.error("Có lỗi xảy ra, vui lòng thử lại.");
      }
    };

    // --- TỪ CHỐI (Dùng Confirm Dialog đẹp) ---
    const rejectReport = async (reportId) => {
      // 🔥 Hộp thoại xác nhận xịn xò
      const confirmed = await toast.confirm(
        "Từ chối báo cáo?",
        "Hành động này không thể hoàn tác.",
        "Từ chối ngay"
      );

      if (confirmed) {
        try {
          await reportApi.cancelReport(reportId);
          removeReportFromLocal(1, reportId);

          toast.success("Đã từ chối báo cáo.");
        } catch (error) {
          toast.error("Lỗi khi từ chối báo cáo.");
        }
      }
    };

    // --- Các hàm Modal ảnh giữ nguyên ---
    const viewImageDetail = (r) => {
      selectedReport.value = r;
      showImageModal.value = true;
    };
    const closeImageModal = () => {
      showImageModal.value = false;
      selectedReport.value = null;
    };

    const handleTabChange = (statusId) => {
      activeTab.value = mapStatusIdToStatus(statusId);
      reports.value = reportsByStatus.value[statusId] || [];
      fetchReportsForTab(statusId);
    };

    onMounted(() => {
      loading.value = true;
      Promise.all([1, 2, 3].map((id) => fetchReportsForTab(id))).finally(
        () => (loading.value = false)
      );
    });

    return {
      activeTab,
      tabs,
      reports,
      filteredReports: computed(() => reports.value),
      getReportCount,
      formatDate,
      updateReportStatus,
      rejectReport,
      viewImageDetail,
      closeImageModal,
      showImageModal,
      selectedReport,
      handleTabChange,
      loading,
    };
  },
};
</script>

<!-- <script>
import { ref, computed, onMounted } from 'vue'
import { API_CONFIG } from '../config/api'

export default {
  name: 'ReportManagement',
  setup() {
    const activeTab = ref('pending')
    const showImageModal = ref(false)
    const selectedReport = ref(null)
    const loading = ref(false)

    const tabs = [
      { status: 'pending', label: 'Đang chờ xử lý', statusId: 1 },
      { status: 'processing', label: 'Đang xử lý', statusId: 2 },
      { status: 'completed', label: 'Đã hoàn thành', statusId: 3 },
    ]

    const reports = ref([])

    // Fetch reports từ API
    const fetchReports = async (statusId) => {
      loading.value = true
      try {
        const url = `${API_CONFIG.BASE_URL}${API_CONFIG.ENDPOINTS.REPORTS}/status/${statusId}`
        const response = await fetch(url, {
          headers: API_CONFIG.DEFAULT_HEADERS,
        })

        if (!response.ok) {
          throw new Error(`HTTP ${response.status}`)
        }

        const data = await response.json()

        // Get base URL without "/api"
        const baseUrlWithoutApi = API_CONFIG.BASE_URL.replace('/api', '')

        // Transform API data to match component structure
        const transformedData = data.map((item) => ({
          reportId: item.reportId,
          status: mapStatusIdToStatus(item.status),
          reporterName: item.userFullName,
          deviceName: item.deviceName,
          description: item.description,
          image: item.imagePath ? `${baseUrlWithoutApi}${item.imagePath}` : null,
          createdDate: new Date(item.reportDate).toISOString().split('T')[0],
        }))

        reports.value = transformedData
      } catch (error) {
        console.error('Error fetching reports:', error)
        reports.value = []
      } finally {
        loading.value = false
      }
    }

    // Map status ID từ API thành status string
    const mapStatusIdToStatus = (statusId) => {
      const statusMap = { 1: 'pending', 2: 'processing', 3: 'completed' }
      return statusMap[statusId] || 'pending'
    }

    // Watch activeTab để fetch dữ liệu khi tab thay đổi
    const handleTabChange = (statusId) => {
      activeTab.value = statusId === 1 ? 'pending' : statusId === 2 ? 'processing' : 'completed'
      // Lấy dữ liệu từ reportsByStatus thay vì fetch lại
      reports.value = reportsByStatus.value[statusId] || []
    }

    // Lưu trữ dữ liệu từng tab để hiển thị count badge đúng
    const reportsByStatus = ref({
      1: [], // pending
      2: [], // processing
      3: [], // completed
    })

    // Fetch dữ liệu cho một tab cụ thể và lưu vào reportsByStatus
    const fetchReportsForTab = async (statusId) => {
      try {
        const url = `${API_CONFIG.BASE_URL}${API_CONFIG.ENDPOINTS.REPORTS}/status/${statusId}`
        const response = await fetch(url, {
          headers: API_CONFIG.DEFAULT_HEADERS,
        })

        if (!response.ok) {
          throw new Error(`HTTP ${response.status}`)
        }

        const data = await response.json()
        const baseUrlWithoutApi = API_CONFIG.BASE_URL.replace('/api', '')

        const transformedData = data.map((item) => ({
          reportId: item.reportId,
          status: mapStatusIdToStatus(item.status),
          reporterName: item.userFullName,
          deviceName: item.deviceName,
          description: item.description,
          image: item.imagePath ? `${baseUrlWithoutApi}${item.imagePath}` : null,
          createdDate: new Date(item.reportDate).toISOString().split('T')[0],
        }))

        reportsByStatus.value[statusId] = transformedData

        // Nếu tab này là tab hiện tại, cập nhật reports để hiển thị
        if (
          activeTab.value ===
          (statusId === 1 ? 'pending' : statusId === 2 ? 'processing' : 'completed')
        ) {
          reports.value = transformedData
        }
      } catch (error) {
        console.error(`Error fetching reports for status ${statusId}:`, error)
        reportsByStatus.value[statusId] = []
      }
    }

    onMounted(() => {
      // Fetch dữ liệu cho cả 3 tab khi component load
      fetchReportsForTab(1)
      fetchReportsForTab(2)
      fetchReportsForTab(3)
    })

    // Computed
    const filteredReports = computed(() => {
      return reports.value
    })

    const getReportCount = (status) => {
      const statusIdMap = { pending: 1, processing: 2, completed: 3 }
      const statusId = statusIdMap[status]
      return reportsByStatus.value[statusId]?.length || 0
    }

    const formatDate = (dateString) => {
      const options = { year: 'numeric', month: '2-digit', day: '2-digit' }
      return new Date(dateString).toLocaleDateString('vi-VN', options)
    }

    const updateReportStatus = (reportId, newStatus) => {
      const report = reports.value.find((r) => r.reportId === reportId)
      if (!report) return

      if (activeTab.value === 'pending') {
        const url = `${API_CONFIG.BASE_URL}${API_CONFIG.ENDPOINTS.REPORTS}/${reportId}/process`
        console.log('Processing report with URL:', url) // Debug log
        fetch(url, {
          method: 'PATCH',
          headers: API_CONFIG.DEFAULT_HEADERS,
        })
          .then((response) => {
            console.log('Response status:', response.status) // Debug log
            if (response.ok) {
              // Remove the report from local list if API call succeeds
              const index = reports.value.findIndex((r) => r.reportId === reportId)
              if (index !== -1) {
                reports.value.splice(index, 1)
                // Update reportsByStatus
                const statusIdIndex = reportsByStatus.value[1].findIndex(
                  (r) => r.reportId === reportId,
                )
                if (statusIdIndex !== -1) {
                  reportsByStatus.value[1].splice(statusIdIndex, 1)
                }
              }
              // Refresh tab 2 (processing) để hiển thị báo cáo mới
              fetchReportsForTab(2)
            } else {
              console.error('Failed to process report:', response.status)
              alert('Không thể xử lý báo cáo. Vui lòng thử lại.')
            }
          })
          .catch((error) => {
            console.error('Error processing report:', error)
            alert('Lỗi khi xử lý báo cáo. Vui lòng thử lại.')
          })
      } else if (activeTab.value === 'processing') {
        // Determine isSuccess based on button clicked
        const isSuccess = newStatus !== 'failed'
        const url = `${API_CONFIG.BASE_URL}${API_CONFIG.ENDPOINTS.REPORTS}/${reportId}/complete?isSuccess=${isSuccess}`
        console.log('Complete report URL:', url)
        console.log('isSuccess value:', isSuccess)
        fetch(url, {
          method: 'PATCH',
          headers: API_CONFIG.DEFAULT_HEADERS,
        })
          .then((response) => {
            console.log('Complete report response status:', response.status)
            return response.text().then((text) => {
              console.log('Complete report response body:', text)
              return response
            })
          })
          .then((response) => {
            if (response.ok) {
              // Remove the report from local list if API call succeeds
              const index = reports.value.findIndex((r) => r.reportId === reportId)
              if (index !== -1) {
                reports.value.splice(index, 1)
                // Update reportsByStatus
                const statusIdIndex = reportsByStatus.value[2].findIndex(
                  (r) => r.reportId === reportId,
                )
                if (statusIdIndex !== -1) {
                  reportsByStatus.value[2].splice(statusIdIndex, 1)
                }
              }
              const message = isSuccess
                ? 'Báo cáo đã được đánh dấu là hoàn thành.'
                : 'Báo cáo đã được đánh dấu là không thể xử lý.'
              alert(message)
              // Refresh tab 3 (completed) hoặc tab khác tùy theo isSuccess
              if (isSuccess) {
                fetchReportsForTab(3)
              }
            } else {
              console.error('Failed to complete report:', response.status)
              alert(`Lỗi từ server: ${response.status} - Vui lòng kiểm tra console.`)
            }
          })
          .catch((error) => {
            console.error('Error completing report:', error)
            alert('Lỗi khi hoàn thành báo cáo. Vui lòng thử lại.')
          })
      }
    }

    const rejectReport = (reportId) => {
      if (confirm('Bạn có chắc muốn từ chối báo cáo này?')) {
        const url = `${API_CONFIG.BASE_URL}${API_CONFIG.ENDPOINTS.REPORTS}/${reportId}/cancel`
        fetch(url, {
          method: 'PATCH',
          headers: API_CONFIG.DEFAULT_HEADERS,
        })
          .then((response) => {
            if (response.ok) {
              // Remove the report from local list if API call succeeds
              const index = reports.value.findIndex((r) => r.reportId === reportId)
              if (index !== -1) {
                reports.value.splice(index, 1)
                // Update reportsByStatus - xóa từ tab hiện tại
                const currentTabStatusId =
                  activeTab.value === 'pending' ? 1 : activeTab.value === 'processing' ? 2 : 3
                const statusIdIndex = reportsByStatus.value[currentTabStatusId].findIndex(
                  (r) => r.reportId === reportId,
                )
                if (statusIdIndex !== -1) {
                  reportsByStatus.value[currentTabStatusId].splice(statusIdIndex, 1)
                }
              }
              alert('Đã từ chối báo cáo thành công.')
            } else {
              console.error('Failed to reject report:', response.status)
              alert('Không thể từ chối báo cáo. Vui lòng thử lại.')
            }
          })
          .catch((error) => {
            console.error('Error rejecting report:', error)
            alert('Lỗi khi từ chối báo cáo. Vui lòng thử lại.')
          })
      }
    }

    const viewImageDetail = (report) => {
      selectedReport.value = report
      showImageModal.value = true
    }

    const closeImageModal = () => {
      showImageModal.value = false
      selectedReport.value = null
    }

    return {
      activeTab,
      tabs,
      reports,
      reportsByStatus,
      filteredReports,
      getReportCount,
      formatDate,
      updateReportStatus,
      rejectReport,
      viewImageDetail,
      closeImageModal,
      showImageModal,
      selectedReport,
      handleTabChange,
      loading,
    }
  },
}
</script> -->

<style scoped>
.report-management {
  padding: 24px;
  background: linear-gradient(135deg, #0f172a 0%, #1e293b 100%);
  min-height: 100vh;
}

.page-header {
  margin-bottom: 32px;
}

.page-header h1 {
  font-size: 28px;
  font-weight: 700;
  color: #eeeeee;
  margin: 0 0 8px 0;
}

.subtitle {
  font-size: 14px;
  color: rgba(238, 238, 238, 0.7);
  margin: 0;
}

/* Tabs */
.tabs-container {
  display: flex;
  gap: 8px;
  margin-bottom: 20px;
  border-bottom: 2px solid #ddd;
}

.tab-button {
  padding: 10px 16px;
  border: none;
  background: none;
  color: rgba(238, 238, 238, 0.7);
  border-radius: 8px 8px 0 0;
  cursor: pointer;
  font-size: 14px;
  font-weight: 500;
  display: flex;
  align-items: center;
  gap: 8px;
  transition: all 0.2s ease;
}

.tab-button:hover {
  background: rgba(0, 173, 181, 0.1);
  color: #00adb5;
}

.tab-button.active {
  background: rgba(0, 173, 181, 0.1);
  color: #00adb5;
  border-bottom: 3px solid #00adb5;
  margin-bottom: -2px;
}

.tab-badge {
  background-color: rgba(0, 173, 181, 0.3);
  padding: 2px 8px;
  border-radius: 12px;
  font-size: 12px;
  font-weight: 600;
}

.tab-button.active .tab-badge {
  background-color: rgba(0, 173, 181, 0.5);
}

/* Reports Section */
.reports-section {
  background-color: #393e46;
  border-radius: 8px;
  box-shadow: 0 1px 3px rgba(0, 0, 0, 0.08);
  padding: 20px;
  border: 1px solid rgba(0, 173, 181, 0.2);
}

.empty-state {
  text-align: center;
  padding: 60px 24px;
}

.empty-icon {
  font-size: 48px;
  margin-bottom: 16px;
}

.empty-state p {
  font-size: 16px;
  color: rgba(238, 238, 238, 0.7);
  margin: 0;
}

.loading-state {
  text-align: center;
  padding: 60px 24px;
}

.loading-state p {
  font-size: 16px;
  color: rgba(238, 238, 238, 0.7);
  margin: 0;
}

/* Reports List */
.reports-list {
  display: flex;
  flex-direction: column;
  gap: 16px;
}

.report-card {
  border: 1px solid rgba(0, 173, 181, 0.2);
  border-radius: 8px;
  overflow: hidden;
  transition: all 0.3s ease;
  background-color: #222831;
}

.report-card:hover {
  box-shadow: 0 4px 12px rgba(0, 173, 181, 0.15);
  border-color: #00adb5;
}

.report-header {
  padding: 16px;
  background-color: #222831;
  border-bottom: 1px solid rgba(0, 173, 181, 0.15);
  display: flex;
  justify-content: flex-start;
  align-items: center;
  gap: 16px;
}

.report-title {
  display: flex;
  align-items: center;
  gap: 12px;
}

.report-title h3 {
  font-size: 16px;
  font-weight: 600;
  color: #eeeeee;
  margin: 0;
}

.status-badge {
  padding: 4px 12px;
  border-radius: 4px;
  font-size: 12px;
  font-weight: 600;
  white-space: nowrap;
}

.status-badge.pending {
  background-color: rgba(255, 193, 7, 0.2);
  color: #ffc107;
}

.status-badge.processing {
  background-color: rgba(0, 173, 181, 0.2);
  color: #00adb5;
}

.status-badge.completed {
  background-color: rgba(76, 175, 80, 0.2);
  color: #4caf50;
}

.report-body {
  padding: 16px;
}

.report-date {
  font-size: 16px;
  color: rgba(238, 238, 238, 0.6);
  white-space: nowrap;
  margin-bottom: 12px;
  font-weight: 500;
}

.report-info {
  font-size: 14px;
  color: rgba(238, 238, 238, 0.8);
}

.report-info p {
  margin: 8px 0;
  line-height: 1.5;
}

.report-info strong {
  color: #eeeeee;
}

.description {
  background-color: rgba(0, 173, 181, 0.1);
  padding: 8px 12px;
  border-radius: 4px;
  margin-top: 8px !important;
  border-left: 3px solid #00adb5;
}

.image-section {
  margin-top: 16px;
  padding-top: 16px;
  border-top: 1px solid rgba(0, 173, 181, 0.15);
}

.image-section p {
  margin-bottom: 8px;
}

.image-container {
  display: flex;
  align-items: center;
  gap: 12px;
}

.image-text {
  padding: 8px 12px;
  background-color: rgba(0, 173, 181, 0.1);
  border: 1px solid rgba(0, 173, 181, 0.3);
  border-radius: 4px;
  color: rgba(238, 238, 238, 0.9);
  font-size: 13px;
  font-weight: 500;
}

.btn-view-detail {
  background-color: #1890ff;
  color: white;
  padding: 8px 16px;
  font-size: 13px;
}

.btn-view-detail:hover {
  background-color: #0050b3;
}

.report-footer {
  padding: 16px;
  background-color: #222831;
  border-top: 1px solid rgba(0, 173, 181, 0.15);
}

.action-buttons {
  display: flex;
  gap: 12px;
}

.btn {
  padding: 8px 16px;
  border: none;
  border-radius: 6px;
  font-size: 14px;
  font-weight: 500;
  cursor: pointer;
  transition: all 0.3s ease;
}

.btn-primary {
  background-color: #00adb5;
  color: #222831;
}

.btn-success {
  background-color: #4caf50;
  color: white;
}

.btn-success:hover {
  background-color: #45a049;
}

.btn-danger {
  background-color: #ef4444;
  color: white;
}

.btn-danger:hover {
  background-color: #dc2626;
}

.btn-warning {
  background-color: #faad14;
  color: #222831;
  font-weight: 600;
}

.btn-warning:hover {
  background-color: #f5a623;
}

.btn-secondary {
  background-color: #555;
  color: #999;
  cursor: not-allowed;
}

.btn:disabled {
  opacity: 0.6;
  cursor: not-allowed;
}

/* Responsive */
@media (max-width: 768px) {
  .report-management {
    padding: 16px;
  }

  .page-header h1 {
    font-size: 24px;
  }

  .tabs-container {
    flex-wrap: wrap;
  }

  .report-header {
    flex-direction: column;
    gap: 8px;
  }

  .action-buttons {
    flex-direction: column;
  }

  .btn {
    width: 100%;
  }
}

/* Modal Styles */
.modal-overlay {
  position: fixed;
  top: 0;
  left: 0;
  width: 100%;
  height: 100%;
  background-color: rgba(0, 0, 0, 0.7);
  display: flex;
  justify-content: center;
  align-items: center;
  z-index: 1000;
}

.modal-content {
  background-color: #222831;
  border: 1px solid rgba(0, 173, 181, 0.3);
  border-radius: 8px;
  box-shadow: 0 8px 32px rgba(0, 0, 0, 0.3);
  max-width: 600px;
  width: 90%;
  max-height: 80vh;
  overflow-y: auto;
}

.modal-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
  padding: 20px;
  border-bottom: 1px solid rgba(0, 173, 181, 0.15);
  background-color: #1a1f26;
}

.modal-header h3 {
  margin: 0;
  color: #eeeeee;
  font-size: 18px;
}

.modal-close {
  background: none;
  border: none;
  color: #eeeeee;
  font-size: 24px;
  cursor: pointer;
  padding: 0;
  width: 32px;
  height: 32px;
  display: flex;
  align-items: center;
  justify-content: center;
  border-radius: 4px;
  transition: all 0.2s ease;
}

.modal-close:hover {
  background-color: rgba(255, 77, 79, 0.2);
  color: #ef4444;
}

.modal-body {
  padding: 20px;
  display: flex;
  flex-direction: column;
  gap: 16px;
}

.modal-image {
  width: 100%;
  max-height: 400px;
  object-fit: contain;
  border-radius: 4px;
  border: 1px solid rgba(0, 173, 181, 0.2);
}

.modal-info {
  padding: 12px;
  background-color: rgba(0, 173, 181, 0.05);
  border-left: 3px solid #00adb5;
  border-radius: 4px;
}

.modal-info p {
  margin: 8px 0;
  color: rgba(238, 238, 238, 0.8);
  font-size: 14px;
}

.modal-info strong {
  color: #eeeeee;
}
</style>
