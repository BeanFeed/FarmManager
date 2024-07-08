<script setup>

import SearchBar from "../../components/SearchBar.vue";
import {onMounted, ref, watch} from "vue";
import {router} from "../../router.js";
import {toast} from "vue3-toastify";
import axios from "axios";
import {backendUrl} from "../../main.js";
import {TokenStore} from "../../store/TokenStore.js";
import TextInput from "../../components/TextInput.vue";

let searchOpt = ref({
  searchWord: "",
  onlyOpen : false,
  sortDescending: false,
});

let tokenStore = TokenStore();
let tickets = ref([]);
let newTicketOpen = ref(false);
let printerName = ref();
let issue = ref("");
let issueCustom = ref("");

onMounted(() => {
  let ticketLoadingToast = toast.loading("Loading tickets...", {
    transition: "bounce",
    closeOnClick: false,
    pauseOnHover: false
  });
  axios.get(backendUrl + "/v1/ticket/gettickets?printerName=" + searchOpt.value.searchWord + "&onlyOpen=" + searchOpt.value.onlyOpen + "&sortDescending=" + searchOpt.value.sortDescending, {headers: {
      Authorization: tokenStore.getAccess + "," + tokenStore.getRefresh
      }}).then(response => {
      tokenStore.setTokens(response.headers["authorization"].split(',')[0],response.headers["authorization"].split(',')[1]);
    toast.update(ticketLoadingToast, {render: "Loaded tickets", type: "success", isLoading: false, autoClose: 2000});
    tickets.value = response.data;
  }).catch(error => {
    toast.update(ticketLoadingToast, {render: error.response.data.length < 30 ? error.response.data : "Error loading tickets. Try refreshing.", type: "error", isLoading: false, autoClose: 2000});
  });
})

watch(searchOpt.value,() => {
  console.log("Query");
  let ticketLoadingToast = toast.loading("Loading tickets...", {
    transition: "bounce",
    closeOnClick: false,
    pauseOnHover: false
  });
  axios.get(backendUrl + "/v1/ticket/gettickets?printerName=" + searchOpt.value.searchWord + "&onlyOpen=" + searchOpt.value.onlyOpen + "&sortDescending=" + searchOpt.value.sortDescending, {headers: {
      Authorization: tokenStore.getAccess + "," + tokenStore.getRefresh
    }}).then(response => {
    tokenStore.setTokens(response.headers["authorization"].split(',')[0],response.headers["authorization"].split(',')[1]);
    toast.update(ticketLoadingToast, {render: "Loaded tickets", type: "success", isLoading: false, autoClose: 2000});
    tickets.value = response.data;
    console.log(tickets.value)
  }).catch(error => {
    toast.update(ticketLoadingToast, {render: error.response.data.length < 30 ? error.response.data : "Error loading tickets. Try refreshing.", type: "error", isLoading: false, autoClose: 2000});
  });
})

let printerOptions = ref([]);
let issueTypes = ref([]);
function newTicket() {
  newTicketOpen.value = true;
  let printerReq = axios.get(backendUrl + "/v1/printer/getprinters", {headers: {
      Authorization: tokenStore.getAccess + "," + tokenStore.getRefresh
    }}).then(response => {
      //console.log(response.headers)
      tokenStore.setTokens(response.headers["authorization"].split(',')[0],response.headers["authorization"].split(',')[1]);
      printerOptions.value = response.data;
  }).catch(error => {
    toast(error.response.data.length < 30 ? error.response.data : error.body, {
      "type": "error",
      "closeOnClick": false,
      "pauseOnFocusLoss": false,
      "transition": "bounce"
    });
  });
  let issueReq = axios.get(backendUrl + "/v1/ticket/getissuetypes", {headers: {
      Authorization: tokenStore.getAccess + "," + tokenStore.getRefresh
    }}).then(response => {
    //console.log(response.headers)
    tokenStore.setTokens(response.headers["authorization"].split(',')[0],response.headers["authorization"].split(',')[1]);
    issueTypes.value = response.data;
  }).catch(error => {
    console.log(error)
    toast(error.response.data.length < 30 ? error.response.data : error.body, {
      "type": "error",
      "closeOnClick": false,
      "pauseOnFocusLoss": false,
      "transition": "bounce"
    });
  });
}

function submitTicket() {
  let data = {
    issue: issue.value === 'custom' ? issueCustom.value : issue.value,
    printerName: printerName.value
  }
  
  let req = axios.post(backendUrl + "/v1/ticket/openticket", data, {headers: {
      Authorization: tokenStore.getAccess + "," + tokenStore.getRefresh
    }}).then(response => {
    newTicketOpen.value = false;
  }).catch()
}
</script>

<template>
  <div class="fixHeight flex items-center justify-center">
    <div class="w-[75vw]">
      <div class="ml:flex items-center">
        <SearchBar @update="args => searchOpt.searchWord=args.value" />
        <div class="flex items-center my-2 ml:my-0">
          <div @click="searchOpt.onlyOpen = !searchOpt.onlyOpen" class="mx-6 flex items-center cursor-pointer">
            <div class="flex items-center justify-center h-6 w-6 bg-green-500 transition-colors ease-in-out duration-500 rounded-md border-2 border-green-500 cursor-pointer mr-1" :class="searchOpt.onlyOpen ? 'bg-opacity-100' : 'bg-opacity-0'">
              <i class="bi bi-check-lg text-white transition-opacity ease-in-out duration-200" :class="searchOpt.onlyOpen ? 'opacity-100' : 'opacity-0'"></i>
            </div>
            <p>Only open tickets</p>
          </div>
          <div @click="searchOpt.sortDescending = !searchOpt.sortDescending" class="bg-white transition-colors ease-in-out duration-200 bg-opacity-0 hover:bg-opacity-30 rounded-md flex items-center p-1 cursor-pointer">
            <i class="bi bi-caret-up-fill text-green-500 text-xl transition-transform transform ease-in-out duration-200 mr-1" :class="searchOpt.sortDescending ? 'rotate-180' : 'rotate-0'"></i>
            <p class="transition-all transform ease-in-out duration-500">{{searchOpt.sortDescending ? 'Descending' : 'Ascending'}}</p>
          </div>
        </div>
        <div class="flex justify-center mb-2">
          <div @click="newTicket" class="bg-green-500 hover:bg-green-600 cursor-pointer px-3 py-1 rounded-xl max-w-56">
            <p class="text-white">Open ticket</p>
          </div>
        </div>
        
      </div>
      <div class="bg-white h-[50vh] rounded-3xl ml:mt-4 p-6">
        <div class="h-full w-full overflow-auto">
          <table
              class="min-w-full text-left text-surface ">
            <thead
                class="border-b border-gray-200 ">
            <tr>
              <th scope="col" class="px-6 py-4">Id</th>
              <th scope="col" class="px-6 py-4">Printer</th>
              <th scope="col" class="px-6 py-4">Issue</th>
              <th scope="col" class="px-6 py-4">Repair</th>
              <th scope="col" class="px-6 py-4">Date Opened</th>
              <th scope="col" class="px-6 py-4">Status</th>
              <th scope="col" class="px-6 py-4">Opened By</th>
              <th scope="col" class="px-6 py-4">Technician</th>
              <th scope="col" class="px-6 py-4"></th>
            </tr>
            </thead>
            <tbody>
            <tr v-for="(ticket, index) in tickets" class="border-b border-gray-200">
              <td class="whitespace-nowrap px-6 py-4">{{ ticket.ticketId }}</td>
              <td class="whitespace-nowrap px-6 py-4">{{ ticket.printer }}</td>
              <td class="whitespace-nowrap px-6 py-4">{{ ticket.issue }}</td>
              <td class="whitespace-nowrap px-6 py-4">{{ ticket.repair === null ? "Pending" : ticket.repair }}</td>
              <td class="whitespace-nowrap px-6 py-4">{{ new Date(ticket.dateOpened).toLocaleString() }}</td>
              <td :title="new Date(ticket.dateClosed).toLocaleString()" class="whitespace-nowrap px-6 py-4">{{ ticket.dateClosed === null ? "Open" : "Closed" }}</td>
              <td class="whitespace-nowrap px-6 py-4">{{ ticket.openedBy }}</td>
              <td class="whitespace-nowrap px-6 py-4">{{ ticket.technician === null ? "Pending" : ticket.technician }}</td>
              <td class="whitespace-nowrap px6-py-4">
                <div class="flex items-center">
                  <div v-if="ticket.dateClosed === null" class="bg-red-500 py-1 px-3 mx-1 rounded-xl hover:bg-red-600 cursor-pointer" >
                    <p class="text-white">Close</p>
                  </div>
                  <div class="bg-gray-200 py-1 px-3 mx-1 rounded-xl hover:bg-gray-300 cursor-pointer">
                    <p class="">Modify</p>
                  </div>
                </div>
              </td>
            </tr>

            </tbody>
          </table>
        </div>
      </div>
    </div>
    
    
    <div v-if="newTicketOpen" class="fixed top-0 left-0 fixHeight w-screen bg-black bg-opacity-25 z-30 flex items-center justify-center">
      <div class="bg-white p-6 rounded-3xl">
        <h1>New Ticket</h1>
        <hr class="my-2">
        <div class="flex flex-col">
          <p class="text-left">Printer</p>
          <select v-model="printerName" class="text-green-500 p-1 bg-gray-200 rounded-lg">
            <option v-for="(printer, index) in printerOptions" :value="printer.name">{{printer.name}}</option>
          </select>
          
          <hr class="my-2">
          <p class="text-left">Issue</p>
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
          <div @click="submitTicket" class="bg-green-500 hover:bg-green-600 cursor-pointer p-2 rounded-lg">
            <p class="text-white">Submit</p>
          </div>
        </div>
        
      </div>
    </div>
  </div>
</template>

<style scoped>

</style>