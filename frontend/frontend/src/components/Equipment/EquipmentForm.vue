<template>
  <form class="form" @submit.prevent="onSubmit">
    <input v-model="model.deviceId" placeholder="Mã" />
    <input v-model="model.deviceName" placeholder="Tên thiết bị" />
    <input v-model="model.deviceType" placeholder="Loại" />
    <input v-model="model.storageLocation" placeholder="Vị trí" />
    <input v-model.number="model.totalQuantity" type="number" placeholder="Tổng SL" />
    <input v-model.number="model.availableQuantity" type="number" placeholder="SL khả dụng" />
    <button type="submit">{{ submitText }}</button>
    <button type="button" v-if="showCancel" @click="$emit('cancel')">Hủy</button>
  </form>
</template>

<script setup>
import { reactive, computed, watch } from 'vue'

const props = defineProps({
  value: { type: Object, default: () => ({}) },
  submitText: { type: String, default: 'Lưu' },
  showCancel: { type: Boolean, default: false }
})
const emit = defineEmits(['submit', 'cancel', 'update:value'])

const model = reactive({
  deviceId: '', deviceName: '', deviceType: '', storageLocation: '', totalQuantity: 0, availableQuantity: 0
})

watch(() => props.value, (v) => { Object.assign(model, v || {}) }, { immediate: true, deep: true })

function onSubmit() {
  emit('submit', { ...model })
}
</script>

<style scoped>
.form { display: flex; flex-wrap: wrap; gap: 8px; }
.form input { padding: 6px 8px; border: 1px solid #e5e7eb; border-radius: 8px; }
.form button { padding: 6px 10px; border: none; border-radius: 8px; background: #3b82f6; color: #fff; cursor: pointer; }
.form button + button { background: #9ca3af; }
</style>


