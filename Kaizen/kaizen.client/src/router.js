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
        { path: 'unauthorized', name: 'Unauthorized', component: () => import('./pages/errors/403.vue'), meta: { public: true } },
        { path: 'company/edit', name: 'Company Edit', component: () => import('./pages/company/edit.vue'), meta: { requiresAuth: true, requiredRoles: ['Dueño'] } },
        { path: 'companies', name: 'Companies Index', component: () => import('./pages/companies/index.vue'), meta: { requiresAuth: true, requiredRoles: ['Superadmin'] } },
        { path: 'companies/:id', name: 'Show Company', component: () => import('./pages/companies/show.vue'), meta: { requiresAuth: true, requiredRoles: ['Administrador', 'Dueño', 'Superadmin'] } },
        { path: 'companies/:id/edit', name: 'Edit Company', component: () => import('./pages/companies/edit.vue'), meta: { requiresAuth: true, requiredRoles: ['Superadmin'] } },
        { path: 'companieslist', name: 'CompaniesList', component: () => import('./pages/companies/companiesList.vue'), meta: { requiresAuth: true, requiredRoles: ['Superadmin'] } },
        { path: 'companieslist/:id', name: 'Companies Show', component: () => import('./pages/companies/show.vue'), meta: { requiresAuth: true, requiredRoles: ['Administrador', 'Superadmin'] } },
        { path: 'companyemployees', name: 'Companies Employees', component: () => import('./pages/employees/ByCompanyIndex.vue'), meta: { requiresAuth: true, requiredRoles: ['Administrador', 'Dueño'] } },
        { path: 'employees', name: 'Employees Index', component: () => import('./pages/employees/index.vue'), meta: { requiresAuth: true, requiredRoles: ['Superadmin'] } },
        { path: 'employees/register', name: 'Register Employee', component: () => import('./pages/employees/register.vue'), meta: { requiresAuth: true, requiredRoles: ['Administrador', 'Dueño'] } },
        { path: 'employees/:id', name: 'Employee Details', component: () => import('./pages/employees/show.vue'), meta: { requiresAuth: true, requiredRoles: ['Administrador', 'Dueño', 'Superadmin'] } },
        { path: 'employees/:id/edit', name: 'Employee Edit', component: () => import('./pages/employees/edit.vue'), meta: { requiresAuth: true, requiredRoles: ['Administrador', 'Dueño', 'Superadmin'] } },
        { path: 'review-hours', name: 'ReviewHours', component: () => import('./pages/review-hours.vue'), meta: { requiresAuth: true, requiredRoles: ['Supervisor'] } },
        { path: 'benefits/create', name: 'Benefit Creation', component: () => import('./pages/benefits/create.vue'), meta: { requiresAuth: true, requiredRoles: ['Administrador', 'Dueño'] } },
        { path: 'benefits/:id/edit', name: 'Edit Benefit', component: () => import('./pages/benefits/edit.vue'), meta: { requiresAuth: true, requiredRoles: ['Administrador', 'Dueño'] } },
        { path: 'benefits/subscribe', name: 'Benefit Subscription', component: () => import('./pages/benefits/subscribe.vue'), meta: { requiresAuth: true, requiredRoles: ['Administrador', 'Supervisor', 'Empleado'] } },
        { path: 'company', name: 'Company', component: () => import('./pages/companies/company.vue'), meta: { requiresAuth: true, requiredRoles: ['Administrador', 'Dueño'] } },
        { path: 'registerhours', name: 'Register Hours', component: () => import('./pages/employees/registerHours.vue'), meta: { requiresAuth: true, requiredRoles: ['Empleado'], requiresRegistersHours: true } },
        { path: 'payroll',name: 'Payroll',component: () => import('./pages/payroll/payroll.vue'), meta: { requiresAuth: true, requiredRoles: ['Administrador', 'Dueño', ] },
        },
      ]
    },
    {
      path: '/auth',
      component: () => import('./layouts/auth.vue'),
      children: [
        { path: 'login', name: 'Login', component: () => import('./pages/auth/login.vue') },
        { path: 'register-company', name: 'RegisterCompany', component: () => import('./pages/companies/register.vue') },
      ],
      meta: { public: true }
    },
    {
      path: '/:pathMatch(.*)*',
      name: 'NotFound',
      component: () => import('./pages/errors/404.vue'),
      meta: { public: true }
    }
  ]
});

router.beforeEach(async (to, from, next) => {
  const isPublic = to.meta.public;
  const requiresAuth = to.meta.requiresAuth;
  const requiredRoles = to.meta.requiredRoles;
  const requiresRegistersHours = to.meta.requiresRegistersHours;

  if (isPublic || !requiresAuth) return next();

  try {
    const res = await axios.get(`${import.meta.env.VITE_API_URL}/api/login/authenticate`, { withCredentials: true });
    const userRole = res.data.role;

    if (requiredRoles && !requiredRoles.includes(userRole)) {
      return next('/unauthorized');
    }

    if (requiresRegistersHours) {
      const userInfo = await axios.get(`${import.meta.env.VITE_API_URL}/api/Auth/userinfo`, { withCredentials: true });
      const registersHours = userInfo.data.registersHours;

      if (!registersHours) {
        return next('/unauthorized'); 
      }
    }

    next();
  } catch (err) {
    next('/auth/login');
  }
});

export default router
