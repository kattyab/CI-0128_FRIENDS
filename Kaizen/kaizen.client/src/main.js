import { createApp } from 'vue';
import App from './App.vue';
import router from './router';
import axios from 'axios';

import "bootstrap/dist/css/bootstrap.min.css";
import "bootstrap";
import './assets/css/main.css';

axios.defaults.withCredentials = true;

const app = createApp(App);

app.use(router);
app.mount('#app');
