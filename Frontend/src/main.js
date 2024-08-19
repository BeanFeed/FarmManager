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

function isDomain(str) {
    // Regular expression to match domain names
    const domainRegex = /^[a-zA-Z0-9]+([\-.]{1}[a-zA-Z0-9]+)*\.[a-zA-Z]{2,}$/;

    // Test the string against the regex
    return domainRegex.test(str);
}

//export const backendUrl = import.meta.env.DEV ? !isDomain(location.hostname) ? location.protocol + "//" + location.hostname + (location.protocol === "http:" ? ":5001" : ":5000") : location.origin : location.origin;
export const backendUrl = location.origin;
console.log(backendUrl);

createApp(App).use(router).use(pinia).mount('#app')
