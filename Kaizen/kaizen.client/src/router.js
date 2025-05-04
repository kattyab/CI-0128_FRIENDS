import { createRouter, createWebHistory } from 'vue-router'
import axios from 'axios'

const router = createRouter({
  history: createWebHistory(import.meta.env.BASE_URL),
  routes: [
    {
      path: '/',
      component: import('./layouts/main.vue'),
      children: [
        { path: '', name: 'Home', component: () => import('./pages/home.vue'), meta: { public: true } },
        { path: 'about', name: 'About', component: () => import('./pages/about.vue'), meta: { public: true } },
        { path: 'landing-page', name: 'Landing-page', component: () => import('./pages/landing-page.vue'), meta: { requiresAuth: true }},
        { path: '', name: 'Home', component: () => import('./components/home.vue') },
        { path: 'about', name: 'About', component: () => import('./components/about.vue') },
        { path: 'roles', name: 'RoleUpdate', component: () => import('@/components/RoleUpdater.vue') }
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
});

router.beforeEach(async (to, from, next) => {
  const isPublic = to.meta.public;
  const requiresAuth = to.meta.requiresAuth;

  if (isPublic || !requiresAuth) {
    return next();
  }
  try {
    await axios.get('/api/login/whoami', { withCredentials: true });
    next(); // user is authenticated
  } catch (err) {
    next('/login'); // not authenticated
  }
});

export default router
