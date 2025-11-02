<!--
  SIDEBAR ĐIỀU HƯỚNG
  - Hiển thị menu điều hướng chính của ứng dụng
  - Hỗ trợ thu gọn/mở rộng với tooltip
  - Nhóm các menu theo chức năng
-->
<template>
  <aside class="sidebar">
    <!-- Admin Login -->
    <div class="nav">
      <div class="login-group">
        <span class="material-icons login">account_circle</span>
        <span class="label">
          {{ loginLabel }}
        </span>
      </div>
    </div>
    <!-- Menu điều hướng chính -->
    <nav class="nav">
      <!-- Lặp qua từng nhóm menu -->
      <template v-for="group in menu" :key="group.title">
        <div class="group">
          <div class="group-title">{{ group.title }}</div>
          <router-link v-for="item in group.items" :key="item.to" :to="item.to">
            <span class="material-icons">{{ item.icon }}</span>
            <span class="label">{{ item.label }}</span>
          </router-link>
        </div>
      </template>
    </nav>
  </aside>
</template>

<script setup>
import { computed } from 'vue'
import { useAuthStore } from '@/stores/auth'

const auth = useAuthStore()
//
const isAdmin = computed(() => auth.roleId === 1)
//

const loginLabel = computed(() =>
  auth.fullname && auth.fullname.trim() !== '' ? auth.fullname : 'Lỗi',
)

// const menu = computed(() => [
//   {
//     title: 'Tổng quan',
//     items: [{ to: '/dashboard', icon: 'analytics', label: 'Bảng điều khiển' }],
//   },
//   // {
//   //   title: 'Quản lý',
//   //   items: [
//   //     { to: '/devices', icon: 'devices', label: 'Thiết bị' },
//   //     { to: '/users', icon: 'person', label: 'Người dùng' },
//   //   ],
//   // },

//   {
//     title: 'Quản lý',
//     items: [
//       { to: '/devices', icon: 'devices', label: 'Thiết bị' },
//       ...(isAdmin.value ? [{ to: '/users', icon: 'person', label: 'Người dùng' }] : []),
//     ],
//   },

//   {
//     title: 'Theo dõi',
//     items: [
//       { to: '/history', icon: 'history', label: 'Lịch sử' },
//       { to: '/reports', icon: 'report', label: 'Thống kê' },
//     ],
//   },
// ])

const menu = computed(() => {
  if (isAdmin.value) {
    // 🧑‍💼 Nếu là admin → hiển thị đầy đủ
    return [
      {
        title: 'Tổng quan',
        items: [{ to: '/dashboard', icon: 'analytics', label: 'Bảng điều khiển' }],
      },
      {
        title: 'Quản lý',
        items: [
          { to: '/devices', icon: 'devices', label: 'Thiết bị' },
          { to: '/users', icon: 'person', label: 'Người dùng' },
        ],
      },
      {
        title: 'Theo dõi',
        items: [
          { to: '/history', icon: 'history', label: 'Lịch sử' },
          { to: '/reports', icon: 'report', label: 'Thống kê' },
          { to: '/requests', icon: 'assignment', label: 'Yêu cầu' }, // 🆕 thêm dòng này
        ],
      },
    ]
  } else {
    // 👤 Nếu là user → chỉ hiện “Thiết bị”
    return [
      {
        title: 'Thiết bị của tôi',
        items: [{ to: '/devices', icon: 'devices', label: 'Thiết bị' }],
      },
    ]
  }
})
</script>

<style scoped>
.sidebar {
  height: 100%;
  background: #ffffff;
  color: #000000;
  padding: 16px 12px;
  box-sizing: border-box;
  border-right: 1px solid #eee;
}
.material-icons {
  color: black;
  margin-bottom: 0;
  vertical-align: middle;
  margin-right: 12px;
}
.material-icons.login {
  font-size: 50px;
  margin-bottom: 4px;
}
.login-link {
  display: flex;
  flex-direction: column;
  align-items: center;
}
.login-group {
  display: flex;
  flex-direction: column;
  align-items: center;
  margin-top: 32px;
}
.nav {
  margin-top: 16px;
}
.group {
  margin-bottom: 12px;
}
.group-title {
  font-size: 12px;
  color: #6b7280;
  padding: 0 12px;
  text-transform: uppercase;
  letter-spacing: 0.04em;
  margin-bottom: 6px;
  font-weight: 600;
}
.nav a {
  display: flex;
  align-items: center;
  gap: 12px;
  color: #000000;
  text-decoration: none;
  padding: 10px 12px;
  border-radius: 8px;
  font-size: 16px;
}
.nav a.router-link-active {
  font-weight: 600;
}
.label {
  white-space: nowrap;
}
</style>
