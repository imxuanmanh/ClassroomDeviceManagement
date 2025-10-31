// /**
//  * STORE XÁC THỰC NGƯỜI DÙNG
//  *
//  * Quản lý trạng thái đăng nhập/đăng xuất của người dùng
//  * Sử dụng Pinia để quản lý state toàn cục
//  */
// import { defineStore } from 'pinia'

// export const useAuthStore = defineStore('auth', {
//   // Trạng thái ban đầu của store
//   state: () => ({
//     // Token xác thực, lấy từ localStorage nếu có
//     token: typeof localStorage !== 'undefined' ? localStorage.getItem('auth.token') || '' : '',
//   }),

//   // Các getter để tính toán dữ liệu từ state
//   getters: {
//     // Kiểm tra người dùng đã đăng nhập chưa
//     isAuthenticated: (state) => !!state.token,
//   },

//   // Các action để thay đổi state
//   actions: {
//     /**
//      * Đăng nhập người dùng
//      * @param {string} token - Token xác thực
//      */
//     login(token) {
//       this.token = token
//       // Lưu token vào localStorage để duy trì đăng nhập
//       try { localStorage.setItem('auth.token', token) } catch (_) {}
//     },

//     /**
//      * Đăng xuất người dùng
//      * Xóa token và thông tin đăng nhập
//      */
//     logout() {
//       this.token = ''
//       // Xóa token khỏi localStorage
//       try { localStorage.removeItem('auth.token') } catch (_) {}
//     },
//   },
// })

import { defineStore } from 'pinia'

export const useAuthStore = defineStore('auth', {
  state: () => ({
    token: localStorage.getItem('token') || null,
    roleId: Number(localStorage.getItem('roleId')) || null,
    fullname: localStorage.getItem('fullname') || null,
    username: localStorage.getItem('username') || null,
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
      this.roleId = user.roleId
      this.username = user.username
      this.fullname = user.fullname

      localStorage.setItem('token', token)
      localStorage.setItem('roleId', user.roleId)
      localStorage.setItem('username', user.username)
      localStorage.setItem('fullname', user.fullname)
    },

    logout() {
      this.token = null
      this.roleId = null
      this.username = null
      this.fullname = null

      localStorage.removeItem('token')
      localStorage.removeItem('roleId')
      localStorage.removeItem('username')
      localStorage.removeItem('fullname')
    },
  },
})
