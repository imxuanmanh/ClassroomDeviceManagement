<template>
  <div class="history-page">
    <h2>Lịch sử Người mượn</h2>
    <p>Danh sách thiết bị mà từng người đã mượn.</p>

    <table>
      <thead>
        <tr>
          <th>Người mượn</th>
          <th>Thiết bị</th>
          <th>Ngày mượn</th>
          <th>Ngày trả</th>
          <th>Trạng thái</th>
        </tr>
      </thead>
      <tbody>
        <tr v-for="(item, index) in borrowHistory" :key="index">
          <td>{{ item.user }}</td>
          <td>{{ item.device }}</td>
          <td>{{ item.borrowDate }}</td>
          <td>{{ item.returnDate || 'Chưa trả' }}</td>
          <td>{{ item.status }}</td>
        </tr>
      </tbody>
    </table>
  </div>
</template>

<script>
export default {
  name: 'UserHistory',
  data() {
    return {
      borrowHistory: [
        {
          user: 'Nguyễn Văn A',
          device: 'Laptop Dell Inspiron 15',
          borrowDate: '2025-09-10',
          returnDate: '2025-09-12',
          status: 'Đã trả',
        },
        {
          user: 'Trần Thị B',
          device: 'Máy chiếu Epson X05',
          borrowDate: '2025-09-11',
          returnDate: null,
          status: 'Đang mượn',
        },
      ],
    }
  },
}
</script>

<style scoped>
.history-page {
  padding: 20px;
}

table {
  width: 100%;
  border-collapse: collapse;
  margin-top: 15px;
}

th,
td {
  border: 1px solid #ddd;
  padding: 8px;
  text-align: center;
}

th {
  background: #f5f5f5;
}
</style>
