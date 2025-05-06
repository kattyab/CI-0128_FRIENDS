import { createRouter, createWebHistory } from 'vue-router'
import axios from 'axios'

const router = createRouter({
  history: createWebHistory(import.meta.env.BASE_URL),
  routes: [
    {
      path: '/',
      component: import('./layouts/main.vue'),
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

        { path: '', name: 'LoginUser', component: () => import('./pages/loginUser.vue') },
        {path: 'register-company', name: 'RegisterCompany', component: () => import('./components/registerCompany.vue')

        }
      ]
    }
  ]
})

export default router
