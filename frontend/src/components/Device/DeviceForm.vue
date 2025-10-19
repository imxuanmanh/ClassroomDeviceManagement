<template>
  <form class="form" @submit.prevent="onSubmit">
    <div class="form-group">
      <label>Mã thiết bị</label>
      <input v-model="model.deviceId" placeholder="Nhập mã thiết bị" />
    </div>
    <div class="form-group">
      <label>Tên thiết bị</label>
      <input v-model="model.deviceName" placeholder="Nhập tên thiết bị" />
    </div>
    <div class="form-group">
      <label>Loại thiết bị</label>
      <input v-model="model.deviceType" placeholder="Nhập loại thiết bị" />
    </div>
    <div class="form-group">
      <label>Thông số kỹ thuật</label>
      <textarea
        v-model="model.specification"
        placeholder="Nhập thông số kỹ thuật"
        rows="3"
      ></textarea>
    </div>
    <div class="form-group">
      <label>Vị trí</label>
      <input v-model="model.storageLocation" placeholder="Nhập vị trí lưu trữ" />
    </div>
    <div class="form-row">
      <div class="form-group half">
        <label>Tổng số lượng</label>
        <input
          v-model.number="model.totalQuantity"
          type="number"
          min="0"
          placeholder="Nhập tổng số lượng"
        />
      </div>
      <div class="form-group half">
        <label>Số lượng khả dụng</label>
        <input
          v-model.number="model.availableQuantity"
          type="number"
          min="0"
          placeholder="Nhập số lượng khả dụng"
        />
      </div>
    </div>
    <div class="form-actions">
      <button type="submit" class="btn-primary">{{ submitText }}</button>
      <button type="button" v-if="showCancel" class="btn-secondary" @click="$emit('cancel')">
        Hủy
      </button>
    </div>
  </form>

  <div v-if="error" class="error">{{ error }}</div>
</template>

<script setup>
import { reactive, watch, ref } from 'vue'

const props = defineProps({
  value: { type: Object, default: () => ({}) },
  submitText: { type: String, default: 'Lưu' },
  showCancel: { type: Boolean, default: false },
})
const emit = defineEmits(['submit', 'cancel', 'update:value'])

const error = ref('')

const model = reactive({
  deviceId: '',
  deviceName: '',
  deviceType: '',
  specification: '',
  storageLocation: '',
  totalQuantity: 0,
  availableQuantity: 0,
})

watch(
  () => props.value,
  (v) => {
    Object.assign(model, v || {})
  },
  { immediate: true, deep: true },
)

function onSubmit() {
  error.value = ''
  if (!model.deviceId || !model.deviceName) {
    error.value = 'Vui lòng nhập Mã và Tên thiết bị'
    return
  }
  emit('submit', { ...model })
}
</script>

<style scoped>
.form {
  display: flex;
  flex-direction: column;
  gap: 16px;
  width: 100%;
}

.form-group {
  display: flex;
  flex-direction: column;
  gap: 6px;
}

.form-row {
  display: flex;
  gap: 16px;
}

.form-group.half {
  width: 50%;
}

.form-group label {
  font-weight: 500;
  color: #374151;
  font-size: 0.875rem;
}

.form-group input,
.form-group textarea {
  padding: 8px 12px;
  border: 1px solid #d1d5db;
  border-radius: 6px;
  font-size: 0.875rem;
  transition: all 0.2s;
  width: 100%;
}

.form-group input:focus,
.form-group textarea:focus {
  outline: none;
  border-color: #3b82f6;
  box-shadow: 0 0 0 3px rgba(59, 130, 246, 0.1);
}

.form-group textarea {
  resize: vertical;
  min-height: 80px;
}

.form-actions {
  display: flex;
  justify-content: flex-end;
  gap: 12px;
  margin-top: 8px;
}

.btn-primary,
.btn-secondary {
  padding: 8px 16px;
  border: none;
  border-radius: 6px;
  font-weight: 500;
  font-size: 0.875rem;
  cursor: pointer;
  transition: all 0.2s;
}

.btn-primary {
  background: #3b82f6;
  color: white;
}

.btn-primary:hover {
  background: #2563eb;
}

.btn-secondary {
  background: #9ca3af;
  color: white;
}

.btn-secondary:hover {
  background: #6b7280;
}

.error {
  margin-top: 12px;
  padding: 8px 12px;
  background: #fee2e2;
  border: 1px solid #fca5a5;
  border-radius: 6px;
  color: #b91c1c;
  font-size: 0.875rem;
}
</style>
