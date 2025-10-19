<template>
  <div class="borrow">
    <h2>Đăng ký mượn thiết bị</h2>
    <form @submit.prevent="onSubmit">
      <label>
        Thiết bị
        <select v-model="deviceId" required>
          <option v-for="d in devices" :key="d.deviceId" :value="d.deviceId">
            {{ d.deviceName }}
          </option>
        </select>
      </label>
      <label>
        Thời gian mượn
        <input v-model="borrowTime" type="datetime-local" required />
      </label>
      <button type="submit">Đăng ký mượn</button>
    </form>
  </div>
</template>

<script setup>
import { ref, onMounted } from 'vue'
import { deviceApi } from '@/config/api'
import { useAuthStore } from '@/stores/auth.js'

const deviceId = ref('')
const borrowTime = ref('')
const devices = ref([])
const auth = useAuthStore()

onMounted(async () => {
  devices.value = await deviceApi.getAll()
})

async function onSubmit() {
  // Gửi request mượn thiết bị lên server
  // Ví dụ: await borrowApi.create({ deviceId: deviceId.value, borrowTime: borrowTime.value })
  alert('Đăng ký mượn thành công!')
}
</script>
