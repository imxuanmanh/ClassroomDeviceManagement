import { createRouter, createWebHistory } from 'vue-router'
import { useAuthStore } from '@/stores/auth.js'

const router = createRouter({
  history: createWebHistory(import.meta.env.BASE_URL),
  routes: [
    {
      path: '/',
      // redirect: '/login',
      redirect: '/dashboard',
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

// // simple auth guard using localStorage token flag
// router.beforeEach((to, from, next) => {
//   const auth = useAuthStore()
//   if (to.meta.requiresAuth && !auth.isAuthenticated) {
//     next({ path: '/login', query: { redirect: to.fullPath } })
//   } else {
//     next()
//   }
// })

export default router
