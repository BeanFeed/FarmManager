<script setup>
import {onMounted, ref, watch} from "vue";
import axios from "axios";
import {TokenStore} from "../../store/TokenStore.js";
import {backendUrl} from "../../main.js";
import {toast} from "vue3-toastify";
import {hasPerm} from "../../utils.js";

let emits = defineEmits(['close']);
let props = defineProps(['ticket']);

let openedBy = ref(null);
let technician = ref(null);
let issue = ref("");
let issueCustom = ref("");
let repair = ref("");
let repairCustom = ref("");
let reopen = ref(false);

let tokenStore = TokenStore();

let allUsers = ref([]);
let allTechs = ref({});
let issueTypes = ref([]);
let repairTypes = ref([]);
onMounted(() => {
  let issueReq = axios.get(backendUrl + "/v1/ticket/getissuetypes" ,{headers: {
      Authorization: tokenStore.getAccess + "," + tokenStore.getRefresh
    }}).then(response => {
    tokenStore.setTokens(response.headers["authorization"].split(',')[0],response.headers["authorization"].split(',')[1]);
    issueTypes.value = response.data;
  }).catch(error => {
    toast(error.response.data.length < 30 ? error.response.data : error.body, {
      "type": "error",
      "closeOnClick": true,
      "autoClose": 2000,
      "pauseOnFocusLoss": false,
      "transition": "bounce"
    });
  });

  let usersReq = axios.get(backendUrl + "/v1/user/getusers" ,{headers: {
      Authorization: tokenStore.getAccess + "," + tokenStore.getRefresh
    }}).then(response => {
    tokenStore.setTokens(response.headers["authorization"].split(',')[0],response.headers["authorization"].split(',')[1]);
    allUsers.value = response.data;
    for (let i = 0; i < response.data.length; i++) {
      if (hasPerm(response.data[i], "Technician")) allTechs.value[allTechs.value.length] = response.data[i];
    }
    console.log(allTechs.value)
  }).catch(error => {
    toast(error.response.data.length < 30 ? error.response.data : error.body, {
      "type": "error",
      "closeOnClick": true,
      "autoClose": 2000,
      "pauseOnFocusLoss": false,
      "transition": "bounce"
    });
  });
})

watch(issue, (value, oldValue) => {
  if (value === "custom") {
    repairTypes.value = [...issueTypes.value];
  } else {
    repairTypes.value = [];
    for (let i = 0; i < issueTypes.value.length; i++) {
      if (issueTypes.value[i].issue === issue.value) repairTypes.value[repairTypes.value.length] = issueTypes.value[i];
    }
  }
});

function cancel() {
  openedBy = ref(null);
  technician = ref(null);
  issue = ref("");
  issueCustom = ref("");
  repair = ref("");
  repairCustom = ref("");
  reopen = ref(false);
  allUsers = ref([]);
  allTechs = ref({});
  issueTypes = ref([]);
  
  emits('close');
}

function submit() {
  let data = {};
  if (openedBy.value !== null) data.openedBy = openedBy.value;
  if (issue.value !== "") {
    if (issue.value === "custom") data.issue = issueCustom.value;
    else data.issue = issue.value;
  }
  if (!reopen.value) {
    if (technician.value !== null) data.technician = technician.value;
    if (repair !== "") {
      if (repair.value === "custom") data.repair = repairCustom.value;
      else data.repair = repair.value;
    }
  } else data.reopen = true;
  data.id = props.ticket.ticketId;
  
  let req = axios.post(backendUrl + "/v1/ticket/modifyticket", data, {headers: {
      Authorization: tokenStore.getAccess + "," + tokenStore.getRefresh
    }}).then(response => {
    tokenStore.setTokens(response.headers["authorization"].split(',')[0],response.headers["authorization"].split(',')[1]);

    openedBy = ref(null);
    technician = ref(null);
    issue = ref("");
    issueCustom = ref("");
    repair = ref("");
    repairCustom = ref("");
    reopen = ref(false);
    allUsers = ref([]);
    allTechs = ref({});
    issueTypes = ref([]);
    
    emits('close')
  }).catch(error => {
    toast(error.response.data.length < 30 ? error.response.data : error.message, {
      "type": "error",
      "closeOnClick": true,
      "autoClose": 2000,
      "pauseOnFocusLoss": false,
      "transition": "bounce"
    });
  })
  
  
}

function deleteTicket() {
  let req = axios.delete(backendUrl + "/v1/ticket/deleteticket?id=" + props.ticket.ticketId,{headers: {
      Authorization: tokenStore.getAccess + "," + tokenStore.getRefresh
    }}).then(response => {
    tokenStore.setTokens(response.headers["authorization"].split(',')[0],response.headers["authorization"].split(',')[1]);

    openedBy = ref(null);
    technician = ref(null);
    issue = ref("");
    issueCustom = ref("");
    repair = ref("");
    repairCustom = ref("");
    reopen = ref(false);
    allUsers = ref([]);
    allTechs = ref({});
    issueTypes = ref([]);

    emits('close')
  }).catch(error => {
    toast(error.response.data.length < 30 ? error.response.data : error.message, {
      "type": "error",
      "closeOnClick": true,
      "autoClose": 2000,
      "pauseOnFocusLoss": false,
      "transition": "bounce"
    });
  })
}
</script>

<template>
  <div class="fixed top-0 left-0 fixHeight w-screen bg-black bg-opacity-25 z-30 flex items-center justify-center">
    <div class="bg-white p-6 rounded-3xl">
      <h1>Modify Ticket</h1>
      <hr class="my-2">
      <div class="flex flex-col">
        <p class="text-left">Opened By</p>
        <select v-model="openedBy" class="text-green-500 p-1 bg-gray-200 rounded-lg">
          <option v-for="(user, index) in allUsers" :value="user.id">{{user.name}}</option>
        </select>
        <hr class="my-2">
        <template v-if="ticket.dateClosed !== null && !reopen">
          <p class="text-left">Technician</p>
          <select v-model="technician" class="text-green-500 p-1 bg-gray-200 rounded-lg">
            <option v-for="(tech, index) in allTechs" :value="tech.id">{{tech.name}}</option>
          </select>
          <hr class="my-2">
        </template>
        <p class="text-left">Issue <span v-if="showRequired" class="text-red-500">Required</span></p>
        <select v-model="issue" class="text-green-500 p-1 bg-gray-200 rounded-lg">
          <option v-for="(issue, index) in issueTypes" :value="issue.issue">{{issue.issue}}</option>
          <option value="custom">Custom</option>
        </select>
        <template v-if="issue==='custom'">
          <div class="max-w-4xl w-full rounded-lg bg-gray-200 mt-1">
            <ul class="flex items-center">
              <li class="mx-1 w-full">
                <input placeholder="New Issue" type="text" v-model="issueCustom" class="w-full text-green-500 focus:outline-none bg-gray-200">
              </li>
            </ul>
          </div>
        </template>
        <hr class="my-2">
        <template v-if="ticket.dateClosed !== null && !reopen">
          <p class="text-left">Repair</p>
          <select v-model="repair" class="text-green-500 p-1 bg-gray-200 rounded-lg">
            <option v-for="(issue, index) in repairTypes" :value="issue.repair">{{issue.repair}}</option>
            <option value="custom">Custom</option>
          </select>
          <template v-if="repair==='custom'">
            <div class="max-w-4xl w-full rounded-lg bg-gray-200 mt-1">
              <ul class="flex items-center">
                <li class="mx-1 w-full">
                  <input placeholder="New Issue" type="text" v-model="repairCustom" class="w-full text-green-500 focus:outline-none bg-gray-200">
                </li>
              </ul>
            </div>
          </template>
          <hr class="my-2">
        </template>
        <template v-if="ticket.dateClosed !== null">
          <div @click="reopen = !reopen" class="flex items-center cursor-pointer">
            <div class="flex items-center justify-center h-6 w-6 bg-green-500 transition-colors ease-in-out duration-500 rounded-md border-2 border-green-500 cursor-pointer mr-1" :class="reopen ? 'bg-opacity-100' : 'bg-opacity-0'">
              <i class="bi bi-check-lg text-white transition-opacity ease-in-out duration-200" :class="reopen ? 'opacity-100' : 'opacity-0'"></i>
            </div>
            <p>Reopen ticket</p>
          </div>
          <hr class="my-2">
        </template>
        <div class="flex justify-center">
          <div @click="submit" class="bg-green-500 hover:bg-green-600 cursor-pointer p-2 rounded-lg mr-1">
            <p class="text-white">Submit</p>
          </div>
          <div @click="cancel" class="bg-gray-200 hover:bg-gray-300 cursor-pointer p-2 rounded-lg mx-1">
            <p>Cancel</p>
          </div>
          <div @click="deleteTicket" class="bg-red-500 hover:bg-red-600 cursor-pointer p-2 rounded-lg ml-1">
            <p class="text-white">Delete</p>
          </div>
        </div>
      </div>
      
    </div>
  </div>  
</template>

<style scoped>

</style>