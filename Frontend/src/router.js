import {createRouter, createWebHistory} from 'vue-router'
import PrinterProfile from "./views/Printer/PrinterProfile.vue";
import PrinterOverview from "./views/Printer/PrinterOverview.vue";
import Login from "./views/Login/Login.vue";
import TicketOverview from "./views/Ticket/TicketOverview.vue";
import SettingsOverview from "./views/Settings/SettingsOverview.vue";
import SettingsAccount from "./views/Settings/SettingsAccount.vue";
import SettingsUsers from "./views/Settings/SettingsUsers.vue";
import SettingsOptions from "./views/Settings/SettingsOptions.vue";
import axios from "axios";
import {backendUrl} from "./main.js";
import {hasPerm} from "./utils.js";



const routes = [
    {
        path: '/',
        redirect: '/printers',
    },
    {
        path: '/printers',
        children: [
            {
                path: "",
                component: PrinterOverview,
                name: 'PrinterOverview'
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
                component: TicketOverview,
                name: 'Tickets'
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
        component: Login,
        name: 'Login'
    }
]

export const router = createRouter({
    history: createWebHistory(),
    routes,
})

router.beforeEach((to,from) => {
    if(from.name !== 'Login') {

        let req = axios.get(backendUrl + "/api/user/me", {withCredentials: true}).then(response => {
            if((to.name === 'Users' && response.data.role !== "Owner") || (to.name === 'Options' && !hasPerm(response.data, 'Head Technician')) || (to.name === 'Tickets' && !hasPerm(response.data, 'Technician'))) return false;
        }).catch(error => {
            return {name: 'Login'}
        });

    }else return true;
})