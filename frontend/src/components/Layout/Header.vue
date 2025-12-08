<template>
  <header class="header">
    <div class="header-left">
      <button class="menu-toggle" @click="$emit('toggle-sidebar')">
        <span class="material-icons">menu</span>
      </button>
      <h1 class="app-title">Quản lý Thiết bị</h1>
    </div>

    <div class="header-right">
      <div class="notification-wrapper" ref="notificationRef">
        <button
          class="notification-btn"
          aria-label="Notifications"
          @click="toggleNotification"
        >
          <img
            src="@/components/icons/notification-bell.png"
            alt="Notifications"
          />
          <span v-if="unreadCount > 0" class="notification-badge">
            {{ unreadCount > 99 ? "99+" : unreadCount }}
          </span>
        </button>

        <div v-if="notificationOpen" class="notification-dropdown">
          <div class="notification-header">
            <h3>Thông báo</h3>
            <div class="notification-actions">
              <button
                @click="markAllAsRead"
                class="mark-read-btn"
                title="Đánh dấu đã đọc tất cả"
              >
                <span class="material-icons">done</span>
              </button>
              <button
                @click="deleteAllNotifications"
                class="delete-all-btn"
                title="Xóa tất cả thông báo"
              >
                <span class="material-icons">close</span>
              </button>
            </div>
          </div>
          <div class="notification-content">
            <div v-if="notifications.length === 0" class="no-notifications">
              <img
                src="@/components/icons/bell-sleep.png"
                alt="No notifications"
              />
              <p>Không có thông báo mới</p>
            </div>

            <ul v-else class="notification-list">
              <li
                v-for="item in notifications"
                :key="item.id"
                class="notification-item"
                :class="{ 'notification-read': item.isRead }"
                @click="onNotificationClick(item)"
              >
                <div class="notif-icon">
                  <i class="bx bx-bell"></i>
                </div>
                <div class="notif-text">
                  <p>{{ item.message }}</p>
                  <small>{{ item.time }}</small>
                </div>
                <span v-if="!item.isRead" class="status-dot"></span>
              </li>
            </ul>
          </div>
        </div>
      </div>

      <div class="profile" @click="toggleDropdown" ref="profileRef">
        <img src="/src/components/icons/logo.png" alt="avatar" class="avatar" />
        <span>{{ auth.fullname || "Admin" }}</span>
        <span class="material-icons">expand_more</span>

        <div v-if="dropdownOpen" class="dropdown">
          <button @click="handleLogout" class="logout-btn">
            <img
              src="@/components/icons/logout-static.png"
              alt="Logout"
              class="dropdown-icon"
            />
            Đăng xuất
          </button>
        </div>
      </div>
    </div>
  </header>
</template>

<script setup>
import { ref, onMounted, onUnmounted, computed, watch } from "vue";
import { useRouter } from "vue-router";
import { useAuthStore } from "@/stores/auth";
import { notificationApi, userApi, authApi } from "@/config/api";
import { useSSE } from "@/composables/useSSE";

const auth = useAuthStore();
const router = useRouter();
const dropdownOpen = ref(false);
const notificationOpen = ref(false);

// Refs cho click outside
const profileRef = ref(null);
const notificationRef = ref(null);

const notifications = ref([]);

// --- CHUẨN BỊ URL VÀ SSE ---

const getUserId = () => {
  if (auth.userId) return auth.userId;
  return localStorage.getItem("userId");
};

const userId = getUserId();
const sseUrl = userId ? notificationApi.getStreamUrl(userId) : "";

// Khởi tạo SSE
const {
  data: sseData,
  connect: connectSSE,
  isConnected,
} = useSSE(sseUrl, {
  autoConnect: false,
  reconnect: true,
});

// --- LOGIC XỬ LÝ DỮ LIỆU ---

watch(sseData, (newVal) => {
  if (!newVal) return;

  const incomingItems = Array.isArray(newVal) ? newVal : [newVal];

  incomingItems.forEach((item) => {
    if (item && !item.deleted) {
      const newNotif = {
        id: item.id,
        message: item.content,
        isRead: !!item.seen,
        time: new Date().toLocaleTimeString(),
      };

      notifications.value.unshift(newNotif);

      // ✅ ĐÃ SỬA: Không được tự tăng unreadCount ở đây vì nó là computed
    }
  });
});

// 2. Lấy thông báo cũ từ API (HTTP GET)
const fetchOldNotifications = async () => {
  if (!userId) return;
  try {
    const result = await userApi.getNotifications(userId);
    const rawData = Array.isArray(result) ? result : result.data || [];

    notifications.value = rawData.map((item) => ({
      id: item.id, // ✅ Bổ sung thêm ID để click hoạt động
      message: item.content,
      time: item.createdAt ? new Date(item.createdAt).toLocaleTimeString() : "",
      isRead: item.isRead ?? item.seen ?? false, // Map cả trường seen nếu backend trả về
    }));
  } catch (error) {
    console.error("Lỗi tải thông báo cũ:", error);
  }
};

// 3. Xử lý nút TICK (Đánh dấu đã đọc tất cả)
async function markAllAsRead() {
  if (!userId) return;

  // Cập nhật UI
  notifications.value.forEach((notif) => {
    notif.isRead = true;
  });

  // Gọi API
  try {
    await userApi.markAllSeen(userId);
  } catch (error) {
    console.error("Lỗi khi đánh dấu đã đọc:", error);
  }
}

// ✅ ĐÃ SỬA HÀM NÀY
const onNotificationClick = async (item) => {
  console.log("🖱️ Click thông báo ID:", item.id);
  // Nếu đã đọc rồi thì thôi
  if (item.isRead) return;

  // 1. Cập nhật giao diện (Vue tự tính lại unreadCount)
  item.isRead = true;

  // 2. Gọi API để lưu xuống Database
  try {
    await notificationApi.markAsRead(item.id);
    console.log(`✅ Đã đọc thông báo: ${item.id}`);
  } catch (error) {
    console.error("❌ Lỗi lưu trạng thái đọc:", error);
  }
};

// 4. Xử lý nút X (Xóa tất cả thông báo)
async function deleteAllNotifications() {
  if (!userId) return;

  const oldNotifications = [...notifications.value];
  notifications.value = [];

  try {
    await userApi.deleteAllNotifications(userId);
  } catch (error) {
    console.error("Lỗi khi xóa thông báo:", error);
    notifications.value = oldNotifications;
    alert("Không thể xóa thông báo. Vui lòng kiểm tra kết nối mạng.");
  }
}

// --- XỬ LÝ CLICK OUTSIDE ---
function handleClickOutside(event) {
  if (profileRef.value && !profileRef.value.contains(event.target)) {
    dropdownOpen.value = false;
  }
  if (notificationRef.value && !notificationRef.value.contains(event.target)) {
    notificationOpen.value = false;
  }
}

// --- LIFECYCLE ---
onMounted(() => {
  if (userId) {
    fetchOldNotifications();
    connectSSE();
  }
  document.addEventListener("click", handleClickOutside);
});

onUnmounted(() => {
  document.removeEventListener("click", handleClickOutside);
});

// Computed tự động đếm số chưa đọc
const unreadCount = computed(() => {
  return notifications.value.filter((notif) => notif.isRead === false).length;
});

// --- CÁC HÀM SỰ KIỆN KHÁC ---
function toggleDropdown(event) {
  event.stopPropagation();
  dropdownOpen.value = !dropdownOpen.value;
  notificationOpen.value = false;
}

function toggleNotification(event) {
  event.stopPropagation();
  notificationOpen.value = !notificationOpen.value;
  dropdownOpen.value = false;
}

function handleLogout() {
  authApi.logout();
}
</script>

<style scoped>
/* GIỮ NGUYÊN CSS CŨ CỦA BẠN */
.header {
  display: flex;
  justify-content: space-between;
  align-items: center;
  height: 70px;
  background: linear-gradient(135deg, #081b29 0%, #0a2332 100%);
  color: white;
  padding: 0 32px;
  box-shadow: 0 4px 20px rgba(0, 239, 255, 0.15);
  border-bottom: 2px solid rgba(0, 239, 255, 0.3);
  position: relative;
  z-index: 5;
}

.header::after {
  content: "";
  position: absolute;
  bottom: -2px;
  left: 0;
  width: 100%;
  height: 2px;
  background: linear-gradient(
    90deg,
    transparent 0%,
    #0ef 20%,
    #0ef 80%,
    transparent 100%
  );
  box-shadow: 0 0 10px rgba(0, 239, 255, 0.5);
}

.header-left {
  display: flex;
  align-items: center;
  gap: 16px;
}

.menu-toggle {
  background: rgba(0, 239, 255, 0.1);
  border: 1px solid rgba(0, 239, 255, 0.3);
  color: #0ef;
  font-size: 24px;
  cursor: pointer;
  width: 42px;
  height: 42px;
  border-radius: 12px;
  display: flex;
  align-items: center;
  justify-content: center;
  transition: all 0.3s ease;
}

.menu-toggle:hover {
  background: rgba(0, 239, 255, 0.2);
  border-color: #0ef;
  box-shadow: 0 0 15px rgba(0, 239, 255, 0.4);
  transform: scale(1.05);
}

.app-title {
  font-size: 20px;
  font-weight: 700;
  letter-spacing: 0.5px;
  margin: 0;
  color: #0ef;
  text-shadow: 0 0 10px rgba(0, 239, 255, 0.5);
}

.header-right {
  display: flex;
  align-items: center;
  gap: 16px;
}

.profile {
  display: flex;
  align-items: center;
  gap: 10px;
  position: relative;
  cursor: pointer;
  padding: 8px 16px;
  border-radius: 12px;
  transition: all 0.3s ease;
  background: rgba(0, 239, 255, 0.05);
  border: 1px solid rgba(0, 239, 255, 0.2);
}

.profile:hover {
  background: rgba(0, 239, 255, 0.1);
  border-color: rgba(0, 239, 255, 0.4);
  box-shadow: 0 0 15px rgba(0, 239, 255, 0.2);
}

.avatar {
  width: 38px;
  height: 38px;
  border-radius: 50%;
  border: 2px solid #0ef;
  box-shadow: 0 0 10px rgba(0, 239, 255, 0.5);
  transition: all 0.3s ease;
}

.profile:hover .avatar {
  box-shadow: 0 0 15px rgba(0, 239, 255, 0.8);
  transform: scale(1.05);
}

.profile span:not(.material-icons) {
  font-weight: 500;
  color: #fff;
}

.profile .material-icons {
  color: #0ef;
  transition: transform 0.3s ease;
}

.profile:hover .material-icons {
  transform: rotate(180deg);
}

/* Dropdown menu */
.dropdown {
  position: absolute;
  right: 0;
  top: 60px;
  background: linear-gradient(135deg, #081b29 0%, #0a2332 100%);
  color: #fff;
  border-radius: 12px;
  box-shadow:
    0 8px 25px rgba(0, 0, 0, 0.5),
    0 0 20px rgba(0, 239, 255, 0.2);
  display: flex;
  flex-direction: column;
  overflow: hidden;
  min-width: 180px;
  z-index: 10;
  border: 2px solid rgba(0, 239, 255, 0.3);
  animation: dropdownSlide 0.3s ease;
}

@keyframes dropdownSlide {
  from {
    opacity: 0;
    transform: translateY(-10px);
  }
  to {
    opacity: 1;
    transform: translateY(0);
  }
}

.dropdown button {
  background: none;
  border: none;
  padding: 14px 16px;
  text-align: left;
  width: 100%;
  cursor: pointer;
  transition: all 0.2s ease;
  color: #fff;
  display: flex;
  align-items: center;
  gap: 10px;
  font-size: 14px;
  font-weight: 500;
}

.dropdown button .material-icons {
  font-size: 20px;
  color: #0ef;
}

.dropdown button:hover {
  background: rgba(0, 239, 255, 0.15);
  color: #0ef;
}

.dropdown button:hover .material-icons {
  transform: scale(1.1);
}

.dropdown button:not(:last-child) {
  border-bottom: 1px solid rgba(0, 239, 255, 0.1);
}

/* Notification Wrapper */
.notification-wrapper {
  position: relative;
}

/* Style cho nút notification */
.notification-btn {
  background: rgba(0, 239, 255, 0.1);
  border: 1px solid rgba(0, 239, 255, 0.3);
  cursor: pointer;
  width: 42px;
  height: 42px;
  border-radius: 12px;
  display: flex;
  align-items: center;
  justify-content: center;
  transition: all 0.3s ease;
  padding: 0;
  position: relative;
}

.notification-btn:hover {
  background: rgba(0, 239, 255, 0.2);
  border-color: #0ef;
  box-shadow: 0 0 15px rgba(0, 239, 255, 0.4);
}

.notification-btn img {
  width: 24px;
  height: 24px;
  object-fit: contain;
}

/* Notification Badge - BADGE SỐ THÔNG BÁO */
.notification-badge {
  position: absolute;
  top: -6px;
  right: -6px;
  background: linear-gradient(135deg, #ff0844 0%, #ff4757 100%) !important;
  color: white !important;
  font-size: 11px;
  font-weight: 700;
  min-width: 20px;
  height: 20px;
  border-radius: 10px;
  display: flex;
  align-items: center;
  justify-content: center;
  padding: 0 5px;
  border: 2px solid #081b29;
  box-shadow:
    0 0 10px rgba(255, 8, 68, 0.6),
    0 0 20px rgba(255, 8, 68, 0.4);
  animation: pulse-badge 2s ease-in-out infinite;
  z-index: 2;
}

@keyframes pulse-badge {
  0%,
  100% {
    transform: scale(1);
    box-shadow:
      0 0 10px rgba(255, 8, 68, 0.6),
      0 0 20px rgba(255, 8, 68, 0.4);
  }
  50% {
    transform: scale(1.1);
    box-shadow:
      0 0 15px rgba(255, 8, 68, 0.8),
      0 0 25px rgba(255, 8, 68, 0.6);
  }
}

/* Notification Dropdown */
.notification-dropdown {
  position: absolute;
  right: 0;
  top: 60px;
  background: linear-gradient(135deg, #081b29 0%, #0a2332 100%);
  border-radius: 12px;
  box-shadow:
    0 8px 25px rgba(0, 0, 0, 0.5),
    0 0 20px rgba(0, 239, 255, 0.2);
  min-width: 320px;
  max-width: 400px;
  z-index: 10;
  border: 2px solid rgba(0, 239, 255, 0.3);
  animation: dropdownSlide 0.3s ease;
  overflow: hidden;
}

.notification-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
  padding: 16px 20px;
  border-bottom: 1px solid rgba(0, 239, 255, 0.2);
  background: rgba(0, 239, 255, 0.05);
}

.notification-header h3 {
  margin: 0;
  font-size: 16px;
  font-weight: 600;
  color: #0ef;
}

.notification-actions {
  display: flex;
  gap: 8px;
  align-items: center;
}

.mark-read-btn,
.delete-all-btn {
  background: rgba(0, 239, 255, 0.1);
  border: 1px solid rgba(0, 239, 255, 0.3);
  border-radius: 8px;
  padding: 6px 8px;
  cursor: pointer;
  transition: all 0.3s ease;
  display: flex;
  align-items: center;
  justify-content: center;
}

.mark-read-btn .material-icons,
.delete-all-btn .material-icons {
  color: #0ef;
  font-size: 20px;
}

.mark-read-btn:hover,
.delete-all-btn:hover {
  background: rgba(0, 239, 255, 0.2);
  border-color: #0ef;
  box-shadow: 0 0 10px rgba(0, 239, 255, 0.3);
}

/* Style riêng cho nút xóa */
.delete-all-btn {
  background: rgba(255, 8, 68, 0.1);
  border-color: rgba(255, 8, 68, 0.3);
}

.delete-all-btn .material-icons {
  color: #ff0844;
}

.delete-all-btn:hover {
  background: rgba(255, 8, 68, 0.2);
  border-color: #ff0844;
  box-shadow: 0 0 10px rgba(255, 8, 68, 0.4);
}

.notification-content {
  max-height: 400px;
  overflow-y: auto;
}

.no-notifications {
  padding: 40px 20px;
  text-align: center;
  color: rgba(255, 255, 255, 0.6);
  font-size: 14px;
}

.notification-list {
  padding: 8px 0;
}

.notification-item {
  display: flex;
  align-items: flex-start;
  gap: 12px;
  padding: 14px 20px;
  border-bottom: 1px solid rgba(0, 239, 255, 0.1);
  transition: all 0.2s ease;
  cursor: pointer;
}

.notification-item:hover {
  background: rgba(255, 255, 255, 0.05);
}

.notification-item:last-child {
  border-bottom: none;
}

/* Style cho thông báo đã đọc - LÀM MỜ */
.notification-read {
  opacity: 0.4;
}

.notification-read .notif-text p {
  color: rgba(255, 255, 255, 0.5);
}

.notification-read .notif-icon {
  color: rgba(0, 239, 255, 0.4);
}

.notif-icon {
  color: #0ef;
  font-size: 22px;
  margin-top: 2px;
}

.notif-text {
  flex: 1;
}

.notif-text p {
  margin: 0 0 6px 0;
  color: #fff;
  font-size: 14px;
  line-height: 1.4;
}

.notif-text small {
  color: rgba(255, 255, 255, 0.5);
  font-size: 12px;
}

.notification-content::-webkit-scrollbar {
  width: 6px;
}

.notification-content::-webkit-scrollbar-track {
  background: rgba(0, 239, 255, 0.05);
}

.notification-content::-webkit-scrollbar-thumb {
  background: rgba(0, 239, 255, 0.3);
  border-radius: 3px;
}

.notification-content::-webkit-scrollbar-thumb:hover {
  background: rgba(0, 239, 255, 0.5);
}

@keyframes ring-animation {
  0%,
  100% {
    transform: rotate(0);
  }
  10%,
  50% {
    transform: rotate(-15deg);
  }
  30%,
  70% {
    transform: rotate(15deg);
  }
  90% {
    transform: rotate(0);
  }
}

.notification-btn:hover img {
  animation: ring-animation 0.7s ease-in-out;
  animation-iteration-count: 1;
  transform-origin: top center;
}

.dropdown-icon {
  width: 20px;
  height: 20px;
  object-fit: contain;
  transition: transform 0.2s ease;
}

@keyframes logout-shake {
  0%,
  100% {
    transform: rotate(0) scale(1.1);
  }
  30% {
    transform: rotate(-8deg) scale(1.1);
  }
  60% {
    transform: rotate(8deg) scale(1.1);
  }
}

.dropdown button:hover .dropdown-icon {
  animation: logout-shake 0.5s ease-in-out;
  animation-iteration-count: 1;
}
</style>
