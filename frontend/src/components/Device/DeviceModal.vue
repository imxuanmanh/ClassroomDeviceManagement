<template>
  <div class="modal-backdrop" @click.self="$emit('close')">
    <div class="modal-content">
      <div class="modal-header">
        <h3>{{ title }}</h3>
        <button class="close-btn" @click="$emit('close')">&times;</button>
      </div>
      <div class="modal-body">
        <DeviceForm
          :value="value"
          :submit-text="submitText"
          :show-cancel="true"
          @submit="$emit('submit', $event)"
          @cancel="$emit('close')"
        />
      </div>
    </div>
  </div>
</template>

<script setup>
import DeviceForm from './DeviceForm.vue'

defineProps({
  title: { type: String, default: 'Thêm thiết bị mới' },
  value: { type: Object, default: () => ({}) },
  submitText: { type: String, default: 'Lưu' },
})

defineEmits(['close', 'submit'])
</script>

<style scoped>
.modal-backdrop {
  position: fixed;
  top: 0;
  left: 0;
  width: 100%;
  height: 100%;
  background: rgba(0, 0, 0, 0.5);
  display: flex;
  align-items: center;
  justify-content: center;
  z-index: 1000;
}

.modal-content {
  background: white;
  border-radius: 8px;
  width: 500px;
  max-width: 90%;
  box-shadow: 0 2px 10px rgba(0, 0, 0, 0.1);
}

.modal-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
  padding: 16px;
  border-bottom: 1px solid #e5e7eb;
}

.modal-header h3 {
  margin: 0;
  font-size: 1.25rem;
  color: #111827;
}

.close-btn {
  background: none;
  border: none;
  font-size: 1.5rem;
  cursor: pointer;
  color: #6b7280;
}

.close-btn:hover {
  color: #111827;
}

.modal-body {
  padding: 16px;
}
</style>
