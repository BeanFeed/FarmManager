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
import {UserStore} from "../../store/UserStore.js";
import {hasPerm} from "../../utils.js";
import PrinterModifyMenu from "./PrinterModifyMenu.vue";
import PrinterAddMenu from "./PrinterAddMenu.vue";

let printers = ref({});

let route = useRoute();
let userStore = UserStore();

onMounted(() => {
  
  getPrinters()
});

function getPrinters() {
  let loadingToast = toast.loading("Loading printers...", {
    transition: "bounce",
    closeOnClick: false,
    pauseOnHover: false
  });
  let req = axios.get(backendUrl + "/api/printer/getprinters?byroom=" + searchOpt.value.byroom, {withCredentials: true}).then(response => {
    printers.value = response.data;
    toast.update(loadingToast, {"closeOnClick": true, render: "Loaded printers", type: "success", isLoading: false, autoClose: 2000});
  }).catch(error => {
    console.log(error.toJSON());
    toast.update(loadingToast, {"closeOnClick": true, render: error.response.data.length < 30 ? error.response.data : "Error loading printers. Try refreshing.", type: "error", isLoading: false, autoClose: 2000});
    if (error.response.data === "User not logged in" || error.response.data === "Session Invalid") {
      router.push('/login');
    }
  })
}

const searchOpt = ref({
  byroom: false,
  search: null
})

function search() {
  axios.get(backendUrl + "/api/printer/getprinters?name=" + searchOpt.value.search + "&byroom=" + searchOpt.value.byroom, {withCredentials: true}).then(response => {
    printers.value = response.data;
  }).catch(error => {
    console.log(error);
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

let newPrinter = ref(false);

let openModify = ref(false)
let modifyingPrinter = ref({});
function modifyPrinter(printer) {
  modifyingPrinter.value = printer;
  openModify.value = true;
}

function closeModifyPrinter() {
  openModify.value = false
  modifyingPrinter.value = {};

  axios.get(backendUrl + "/api/printer/getprinters?byroom=" + searchOpt.value.byroom, {withCredentials: true}).then(response => {
    printers.value = response.data;
  }).catch(error => {
    console.log(error);
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

function closeNewPrinter() {
  newPrinter.value = false;

  axios.get(backendUrl + "/api/printer/getprinters?byroom=" + searchOpt.value.byroom, {withCredentials: true}).then(response => {
    printers.value = response.data;
  }).catch(error => {
    console.log(error);
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
</script>

<template>
<div class="fixHeight w-screen flex items-center justify-center p-3">
  <div class="w-[75vw]">
    <div class="ml:flex items-center">
      <SearchBar  @update="args => {searchOpt.search = args.value; search(searchOpt)}" />
      <div @click="searchOpt.byroom = !searchOpt.byroom; getPrinters()" class="mr-6 ml:ml-6 flex items-center cursor-pointer">
        <div class="flex items-center justify-center h-6 w-6 bg-green-500 transition-colors ease-in-out duration-500 rounded-md border-2 border-green-500 cursor-pointer mr-1" :class="searchOpt.byroom ? 'bg-opacity-100' : 'bg-opacity-0'">
          <i class="bi bi-check-lg text-white transition-opacity ease-in-out duration-200" :class="searchOpt.byroom ? 'opacity-100' : 'opacity-0'"></i>
        </div>
        <p>Sort By Room</p>
      </div>
      <a @click="newPrinter = true" class="bg-green-500 hover:bg-green-600 text-white hover:text-white cursor-pointer h-10 mx-1 mt-2 ml:mt-0 px-3 py-1 rounded-xl flex items-center justify-center">
        Add Printer
      </a>
    </div>
    
    <div class="bg-white h-[50vh] p-6 rounded-3xl mt-2 ml:mt-4">
      <div class="w-full h-full overflow-auto">
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
            <td class="whitespace-nowrap px-6 py-4"><div class="flex items-center">
              <a @click="router.push('/printers/profile/' + printer.name)" class="bg-green-500 p-2 rounded-2xl hover:bg-green-600 text-white hover:text-white cursor-pointer">View printer</a>
              <template v-if="hasPerm(userStore, 'Head Technician')">
                <a @click="modifyPrinter(printer)" class="bg-gray-200 p-2 rounded-2xl hover:bg-gray-300 text-black hover:text-black cursor-pointer h-full px-3 flex justify-center items-center ml-2">Modify</a>
              </template>
            </div></td>
          </tr>

          </tbody>
        </table>
      </div>
      
    </div>
    
  </div>
  
  <PrinterModifyMenu v-if="openModify" :printer="modifyingPrinter" @close="closeModifyPrinter"/>
  <PrinterAddMenu v-if="newPrinter" @close="closeNewPrinter"/>
  
</div>
</template>

<style scoped>

</style>