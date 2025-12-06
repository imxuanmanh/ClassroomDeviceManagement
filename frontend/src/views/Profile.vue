<template>
  <div class="profile-page">
    <div class="background-blobs">
      <div class="blob blob-1"></div>
      <div class="blob blob-2"></div>
    </div>

    <div class="container">
      <div class="page-header animate-fade-in">
        <div class="header-title">
          <div class="header-bar"></div>
          <h1>Thông tin cá nhân</h1>
        </div>
        <p class="header-subtitle">Quản lý thông tin tài khoản và bảo mật của bạn</p>
      </div>

      <div class="profile-grid">
        <div class="main-column animate-slide-up">
          <div class="glass-card profile-card">
            <div class="cover-image">
              <div class="cover-overlay"></div>
              <div class="avatar-section">
                <div class="avatar-wrapper">
                  <div class="avatar-frame">
                    <img :src="profile.avatar" alt="Avatar" />
                  </div>
                  <input
                    type="file"
                    ref="fileInput"
                    class="hidden"
                    accept="image/*"
                    @change="handleAvatarChange"
                  />

                  <button v-if="isEditing" @click="triggerFileInput" class="btn-change-avatar">
                    <svg
                      width="24"
                      height="24"
                      fill="none"
                      stroke="currentColor"
                      viewBox="0 0 24 24"
                    >
                      <path
                        stroke-linecap="round"
                        stroke-linejoin="round"
                        stroke-width="2"
                        d="M3 9a2 2 0 012-2h.93a2 2 0 001.664-.89l.812-1.22A2 2 0 0110.07 4h3.86a2 2 0 011.664.89l.812 1.22A2 2 0 0018.07 7H19a2 2 0 012 2v9a2 2 0 01-2 2H5a2 2 0 01-2-2V9z"
                      />
                      <path
                        stroke-linecap="round"
                        stroke-linejoin="round"
                        stroke-width="2"
                        d="M15 13a3 3 0 11-6 0 3 3 0 016 0z"
                      />
                    </svg>
                    <span>Thay ảnh</span>
                  </button>
                </div>
              </div>

              <button
                @click="toggleEdit"
                class="btn-edit"
                :class="{ 'btn-edit-active': isEditing }"
              >
                <svg
                  v-if="!isEditing"
                  width="16"
                  height="16"
                  fill="none"
                  stroke="currentColor"
                  viewBox="0 0 24 24"
                >
                  <path
                    stroke-linecap="round"
                    stroke-linejoin="round"
                    stroke-width="2"
                    d="M15.232 5.232l3.536 3.536m-2.036-5.036a2.5 2.5 0 113.536 3.536L6.5 21.036H3v-3.572L16.732 3.732z"
                  />
                </svg>
                <svg
                  v-else
                  width="16"
                  height="16"
                  fill="none"
                  stroke="currentColor"
                  viewBox="0 0 24 24"
                >
                  <path
                    stroke-linecap="round"
                    stroke-linejoin="round"
                    stroke-width="2"
                    d="M6 18L18 6M6 6l12 12"
                  />
                </svg>
                {{ isEditing ? 'Hủy' : 'Chỉnh sửa' }}
              </button>
            </div>

            <div class="profile-content">
              <div class="user-identity">
                <h2 class="user-name">{{ profile.name }}</h2>
                <div class="role-badge">
                  <svg width="14" height="14" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                    <path
                      stroke-linecap="round"
                      stroke-linejoin="round"
                      stroke-width="2"
                      d="M9 12l2 2 4-4m5.618-4.016A11.955 11.955 0 0112 2.944a11.955 11.955 0 01-8.618 3.04A12.02 12.02 0 003 9c0 5.591 3.824 10.29 9 11.622 5.176-1.332 9-6.03 9-11.622 0-1.042-.133-2.052-.382-3.016z"
                    />
                  </svg>
                  {{ profile.role }}
                </div>
              </div>

              <div class="form-grid">
                <div class="form-group">
                  <label>
                    <svg
                      width="16"
                      height="16"
                      fill="none"
                      stroke="currentColor"
                      viewBox="0 0 24 24"
                    >
                      <path
                        stroke-linecap="round"
                        stroke-linejoin="round"
                        stroke-width="2"
                        d="M16 7a4 4 0 11-8 0 4 4 0 018 0zM12 14a7 7 0 00-7 7h14a7 7 0 00-7-7z"
                      />
                    </svg>
                    Tên hiển thị
                  </label>
                  <input
                    v-if="isEditing"
                    v-model="tempProfile.name"
                    type="text"
                    class="form-input"
                    placeholder="Nhập tên của bạn"
                  />
                  <div v-else class="info-display">{{ profile.name }}</div>
                </div>

                <div class="form-group">
                  <label>
                    <svg
                      width="16"
                      height="16"
                      fill="none"
                      stroke="currentColor"
                      viewBox="0 0 24 24"
                    >
                      <path
                        stroke-linecap="round"
                        stroke-linejoin="round"
                        stroke-width="2"
                        d="M3 8l7.89 5.26a2 2 0 002.22 0L21 8M5 19h14a2 2 0 002-2V7a2 2 0 00-2-2H5a2 2 0 00-2 2v10a2 2 0 002 2z"
                      />
                    </svg>
                    Email
                  </label>
                  <input
                    v-if="isEditing"
                    v-model="tempProfile.email"
                    type="email"
                    class="form-input"
                    placeholder="email@example.com"
                  />
                  <div v-else class="info-display">{{ profile.email }}</div>
                </div>

                <div class="form-group">
                  <label>
                    <svg
                      width="16"
                      height="16"
                      fill="none"
                      stroke="currentColor"
                      viewBox="0 0 24 24"
                    >
                      <path
                        stroke-linecap="round"
                        stroke-linejoin="round"
                        stroke-width="2"
                        d="M3 5a2 2 0 012-2h3.28a1 1 0 01.948.684l1.498 4.493a1 1 0 01-.502 1.21l-2.257 1.13a11.042 11.042 0 005.516 5.516l1.13-2.257a1 1 0 011.21-.502l4.493 1.498a1 1 0 01.684.949V19a2 2 0 01-2 2h-1C9.716 21 3 14.284 3 6V5z"
                      />
                    </svg>
                    Số điện thoại
                  </label>
                  <input
                    v-if="isEditing"
                    v-model="tempProfile.phone"
                    type="tel"
                    class="form-input"
                    placeholder="+84 xxx xxx xxx"
                  />
                  <div v-else class="info-display">{{ profile.phone }}</div>
                </div>

                <div class="form-group">
                  <label>
                    <svg
                      width="16"
                      height="16"
                      fill="none"
                      stroke="currentColor"
                      viewBox="0 0 24 24"
                    >
                      <path
                        stroke-linecap="round"
                        stroke-linejoin="round"
                        stroke-width="2"
                        d="M8 7V3m8 4V3m-9 8h10M5 21h14a2 2 0 002-2V7a2 2 0 00-2-2H5a2 2 0 00-2 2v12a2 2 0 002 2z"
                      />
                    </svg>
                    Ngày tham gia
                  </label>
                  <div class="info-display">{{ profile.joinDate }}</div>
                </div>
              </div>

              <div class="action-footer">
                <div v-if="isEditing" class="action-buttons">
                  <button @click="toggleEdit" class="btn btn-secondary" :disabled="isLoading">
                    Hủy
                  </button>
                  <button @click="handleSave" class="btn btn-primary" :disabled="isLoading">
                    <svg v-if="isLoading" class="spinner" viewBox="0 0 24 24">
                      <circle
                        class="opacity-25"
                        cx="12"
                        cy="12"
                        r="10"
                        stroke="currentColor"
                        stroke-width="4"
                      ></circle>
                      <path
                        class="opacity-75"
                        fill="currentColor"
                        d="M4 12a8 8 0 018-8V0C5.373 0 0 5.373 0 12h4zm2 5.291A7.962 7.962 0 014 12H0c0 3.042 1.135 5.824 3 7.938l3-2.647z"
                      ></path>
                    </svg>
                    <span v-else>Lưu thay đổi</span>
                  </button>
                </div>
              </div>
            </div>
          </div>
        </div>

        <div class="sidebar animate-slide-up-delay">
          <div class="glass-card side-card">
            <h3 class="side-title">
              <div class="icon-box">
                <svg width="16" height="16" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                  <path
                    stroke-linecap="round"
                    stroke-linejoin="round"
                    stroke-width="2"
                    d="M9 19v-6a2 2 0 00-2-2H5a2 2 0 00-2 2v6a2 2 0 002 2h2a2 2 0 002-2zm0 0V9a2 2 0 012-2h2a2 2 0 012 2v10m-6 0a2 2 0 002 2h2a2 2 0 002-2m0 0V5a2 2 0 012-2h2a2 2 0 012 2v14a2 2 0 01-2 2h-2a2 2 0 01-2-2z"
                  />
                </svg>
              </div>
              Thống kê hoạt động
            </h3>
            <div class="stats-list">
              <div v-for="(stat, idx) in stats" :key="idx" class="stat-item">
                <div class="stat-icon-wrapper">
                  <component :is="stat.icon" class="stat-svg" />
                </div>
                <div class="stat-text">
                  <p class="stat-label">{{ stat.label }}</p>
                  <p class="stat-value">{{ stat.value }}</p>
                </div>
              </div>
            </div>
          </div>

          <div class="glass-card side-card">
            <h3 class="side-title">
              <div class="icon-box">
                <svg width="16" height="16" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                  <path
                    stroke-linecap="round"
                    stroke-linejoin="round"
                    stroke-width="2"
                    d="M12 8v4l3 3m6-3a9 9 0 11-18 0 9 9 0 0118 0z"
                  />
                </svg>
              </div>
              Hoạt động gần đây
            </h3>
            <div class="timeline">
              <div class="timeline-line"></div>
              <div v-for="(activity, idx) in recentActivities" :key="idx" class="timeline-item">
                <span class="timeline-dot"></span>
                <div class="timeline-content">
                  <p class="timeline-action">{{ activity.action }}</p>
                  <div class="timeline-meta">
                    <span class="time">{{ activity.time }}</span>
                    <span class="dot-separator"></span>
                    <span class="device">
                      <svg
                        width="12"
                        height="12"
                        fill="none"
                        stroke="currentColor"
                        viewBox="0 0 24 24"
                      >
                        <path
                          stroke-linecap="round"
                          stroke-linejoin="round"
                          stroke-width="2"
                          d="M9.75 17L9 20l-1 1h8l-1-1-.75-3M3 13h18M5 17h14a2 2 0 002-2V5a2 2 0 00-2-2H5a2 2 0 00-2 2v10a2 2 0 002 2z"
                        />
                      </svg>
                      {{ activity.device }}
                    </span>
                  </div>
                </div>
              </div>
            </div>
          </div>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup>
import { ref, h } from 'vue'

const isEditing = ref(false)
const isLoading = ref(false)
const fileInput = ref(null)

const profile = ref({
  name: 'Nguyễn Văn Admin',
  email: 'admin@system.com',
  phone: '+84 987 654 321',
  role: 'Quản trị viên cấp cao',
  joinDate: '15/01/2024',
  avatar: 'https://ui-avatars.com/api/?name=Admin&background=0ea5e9&color=fff&size=200&bold=true',
})

const tempProfile = ref({ ...profile.value })

// Icons as render functions
const MonitorIcon = () =>
  h('svg', { fill: 'none', viewBox: '0 0 24 24', stroke: 'currentColor' }, [
    h('path', {
      'stroke-linecap': 'round',
      'stroke-linejoin': 'round',
      'stroke-width': '2',
      d: 'M9.75 17L9 20l-1 1h8l-1-1-.75-3M3 13h18M5 17h14a2 2 0 002-2V5a2 2 0 00-2-2H5a2 2 0 00-2 2v10a2 2 0 002 2z',
    }),
  ])

const ActivityIcon = () =>
  h('svg', { fill: 'none', viewBox: '0 0 24 24', stroke: 'currentColor' }, [
    h('path', {
      'stroke-linecap': 'round',
      'stroke-linejoin': 'round',
      'stroke-width': '2',
      d: 'M13 7h8m0 0v8m0-8l-8 8-4-4-6 6',
    }),
  ])

const ShieldIcon = () =>
  h('svg', { fill: 'none', viewBox: '0 0 24 24', stroke: 'currentColor' }, [
    h('path', {
      'stroke-linecap': 'round',
      'stroke-linejoin': 'round',
      'stroke-width': '2',
      d: 'M9 12l2 2 4-4m5.618-4.016A11.955 11.955 0 0112 2.944a11.955 11.955 0 01-8.618 3.04A12.02 12.02 0 003 9c0 5.591 3.824 10.29 9 11.622 5.176-1.332 9-6.03 9-11.622 0-1.042-.133-2.052-.382-3.016z',
    }),
  ])

const stats = ref([
  { icon: MonitorIcon, label: 'Thiết bị đã kết nối', value: '05' },
  { icon: ActivityIcon, label: 'Giờ làm việc (tháng)', value: '164h' },
  { icon: ShieldIcon, label: 'Trạng thái bảo mật', value: 'An toàn' },
])

const recentActivities = ref([
  { action: 'Đổi mật khẩu bảo mật', time: '10 phút trước', device: 'Chrome - Windows' },
  { action: 'Cập nhật ảnh đại diện', time: '2 giờ trước', device: 'Safari - iPhone 14' },
  { action: 'Đăng nhập hệ thống', time: '1 ngày trước', device: 'Chrome - Windows' },
  { action: 'Xuất báo cáo tháng', time: '3 ngày trước', device: 'Firefox - MacOS' },
])

const toggleEdit = () => {
  if (isEditing.value) {
    tempProfile.value = { ...profile.value }
  }
  isEditing.value = !isEditing.value
}

const triggerFileInput = () => {
  fileInput.value.click()
}

const handleAvatarChange = (event) => {
  const file = event.target.files[0]
  if (file) {
    const reader = new FileReader()
    reader.onload = (e) => {
      profile.value.avatar = e.target.result
    }
    reader.readAsDataURL(file)
  }
}

const handleSave = async () => {
  isLoading.value = true
  await new Promise((resolve) => setTimeout(resolve, 1000))
  profile.value = { ...tempProfile.value }
  isEditing.value = false
  isLoading.value = false
}
</script>

<style scoped>
/* Base Layout & Colors */
.profile-page {
  min-height: 100vh;
  background: linear-gradient(135deg, #0f172a 0%, #1e293b 100%);
  padding: 24px;
  position: relative;
  overflow-x: hidden;
  color: #f1f5f9;
  font-family: ui-sans-serif, system-ui, sans-serif;
}

.container {
  max-width: 1280px;
  margin: 0 auto;
  position: relative;
  z-index: 10;
}

.hidden {
  display: none;
}

/* Animated Background Blobs */
.background-blobs {
  position: absolute;
  inset: 0;
  overflow: hidden;
  pointer-events: none;
}
.blob {
  position: absolute;
  border-radius: 50%;
  filter: blur(60px);
}
.blob-1 {
  top: -20px;
  left: -20px;
  width: 300px;
  height: 300px;
  background: rgba(6, 182, 212, 0.05); /* Cyan */
  animation: pulse 6s infinite ease-in-out;
}
.blob-2 {
  bottom: -20px;
  right: -20px;
  width: 400px;
  height: 400px;
  background: rgba(59, 130, 246, 0.05); /* Blue */
  animation: pulse 10s infinite ease-in-out;
}

/* Header */
.page-header {
  margin-bottom: 32px;
}
.header-title {
  display: flex;
  align-items: center;
  gap: 12px;
  margin-bottom: 8px;
}
.header-bar {
  width: 4px;
  height: 32px;
  background: linear-gradient(to bottom, #00adb5, #3b82f6);
  border-radius: 99px;
}
.header-title h1 {
  font-size: 30px;
  font-weight: 700;
  color: white;
  margin: 0;
}
.header-subtitle {
  color: #94a3b8;
  margin-left: 28px;
  font-size: 15px;
}

/* Grid Layout */
.profile-grid {
  display: grid;
  grid-template-columns: 1fr;
  gap: 24px;
}
@media (min-width: 1024px) {
  .profile-grid {
    grid-template-columns: 2fr 1fr;
  }
}

/* Cards (Glassmorphism) */
.glass-card {
  background: rgba(255, 255, 255, 0.03);
  backdrop-filter: blur(16px);
  border-radius: 24px;
  border: 1px solid rgba(255, 255, 255, 0.05);
  box-shadow: 0 10px 30px rgba(0, 0, 0, 0.2);
  overflow: hidden;
}

/* --- MAIN PROFILE CARD --- */
.cover-image {
  position: relative;
  height: 140px;
  background: linear-gradient(to right, #00adb5, #3b82f6, #a855f7);
}
.cover-overlay {
  position: absolute;
  inset: 0;
  background: rgba(0, 0, 0, 0.2);
}
.avatar-section {
  position: absolute;
  bottom: -60px;
  left: 32px;
}
.avatar-wrapper {
  position: relative;
  width: 128px;
  height: 128px;
  group: hover;
}
.avatar-frame {
  width: 100%;
  height: 100%;
  border-radius: 20px;
  border: 4px solid #1e293b;
  background: #1e293b;
  overflow: hidden;
  box-shadow: 0 10px 20px rgba(0, 0, 0, 0.3);
  transition: transform 0.3s;
}
.avatar-wrapper:hover .avatar-frame {
  transform: scale(1.05);
}
.avatar-frame img {
  width: 100%;
  height: 100%;
  object-fit: cover;
}
.btn-change-avatar {
  position: absolute;
  inset: 0;
  background: rgba(0, 0, 0, 0.7);
  backdrop-filter: blur(2px);
  border-radius: 16px;
  display: flex;
  flex-direction: column;
  align-items: center;
  justify-content: center;
  color: white;
  opacity: 0;
  transition: opacity 0.3s;
  cursor: pointer;
  border: none;
}
.avatar-wrapper:hover .btn-change-avatar {
  opacity: 1;
}
.btn-change-avatar span {
  font-size: 12px;
  font-weight: 600;
  margin-top: 4px;
}

/* Edit Button Top Right */
.btn-edit {
  position: absolute;
  top: 16px;
  right: 16px;
  padding: 8px 16px;
  border-radius: 12px;
  font-weight: 500;
  display: flex;
  align-items: center;
  gap: 8px;
  backdrop-filter: blur(4px);
  transition: all 0.2s;
  cursor: pointer;
  border: none;
  background: rgba(255, 255, 255, 0.9);
  color: #0f172a;
}
.btn-edit:hover {
  background: #ffffff;
}
.btn-edit-active {
  background: rgba(255, 255, 255, 0.15);
  color: white;
}
.btn-edit-active:hover {
  background: rgba(255, 255, 255, 0.25);
}

/* Profile Content */
.profile-content {
  padding: 80px 32px 32px 32px;
}
.user-identity {
  margin-bottom: 24px;
}
.user-name {
  font-size: 24px;
  font-weight: 700;
  color: white;
  margin: 0 0 8px 0;
}
.role-badge {
  display: inline-flex;
  align-items: center;
  gap: 6px;
  padding: 4px 12px;
  border-radius: 99px;
  background: rgba(0, 173, 181, 0.1);
  border: 1px solid rgba(0, 173, 181, 0.2);
  color: #00adb5;
  font-size: 14px;
  font-weight: 500;
}

/* Form Styles */
.form-grid {
  display: grid;
  grid-template-columns: 1fr;
  gap: 16px;
}
@media (min-width: 768px) {
  .form-grid {
    grid-template-columns: 1fr 1fr;
  }
}
.form-group {
  position: relative;
}
.form-group label {
  color: #94a3b8;
  font-size: 12px;
  font-weight: 700;
  text-transform: uppercase;
  letter-spacing: 0.05em;
  margin-bottom: 8px;
  display: flex;
  align-items: center;
  gap: 8px;
}
.form-input {
  width: 100%;
  background: rgba(15, 23, 42, 0.5);
  color: white;
  padding: 12px 16px;
  border-radius: 12px;
  border: 1px solid #334155;
  font-size: 15px;
  transition: all 0.2s;
  outline: none;
}
.form-input:focus {
  border-color: #00adb5;
  box-shadow: 0 0 0 3px rgba(0, 173, 181, 0.15);
}
.info-display {
  background: rgba(15, 23, 42, 0.3);
  padding: 12px 16px;
  border-radius: 12px;
  border: 1px solid rgba(51, 65, 85, 0.5);
  color: white;
  font-weight: 500;
}

/* Updated Action Footer Styles */
.action-footer {
  display: flex;
  margin-top: 24px;
  padding-top: 24px;
  border-top: 1px solid rgba(255, 255, 255, 0.05);
  /* Changed to flex-end to keep buttons on the right after removing left button */
  justify-content: flex-end;
}

@media (min-width: 640px) {
  .action-footer {
    flex-direction: row;
    align-items: center;
  }
}

.action-buttons {
  display: flex;
  gap: 12px;
}
.btn {
  padding: 10px 24px;
  border-radius: 12px;
  font-weight: 600;
  cursor: pointer;
  border: none;
  transition: all 0.2s;
  font-size: 15px;
  display: flex;
  align-items: center;
  justify-content: center;
  gap: 8px;
}
.btn:disabled {
  opacity: 0.6;
  cursor: not-allowed;
}
.btn-secondary {
  background: rgba(51, 65, 85, 0.5);
  color: white;
  backdrop-filter: blur(4px);
}
.btn-secondary:hover:not(:disabled) {
  background: #334155;
}
.btn-primary {
  background: linear-gradient(to right, #00adb5, #3b82f6);
  color: white;
  box-shadow: 0 4px 15px rgba(0, 173, 181, 0.25);
  min-width: 140px;
}
.btn-primary:hover:not(:disabled) {
  filter: brightness(1.1);
}

/* --- SIDEBAR --- */
.side-card {
  padding: 24px;
  margin-bottom: 24px;
}
.side-title {
  font-size: 18px;
  font-weight: 700;
  color: white;
  margin: 0 0 16px 0;
  display: flex;
  align-items: center;
  gap: 8px;
}
.icon-box {
  width: 32px;
  height: 32px;
  border-radius: 8px;
  background: rgba(0, 173, 181, 0.15);
  display: flex;
  align-items: center;
  justify-content: center;
  color: #00adb5;
}

/* Stats List */
.stats-list {
  display: flex;
  flex-direction: column;
  gap: 12px;
}
.stat-item {
  background: rgba(15, 23, 42, 0.3);
  padding: 16px;
  border-radius: 12px;
  border: 1px solid rgba(51, 65, 85, 0.5);
  display: flex;
  align-items: center;
  gap: 12px;
  transition: all 0.3s;
}
.stat-item:hover {
  border-color: rgba(0, 173, 181, 0.3);
  background: rgba(15, 23, 42, 0.5);
}
.stat-icon-wrapper {
  padding: 8px;
  background: rgba(6, 182, 212, 0.1);
  border-radius: 8px;
}
.stat-item:hover .stat-icon-wrapper {
  transform: scale(1.1);
  transition: transform 0.2s;
}
.stat-svg {
  width: 20px;
  height: 20px;
  color: #00adb5;
}
.stat-text {
  flex: 1;
}
.stat-label {
  font-size: 12px;
  text-transform: uppercase;
  color: #94a3b8;
  font-weight: 600;
  margin: 0;
}
.stat-value {
  font-size: 18px;
  font-weight: 700;
  color: white;
  margin: 0;
}

/* Timeline */
.timeline {
  position: relative;
  padding-left: 20px;
  display: flex;
  flex-direction: column;
  gap: 24px;
}
.timeline-line {
  position: absolute;
  left: 7px;
  top: 0;
  bottom: 0;
  width: 2px;
  background: linear-gradient(
    to bottom,
    rgba(0, 173, 181, 0.5),
    rgba(59, 130, 246, 0.3),
    transparent
  );
}
.timeline-item {
  position: relative;
}
.timeline-dot {
  position: absolute;
  left: -20px;
  top: 4px;
  width: 14px;
  height: 14px;
  border-radius: 50%;
  background: linear-gradient(135deg, #00adb5, #3b82f6);
  border: 2px solid #1e293b;
  box-shadow: 0 0 10px rgba(0, 173, 181, 0.5);
}
.timeline-item:hover .timeline-dot {
  transform: scale(1.25);
  transition: transform 0.2s;
}
.timeline-content {
  background: rgba(15, 23, 42, 0.3);
  padding: 12px;
  border-radius: 12px;
  border: 1px solid rgba(51, 65, 85, 0.5);
  transition: all 0.3s;
}
.timeline-item:hover .timeline-content {
  border-color: rgba(0, 173, 181, 0.3);
  background: rgba(15, 23, 42, 0.5);
}
.timeline-action {
  color: white;
  font-weight: 500;
  font-size: 14px;
  margin: 0 0 8px 0;
}
.timeline-meta {
  display: flex;
  align-items: center;
  gap: 8px;
  font-size: 12px;
}
.time {
  color: #64748b;
}
.dot-separator {
  width: 4px;
  height: 4px;
  background: #475569;
  border-radius: 50%;
}
.device {
  color: #00adb5;
  display: flex;
  align-items: center;
  gap: 4px;
}

/* Spinner Animation */
.spinner {
  width: 20px;
  height: 20px;
  animation: spin 1s linear infinite;
}
@keyframes spin {
  from {
    transform: rotate(0deg);
  }
  to {
    transform: rotate(360deg);
  }
}

/* Animations */
@keyframes fade-in {
  from {
    opacity: 0;
  }
  to {
    opacity: 1;
  }
}
@keyframes slide-up {
  from {
    opacity: 0;
    transform: translateY(20px);
  }
  to {
    opacity: 1;
    transform: translateY(0);
  }
}
@keyframes pulse {
  0%,
  100% {
    opacity: 1;
  }
  50% {
    opacity: 0.5;
  }
}

.animate-fade-in {
  animation: fade-in 0.6s ease-out forwards;
}
.animate-slide-up {
  animation: slide-up 0.8s cubic-bezier(0.16, 1, 0.3, 1) forwards;
}
.animate-slide-up-delay {
  opacity: 0;
  animation: slide-up 0.8s cubic-bezier(0.16, 1, 0.3, 1) 0.2s forwards;
}
</style>
