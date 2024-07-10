import { createApp } from 'vue'
import './style.css'
import 'bootstrap-icons/font/bootstrap-icons.css'
import "vue3-toastify/dist/index.css";

import App from './App.vue'
import {router} from "./router.js";

import {createPinia} from "pinia";
import piniaPluginPersistedstate from 'pinia-plugin-persistedstate'
import {useBrowserLocation} from "@vueuse/core";

const pinia = createPinia();
pinia.use(piniaPluginPersistedstate)

const location = useBrowserLocation().value;
export const backendUrl = import.meta.env.DEV ? location.protocol + "//" + location.hostname + (location.protocol === "http:" ? ":5001" : ":5000") : "https://api." + location.hostname


createApp(App).use(router).use(pinia).mount('#app')
