import { createMemoryHistory, createRouter } from 'vue-router'
import {createPinia} from "pinia";
import PrintersPage from "./views/PrinterPage/PrinterOverview.vue";
import PrinterProfile from "./views/PrinterPage/PrinterProfile.vue";
import PrinterOverview from "./views/PrinterPage/PrinterOverview.vue";



const routes = [
    {
        path: '/',
        redirect: '/printers'
    },
    {
        path: '/printers',
        children: [
            {
                path: "",
                component: PrinterOverview
            },
            {
                path: "profile/:printerName",
                component: PrinterProfile
            }
        ]
    }
]

export const router = createRouter({
    history: createMemoryHistory(),
    routes,
})