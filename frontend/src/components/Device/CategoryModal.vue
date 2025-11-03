<template>
  <div class="modal-overlay">
    <div class="modal">
      <h3>{{ title }}</h3>
      <input
        v-model="form.name"
        type="text"
        placeholder="Nhập tên loại thiết bị..."
        class="input"
      />

      <div class="modal-actions">
        <button class="btn btn-primary" @click="submitForm">
          {{ submitText || 'Thêm' }}
        </button>
        <button class="btn btn-secondary" @click="$emit('close')">Hủy</button>
      </div>
    </div>
  </div>
</template>

<script setup>
import { ref, watch, defineProps, defineEmits } from 'vue'

const props = defineProps({
  value: Object,
  title: String,
  submitText: String,
})
const emit = defineEmits(['submit', 'close'])

const form = ref({ name: '' })

watch(
  () => props.value,
  (val) => {
    form.value = val ? { ...val } : { name: '' }
  },
  { immediate: true },
)

function submitForm() {
  if (!form.value.name.trim()) {
    alert('Vui lòng nhập tên loại thiết bị!')
    return
  }
  emit('submit', form.value)
}
</script>

<style scoped>
.modal-overlay {
  position: fixed;
  inset: 0;
  background: rgba(0, 0, 0, 0.45);
  display: flex;
  align-items: center;
  justify-content: center;
  z-index: 999;
}

.modal {
  background: #fff;
  padding: 24px;
  border-radius: 12px;
  width: 320px;
  box-shadow: 0 6px 20px rgba(0, 0, 0, 0.2);
  text-align: center;
  animation: fadeIn 0.25s ease;
}

h3 {
  margin-bottom: 16px;
  color: #111827;
  font-size: 18px;
}

.input {
  width: 100%;
  padding: 8px 10px;
  border: 1px solid #d1d5db;
  border-radius: 8px;
  margin-bottom: 16px;
  outline: none;
}

.input:focus {
  border-color: #2563eb;
  box-shadow: 0 0 0 2px rgba(37, 99, 235, 0.1);
}

.modal-actions {
  display: flex;
  justify-content: center;
  gap: 10px;
}

.btn {
  padding: 8px 16px;
  border-radius: 8px;
  cursor: pointer;
  border: none;
  font-weight: 600;
}

.btn-primary {
  background: #2563eb;
  color: white;
}

.btn-primary:hover {
  background: #1e40af;
}

.btn-secondary {
  background: #e5e7eb;
}

.btn-secondary:hover {
  background: #d1d5db;
}

@keyframes fadeIn {
  from {
    opacity: 0;
    transform: translateY(8px);
  }
  to {
    opacity: 1;
    transform: translateY(0);
  }
}
</style>
