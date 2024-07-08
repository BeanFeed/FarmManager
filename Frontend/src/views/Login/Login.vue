<script setup>
import TextInput from "../../components/TextInput.vue";
import {ref} from "vue";
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

let userStore = UserStore();
let tokenStore = TokenStore();
function login() {
  if (!username.value || !username.value) {failed.value = true; return;}
  let data = {
    name: username.value,
    password: password.value,
  }
  
  let req = axios.post(backendUrl + "/v1/user/login", data ).then(response => {
    tokenStore.setTokens(response.headers["authorization"].split(',')[0],response.headers["authorization"].split(',')[1]);
    
    let req2 = axios.get(backendUrl + "/v1/user/me", {headers: {
      Authorization: tokenStore.getAccess + "," + tokenStore.getRefresh
      }}).then(res => {
      userStore.setUser({
        name: res.data.name,
        id: res.data.id,
        role: res.data.role === "HeadTechnician" ? "HeadTechnician" : res.data.role
      });
      failed.value = false;
      console.log("Path: " + route.query.returnPath)
      if (typeof route.query.returnPath == "string") {
        console.log("To Path")
        router.push(route.query.returnPath);
        return;
      }
      router.push('/printers')
    });
    
     
  }).catch(error => {
    toast(error.response.data.length < 30 ? error.response.data : error.body, {
      "type": "error",
      "closeOnClick": false,
      "pauseOnFocusLoss": false,
      "transition": "bounce"
    });
  });
}
</script>

<template>
<div class="flex items-center justify-center fixHeight">
  <div>
    <h1 class="mb-4">Farm Manager</h1>
    <div class="bg-white p-6 rounded-3xl">
      <h1>Login</h1>
      <p class="text-left my-2" :class="failed ? 'text-red-500' : ''">Name</p>
      <div class="max-w-4xl w-full p-3 rounded-3xl bg-gray-100">
        <ul class="flex items-center">
          <li class="mx-1 w-full">
            <input type="text" v-model="username" class="w-full text-green-500 focus:outline-none bg-gray-100">
          </li>
        </ul>
      </div>
      <p class="text-left mt-4 mb-2" :class="failed ? 'text-red-500' : ''">Password</p>
      <div class="max-w-4xl w-full p-3 rounded-3xl bg-gray-100">
        <ul class="flex items-center">
          <li class="mx-1 w-full">
            <input type="password" v-model="password" class="w-full text-green-500 focus:outline-none bg-gray-100">
          </li>
        </ul>
      </div>
      <div @click="login" class="bg-green-500 text-white p-3 rounded-3xl mt-4 cursor-pointer hover:bg-green-600">
        <h3>Login</h3>
      </div>
    </div>
  </div>
</div>
</template>

<style scoped>

</style>