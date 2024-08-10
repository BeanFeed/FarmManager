<script setup>
import {onMounted, ref, watch} from "vue";
import axios from "axios";
import {backendUrl} from "../../main.js";
import {toast} from "vue3-toastify";
import {hasPerm} from "../../utils.js";
import {router} from "../../router.js";
import {useRoute} from "vue-router";

let emits = defineEmits(['close']);
let props = defineProps(['ticket']);
let route = useRoute();

let openedBy = ref(null);
let technician = ref(null);
let issue = ref("");
let issueCustom = ref("");
let repair = ref("");
let repairCustom = ref("");
let reopen = ref(false);

let allUsers = ref([]);
let allTechs = ref({});
let issueTypes = ref([]);
let issueNoDupes = ref([]);
let repairTypes = ref([]);

let loading = ref(false);
let delloading = ref(false);

onMounted(() => {
  axios.get(backendUrl + "/api/ticket/getissuevariants" ,{withCredentials: true}).then(response => {
    issueTypes.value = response.data;
    
    if (issueTypes.value.includes(props.ticket.issue)) {
      axios.get(backendUrl + "/api/ticket/getissuevariants?repairByIssue=" + props.ticket.issue, {withCredentials: true}).then(response => {
        repairTypes.value = response.data;
      }).catch(error => {
        toast(error.response.data.length < 30 ? error.response.data : error.body, {
          "type": "error",
          "closeOnClick": true,
          "autoClose": 2000,
          "pauseOnFocusLoss": false,
          "transition": "bounce"
        });
      })
    }
    
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

  axios.get(backendUrl + "/api/user/getusers" ,{withCredentials: true}).then(response => {
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
    if (error.response.data === "User not logged in" || error.response.data === "Session Invalid") {
      router.push('/login');
    }
  });
})

watch(issue, (value) => {
  if (value !== "custom") {
    axios.get(backendUrl + "/api/ticket/getissuevariants?repairByIssue=" + value, {withCredentials: true}).then(response => {
      repairTypes.value = response.data;
    }).catch(error => {
      toast(error.response.data.length < 30 ? error.response.data : error.body, {
        "type": "error",
        "closeOnClick": true,
        "autoClose": 2000,
        "pauseOnFocusLoss": false,
        "transition": "bounce"
      });
    })
  } else {
    repairTypes.value = [];
  }
});

function close() {
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
  loading.value = false;
  delloading.value = false;
  
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
  loading.value = true;
  
  axios.post(backendUrl + "/api/ticket/modifyticket", data, {withCredentials: true}).then(() => {
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
    loading.value = false;
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

function deleteTicket() {
  if (!confirm("Are you sure you want to delete this ticket?")) {
    close();
    return;
  }
  
  delloading.value = true;
  axios.delete(backendUrl + "/api/ticket/deleteticket?id=" + props.ticket.ticketId,{withCredentials: true}).then(() => {
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
    delloading.value = false;
    emits('close')
  }).catch(error => {
    delloading.value = false;
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
  <div class="fixed top-0 left-0 fixHeight w-screen bg-black bg-opacity-25 z-30 flex items-center justify-center" @click.self="$emit('close')">
    <form v-on:submit.prevent="submit" class="bg-white p-6 rounded-3xl w-96">
      <h1>Modify Ticket</h1>
      <hr class="my-2">
      <div class="flex flex-col">
        <p class="text-left">Opened By</p>
        <select v-model="openedBy" class="text-green-500 p-1 bg-gray-200 rounded-lg">
          <option v-for="(user) in allUsers" :value="user.id">{{user.name}}</option>
        </select>
        <hr class="my-2">
        <template v-if="ticket.dateClosed !== null && !reopen">
          <p class="text-left">Technician</p>
          <select v-model="technician" class="text-green-500 p-1 bg-gray-200 rounded-lg">
            <option v-for="(tech) in allTechs" :value="tech.id">{{tech.name}}</option>
          </select>
          <hr class="my-2">
        </template>
        <p class="text-left">Issue <span v-if="showRequired" class="text-red-500">Required</span></p>
        <select v-model="issue" class="text-green-500 p-1 bg-gray-200 rounded-lg">
          <option v-for="(issue) in issueTypes" :value="issue">{{issue}}</option>
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
            <option v-for="(repair) in repairTypes" :value="repair">{{repair}}</option>
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
          <button type="submit" class="w-full bg-green-500 hover:bg-green-600 disabled:bg-green-700 enabled:cursor-pointer p-2 rounded-lg mr-1 text-white" :disabled="loading || delloading">
            <template v-if="!loading">
              Submit
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
          <input @click="close" class="w-full bg-gray-200 hover:bg-gray-300 cursor-pointer p-2 rounded-lg mx-1" type="button" value="Cancel">
          <button @click="deleteTicket" @submit.prevent="" class="w-full bg-red-500 hover:bg-red-600 disabled:bg-red-700 enabled:cursor-pointer p-2 rounded-lg mr-1 text-white" :disabled="loading || delloading">
            <template v-if="!delloading">
              Delete
            </template>
            <template v-else>
              <div class="text-center">
                <div role="status">
                  <svg aria-hidden="true" class="inline w-5 h-5 text-red-500 animate-spin fill-white" viewBox="0 0 100 101" fill="none" xmlns="http://www.w3.org/2000/svg">
                    <path d="M100 50.5908C100 78.2051 77.6142 100.591 50 100.591C22.3858 100.591 0 78.2051 0 50.5908C0 22.9766 22.3858 0.59082 50 0.59082C77.6142 0.59082 100 22.9766 100 50.5908ZM9.08144 50.5908C9.08144 73.1895 27.4013 91.5094 50 91.5094C72.5987 91.5094 90.9186 73.1895 90.9186 50.5908C90.9186 27.9921 72.5987 9.67226 50 9.67226C27.4013 9.67226 9.08144 27.9921 9.08144 50.5908Z" fill="currentColor"/>
                    <path d="M93.9676 39.0409C96.393 38.4038 97.8624 35.9116 97.0079 33.5539C95.2932 28.8227 92.871 24.3692 89.8167 20.348C85.8452 15.1192 80.8826 10.7238 75.2124 7.41289C69.5422 4.10194 63.2754 1.94025 56.7698 1.05124C51.7666 0.367541 46.6976 0.446843 41.7345 1.27873C39.2613 1.69328 37.813 4.19778 38.4501 6.62326C39.0873 9.04874 41.5694 10.4717 44.0505 10.1071C47.8511 9.54855 51.7191 9.52689 55.5402 10.0491C60.8642 10.7766 65.9928 12.5457 70.6331 15.2552C75.2735 17.9648 79.3347 21.5619 82.5849 25.841C84.9175 28.9121 86.7997 32.2913 88.1811 35.8758C89.083 38.2158 91.5421 39.6781 93.9676 39.0409Z" fill="currentFill"/>
                  </svg>
                  <span class="sr-only">Loading...</span>
                </div>
              </div>
            </template>
          </button>
        </div>
      </div>
      
    </form>
  </div>  
</template>

<style scoped>

</style>