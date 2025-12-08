import { defineStore } from "pinia";

// ❌ SỬA 1: TẮT CHẾ ĐỘ DEV ĐỂ TRÁNH LOGIN ẢO
const DEV_BYPASS_LOGIN = false;

export const useAuthStore = defineStore("auth", {
  state: () => ({
    // ❌ SỬA 2: ĐỔI "token" THÀNH "authToken" ĐỂ KHỚP VỚI API.JS
    token:
      localStorage.getItem("authToken") ||
      (DEV_BYPASS_LOGIN ? "DEV_ADMIN_TOKEN" : null),
    userId:
      Number(localStorage.getItem("userId")) || (DEV_BYPASS_LOGIN ? 1 : null),
    roleId:
      Number(localStorage.getItem("roleId")) || (DEV_BYPASS_LOGIN ? 1 : null),
    fullname:
      localStorage.getItem("fullname") ||
      (DEV_BYPASS_LOGIN ? "Admin Dev" : null),
    username:
      localStorage.getItem("username") || (DEV_BYPASS_LOGIN ? "admin" : null),
  }),

  getters: {
    isAuthenticated: (state) => {
      const hasToken = !!state.token;
      return hasToken;
    },

    roleName: (state) => {
      switch (state.roleId) {
        case 1:
          return "Admin";
        case 2:
          return "User";
        case 3:
          return "Guest";
        default:
          return "Unknown";
      }
    },
  },

  actions: {
    login(token, user) {
      this.token = token;
      this.userId = user.userId;
      this.roleId = user.roleId;
      this.username = user.username;
      this.fullname = user.fullname; // Lưu ý check xem API trả về fullName hay fullname

      // ❌ SỬA 3: LƯU KEY LÀ "authToken"
      localStorage.setItem("authToken", token);
      localStorage.setItem("userId", String(user.userId));
      localStorage.setItem("roleId", String(user.roleId));
      localStorage.setItem("username", user.username);
      localStorage.setItem("fullname", user.fullname);
    },

    logout() {
      this.token = null;
      this.userId = null;
      this.roleId = null;
      this.username = null;
      this.fullname = null;

      // ❌ SỬA 4: XÓA ĐÚNG KEY
      localStorage.removeItem("authToken");
      localStorage.removeItem("userId");
      localStorage.removeItem("roleId");
      localStorage.removeItem("username");
      localStorage.removeItem("fullname");

      // Force reload để xóa sạch state
      window.location.href = "/login";
    },
  },
});
