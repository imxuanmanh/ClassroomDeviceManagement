import { createRouter, createWebHistory } from 'vue-router'
import '@fortawesome/fontawesome-free/css/all.min.css'
import Dashboard from '@/views/Dashboard.vue'
import UserManagement from '@/views/UserManagement.vue'
import EquipmentManagement from '@/views/EquipmentManagement.vue'
import History from '@/views/History.vue'
import Reports from '@/views/Reports.vue'

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
  ],
})

export default router
