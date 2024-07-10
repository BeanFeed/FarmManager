<script setup>
import {onMounted, ref} from "vue";
import axios from "axios";
import {backendUrl} from "../../main.js";
import {toast} from "vue3-toastify";
import {router} from "../../router.js";

let issueTypes = ref([]);
let locations = ref([]);

onMounted(() => {
  refreshIssueTypes();
  refreshLocations();
})

function refreshIssueTypes() {
  let typeReq = axios.get(backendUrl + "/v1/ticket/getissuetypes", {withCredentials: true}).then(response => {
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
  let locationReq =  axios.get(backendUrl + "/v1/ticket/getlocations", {withCredentials: true}).then(response => {
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

function deleteIssueType(issue, repair) {
  //TODO: implement and add create issue type. Also implement locations
}
</script>

<template>
  <div class="flex items-center justify-center">
    <div class="w-full">
      <h1>Issue Types</h1>
      <hr class="my-2">
      <div class="h-[45vh] overflow-y-auto">
        <table
            class="min-w-full text-left text-surface ">
          <thead
              class="border-b border-gray-200 ">
          <tr>
            <th scope="col" class="px-6 py-4">Issue</th>
            <th scope="col" class="px-6 py-4">Repair</th>
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
    <div class="w-full">
      <h1>Locations</h1>
    </div>
  </div>
</template>

<style scoped>

</style>