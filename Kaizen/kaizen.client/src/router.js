import { createWebHistory, createRouter } from "vue-router";

import HomePage from "./pages/home.vue";
import AboutPage from "./pages/about.vue";
import LoginPage from "./pages/login.vue";

const routes = [
  { path: "/", component: HomePage },
  { path: "/about", component: AboutPage },
  { path: "/login", component: LoginPage, meta: { hide_header: true, hide_footer: true, hide_menu: true } },
];

const router = createRouter({
  history: createWebHistory(),
  routes,
});

export default router;
