import { createRouter, createWebHistory } from 'vue-router'

import MainLayout from './pages/MainLayout.vue'
import AuthLayout from './pages/AuthLayout.vue'

const router = createRouter({
  history: createWebHistory(import.meta.env.BASE_URL),
  routes: [
    {
      path: '/',
      component: MainLayout,
      children: [
        { path: '', name: 'Home', component: () => import('./components/home.vue') },
        { path: 'about', name: 'About', component: () => import('./components/about.vue') },
        { path: 'roles', name: 'RoleUpdate', component: () => import('@/components/RoleUpdater.vue') }
      ]
    },
    {
      path: '/login',
      component: AuthLayout,        
      children: [
        { path: '', name: 'Login', component: () => import('./components/login-user.vue') }
      ],
      meta: { public: true }
    }
  ]
})

export default router
