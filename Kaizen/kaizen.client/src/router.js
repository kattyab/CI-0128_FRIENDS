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
        { path: 'about', name: 'About', component: () => import('./components/about.vue') }
      ]
    },
    {
      path: '/login',
      component: AuthLayout,
      meta: { public: true },        
      children: [

        { path: '', name: 'LoginUser', component: () => import('./components/loginUser.vue') },

        {path: 'register-company', name: 'RegisterCompany', component: () => import('./components/registerCompany.vue')

        }
      ]
    }
  ]
})

export default router
