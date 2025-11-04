<template>
  <div class="profile">
    <div class="card">
      <h2>Thông tin quản trị</h2>
      <div class="row"><span class="label">Tên:</span><span class="value">Admin</span></div>
      <div class="row">
        <span class="label">Email:</span><span class="value">admin@example.com</span>
      </div>
    </div>
  </div>
</template>

<style scoped>
.profile {
  min-height: calc(100vh - 80px);
  display: flex;
  align-items: center;
  justify-content: center;
  padding: 24px;
}
.card {
  background: #fff;
  padding: 24px;
  border-radius: 12px;
  box-shadow: 0 10px 30px rgba(0, 0, 0, 0.08);
  min-width: 320px;
}
.row {
  display: flex;
  gap: 8px;
  margin-top: 8px;
}
.label {
  color: #6b7280;
  min-width: 80px;
}
.value {
  color: #111827;
  font-weight: 600;
}
</style>
