import { createRouter, createWebHistory } from 'vue-router'
import { useAuthStore } from '@/stores/auth.js'
import Layout from '@/components/Layout/Layout.vue'

const router = createRouter({
  history: createWebHistory(import.meta.env.BASE_URL),
  routes: [
    {
      path: '/',
      redirect: '/dashboard',
    },

    // Login page - KHÔNG có Layout
    {
      path: '/login',
      component: () => import('@/views/Login.vue'),
      meta: { transition: 'login' },
    },

    // Tất cả các trang khác - CÓ Layout
    {
      path: '/',
      component: Layout,
      meta: { requiresAuth: true },
      children: [
        {
          path: 'dashboard',
          component: () => import('@/views/Dashboard.vue'),
          meta: { transition: 'login' },
        },
        {
          path: 'devices',
          component: () => import('@/views/DeviceManagement.vue'),
        },
        {
          path: 'users',
          component: () => import('@/views/UserManagement.vue'),
        },

        {
          path: 'history',
          component: () => import('@/views/History.vue'),
        },
        {
          path: 'history-user',
          component: () => import('@/views/HistoryUser.vue'),
        },
        {
          path: 'user-borrow',
          component: () => import('@/views/UserBorrow.vue'),
        },
        {
          path: 'profile',
          component: () => import('@/views/Profile.vue'),
        },
        {
          path: 'requests',
          component: () => import('@/views/Requests.vue'),
        },
        {
          path: '/reports',
          component: () => import('@/views/ReportManagement.vue'),
        },
      ],
    },
  ],
})

router.beforeEach((to, from, next) => {
  const auth = useAuthStore()
  const isLoggedIn = auth.isAuthenticated
  const roleId = Number(auth.roleId)

  // Nếu chưa login mà vào trang cần auth
  if (to.meta.requiresAuth && !isLoggedIn) {
    return next('/login')
  }

  // Nếu đã login mà vào trang login → điều hướng theo role
  if (isLoggedIn && to.path === '/login') {
    return roleId === 1 ? next('/dashboard') : next('/user-borrow')
  }

  // Nếu user 2-3 vào trang admin
  if (
    isLoggedIn &&
    (roleId === 2 || roleId === 3) &&
    ['/dashboard', '/devices', '/users', '/reports', '/history'].includes(to.path)
  ) {
    return next('/user-borrow')
  }

  // Nếu admin vào trang user
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
//       redirect: '/user-borrow',
//     },

//     { path: '/login', component: () => import('@/views/Login.vue') },

//     {
//       path: '/dashboard',
//       component: () => import('@/views/Dashboard.vue'),
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
//       path: '/history-user',
//       component: () => import('@/views/HistoryUser.vue'),
//       meta: { requiresAuth: true },
//     },
//     {
//       path: '/user-borrow',
//       component: () => import('@/views/UserBorrow.vue'),
//       meta: { requiresAuth: true },
//     },
//     {
//       path: '/profile',
//       component: () => import('@/views/Profile.vue'),
//       meta: { requiresAuth: true },
//     },

//     {
//       path: '/requests',
//       component: () => import('@/views/Requests.vue'),
//       meta: { requiresAuth: true },
//     },
//   ],
// })

// router.beforeEach((to, from, next) => {
//   const auth = useAuthStore()
//   const isLoggedIn = auth.isAuthenticated
//   const roleId = Number(auth.roleId) // ✅ ép số

//   // Nếu chưa login mà vào trang cần auth
//   if (to.meta.requiresAuth && !isLoggedIn) {
//     return next('/login')
//   }

//   // ✅ Nếu đã login mà vào trang login → điều hướng theo role
//   if (isLoggedIn && to.path === '/login') {
//     return roleId === 1 ? next('/dashboard') : next('/user-borrow')
//   }

//   // ✅ Nếu user 2-3 vào trang admin
//   if (
//     isLoggedIn &&
//     (roleId === 2 || roleId === 3) &&
//     ['/dashboard', '/devices', '/users', '/reports', '/history'].includes(to.path)
//   ) {
//     return next('/user-borrow')
//   }

//   // ✅ Nếu admin vào trang user
//   if (isLoggedIn && roleId === 1 && ['/user-borrow'].includes(to.path)) {
//     return next('/dashboard')
//   }

//   next()
// })

// export default router
