<template>
  <section class="users">
    <header class="page-header">
      <h2>Người mượn</h2>
    </header>
    <div class="form-box">
      <input v-model="form.id" placeholder="Mã người mượn" />
      <input v-model="form.name" placeholder="Tên người mượn" />
      <button @click="save">{{ editingIndex !== null ? 'Cập nhật' : 'Thêm' }}</button>
      <button v-if="editingIndex !== null" @click="cancel">Hủy</button>
    </div>
    <table class="table">
      <thead>
        <tr><th>Mã</th><th>Tên</th><th>Hành động</th></tr>
      </thead>
      <tbody>
        <tr v-for="(b, i) in borrowers" :key="b.id">
          <td>{{ b.id }}</td>
          <td>{{ b.name }}</td>
          <td>
            <button @click="startEdit(b, i)">Sửa</button>
            <button @click="remove(i)">Xóa</button>
          </td>
        </tr>
      </tbody>
    </table>
  </section>
</template>

<script setup>
import { ref } from 'vue'
import { useBorrowersStore } from '@/stores/borrowers.js'

const store = useBorrowersStore()
const borrowers = store.borrowers
const form = ref({ id: '', name: '' })
const editingIndex = ref(null)

function save() {
  if (!form.value.id || !form.value.name) return
  if (editingIndex.value !== null) {
    store.updateBorrower(editingIndex.value, form.value)
    editingIndex.value = null
  } else {
    store.addBorrower({ ...form.value })
  }
  form.value = { id: '', name: '' }
}
function cancel() { form.value = { id: '', name: '' }; editingIndex.value = null }
function remove(i) { store.removeBorrower(i) }
function startEdit(b, i) { form.value = { ...b }; editingIndex.value = i }
</script>

<style scoped>
.users { padding: 16px 12px; }
.page-header { display: flex; align-items: center; justify-content: space-between; margin-bottom: 12px; }
.form-box { display: flex; gap: 8px; margin-bottom: 12px; }
.form-box input { padding: 6px 8px; border: 1px solid #e5e7eb; border-radius: 8px; }
.table { width: 100%; border-collapse: collapse; background: #fff; border-radius: 12px; overflow: hidden; }
.table th, .table td { border: 1px solid #eee; padding: 10px; text-align: left; }
.table th { background: #f8fafc; }
.table button { padding: 6px 10px; border: none; border-radius: 8px; background: #3b82f6; color: #fff; cursor: pointer; }
.table button + button { margin-left: 6px; background: #ef4444; }
</style>
