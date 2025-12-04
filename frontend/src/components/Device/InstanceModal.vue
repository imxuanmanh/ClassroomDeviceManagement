<template>
  <div class="modal-overlay" @click.self="$emit('close')">
    <div class="modal-container">
      <div class="modal-header">
        <h3>{{ title }}</h3>
        <button class="close-btn" @click="$emit('close')">✕</button>
      </div>

      <form @submit.prevent="handleSubmit" class="modal-body">
        <div class="form-group">
          <input
            v-model="formData.instanceCode"
            type="text"
            placeholder="Nhập mã thiết bị (VD: CAM-001)"
            required
            ref="inputFocus"
          />
        </div>

        <div class="modal-footer">
          <button type="button" class="btn-cancel" @click="$emit('close')">Hủy</button>
          <button type="submit" class="btn-submit">{{ submitText }}</button>
        </div>
      </form>
    </div>
  </div>
</template>

<script setup>
import { ref, watch, onMounted } from 'vue'

const props = defineProps({
  title: { type: String, default: 'Thêm thiết bị mới' },
  submitText: { type: String, default: 'Thêm' },
  modelId: { type: Number, required: true },
  modelName: { type: String, required: true },
  // Vẫn nhận props này để xử lý ngầm, dù không hiển thị ra
  initialLocation: { type: String, default: 'Kho' },
  value: { type: Object, default: () => ({}) },
})

const emit = defineEmits(['submit', 'close'])
const inputFocus = ref(null)

const formData = ref({
  instanceCode: '',
})

onMounted(() => {
  if (inputFocus.value) inputFocus.value.focus()
})

watch(
  () => props.value,
  (newValue) => {
    if (newValue) {
      formData.value.instanceCode = newValue.instanceCode || ''
    }
  },
  { immediate: true },
)

function handleSubmit() {
  // Logic vẫn giữ nguyên: Lấy location từ props truyền xuống API
  emit('submit', {
    modelId: props.modelId,
    instanceCode: formData.value.instanceCode,
    currentLocation: props.initialLocation, // Vẫn gửi giá trị này đi ngầm
    statusId: 1,
  })
}
</script>

<style scoped>
/* Giữ nguyên CSS cũ */
.modal-overlay {
  position: fixed;
  top: 0;
  left: 0;
  right: 0;
  bottom: 0;
  background: rgba(0, 0, 0, 0.75);
  display: flex;
  align-items: center;
  justify-content: center;
  z-index: 1000;
  backdrop-filter: blur(4px);
}
.modal-container {
  background: #393e46;
  border-radius: 12px;
  width: 90%;
  max-width: 500px;
  max-height: 90vh;
  overflow: hidden;
  box-shadow: 0 10px 40px rgba(0, 0, 0, 0.5);
  border: 1px solid rgba(0, 173, 181, 0.3);
}
.modal-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
  padding: 20px 24px;
  background: #222831;
  border-bottom: 1px solid rgba(0, 173, 181, 0.2);
}
.modal-header h3 {
  margin: 0;
  color: #00adb5;
  font-size: 18px;
  font-weight: 600;
}
.close-btn {
  background: none;
  border: none;
  color: #eeeeee;
  font-size: 24px;
  cursor: pointer;
  padding: 0;
  width: 32px;
  height: 32px;
  display: flex;
  align-items: center;
  justify-content: center;
  border-radius: 4px;
  transition: all 0.2s ease;
}
.close-btn:hover {
  background: rgba(239, 68, 68, 0.2);
  color: #ef4444;
}
.modal-body {
  padding: 24px;
  overflow-y: auto;
  max-height: calc(90vh - 140px);
}
/* Xóa CSS .info-section, .info-item... nếu muốn gọn file, nhưng để lại cũng không ảnh hưởng */
.form-group {
  margin-bottom: 20px;
}
.form-group label {
  display: block;
  margin-bottom: 8px;
  color: #eeeeee;
  font-weight: 500;
  font-size: 14px;
}
.required {
  color: #ef4444;
}
.form-group input {
  width: 100%;
  padding: 10px 12px;
  background: #222831;
  border: 1px solid rgba(0, 173, 181, 0.2);
  border-radius: 8px;
  color: #eeeeee;
  font-size: 14px;
  transition: all 0.3s ease;
  font-family: inherit;
}
.form-group input:focus {
  outline: none;
  border-color: #00adb5;
  box-shadow: 0 0 0 3px rgba(0, 173, 181, 0.1);
}
.form-group input::placeholder {
  color: rgba(238, 238, 238, 0.4);
}
.modal-footer {
  display: flex;
  gap: 12px;
  justify-content: flex-end;
  margin-top: 32px;
  padding-top: 20px;
  border-top: 1px solid rgba(0, 173, 181, 0.1);
}
.btn-cancel,
.btn-submit {
  padding: 10px 24px;
  border: none;
  border-radius: 8px;
  font-weight: 600;
  font-size: 14px;
  cursor: pointer;
  transition: all 0.3s ease;
}
.btn-cancel {
  background: #222831;
  color: #eeeeee;
  border: 1px solid rgba(0, 173, 181, 0.2);
}
.btn-cancel:hover {
  background: #2d3440;
  border-color: #00adb5;
}
.btn-submit {
  background: linear-gradient(135deg, #00adb5 0%, #007b82 100%);
  color: #ffffff;
  box-shadow: 0 2px 8px rgba(0, 173, 181, 0.3);
}
.btn-submit:hover {
  background: linear-gradient(135deg, #00c9d4 0%, #00adb5 100%);
  transform: translateY(-2px);
  box-shadow: 0 4px 12px rgba(0, 173, 181, 0.5);
}
.btn-submit:active {
  transform: translateY(0);
}
</style>
