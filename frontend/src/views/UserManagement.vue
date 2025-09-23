<template>
  <section class="users">
    <header class="page-header">
      <h2>Người dùng</h2>
      <div class="actions"></div>
    </header>
    <div class="form-box">
      <input v-model="form.id" placeholder="Mã người mượn" />
      <input v-model="form.name" placeholder="Tên người mượn" />
      <button @click="save">{{ editingIndex !== null ? 'Cập nhật' : 'Thêm' }}</button>
      <button v-if="editingIndex !== null" @click="cancel">Hủy</button>
    </div>
    <div v-if="error" class="error">{{ error }}</div>
    <table class="table">
      <thead>
        <tr><th>Mã</th><th>Tên</th><th>Hành động</th></tr>
      </thead>
      <tbody>
        <tr v-for="(u, i) in users" :key="u.id">
          <td>{{ u.id }}</td>
          <td>{{ u.name }}</td>
          <td>
            <button @click="startEdit(u, i)">Sửa</button>
            <button @click="remove(i)">Xóa</button>
          </td>
        </tr>
      </tbody>
    </table>
  </section>
</template>

<script setup>
import { ref, onMounted } from 'vue'
import { userApi } from '@/config/api.js'

const users = ref([])
const form = ref({ id: '', name: '' })
const editingIndex = ref(null)
const loading = ref(false)
const error = ref('')

async function fetchUsers(){
  loading.value = true
  error.value = ''
  try{
    const list = await userApi.getAll()
    users.value = (Array.isArray(list) ? list : []).map(u => ({
      id: u.id || u.userId || '',
      name: u.name || u.fullName || '',
    }))
  }catch(err){
    console.error(err)
    error.value = 'Không thể tải danh sách người dùng'
  }finally{
    loading.value = false
  }
}

async function save() {
  if (!form.value.id || !form.value.name) return
  loading.value = true
  error.value = ''
  try{
    if (editingIndex.value !== null) {
      const current = users.value[editingIndex.value]
      const id = current?.id
      await userApi.update(id, { id, name: form.value.name })
      editingIndex.value = null
    } else {
      await userApi.create({ id: form.value.id, name: form.value.name })
    }
    await fetchUsers()
    form.value = { id: '', name: '' }
  }catch(err){
    console.error(err)
    error.value = 'Không thể lưu người dùng'
  }finally{
    loading.value = false
  }
}

function cancel() { form.value = { id: '', name: '' }; editingIndex.value = null }

async function remove(i) {
  loading.value = true
  error.value = ''
  try{
    const id = users.value[i]?.id
    await userApi.delete(id)
    await fetchUsers()
  }catch(err){
    console.error(err)
    error.value = 'Không thể xóa người dùng'
  }finally{
    loading.value = false
  }
}

function startEdit(u, i) { form.value = { ...u }; editingIndex.value = i }

onMounted(fetchUsers)
</script>

<style scoped>
.users { padding: 16px 12px; }
.page-header { display: flex; align-items: center; justify-content: space-between; gap: 12px; margin-bottom: 16px; }
.page-header h2 { margin: 0; color: #111827; }
.actions { display: flex; gap: 8px; }
.form-box { display: flex; gap: 8px; margin-bottom: 12px; }
.form-box input { padding: 6px 8px; border: 1px solid #e5e7eb; border-radius: 8px; }
.table { width: 100%; border-collapse: collapse; background: #fff; border-radius: 12px; overflow: hidden; }
.table th, .table td { border: 1px solid #eee; padding: 10px; text-align: left; }
.table th { background: #f8fafc; }
.table button { padding: 6px 10px; border: none; border-radius: 8px; background: #3b82f6; color: #fff; cursor: pointer; }
.table button + button { margin-left: 6px; background: #ef4444; }
.error { color: #b91c1c; margin-bottom: 8px; }
</style>
