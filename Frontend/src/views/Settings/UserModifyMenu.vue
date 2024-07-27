<script setup>
import {ref} from "vue";
import axios from "axios";
import {backendUrl} from "../../main.js";
import {toast} from "vue3-toastify";
import {router} from "../../router.js";
import {useRoute} from "vue-router";

let emits = defineEmits(['close']);
let props = defineProps(['user']);

let route = useRoute();

let newName = ref("");
let newRole = ref("");
let newPassword = ref("");

function submit() {
  let data = {
    id: props.user.id
  };
  
  if (newName.value !== "") data.name = newName.value;
  if (newRole.value !== "") data.role = newRole.value;
  if (newPassword.value !== "") data.password = newPassword.value;
  
  let req = axios.post(backendUrl + "/v1/user/modifyuser", data, {withCredentials: true}).then(response => {
    close();
  }).catch(error => {
    toast(error.response.data.length < 30 ? error.response.data : error.message, {
      "type": "error",
      "closeOnClick": true,
      "autoClose": 2000,
      "pauseOnFocusLoss": false,
      "transition": "bounce"
    });
    if (error.response.data === "User not logged in" || error.response.data === "Session Invalid") {
      router.push('/login');
    }
    close();
  })
}

function deleteUser() {
  let req = axios.delete(backendUrl + "/v1/user/deleteuser?userId=" + props.user.id,  {withCredentials: true}).then(response => {
    close();
  }).catch(error => {
    toast(error.response.data.length < 30 ? error.response.data : error.message, {
      "type": "error",
      "closeOnClick": true,
      "autoClose": 2000,
      "pauseOnFocusLoss": false,
      "transition": "bounce"
    });
  });
}

function close() {
  let newName = ref("");
  let newRole = ref("");
  let newPassword = ref("");
  
  emits('close');
}
</script>

<template>
  <div class="fixed top-0 left-0 fixHeight w-screen bg-black bg-opacity-25 z-30 flex items-center justify-center" @click.self="$emit('close')">
    <div class="bg-white p-6 rounded-3xl">
      <h1>Modify User</h1>
      <hr class="my-2">
      <p class="text-center">Current Name: {{user.name}}</p>
      <hr class="my-2">
      <p class="text-left">Name</p>
      <div class="max-w-4xl w-full rounded-lg bg-gray-200 mt-1">
        <ul class="flex items-center">
          <li class="mx-1 w-full">
            <input type="text" v-model="newName" class="w-full text-green-500 focus:outline-none bg-gray-200">
          </li>
        </ul>
      </div>
      <hr class="my-2">
      <p class="text-left">Role</p>
      <select v-model="newRole" class="text-green-500 p-1 bg-gray-200 rounded-lg w-full">
        <option value="Member">Member</option>
        <option value="Technician">Technician</option>
        <option value="HeadTechnician">Head Technician</option>
      </select>
      <hr class="my-2">
      <p class="text-left">Password</p>
      <div class="max-w-4xl w-full rounded-lg bg-gray-200 mt-1">
        <ul class="flex items-center">
          <li class="mx-1 w-full">
            <input type="text" v-model="newPassword" class="w-full text-green-500 focus:outline-none bg-gray-200">
          </li>
        </ul>
      </div>
      <hr class="my-2">
      <div class="flex justify-center">
        <div @click="submit" class="w-full bg-green-500 hover:bg-green-600 cursor-pointer p-2 rounded-lg mr-1">
          <p class="text-white">Submit</p>
        </div>
        <div @click="close" class="w-full bg-gray-200 hover:bg-gray-300 cursor-pointer p-2 rounded-lg mx-1">
          <p>Cancel</p>
        </div>
        <div @click="deleteUser" class="w-full bg-red-500 hover:bg-red-600 cursor-pointer p-2 rounded-lg ml-1">
          <p class="text-white">Delete</p>
        </div>
      </div>
    </div>
  </div>
</template>

<style scoped>

</style>