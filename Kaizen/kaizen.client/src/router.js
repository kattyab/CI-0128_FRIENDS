import { createRouter, createWebHistory } from 'vue-router'
import axios from 'axios'

const router = createRouter({
  history: createWebHistory(import.meta.env.BASE_URL),
  routes: [
    {
      path: '/',
      component: () => import('./layouts/main.vue'),
      children: [
        { path: '', name: 'Home', component: () => import('./pages/home.vue'), meta: { public: true } },
        { path: 'about', name: 'About', component: () => import('./pages/about.vue'), meta: { public: true } },
        { path: 'landing-page', name: 'Landing-page', component: () => import('./pages/landing-page.vue'), meta: { requiresAuth: true } },
        { path: 'unauthorized', name: 'Unauthorized', component: () => import('./pages/unauthorized.vue'), meta: { public: true } },
        // { path: 'about', name: 'About', component: () => import('./pages/about.vue'), meta: { requiresAuth: true, requiredRoles: ['Administrador', 'Superadmin'] } } role restriction example
        { path: 'landing-page', name: 'Landing-page', component: () => import('./pages/landing-page.vue'), meta: { requiresAuth: true }},
        {
          path: 'registeremployee', name: 'RegisterEmployee', component: () => import('./pages/register-employee.vue'), meta: { requiresAuth: true, requiredRoles: ['Administrador', 'Dueño'] },  
        },
        { path: 'companies', name: 'Companies Index', component: () => import('./pages/companies/index.vue'), meta: { requiresAuth: true } },
        { path: 'companies/:id', name: 'Companies Show', component: () => import('./pages/companies/show.vue'), meta: { requiresAuth: true } },
        { path: 'employees', name: 'Employees Index', component: () => import('./pages/employees/index.vue'), meta: { requiresAuth: true } },
        { path: 'companieslist', name: 'CompaniesList', component: () => import('./components/Companies_list.vue'), meta: { requiresAuth: true, requiredRoles: ['Superadmin'] }},
      ]
    },
    {
      path: '/login',
      component: () => import('./layouts/auth.vue'),
      children: [
        { path: '', name: 'Login', component: () => import('./pages/login-user.vue') },
        //{ path: 'register-company', name: 'RegisterCompany', component: () => import('./components/registerCompany.vue') },
      ],
      meta: { public: true }
    }
  ]
});

router.beforeEach(async (to, from, next) => {
  const isPublic = to.meta.public;
  const requiresAuth = to.meta.requiresAuth;
  const requiredRoles = to.meta.requiredRoles;

  if (isPublic || !requiresAuth) return next();

  try {
    const res = await axios.get('/api/login/authenticate', { withCredentials: true });
    const userRole = res.data.role;

    if (requiredRoles && !requiredRoles.includes(userRole)) {
      return next('/unauthorized');
    }

    next();
  } catch (err) {
    next('/login');
  }
});


export default router
