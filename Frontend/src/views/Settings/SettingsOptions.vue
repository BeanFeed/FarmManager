<script setup>
import {onMounted, ref} from "vue";
import axios from "axios";
import {backendUrl} from "../../main.js";
import {toast} from "vue3-toastify";
import {router} from "../../router.js";
import IssueTypeAddMenu from "./IssueTypeAddMenu.vue";
import LocationAddMenu from "./LocationAddMenu.vue";
import PrinterTypeAddMenu from "./PrinterTypeAddMenu.vue";

let issueTypes = ref([]);
let locations = ref([]);
let printerTypes = ref([]);

onMounted(() => {
  refreshIssueTypes();
  refreshLocations();
  refreshPrinterTypes();
})

function refreshIssueTypes() {
  let typeReq = axios.get(backendUrl + "/api/ticket/getissuetypes", {withCredentials: true}).then(response => {
    issueTypes.value = response.data;
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
  })
}

function refreshLocations() {
  let locationReq =  axios.get(backendUrl + "/api/ticket/getlocations", {withCredentials: true}).then(response => {
    locations.value = response.data;
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
}

function refreshPrinterTypes() {
  let printerReq = axios.get(backendUrl + "/api/printer/getprintertypes", {withCredentials: true}).then(response => {
    printerTypes.value = response.data;
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
}

let openNewIssueType = ref(false);
let openNewLocation = ref(false);
let openNewPrinterType = ref(false);

function deleteIssueType(issue, repair) {
  if (!confirm("Are you sure you want to delete this issue type?")) return;
  
  let req = axios.delete(backendUrl + "/api/ticket/removeissuetype?issue=" + issue + "&repair=" + repair, {withCredentials: true}).then(response => {
    refreshIssueTypes();
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
  })
}

function deleteLocation(locationName) {
  if (!confirm("Are you sure you want to delete this location?")) return;

  let req = axios.delete(backendUrl + "/api/ticket/removelocation?name=" + locationName, {withCredentials: true}).then(response => {
    refreshLocations();
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
  })
}

function deletePrinterType(id) {
  if (!confirm("Are you sure you want to delete this printer type?")) return;
  
  let req = axios.delete(backendUrl + "/api/printer/removeprintertype?id=" + id,  {withCredentials: true}).then(response => {
    refreshPrinterTypes();
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
}
</script>

<template>
  <div class="items-center justify-center">
    <div class="w-full">
      <h1>Issue Types</h1>
      <hr class="my-2">
      <div class="max-h-[30vh] overflow-auto">
        <table
            class="min-w-full text-left text-surface ">
          <thead
              class="border-b border-gray-200 ">
          <tr>
            <th scope="col" class="px-6 py-4">Issue</th>
            <th scope="col" class="px-6 py-4">Repair</th>
            <th scope="col" class="px-6 py-4">
              <div class="flex items-center">
                <div @click="openNewIssueType = true" class="text-white bg-green-500 py-1 px-6 rounded-xl hover:bg-green-600 cursor-pointer">Add</div>
              </div>
            </th>
          </tr>
          </thead>
          <tbody>
          <tr v-for="(issue, index) in issueTypes" class="border-b border-gray-200">
            <td class="whitespace-nowrap px-6 py-4">{{ issue.issue }}</td>
            <td class="whitespace-nowrap px-6 py-4">{{ issue.repair }}</td>
            <td class="whitespace-nowrap px-6 py-4">
              <div class="flex items-center">
                <div @click="deleteIssueType(issue.issue, issue.repair)" class="text-white bg-red-500 py-1 px-3 mx-1 rounded-xl hover:bg-red-600 cursor-pointer">Delete</div>
              </div>
            </td>
          </tr>

          </tbody>
        </table>
      </div>
      
    </div>
    <div class="w-full border-t-1">
      <h1>Locations</h1>
      <hr class="my-2">
      <div class="max-h-[30vh] overflow-auto">
        <table
            class="min-w-full text-left text-surface ">
          <thead
              class="border-b border-gray-200 ">
          <tr>
            <th scope="col" class="px-6 py-4">Location</th>
            <th scope="col" class="px-6 py-4">
              <div class="flex items-center">
                <div @click="openNewLocation = true" class="text-white bg-green-500 py-1 px-6 rounded-xl hover:bg-green-600 cursor-pointer">Add</div>
              </div>
            </th>
          </tr>
          </thead>
          <tbody>
          <tr v-for="(location, index) in locations" class="border-b border-gray-200">
            <td class="whitespace-nowrap px-6 py-4">{{ location.name }}</td>
            <td class="whitespace-nowrap px-6 py-4">
              <div class="flex items-center">
                <div @click="deleteLocation(location.name)" class="text-white bg-red-500 py-1 px-3 mx-1 rounded-xl hover:bg-red-600 cursor-pointer">Delete</div>
              </div>
            </td>
          </tr>

          </tbody>
        </table>
      </div>
    </div>
    <div class="w-full border-t-1">
      <h1>Printer Types</h1>
      <hr class="my-2">
      <div class="max-h-[30vh] overflow-auto">
        <table
            class="min-w-full text-left text-surface ">
          <thead
              class="border-b border-gray-200 ">
          <tr>
            <th scope="col" class="px-6 py-4">Brand</th>
            <th scope="col" class="px-6 py-4">Model</th>
            <th scope="col" class="px-6 py-4">
              <div class="flex items-center">
                <div @click="openNewPrinterType = true" class="text-white bg-green-500 py-1 px-6 rounded-xl hover:bg-green-600 cursor-pointer">Add</div>
              </div>
            </th>
          </tr>
          </thead>
          <tbody>
          <tr v-for="(printerType, index) in printerTypes" class="border-b border-gray-200">
            <td class="whitespace-nowrap px-6 py-4">{{ printerType.brand }}</td>
            <td class="whitespace-nowrap px-6 py-4">{{ printerType.model }}</td>
            <td class="whitespace-nowrap px-6 py-4">
              <div class="flex items-center">
                <div @click="deletePrinterType(printerType.printerTypeId)" class="text-white bg-red-500 py-1 px-3 mx-1 rounded-xl hover:bg-red-600 cursor-pointer">Delete</div>
              </div>
            </td>
          </tr>

          </tbody>
        </table>
      </div>
    </div>
		<LocationAddMenu v-if="openNewLocation" @close="openNewLocation = false; refreshLocations();" />
    <IssueTypeAddMenu v-if="openNewIssueType" @close="openNewIssueType = false; refreshIssueTypes();"/>
    <PrinterTypeAddMenu v-if="openNewPrinterType" @close="openNewPrinterType = false; refreshPrinterTypes();"/>
  </div>
</template>

<style scoped>

</style>