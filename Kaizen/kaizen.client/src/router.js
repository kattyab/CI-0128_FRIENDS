import { createRouter, createWebHistory } from 'vue-router'

const router = createRouter({
  history: createWebHistory(import.meta.env.BASE_URL),
  routes: [
    {
      path: '/',
      component: import('./layouts/main.vue'),
      children: [
        { path: '', name: 'Home', component: () => import('./pages/home.vue') },
        { path: 'about', name: 'About', component: () => import('./pages/about.vue') }
      ]
    },
    {
      path: '/login',
      component: import('./layouts/auth.vue'),
      children: [
        { path: '', name: 'Login', component: () => import('./pages/login-user.vue') }
      ],
      meta: { public: true }
    }
  ]
})

export default router
