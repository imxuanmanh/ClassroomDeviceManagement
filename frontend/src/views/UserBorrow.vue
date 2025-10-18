<!-- <template>
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
</script> -->

<template>
  <div class="borrow-page">
    <div class="borrow-card">
      <h2>📋 Đăng ký mượn thiết bị</h2>

      <form @submit.prevent="onSubmit">
        <!-- Thiết bị -->
        <div class="form-group">
          <label for="device">Thiết bị</label>
          <select id="device" v-model="deviceId" required>
            <option value="" disabled>-- Chọn thiết bị --</option>
            <option v-for="d in devices" :key="d.deviceId" :value="d.deviceId">
              {{ d.deviceName }}
            </option>
          </select>
        </div>

        <!-- Thời gian mượn -->
        <div class="form-group">
          <label for="borrowTime">Thời gian mượn</label>
          <input id="borrowTime" v-model="borrowTime" type="datetime-local" required />
        </div>

        <!-- Nút submit -->
        <button type="submit" class="btn-submit">Đăng ký mượn</button>
      </form>

      <!-- Thông báo trạng thái -->
      <p v-if="message" class="message">{{ message }}</p>
    </div>
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
const message = ref('')

onMounted(async () => {
  try {
    devices.value = await deviceApi.getAll()
  } catch (err) {
    console.error(err)
    message.value = 'Không thể tải danh sách thiết bị!'
  }
})

async function onSubmit() {
  if (!deviceId.value || !borrowTime.value) {
    message.value = 'Vui lòng chọn thiết bị và thời gian mượn.'
    return
  }

  try {
    // Nếu bạn có borrowApi, thay dòng dưới:
    // await borrowApi.create({ deviceId: deviceId.value, borrowTime: borrowTime.value, userId: auth.user.id })
    message.value = '✅ Đăng ký mượn thành công!'
    deviceId.value = ''
    borrowTime.value = ''
  } catch (err) {
    console.error(err)
    message.value = '❌ Có lỗi xảy ra, vui lòng thử lại.'
  }
}
</script>

<style scoped>
/* Tổng thể trang */
.borrow-page {
  display: flex;
  justify-content: center;
  align-items: center;
  min-height: calc(100vh - 100px);
  background: #f5f8fa;
}

/* Thẻ form */
.borrow-card {
  background: #ffffff;
  padding: 2rem 2.5rem;
  border-radius: 16px;
  box-shadow: 0 4px 15px rgba(0, 0, 0, 0.1);
  width: 400px;
  text-align: center;
  animation: fadeIn 0.4s ease;
}

.borrow-card h2 {
  margin-bottom: 1.5rem;
  color: #2c3e50;
  font-weight: 700;
}

/* Nhóm form */
.form-group {
  text-align: left;
  margin-bottom: 1.2rem;
}

label {
  display: block;
  font-weight: 600;
  color: #34495e;
  margin-bottom: 0.4rem;
}

select,
input {
  width: 100%;
  padding: 10px;
  border-radius: 8px;
  border: 1px solid #dcdfe3;
  font-size: 15px;
  outline: none;
  transition: 0.2s;
}

select:focus,
input:focus {
  border-color: #3498db;
  box-shadow: 0 0 4px rgba(52, 152, 219, 0.4);
}

/* Nút submit */
.btn-submit {
  width: 100%;
  background-color: #3498db;
  color: white;
  padding: 10px;
  border: none;
  border-radius: 8px;
  font-size: 16px;
  font-weight: 600;
  cursor: pointer;
  transition: background-color 0.2s;
}

.btn-submit:hover {
  background-color: #2980b9;
}

/* Thông báo */
.message {
  margin-top: 1rem;
  font-weight: 600;
  color: #2c3e50;
}

/* Hiệu ứng nhỏ */
@keyframes fadeIn {
  from {
    opacity: 0;
    transform: translateY(10px);
  }
  to {
    opacity: 1;
    transform: translateY(0);
  }
}
</style>
