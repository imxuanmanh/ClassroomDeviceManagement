import { createRouter, createWebHistory } from "vue-router";
import { useAuthStore } from "@/stores/auth.js";
import Layout from "@/components/Layout/Layout.vue";

const router = createRouter({
  history: createWebHistory(import.meta.env.BASE_URL),
  routes: [
    {
      path: "/",
      redirect: "/dashboard",
    },

    // Login page - KHÔNG có Layout
    {
      path: "/login",
      component: () => import("@/views/Login.vue"),
      meta: { transition: "login" },
    },

    // Tất cả các trang khác - CÓ Layout
    {
      path: "/",
      component: Layout,
      meta: { requiresAuth: true },
      children: [
        {
          path: "dashboard",
          component: () => import("@/views/Dashboard.vue"),
          meta: { transition: "login" },
        },
        {
          path: "devices",
          component: () => import("@/views/DeviceManagement.vue"),
        },
        {
          path: "users",
          component: () => import("@/views/UserManagement.vue"),
        },
        {
          path: "history-user",
          component: () => import("@/views/HistoryUser.vue"),
        },
        {
          path: "user-borrow",
          component: () => import("@/views/UserBorrow.vue"),
        },
        {
          path: "requests",
          component: () => import("@/views/Requests.vue"),
        },
        {
          path: "/reports",
          component: () => import("@/views/ReportManagement.vue"),
        },
      ],
    },
  ],
});

router.beforeEach((to, from, next) => {
  const auth = useAuthStore();
  const isLoggedIn = auth.isAuthenticated;
  const roleId = Number(auth.roleId);

  console.log("🔹 Navigation:", from.path, "→", to.path);
  console.log("🔹 isLoggedIn:", isLoggedIn, "| roleId:", roleId);

  // ✅ 1. Nếu đã login mà vào trang login → điều hướng theo role
  if (to.path === "/login" && isLoggedIn) {
    console.log("✅ Đã login, redirect từ /login");
    return roleId === 1 ? next("/dashboard") : next("/user-borrow");
  }

  // ✅ 2. Nếu chưa login mà vào trang cần auth → về login
  if (to.meta.requiresAuth && !isLoggedIn) {
    console.log("❌ Chưa login, redirect về /login");
    return next("/login");
  }

  // ✅ 3. Nếu user (role 2-3) cố vào trang admin
  if (
    isLoggedIn &&
    (roleId === 2 || roleId === 3) &&
    ["/dashboard", "/devices", "/users", "/reports", "/history"].includes(
      to.path
    )
  ) {
    console.log("⚠️ User không được vào trang admin");
    return next("/user-borrow");
  }

  // ✅ 4. Nếu admin (role 1) cố vào trang user
  if (isLoggedIn && roleId === 1 && to.path === "/user-borrow") {
    console.log("⚠️ Admin không được vào trang user");
    return next("/dashboard");
  }

  // ✅ 5. Cho phép navigation
  console.log("✅ Navigation OK");
  next();
});

export default router;
