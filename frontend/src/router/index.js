import { createRouter, createWebHistory } from 'vue-router'
import { useAuthStore } from '@/stores/auth.js'

const router = createRouter({
  history: createWebHistory(import.meta.env.BASE_URL),
  routes: [
    {
      path: '/',
      redirect: '/login',
      //redirect: '/usermanagement',
    },

    { path: '/login', component: () => import('@/views/Login.vue') },

    {
      path: '/dashboard',
      component: () => import('@/views/Dashboard.vue'),
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
      component: () => import('@/views/Profile.vue'),
      meta: { requiresAuth: true },
    },

    {
      path: '/requests',
      component: () => import('@/views/Requests.vue'),
      meta: { requiresAuth: true },
    },
  ],
})

router.beforeEach((to, from, next) => {
  const auth = useAuthStore()
  const isLoggedIn = auth.isAuthenticated
  const roleId = Number(auth.roleId) // ✅ ép số

  // Nếu chưa login mà vào trang cần auth
  if (to.meta.requiresAuth && !isLoggedIn) {
    return next('/login')
  }

  // ✅ Nếu đã login mà vào trang login → điều hướng theo role
  if (isLoggedIn && to.path === '/login') {
    return roleId === 1 ? next('/dashboard') : next('/user-borrow')
  }

  // ✅ Nếu user 2-3 vào trang admin
  if (
    isLoggedIn &&
    (roleId === 2 || roleId === 3) &&
    ['/dashboard', '/devices', '/users', '/reports', '/history'].includes(to.path)
  ) {
    return next('/user-borrow')
  }

  // ✅ Nếu admin vào trang user
  if (isLoggedIn && roleId === 1 && ['/user-borrow'].includes(to.path)) {
    return next('/dashboard')
  }

  next()
})

export default router

// import { createRouter, createWebHistory } from 'vue-router'
// import { useAuthStore } from '@/stores/auth.js'

// const router = createRouter({
//   history: createWebHistory(import.meta.env.BASE_URL),
//   routes: [
//     {
//       path: '/',
//       //redirect: '/login',
//       redirect: '/usermanagement',
//     },
//     {
//       path: '/login',
//       component: () => import('@/views/Login.vue'),
//     },
//     {
//       path: '/dashboard',
//       component: () => import('@/views/UserBorrow.vue'),
//       meta: { requiresAuth: true },
//     },
//     {
//       path: '/devices',
//       component: () => import('@/views/DeviceManagement.vue'),
//       meta: { requiresAuth: true },
//     },
//     {
//       path: '/users',
//       component: () => import('@/views/UserManagement.vue'),
//       meta: { requiresAuth: true },
//     },
//     {
//       path: '/reports',
//       component: () => import('@/views/Reports.vue'),
//       meta: { requiresAuth: true },
//     },
//     {
//       path: '/history',
//       component: () => import('@/views/History.vue'),
//       meta: { requiresAuth: true },
//     },
//     {
//       path: '/user-borrow',
//       component: () => import('@/views/UserBorrow.vue'),
//       meta: { requiresAuth: true },
//     },
//     {
//       path: '/profile',
//       name: 'profile',
//       component: () => import('@/views/Profile.vue'),
//       meta: { requiresAuth: true },
//     },
//   ],
// })

// router.beforeEach((to, from, next) => {
//   const auth = useAuthStore()
//   const isLoggedIn = auth.isAuthenticated
//   const role = auth.role // lấy role từ Pinia (đã lưu khi login)

//   // Nếu route cần đăng nhập mà chưa login
//   if (to.meta.requiresAuth && !isLoggedIn) {
//     return next({ path: '/login', query: { redirect: to.fullPath } })
//   }

//   // Nếu đã login → kiểm tra role
//   if (isLoggedIn) {
//     // Nếu là user mà lại vào trang của admin
//     if (
//       role === 'user' &&
//       ['/dashboard', '/devices', '/users', '/reports', '/history'].includes(to.path)
//     ) {
//       return next('/user-borrow')
//     }

//     // Nếu là admin mà lại vào trang user
//     if (role === 'admin' && ['/user-borrow'].includes(to.path)) {
//       return next('/dashboard')
//     }
//   }

//   next()
// })

// export default router
