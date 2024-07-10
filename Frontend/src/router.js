import {createRouter, createWebHistory} from 'vue-router'
import PrinterProfile from "./views/Printer/PrinterProfile.vue";
import PrinterOverview from "./views/Printer/PrinterOverview.vue";
import Login from "./views/Login/Login.vue";
import TicketOverview from "./views/Ticket/TicketOverview.vue";
import SettingsOverview from "./views/Settings/SettingsOverview.vue";
import SettingsAccount from "./views/Settings/SettingsAccount.vue";
import SettingsUsers from "./views/Settings/SettingsUsers.vue";
import SettingsOptions from "./views/Settings/SettingsOptions.vue";



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
        path: '/settings',
        component: SettingsOverview,
        redirect: '/settings/account',
        children: [
            {
                path: '',
                redirect: '/settings/account'
            },
            {
                path: 'account',
                component: SettingsAccount,
                name: 'Account'
            },
            {
                path: 'users',
                component: SettingsUsers,
                name: 'Users'
            },
            {
                path: 'options',
                component: SettingsOptions,
                name: 'Options'
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