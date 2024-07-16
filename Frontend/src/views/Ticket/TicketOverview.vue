<script setup>

import SearchBar from "../../components/SearchBar.vue";
import {onMounted, ref, watch} from "vue";
import {router} from "../../router.js";
import {toast} from "vue3-toastify";
import axios from "axios";
import {backendUrl} from "../../main.js";
import TicketCreateMenu from "./TicketCreateMenu.vue";
import TicketModifyMenu from "./TicketModifyMenu.vue";
import {UserStore} from "../../store/UserStore.js";
import {useRoute} from "vue-router";
import {hasPerm} from "../../utils.js";

let searchOpt = ref({
  searchWord: "",
  onlyOpen : false,
  sortDescending: false,
});

let tickets = ref([]);
let newTicketOpen = ref(false);
let closingTicket = ref({
  id: 0,
  printerName: null,
  issue: null,
  repair: ""
})
let closeTicketOpen = ref(false);

let repair = ref("");
let repairCustom = ref("");

let modifyingTicket = ref({});
let showModify = ref(false);
let userStore = UserStore();
let route = useRoute();
onMounted(() => {
  let ticketLoadingToast = toast.loading("Loading tickets...", {
    transition: "bounce",
    closeOnClick: false,
    pauseOnHover: false
  });
  axios.get(backendUrl + "/v1/ticket/gettickets?printerName=" + searchOpt.value.searchWord + "&onlyOpen=" + searchOpt.value.onlyOpen + "&sortDescending=" + searchOpt.value.sortDescending, {withCredentials: true}).then(response => {
    toast.update(ticketLoadingToast, {"closeOnClick": true, render: "Loaded tickets", type: "success", isLoading: false, autoClose: 2000});
    tickets.value = response.data;
  }).catch(error => {
    toast.update(ticketLoadingToast, {"closeOnClick": true, render: error.response.data.length < 30 ? error.response.data : "Error loading tickets. Try refreshing.", type: "error", isLoading: false, autoClose: 2000});
    if (error.response.data === "User not logged in" || error.response.data === "Session Invalid") {
      router.push('/login');
    }
  });
})

watch(searchOpt.value,() => {
  axios.get(backendUrl + "/v1/ticket/gettickets?printerName=" + searchOpt.value.searchWord + "&onlyOpen=" + searchOpt.value.onlyOpen + "&sortDescending=" + searchOpt.value.sortDescending, {withCredentials: true}).then(response => {
    tickets.value = response.data;
    console.log(tickets.value)
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

let issueTypes = ref([]);

function refreshTickets(){
  let ticketLoadingToast = toast.loading("Loading tickets...", {
    transition: "bounce",
    closeOnClick: false,
    pauseOnHover: false
  });
  axios.get(backendUrl + "/v1/ticket/gettickets?printerName=" + searchOpt.value.searchWord + "&onlyOpen=" + searchOpt.value.onlyOpen + "&sortDescending=" + searchOpt.value.sortDescending, {withCredentials: true}).then(response => {
    toast.update(ticketLoadingToast, {"closeOnClick": true, render: "Loaded tickets", type: "success", isLoading: false, autoClose: 2000});
    tickets.value = response.data;
    console.log(tickets.value)
  }).catch(error => {
    toast.update(ticketLoadingToast, {"closeOnClick": true, render: error.response.data.length < 30 ? error.response.data : "Error loading tickets. Try refreshing.", type: "error", isLoading: false, autoClose: 2000});
    if (error.response.data === "User not logged in" || error.response.data === "Session Invalid") {
      router.push('/login');
    }
  });
}

function closeTicket(ticket) {
  console.log(ticket);
  let issueReq = axios.get(backendUrl + "/v1/ticket/getissuevariants?repairByIssue=" + ticket.issue, {withCredentials: true}).then(response => {
    
    issueTypes.value = response.data;
    
    closingTicket.value.id = ticket.ticketId;
    closingTicket.value.issue = ticket.issue;
    closingTicket.value.printerName = ticket.printer;
    closeTicketOpen.value = true;
    console.log("Open menu")
    
  }).catch(error => {
    console.log(error)
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

function submitCloseTicket() {
  let data = {
    id: closingTicket.value.id,
    repair: repair.value === 'custom' ? repairCustom.value : repair.value
  }
  repair.value = "";
  repairCustom.value = "";
  let req = axios.post(backendUrl + "/v1/ticket/closeticket", data, {withCredentials: true}).then(response => {
    closeTicketOpen.value = false;
    let ticketLoadingToast = toast.loading("Loading tickets...", {
      transition: "bounce",
      closeOnClick: false,
      pauseOnHover: false
    });
    axios.get(backendUrl + "/v1/ticket/gettickets?printerName=" + searchOpt.value.searchWord + "&onlyOpen=" + searchOpt.value.onlyOpen + "&sortDescending=" + searchOpt.value.sortDescending, {withCredentials: true}).then(response => {
      toast.update(ticketLoadingToast, {"closeOnClick": true, render: "Loaded tickets", type: "success", isLoading: false, autoClose: 2000});
      tickets.value = response.data;
      console.log(tickets.value)
    }).catch(error => {
      toast.update(ticketLoadingToast, {"closeOnClick": true, render: error.response.data.length < 30 ? error.response.data : "Error loading tickets. Try refreshing.", type: "error", isLoading: false, autoClose: 2000});
      if (error.response.data === "User not logged in" || error.response.data === "Session Invalid") {
        router.push('/login');
      }
    });
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

function closeCloseTicket() {
  closingTicket.value = {
    id: 0,
    printerName: null,
    issue: null,
    repair: ""
  }
  closeTicketOpen.value = false;
}
</script>

<template>
  <div class="fixHeight flex items-center justify-center">
    <div class="w-[75vw]">
      <div class="ml:flex items-center">
        <SearchBar @update="args => searchOpt.searchWord=args.value" />
        <div class="ml:flex ml:items-center my-2 ml:my-0">
          <div class="flex items-center">
            <div @click="searchOpt.onlyOpen = !searchOpt.onlyOpen" class="mr-6 ml:ml-6 flex items-center cursor-pointer">
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
          <div @click="newTicketOpen = true" class="bg-green-500 hover:bg-green-600 cursor-pointer mx-1 px-3 py-1 rounded-xl ml:max-w-56">
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
                    <p @click="closeTicket(ticket)" class="text-white">Close</p>
                  </div>
                  <div v-if="hasPerm(userStore, 'Head Technician')" class="bg-gray-200 py-1 px-3 mx-1 rounded-xl hover:bg-gray-300 cursor-pointer">
                    <p @click="modifyingTicket = ticket; showModify = true;">Modify</p>
                  </div>
                </div>
              </td>
            </tr>

            </tbody>
          </table>
        </div>
      </div>
    </div>
    
    <!--Hidden Menus-->
    <TicketCreateMenu @complete="refreshTickets(); newTicketOpen = false;" v-if="newTicketOpen" />
    <TicketModifyMenu v-if="showModify" :ticket="modifyingTicket" @close="showModify = false; modifyingTicket = {}; refreshTickets();" />
    <div v-if="closeTicketOpen" class="fixed top-0 left-0 fixHeight w-screen bg-black bg-opacity-25 z-30 flex items-center justify-center">
      <div class="bg-white p-6 rounded-3xl flex flex-col">
        <h1>Close Ticket</h1>
        <hr class="my-2">
        <p class="text-left">Printer: {{closingTicket.printerName}}</p>
        <p class="text-left">Issue: {{closingTicket.issue}}</p>
        <hr class="my-2">
        <p class="text-left">Repair</p>
        <select v-model="repair" class="text-green-500 p-1 bg-gray-200 rounded-lg">
          <option v-for="(issue, index) in issueTypes" :value="issue">{{issue}}</option>
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
        <div class="flex justify-center">
          <div @click="submitCloseTicket" class="bg-green-500 hover:bg-green-600 cursor-pointer p-2 rounded-lg mr-1">
            <p class="text-white">Submit</p>
          </div>
          <div @click="closeCloseTicket" class="bg-gray-200 hover:bg-gray-300 cursor-pointer p-2 rounded-lg ml-1">
            <p>Cancel</p>
          </div>
        </div>
        
      </div>
    </div>
  </div>
</template>

<style scoped>

</style>