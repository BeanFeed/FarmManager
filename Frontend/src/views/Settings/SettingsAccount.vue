<script setup>
import {UserStore} from "../../store/UserStore.js";
import {ref} from "vue";
import axios from "axios";
import {toast} from "vue3-toastify";
import {router} from "../../router.js";
import {backendUrl} from "../../main.js";

let oldPassword = ref("");
let newPassword = ref("");
let retype = ref("");

let userStore = UserStore();

function changePassword() {
  let data = {
    oldPassword: oldPassword.value,
    newPassword: newPassword.value
  };
  
  if (newPassword.value !== retype.value) {
    toast("Password doesn't match", {
      "type": "error",
      "closeOnClick": true,
      "autoClose": 2000,
      "pauseOnFocusLoss": false,
      "transition": "bounce"
    });
    return;
  }
  
  let req = axios.post(backendUrl + "/v1/user/changepassword", data, {withCredentials: true}).then(response => {
    router.push('/login');
  }).catch(error => {
    toast(error.response.data.length < 30 ? error.response.data : error.body, {
      "type": "error",
      "closeOnClick": true,
      "autoClose": 2000,
      "pauseOnFocusLoss": false,
      "transition": "bounce"
    });
  });
}
</script>

<template>
  <div class="h-full">
    <div class="w-full">
      <div class="spaceChildren container-text-left">
        <h1 class="">{{userStore.name}}</h1>
        <p class="text-xl">Role: {{userStore.role}}</p>
        <p>Id: {{userStore.id}}</p>
      </div>
      
    </div>
    <div class="flex items-center justify-center">
      <div>
        <div class="w-1/8 container-text-left">
          <h2 class="text-4xl">Change Password</h2>
          <p class="mb-1 my-5">Old password</p>
          <div class="max-w-4xl w-full rounded-lg bg-gray-200 mt-1">
            <ul class="flex items-center">
              <li class="mx-1 w-full">
                <input type="password" v-model="oldPassword" class="w-full text-green-500 focus:outline-none bg-gray-200">
              </li>
            </ul>
          </div>
          <hr class="my-2">
          <p class="mb-1">New password</p>
          <div class="max-w-4xl w-full rounded-lg bg-gray-200 mt-1">
            <ul class="flex items-center">
              <li class="mx-1 w-full">
                <input type="password" v-model="newPassword" class="w-full text-green-500 focus:outline-none bg-gray-200">
              </li>
            </ul>
          </div>
          <hr class="my-2">
          <p class="mb-1">Retype password</p>
          <div class="max-w-4xl w-full rounded-lg bg-gray-200 mt-1">
            <ul class="flex items-center">
              <li class="mx-1 w-full">
                <input type="password" v-model="retype" class="w-full text-green-500 focus:outline-none bg-gray-200">
              </li>
            </ul>
          </div>
          <hr class="my-2">
          <div @click="changePassword" class="bg-green-500 p-2 rounded-2xl cursor-pointer hover:bg-green-600">
            <p class="text-white">Submit</p>
          </div>

        </div>

      </div>
      
    </div>
  </div>
</template>

<style scoped>
.spaceChildren * {
  margin: 0.75rem 0;
}


</style>