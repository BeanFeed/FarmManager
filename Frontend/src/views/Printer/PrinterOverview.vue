<script setup>
import TextInput from "../../components/SearchBar.vue";
import axios from "axios";
import {useRoute} from "vue-router";
import {onMounted, ref, watch} from "vue";
import {useBrowserLocation} from "@vueuse/core";
import { toast } from "vue3-toastify";
import {backendUrl} from "../../main.js";
import SearchBar from "../../components/SearchBar.vue";
import {router} from "../../router.js";
import {TokenStore} from "../../store/TokenStore.js";

let printers = ref({});
let tokenStore = TokenStore();

onMounted(() => {
  
  console.log()
  
  let loadingToast = toast.loading("Loading printers...", {
    transition: "bounce",
    closeOnClick: false,
    pauseOnHover: false
  });
  let req = axios.get(backendUrl + "/v1/printer/getprinters", {headers: {
      Authorization: tokenStore.getAccess + "," + tokenStore.getRefresh
    }}).then(response => {
    tokenStore.setTokens(response.headers["authorization"].split(',')[0],response.headers["authorization"].split(',')[1]);
    printers.value = response.data;
    toast.update(loadingToast, {render: "Loaded printers", type: "success", isLoading: false, autoClose: 2000});
  }).catch(error => {
    console.log(error.toJSON());
    toast.update(loadingToast, {render: error.response.data.length < 30 ? error.response.data : "Error loading printers. Try refreshing.", type: "error", isLoading: false, autoClose: 2000});
    
  })
});

function search(name) {
  axios.get(backendUrl + "/v1/printer/getprinters?name=" + name, {headers: {
      Authorization: tokenStore.getAccess + "," + tokenStore.getRefresh
    }}).then(response => {
    tokenStore.setTokens(response.headers["authorization"].split(',')[0],response.headers["authorization"].split(',')[1]);
    printers.value = response.data;
  }).catch(error => {
    console.log(error);
    toast(error.response.data.length < 30 ? error.response.data : error.message, {
      "type": "error",
      "closeOnClick": false,
      "pauseOnFocusLoss": false,
      "transition": "bounce"
    });
  })
}
</script>

<template>
<div class="fixHeight w-screen flex items-center justify-center p-3">
  <div class="w-[75vw]">
    <SearchBar  @update="args => search(args.value)" />
    <div class="bg-white h-[50vh] p-6 rounded-3xl mt-4">
      <div class="w-full h-full overflow-auto scrollbar scrollbar-track-white">
        <table
            class="min-w-full text-left text-surface ">
          <thead
              class="border-b border-gray-200 ">
          <tr>
            <th scope="col" class="px-6 py-4">Name</th>
            <th scope="col" class="px-6 py-4">Serial Number</th>
            <th scope="col" class="px-6 py-4">Brand</th>
            <th scope="col" class="px-6 py-4">Model</th>
            <th scope="col" class="px-6 py-4">Location</th>
            <th scope="col" class="px-6 py-4">Purchase Date</th>
            <th scope="col" class="px-6 py-4"></th>
          </tr>
          </thead>
          <tbody>
          <tr v-for="(printer, index) in printers" class="border-b border-gray-200">
            <td class="whitespace-nowrap px-6 py-4">{{ printer.name }}</td>
            <td class="whitespace-nowrap px-6 py-4">{{ printer.serialNumber }}</td>
            <td class="whitespace-nowrap px-6 py-4">{{ printer.brand }}</td>
            <td class="whitespace-nowrap px-6 py-4">{{ printer.model }}</td>
            <td class="whitespace-nowrap px-6 py-4">{{ printer.location.name }}</td>
            <td class="whitespace-nowrap px-6 py-4">{{ new Date(printer.purchaseDate).toLocaleDateString()}}</td>
            <td class="whitespace-nowrap px-6 py-4"><a @click="router.push('/printers/profile/' + printer.name)" class="bg-green-500 p-2 rounded-2xl text-white hover:bg-green-600 hover:text-white cursor-pointer">View printer</a></td>
          </tr>

          </tbody>
        </table>
      </div>
      
    </div>
    
  </div>
  
  
</div>
</template>

<style scoped>

</style>