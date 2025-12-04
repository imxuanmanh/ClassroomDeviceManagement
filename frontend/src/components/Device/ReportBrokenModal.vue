<template>
  <transition name="modal-fade">
    <div v-if="isOpen" class="modal-overlay" @click.self="closeModal">
      <div class="modal-container">
        <!-- Header -->
        <div class="modal-header">
          <h2>⚠️ Báo cáo thiết bị hỏng</h2>
          <button class="close-btn" @click="closeModal">✕</button>
        </div>

        <!-- Body -->
        <div class="modal-body">
          <!-- Thông tin thiết bị -->
          <div class="device-info">
            <div class="info-row">
              <span class="label">Tên thiết bị:</span>
              <span class="value">{{ deviceData.deviceName }}</span>
            </div>
            <div class="info-row">
              <span class="label">Mã thiết bị:</span>
              <span class="value">{{ deviceData.deviceCode }}</span>
            </div>
          </div>

          <!-- Form -->
          <form @submit.prevent="handleSubmit">
            <!-- Mô tả lỗi -->
            <div class="form-group">
              <label for="description">Mô tả lỗi <span class="required">*</span></label>
              <textarea
                id="description"
                v-model="formData.description"
                placeholder="Vui lòng mô tả chi tiết tình trạng hỏng hóc của thiết bị..."
                rows="5"
                required
              ></textarea>
            </div>

            <!-- Upload ảnh -->
            <div class="form-group">
              <label>Hình ảnh minh họa (tùy chọn)</label>
              <div class="upload-area">
                <input
                  type="file"
                  ref="fileInput"
                  @change="handleFileChange"
                  accept="image/*"
                  hidden
                />
                <button type="button" class="upload-btn" @click="triggerFileInput">
                  📷 Chọn ảnh
                </button>
                <span v-if="formData.image" class="file-name">{{ formData.image.name }}</span>
              </div>

              <!-- Preview ảnh -->
              <div v-if="imagePreview" class="image-preview">
                <img :src="imagePreview" alt="Preview" />
                <button type="button" class="remove-image-btn" @click="removeImage">✕</button>
              </div>
            </div>
          </form>
        </div>

        <!-- Footer -->
        <div class="modal-footer">
          <button type="button" class="cancel-btn" @click="closeModal">Hủy</button>
          <button type="button" class="submit-btn" @click="handleSubmit" :disabled="!isFormValid">
            Gửi báo cáo
          </button>
        </div>
      </div>
    </div>
  </transition>
</template>

<script setup>
import { ref, computed, watch } from 'vue'

const props = defineProps({
  isOpen: {
    type: Boolean,
    default: false,
  },
  deviceData: {
    type: Object,
    default: () => ({
      requestId: null,
      deviceName: '',
      deviceCode: '',
    }),
  },
})

const emit = defineEmits(['close', 'submit'])

// Form data
const formData = ref({
  description: '',
  image: null,
})

const imagePreview = ref(null)
const fileInput = ref(null)

// Computed
const isFormValid = computed(() => {
  return formData.value.description.trim().length > 0 && formData.value.image !== null
})

// Methods
function closeModal() {
  resetForm()
  emit('close')
}

function resetForm() {
  formData.value = {
    description: '',
    image: null,
  }
  imagePreview.value = null
}

function triggerFileInput() {
  fileInput.value?.click()
}

function handleFileChange(event) {
  const file = event.target.files[0]
  if (file) {
    // Kiểm tra loại file
    const allowedTypes = ['image/jpeg', 'image/jpg', 'image/png']
    if (!allowedTypes.includes(file.type)) {
      alert('❌ Chỉ chấp nhận file JPG, JPEG hoặc PNG!')
      return
    }

    // Kiểm tra kích thước file (max 5MB)
    const maxSize = 5 * 1024 * 1024 // 5MB
    if (file.size > maxSize) {
      alert('❌ File quá lớn! Vui lòng chọn file nhỏ hơn 5MB.')
      return
    }

    formData.value.image = file

    // Tạo preview
    const reader = new FileReader()
    reader.onload = (e) => {
      imagePreview.value = e.target.result
    }
    reader.readAsDataURL(file)
  }
}

function removeImage() {
  formData.value.image = null
  imagePreview.value = null
  if (fileInput.value) {
    fileInput.value.value = ''
  }
}

function handleSubmit() {
  if (!isFormValid.value) return

  emit('submit', {
    requestId: props.deviceData.requestId,
    description: formData.value.description,
    image: formData.value.image,
  })

  closeModal()
}

// Watch để reset form khi modal đóng
watch(
  () => props.isOpen,
  (newVal) => {
    if (!newVal) {
      resetForm()
    }
  },
)
</script>

<style scoped>
/* Modal overlay */
.modal-overlay {
  position: fixed;
  top: 0;
  left: 0;
  right: 0;
  bottom: 0;
  background: rgba(0, 0, 0, 0.7);
  display: flex;
  justify-content: center;
  align-items: center;
  z-index: 9999;
  padding: 20px;
}

/* Modal container */
.modal-container {
  background: #393e46;
  border-radius: 12px;
  width: 100%;
  max-width: 600px;
  max-height: 90vh;
  overflow: hidden;
  display: flex;
  flex-direction: column;
  box-shadow: 0 10px 40px rgba(0, 0, 0, 0.5);
  border: 1px solid rgba(0, 173, 181, 0.3);
}

/* Header */
.modal-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
  padding: 20px 24px;
  border-bottom: 1px solid rgba(0, 173, 181, 0.2);
  background: #222831;
}

.modal-header h2 {
  color: #00adb5;
  font-size: 20px;
  margin: 0;
  text-shadow: 0 0 10px rgba(0, 173, 181, 0.3);
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
  border-radius: 6px;
  transition: all 0.2s ease;
}

.close-btn:hover {
  background: rgba(239, 68, 68, 0.2);
  color: #ef4444;
}

/* Body */
.modal-body {
  padding: 24px;
  overflow-y: auto;
  flex: 1;
}

/* Device info */
.device-info {
  background: #222831;
  padding: 16px;
  border-radius: 8px;
  margin-bottom: 24px;
  border: 1px solid rgba(0, 173, 181, 0.2);
}

.info-row {
  display: flex;
  justify-content: space-between;
  padding: 8px 0;
  color: #eeeeee;
}

.info-row:not(:last-child) {
  border-bottom: 1px solid rgba(0, 173, 181, 0.1);
}

.info-row .label {
  font-weight: 600;
  color: #00adb5;
}

.info-row .value {
  color: #eeeeee;
}

/* Form */
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

textarea {
  width: 100%;
  padding: 12px;
  border: 1px solid rgba(0, 173, 181, 0.3);
  border-radius: 6px;
  background: #222831;
  color: #eeeeee;
  font-family: inherit;
  font-size: 14px;
  resize: vertical;
  transition: border-color 0.2s ease;
}

textarea:focus {
  outline: none;
  border-color: #00adb5;
  box-shadow: 0 0 0 3px rgba(0, 173, 181, 0.1);
}

textarea::placeholder {
  color: rgba(238, 238, 238, 0.4);
}

/* Upload area */
.upload-area {
  display: flex;
  align-items: center;
  gap: 12px;
}

.upload-btn {
  background: #222831;
  color: #eeeeee;
  border: 1px solid rgba(0, 173, 181, 0.3);
  padding: 10px 20px;
  border-radius: 6px;
  cursor: pointer;
  font-weight: 500;
  transition: all 0.2s ease;
  font-size: 14px;
}

.upload-btn:hover {
  background: #00adb5;
  color: #222831;
  border-color: #00adb5;
}

.file-name {
  color: #00adb5;
  font-size: 14px;
  font-weight: 500;
}

/* Image preview */
.image-preview {
  position: relative;
  margin-top: 12px;
  border-radius: 8px;
  overflow: hidden;
  border: 2px solid rgba(0, 173, 181, 0.3);
  max-width: 300px;
}

.image-preview img {
  width: 100%;
  height: auto;
  display: block;
}

.remove-image-btn {
  position: absolute;
  top: 8px;
  right: 8px;
  background: rgba(239, 68, 68, 0.9);
  color: white;
  border: none;
  width: 32px;
  height: 32px;
  border-radius: 50%;
  cursor: pointer;
  font-size: 18px;
  display: flex;
  align-items: center;
  justify-content: center;
  transition: all 0.2s ease;
}

.remove-image-btn:hover {
  background: #ef4444;
  transform: scale(1.1);
}

/* Footer */
.modal-footer {
  display: flex;
  justify-content: flex-end;
  gap: 12px;
  padding: 20px 24px;
  border-top: 1px solid rgba(0, 173, 181, 0.2);
  background: #222831;
}

.cancel-btn,
.submit-btn {
  padding: 10px 24px;
  border: none;
  border-radius: 6px;
  font-weight: 600;
  font-size: 14px;
  cursor: pointer;
  transition: all 0.3s ease;
}

.cancel-btn {
  background: transparent;
  color: #eeeeee;
  border: 1px solid rgba(238, 238, 238, 0.3);
}

.cancel-btn:hover {
  background: rgba(238, 238, 238, 0.1);
}

.submit-btn {
  background: linear-gradient(135deg, #f59e0b 0%, #d97706 100%);
  color: #222831;
}

.submit-btn:hover:not(:disabled) {
  background: linear-gradient(135deg, #d97706 0%, #b45309 100%);
  transform: translateY(-2px);
  box-shadow: 0 4px 12px rgba(245, 158, 11, 0.4);
}

.submit-btn:disabled {
  opacity: 0.5;
  cursor: not-allowed;
}

/* Animation */
.modal-fade-enter-active,
.modal-fade-leave-active {
  transition: opacity 0.3s ease;
}

.modal-fade-enter-from,
.modal-fade-leave-to {
  opacity: 0;
}

.modal-fade-enter-active .modal-container,
.modal-fade-leave-active .modal-container {
  transition: transform 0.3s ease;
}

.modal-fade-enter-from .modal-container,
.modal-fade-leave-to .modal-container {
  transform: scale(0.9);
}

/* Scrollbar */
.modal-body::-webkit-scrollbar {
  width: 8px;
}

.modal-body::-webkit-scrollbar-track {
  background: #222831;
  border-radius: 4px;
}

.modal-body::-webkit-scrollbar-thumb {
  background: #00adb5;
  border-radius: 4px;
}

.modal-body::-webkit-scrollbar-thumb:hover {
  background: #009fa7;
}
</style>
