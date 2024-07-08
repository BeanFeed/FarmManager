<script setup>
import {onMounted, ref, watch} from "vue";
import {router} from "../../router.js";
import {useRoute} from "vue-router";
import {useWindowSize} from "@vueuse/core";
import {UserStore} from "../../store/UserStore.js";
import {hasPerm} from "../../utils.js";
import {TokenStore} from "../../store/TokenStore.js";
let route = useRoute()
let isOpenState = ref(false)
const { width } = useWindowSize()

const userStore = UserStore();
const tokenStore = TokenStore();

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
  }
})




function getColor(setRoute) {
  if (route.fullPath.startsWith(setRoute)) return "bg-gray-200";
  return "bg-white";
}

function logout() {
  userStore.clearUser();
  tokenStore.clearTokens();
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
      <li v-if="hasPerm(userStore, options.Printers.role)" @click="router.push(options.Printers.route)" :class="getColor(options.Printers.route)" class=" z-10 h-24 border-b-2 flex items-center justify-center cursor-pointer">
        <div >
          <i class="text-green-500 text-2xl" :class="options.Printers.icon"></i>
          <h3 class="text-green-500">{{ options.Printers.text }}</h3>
        </div>
      </li>
      <li v-if="hasPerm(userStore, options.Tickets.role)" @click="router.push(options.Tickets.route)" :class="getColor(options.Tickets.route)" class=" z-10 h-24 border-b-2 flex items-center justify-center cursor-pointer">
        <div >
          <i class="text-green-500 text-2xl" :class="options.Tickets.icon"></i>
          <h3 class="text-green-500">{{ options.Tickets.text }}</h3>
        </div>
      </li>
      

    </ul>
  </div>
  
  <div @click="userStore.isLoggedIn ? logout() : router.push('/login')" class="w-24 h-24 bg-white absolute left-0 bottom-0 border-t-2 flex items-center justify-center cursor-pointer">
    <div>
      <i class="text-green-500 text-4xl" :class="userStore.isLoggedIn ? 'bi bi-box-arrow-in-left' :'bi bi-box-arrow-right'"></i>
      <h3 class="text-green-500">{{userStore.isLoggedIn ? "Logout" : "Login"}}</h3>
    </div>
    
  </div>
  
  
</div>
  <div v-if="!route.fullPath.startsWith('/login') && width < 800" class="z-10 bg-black bg-opacity-50 fixHeight w-screen fixed top-0 -left-[100vw] transition-transform duration-500 ease-in-out transform" :class="isOpenState || width > 800 ? 'translate-x-full' : 'translate-x-0'">
    
  </div>
</template>

<style scoped>
.fixHeight {
  min-height: 100vh;
  min-height: -webkit-fill-available;
}

.listHeight {
  max-height: calc(100vh - 6rem);
  max-height: calc(-webkit-fill-available - 6rem);
}
</style>