<script setup>

import {router} from "../../router.js";
import {hasPerm} from "../../utils.js";
import {onMounted, ref} from "vue";
import axios from "axios";
import {toast} from "vue3-toastify";
import {backendUrl} from "../../main.js";
import UserModifyMenu from "./UserModifyMenu.vue";
import SearchBar from "../../components/SearchBar.vue";
import UserAddMenu from "./UserAddMenu.vue";
import {useRoute} from "vue-router";
import {UserStore} from "../../store/UserStore.js";

let users = ref([]);
let route = useRoute();
let userStore = UserStore();

onMounted(() => {
  if (userStore.role !== "Owner") {
    router.back();
    return;
  }
  
  let userLoadingToast = toast.loading("Loading users...", {
    transition: "bounce",
    closeOnClick: false,
    pauseOnHover: false
  });
  let userReq = axios.get(backendUrl + "/v1/user/getusers", {withCredentials: true}).then(response => {
    users.value = response.data;
    toast.update(userLoadingToast, {"closeOnClick": true, render: "Loaded users", type: "success", isLoading: false, autoClose: 2000});
  }).catch(error => {
    toast.update(userLoadingToast, {"closeOnClick": true, render: error.response.data.length < 30 ? error.response.data : "Error loading users. Try refreshing.", type: "error", isLoading: false, autoClose: 2000});
  })
});

let modifyUser = ref(false);
let modifyingUser = ref({});

function refresh() {
  let userReq = axios.get(backendUrl + "/v1/user/getusers", {withCredentials: true}).then(response => {
    users.value = response.data;
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
  })
}

function search(name) {
  let userReq = axios.get(backendUrl + "/v1/user/getusers?name=" + name, {withCredentials: true}).then(response => {
    users.value = response.data;
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
  })
}

let newUserOpen = ref(false);
</script>

<template>
  <div>
    <div class="ml:flex items-center">
      <SearchBar @update="args => search(args.value)" alt-color="bg-gray-200"/>
      <a @click="newUserOpen = true" class="bg-green-500 hover:bg-green-600 text-white hover:text-white cursor-pointer h-10 mx-1 mt-2 ml:mt-0 px-3 py-1 rounded-xl flex items-center justify-center">
        Add User
      </a>
    </div>
    <div class="w-full h-full overflow-auto">
      <table
          class="min-w-full text-left text-surface ">
        <thead
            class="border-b border-gray-200 ">
        <tr>
          <th scope="col" class="px-6 py-4">Id</th>
          <th scope="col" class="px-6 py-4">Name</th>
          <th scope="col" class="px-6 py-4">Role</th>
          <th scope="col" class="px-6 py-4"></th>
        </tr>
        </thead>
        <tbody>
        <tr v-for="(user, index) in users" class="border-b border-gray-200">
          <td class="whitespace-nowrap px-6 py-4">{{ user.id }}</td>
          <td class="whitespace-nowrap px-6 py-4">{{ user.name }}</td>
          <td class="whitespace-nowrap px-6 py-4">{{ user.role }}</td>
          <td class="whitespace-nowrap px-6 py-4">
            <div class="flex items-center">
              <div @click="modifyUser = true; modifyingUser = user;" class="bg-gray-200 py-1 px-3 mx-1 rounded-xl hover:bg-gray-300 cursor-pointer">Modify</div>
            </div>
          </td>
        </tr>

        </tbody>
      </table>
    </div>
    
    <UserModifyMenu v-if="modifyUser" :user="modifyingUser" @close="modifyingUser = {}; modifyUser = false; refresh();"/>
    <UserAddMenu v-if="newUserOpen" @close="newUserOpen = false; refresh();"/>
  </div>
</template>

<style scoped>

</style>