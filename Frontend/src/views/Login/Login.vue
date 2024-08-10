<script setup>
import TextInput from "../../components/TextInput.vue";
import {onMounted, ref, watch} from "vue";
import axios from "axios";
import {useBrowserLocation} from "@vueuse/core";
import {backendUrl} from "../../main.js";
import {UserStore} from "../../store/UserStore.js";
import {toast} from "vue3-toastify";
import {router} from "../../router.js";
import {useRoute} from "vue-router";
import {TokenStore} from "../../store/TokenStore.js";

let username = ref();
let password = ref();
let failed = ref(false);
let route = useRoute();
let loading = ref(false);

let userStore = UserStore();
let tokenStore = TokenStore();
function login() {
  loading.value = true;
  if (!username.value || !username.value) {failed.value = true; loading.value = false; return;}
  let data = {
    name: username.value,
    password: password.value,
  }
  
  let req = axios.post(backendUrl + "/api/user/login", data, {withCredentials: true}).then(response => {
    loading.value = false;
    let req2 = axios.get(backendUrl + "/api/user/me", {withCredentials: true}).then(res => {
      loading.value = false;
      userStore.setUser({
        name: res.data.name,
        id: res.data.id,
        role: res.data.role === "HeadTechnician" ? "Head Technician" : res.data.role
      });
      failed.value = false;
      console.log(typeof route.query.returnPath)

    }).catch(error => {
      userStore.clearUser();
      loading.value = false;
      toast(error.response.data.length < 30 ? error.response.data : error.body, {
        "type": "error",
        "closeOnClick": true,
        "autoClose": 2000,
        "pauseOnFocusLoss": false,
        "transition": "bounce"
      });
    });
    
     
  }).catch(error => {
    loading.value = false;
    toast(error.response.data.length < 30 ? error.response.data : error.body, {
      "type": "error",
      "closeOnClick": true,
      "autoClose": 2000,
      "pauseOnFocusLoss": false,
      "transition": "bounce"
    });
  });
}

watch(userStore, () => {
  if(userStore.id !== null) router.push('/printers');
});

onMounted(() => {
  if(userStore.id !== null) router.push('/printers');
})
</script>

<template>
<div class="flex items-center justify-center fixHeight">
  <div>
    <h1 class="mb-4">Farm Manager</h1>
    <form v-on:submit.prevent="login" class="bg-white p-6 rounded-3xl">
      <h1>Login</h1>
      <p class="text-left my-2">Name <span v-if="failed" class="text-red-500">Required</span></p>
      <div class="max-w-4xl w-full p-3 rounded-3xl bg-gray-100">
        <ul class="flex items-center">
          <li class="mx-1 w-full">
            <input type="text" v-model="username" class="w-full text-green-500 focus:outline-none bg-gray-100">
          </li>
        </ul>
      </div>
      <p class="text-left mt-4 mb-2">Password <span v-if="failed" class="text-red-500">Required</span></p>
      <div class="max-w-4xl w-full p-3 rounded-3xl bg-gray-100">
        <ul class="flex items-center">
          <li class="mx-1 w-full">
            <input type="password" v-model="password" class="w-full text-green-500 focus:outline-none bg-gray-100">
          </li>
        </ul>
      </div>
      <button class="w-full bg-green-500 text-white p-3 rounded-3xl mt-4 cursor-pointer hover:bg-green-600" @click="login">
        <template v-if="!loading">
          Login
        </template>
        <template v-else>
          <div class="text-center">
            <div role="status">
              <svg aria-hidden="true" class="inline w-5 h-5 text-green-500 animate-spin fill-white" viewBox="0 0 100 101" fill="none" xmlns="http://www.w3.org/2000/svg">
                <path d="M100 50.5908C100 78.2051 77.6142 100.591 50 100.591C22.3858 100.591 0 78.2051 0 50.5908C0 22.9766 22.3858 0.59082 50 0.59082C77.6142 0.59082 100 22.9766 100 50.5908ZM9.08144 50.5908C9.08144 73.1895 27.4013 91.5094 50 91.5094C72.5987 91.5094 90.9186 73.1895 90.9186 50.5908C90.9186 27.9921 72.5987 9.67226 50 9.67226C27.4013 9.67226 9.08144 27.9921 9.08144 50.5908Z" fill="currentColor"/>
                <path d="M93.9676 39.0409C96.393 38.4038 97.8624 35.9116 97.0079 33.5539C95.2932 28.8227 92.871 24.3692 89.8167 20.348C85.8452 15.1192 80.8826 10.7238 75.2124 7.41289C69.5422 4.10194 63.2754 1.94025 56.7698 1.05124C51.7666 0.367541 46.6976 0.446843 41.7345 1.27873C39.2613 1.69328 37.813 4.19778 38.4501 6.62326C39.0873 9.04874 41.5694 10.4717 44.0505 10.1071C47.8511 9.54855 51.7191 9.52689 55.5402 10.0491C60.8642 10.7766 65.9928 12.5457 70.6331 15.2552C75.2735 17.9648 79.3347 21.5619 82.5849 25.841C84.9175 28.9121 86.7997 32.2913 88.1811 35.8758C89.083 38.2158 91.5421 39.6781 93.9676 39.0409Z" fill="currentFill"/>
              </svg>
              <span class="sr-only">Loading...</span>
            </div>
          </div>
        </template>
      </button>
    </form>
  </div>
</div>
</template>

<style scoped>

</style>