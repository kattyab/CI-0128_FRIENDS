import { createRouter, createWebHistory } from 'vue-router';
import EmployeeDetail from '../components/EmployeeDetail.vue';

const routes = [
  {
    path: '/',
    name: 'EmployeeDetail',
    component: EmployeeDetail,
  },
];

const router = createRouter({
  history: createWebHistory(),
  routes,
});

export default router;
