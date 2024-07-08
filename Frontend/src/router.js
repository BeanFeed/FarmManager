import {createRouter, createWebHistory} from 'vue-router'
import PrinterProfile from "./views/Printer/PrinterProfile.vue";
import PrinterOverview from "./views/Printer/PrinterOverview.vue";
import Login from "./views/Login/Login.vue";
import TicketOverview from "./views/Ticket/TicketOverview.vue";



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
    },
    {
        path: '/tickets',
        children: [
            {
                path: '',
                component: TicketOverview
            }
        ]
    },
    {
        path: '/login',
        component: Login
    }
]

export const router = createRouter({
    history: createWebHistory(),
    routes,
})