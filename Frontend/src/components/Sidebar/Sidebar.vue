<script setup>
import {onMounted, ref, watch} from "vue";
import {router} from "../../router.js";
import {useRoute} from "vue-router";
import {useWindowSize} from "@vueuse/core";
import {UserStore} from "../../store/UserStore.js";
import {hasPerm} from "../../utils.js";
import axios from "axios";
import {backendUrl} from "../../main.js";
import {toast} from "vue3-toastify";
let route = useRoute()
let isOpenState = ref(false)
const { width } = useWindowSize()

const userStore = UserStore();

let options = ref({
  Printers: {
    icon: "bi bi-printer",
    text: "Printers",
    route: "/printers",
    role: "Member"
  },
  Tickets: {
    icon: "bi bi-ticket-detailed",
    text: "Tickets",
    route: "/tickets",
    role: "Technician"
  },
  Settings: {
    icon: "bi bi-gear",
    text: "Settings",
    route: "/settings/account",
    role: "Member"
  }
})


onMounted(() => {
  let req = axios.get(backendUrl + "/v1/user/me", {withCredentials: true}).then(res => {
    userStore.setUser({
      name: res.data.name,
      id: res.data.id,
      role: res.data.role === "HeadTechnician" ? "Head Technician" : res.data.role
    });
  }).catch(error => {
    toast(error.response.data.length < 30 ? error.response.data : error.body, {
      "type": "error",
      "closeOnClick": true,
      "autoClose": 2000,
      "pauseOnFocusLoss": false,
      "transition": "bounce"
    });
    if (error.response.data === "User not logged in" || error.response.data === "Session Invalid") {
      router.push('/login');
    }
  });
})

function getColor(setRoute) {
  if (route.fullPath.startsWith(setRoute)) return "bg-gray-200";
  return "bg-white";
}

function logout() {
  userStore.clearUser();
  let req = axios.post(backendUrl + "/v1/user/logout", {}, {withCredentials: true}).then().catch();
  router.push("/login")
}
</script>

<template>
<div v-if="!route.fullPath.startsWith('/login')" class="z-20 bg-white w-24 fixed top-0 -left-24 transition-transform duration-500 ease-in-out transform fixHeight" :class="isOpenState || width > 800 ? 'translate-x-full' : 'translate-x-0'">
  <div v-if="width <= 800" class="pointer-events-none flex items-center justify-center absolute top-0 left-24 h-24 w-24">
    <div @click="isOpenState = !isOpenState" class="pointer-events-auto bg-green-500 absolute p-0 rounded-3xl h-10 w-10 flex items-center justify-center cursor-pointer">
      <i class="bi bi-chevron-left text-2xl text-white transition-transform duration-500 ease-in-out" :class="isOpenState ? 'rotate-0' : 'rotate-180'"></i>
    </div>
  </div>
  <div class="overflow-auto listHeight">
    <ul>
      <li v-if="hasPerm(userStore, options.Printers.role)" @click="isOpenState = !isOpenState; router.push(options.Printers.route)" :class="getColor(options.Printers.route)" class=" z-10 h-24 border-b-2 flex items-center justify-center cursor-pointer">
        <div >
          <i class="text-green-500 text-2xl" :class="options.Printers.icon"></i>
          <h3 class="text-green-500">{{ options.Printers.text }}</h3>
        </div>
      </li>
      <li v-if="hasPerm(userStore, options.Tickets.role)" @click="isOpenState = !isOpenState; router.push(options.Tickets.route)" :class="getColor(options.Tickets.route)" class=" z-10 h-24 border-b-2 flex items-center justify-center cursor-pointer">
        <div >
          <i class="text-green-500 text-2xl" :class="options.Tickets.icon"></i>
          <h3 class="text-green-500">{{ options.Tickets.text }}</h3>
        </div>
      </li>
      <li v-if="hasPerm(userStore, options.Settings.role)" @click="isOpenState = !isOpenState; router.push(options.Settings.route)" :class="getColor(options.Settings.route)" class=" z-10 h-24 border-b-2 flex items-center justify-center cursor-pointer">
        <div>
          <i class="text-green-500 text-2xl" :class="options.Settings.icon"></i>
          <h3 class="text-green-500">{{ options.Settings.text }}</h3>
        </div>
      </li>

      <li @click="userStore.isLoggedIn ? logout() : router.push('/login')" class="w-24 h-24 bg-white border-b-2 flex items-center justify-center cursor-pointer">
        <div>
          <i class="text-green-500 text-4xl" :class="userStore.isLoggedIn ? 'bi bi-box-arrow-in-left' :'bi bi-box-arrow-right'"></i>
          <h3 class="text-green-500">{{userStore.isLoggedIn ? "Logout" : "Login"}}</h3>
        </div>

      </li>

    </ul>
  </div>
  
  
  
  
</div>
  <div @click="isOpenState = !isOpenState" v-if="!route.fullPath.startsWith('/login') && width <= 800" class="z-10 bg-black bg-opacity-50 fixHeight w-screen fixed top-0 -left-[100vw] transition-transform duration-500 ease-in-out transform" :class="isOpenState && width <= 800 ? 'translate-x-full' : 'translate-x-0'">
    
  </div>
</template>

<style scoped>
.fixHeight {
  min-height: 100vh;
}

.listHeight {
  max-height: calc(100vh - 6rem);
  max-height: calc(-webkit-fill-available - 6rem);
}
</style>