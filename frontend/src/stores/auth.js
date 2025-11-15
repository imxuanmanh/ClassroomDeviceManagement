import { defineStore } from 'pinia'

export const useAuthStore = defineStore('auth', {
  state: () => ({
    // token: localStorage.getItem('token') || null,
    // userId: Number(localStorage.getItem('userId')) || null, // ✅ thêm dòng này
    // roleId: Number(localStorage.getItem('roleId')) || null,
    // fullname: localStorage.getItem('fullname') || null,
    // username: localStorage.getItem('username') || null,

    //Đây là nơi mà bỏ qua phần đăng nhập mà vào trang web được luôn
    token: localStorage.getItem('token') || 'DEV_ADMIN_TOKEN', // <-- Ép 1 token giả
    userId: Number(localStorage.getItem('userId')) || 1, // <-- Ép 1 ID giả
    roleId: Number(localStorage.getItem('roleId')) || 1, // <-- Ép roleId = 1 (Admin)
    fullname: localStorage.getItem('fullname') || 'Admin (Dev Mode)',
    username: localStorage.getItem('username') || 'admin_dev',
  }),

  getters: {
    isAuthenticated: (state) => !!state.token,
    roleName: (state) => {
      switch (state.roleId) {
        case 1:
          return 'Admin'
        case 2:
          return 'User'
        default:
          return 'Guest'
      }
    },
  },

  actions: {
    login(token, user) {
      this.token = token
      this.userId = user.userId // ✅ thêm dòng này
      this.roleId = user.roleId
      this.username = user.username
      this.fullname = user.fullname

      localStorage.setItem('token', token)
      localStorage.setItem('userId', user.userId) // ✅ thêm dòng này
      localStorage.setItem('roleId', user.roleId)
      localStorage.setItem('username', user.username)
      localStorage.setItem('fullname', user.fullname)
    },

    logout() {
      this.token = null
      this.userId = null // ✅ thêm dòng này
      this.roleId = null
      this.username = null
      this.fullname = null

      localStorage.removeItem('token')
      localStorage.removeItem('userId') // ✅ thêm dòng này
      localStorage.removeItem('roleId')
      localStorage.removeItem('username')
      localStorage.removeItem('fullname')
    },
  },
})
