import { createRouter, createWebHistory } from 'vue-router'
import '@fortawesome/fontawesome-free/css/all.min.css'
import Dashboard from '@/views/Dashboard.vue'
import UserManagement from '@/views/UserManagement.vue'
import EquipmentManagement from '@/views/EquipmentManagement.vue'
import History from '@/views/History.vue'
import Reports from '@/views/Reports.vue'
import Login from '@/views/Login.vue'

const router = createRouter({
  history: createWebHistory(import.meta.env.BASE_URL),
  routes: [
    {
      path: '/',
      name: 'dashboard',
      component: Dashboard,
    },
    {
    path: '/equipment',
    name: 'equipment',
    component: EquipmentManagement,
    },
    {
      path: '/users',
      name: 'users',
      component: UserManagement,
    },
    {
      path: '/history',
      name: 'history',
      component: History,
    },
    {
      path: '/reports',
      name: 'reports',
      component: Reports,
    },
    {
      path: '/login',
      name: 'login',
      component: Login,
    },
    {
      path: '/profile',
      name: 'profile',
      component: () => import('@/views/Profile.vue'),
      meta: { requiresAuth: true }
    },
  ],
})

// simple auth guard using localStorage token flag
router.beforeEach((to, from, next) => {
  const isAuthenticated = !!localStorage.getItem('auth.token')
  if (to.meta && to.meta.requiresAuth && !isAuthenticated) {
    next({ name: 'login', query: { redirect: to.fullPath } })
    return
  }
  next()
})

export default router
