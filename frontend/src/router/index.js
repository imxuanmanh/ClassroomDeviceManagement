import { createRouter, createWebHistory } from 'vue-router'
import { useAuthStore } from '@/stores/auth.js'

const router = createRouter({
  history: createWebHistory(import.meta.env.BASE_URL),
  routes: [
    {
      path: '/',
      redirect: '/login',
      // redirect: '/usermanagement',
    },
    {
      path: '/login',
      component: () => import('@/views/Login.vue'),
    },
    {
      path: '/dashboard',
      component: () => import('@/views/UserBorrow.vue'),
      meta: { requiresAuth: true },
    },
    {
      path: '/devices',
      component: () => import('@/views/DeviceManagement.vue'),
      meta: { requiresAuth: true },
    },
    {
      path: '/users',
      component: () => import('@/views/UserManagement.vue'),
      meta: { requiresAuth: true },
    },
    {
      path: '/reports',
      component: () => import('@/views/Reports.vue'),
      meta: { requiresAuth: true },
    },
    {
      path: '/history',
      component: () => import('@/views/History.vue'),
      meta: { requiresAuth: true },
    },
    {
      path: '/user-borrow',
      component: () => import('@/views/UserBorrow.vue'),
      meta: { requiresAuth: true },
    },
    {
      path: '/profile',
      name: 'profile',
      component: () => import('@/views/Profile.vue'),
      meta: { requiresAuth: true },
    },
  ],
})

// simple auth guard using localStorage token flag
// router.beforeEach((to, from, next) => {
//   const auth = useAuthStore()
//   if (to.meta.requiresAuth && !auth.isAuthenticated) {
//     next({ path: '/login', query: { redirect: to.fullPath } })
//   } else {
//     next()
//   }
// })

router.beforeEach((to, from, next) => {
  const auth = useAuthStore()
  const isLoggedIn = auth.isAuthenticated
  const role = auth.role // lấy role từ Pinia (đã lưu khi login)

  // Nếu route cần đăng nhập mà chưa login
  if (to.meta.requiresAuth && !isLoggedIn) {
    return next({ path: '/login', query: { redirect: to.fullPath } })
  }

  // Nếu đã login → kiểm tra role
  if (isLoggedIn) {
    // Nếu là user mà lại vào trang của admin
    if (
      role === 'user' &&
      ['/dashboard', '/devices', '/users', '/reports', '/history'].includes(to.path)
    ) {
      return next('/user-borrow')
    }

    // Nếu là admin mà lại vào trang user
    if (role === 'admin' && ['/user-borrow'].includes(to.path)) {
      return next('/dashboard')
    }
  }

  next()
})

export default router
