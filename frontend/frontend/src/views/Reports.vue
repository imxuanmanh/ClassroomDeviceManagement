<template>
  <div class="report-page">
    <h2>Báo cáo Thống kê</h2>

    <div class="summary">
      <p><b>Tổng số thiết bị:</b> {{ totalDevices }}</p>
      <p><b>Đang mượn:</b> {{ borrowed }}</p>
      <p><b>Khả dụng:</b> {{ available }}</p>
      <p><b>Hỏng:</b> {{ broken }}</p>
    </div>

    <canvas id="deviceChart" style="max-width: 300px; max-height: 300px"></canvas>
  </div>
</template>

<script>
import Chart from 'chart.js/auto' // ✅ import đúng cách

export default {
  name: 'Reports',
  data() {
    return {
      totalDevices: 10,
      borrowed: 3,
      available: 6,
      broken: 1,
    }
  },
  mounted() {
    new Chart(document.getElementById('deviceChart'), {
      type: 'pie',
      data: {
        labels: ['Khả dụng', 'Đang mượn', 'Hỏng'],
        datasets: [
          {
            data: [this.available, this.borrowed, this.broken],
            backgroundColor: ['#4CAF50', '#FF9800', '#F44336'],
          },
        ],
      },
    })
  },
}
</script>
