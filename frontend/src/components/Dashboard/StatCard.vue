<!-- <script setup>
defineProps({
  title: String,
  value: String,
  color: {
    type: String,
    default: '',
  },
})
</script>

<template>
  <div class="stat-card" :style="{ backgroundColor: color }">
    <h3>{{ title }}</h3>
    <p>{{ value }}</p>
  </div>
</template>

<style scoped>
.stat-card {
  padding: 1.25rem;
  border-radius: 12px;
  color: black;
  display: flex;
  align-items: center;
  gap: 12px;
  font-family: 'Inter', sans-serif;
  box-shadow: 0 4px 6px rgba(0, 0, 0, 0.08);
  transition:
    transform 0.2s ease,
    box-shadow 0.2s ease;
}
</style> -->

<template>
  <!-- Thẻ chính của card, có viền trên đổi màu theo prop "color" -->
  <div class="stat-card" :style="{ borderTopColor: color }">
    <!-- Ô icon, có nền màu lấy từ prop "color" -->
    <div class="icon-box" :style="{ backgroundColor: color }">
      <span class="icon">{{ icon }}</span>
      <!-- Hiển thị icon -->
    </div>

    <!-- Nội dung của card: số liệu + nhãn -->
    <div class="content">
      <h3>{{ value }}</h3>
      <!-- Giá trị chính (số liệu) -->
      <p>{{ title }}</p>
      <!-- Tiêu đề mô tả -->
    </div>
  </div>
</template>

<script setup>
/**
 * Nhận props từ component cha
 * - title: tên/nhãn (vd: Thiết bị, Đang mượn...)
 * - value: số liệu (vd: 0, 10, 25)
 * - color: màu sắc chủ đạo (dùng cho border top và icon)
 * - icon: ký hiệu hoặc emoji hiển thị trong ô icon
 */
defineProps({
  title: String,
  value: String,
  color: String,
  icon: String,
})
</script>

<style scoped>
/* Thẻ chứa toàn bộ card */
.stat-card {
  display: flex; /* icon + content nằm ngang */
  align-items: center; /* căn giữa theo chiều dọc */
  gap: 12px; /* khoảng cách giữa icon và content */
  background: #fff; /* nền trắng */
  padding: 16px; /* khoảng cách bên trong */
  border-radius: 12px; /* bo góc */
  box-shadow: 0 2px 6px rgba(0, 0, 0, 0.08); /* đổ bóng nhẹ */
  transition: transform 0.2s ease; /* hiệu ứng hover */
  border-top: 4px solid transparent; /* viền trên đổi màu bằng prop */
}

/* Hiệu ứng hover: nổi lên nhẹ */
.stat-card:hover {
  transform: translateY(-3px);
}

/* Ô chứa icon */
.icon-box {
  display: flex; /* căn giữa icon */
  align-items: center;
  justify-content: center;
  width: 48px; /* chiều rộng */
  height: 48px; /* chiều cao */
  border-radius: 10px; /* bo góc nhẹ */
  color: white; /* icon màu trắng */
  font-size: 22px; /* size icon */
  flex-shrink: 0; /* không co lại khi thiếu chỗ */
}

/* Giá trị chính (số liệu) */
.content h3 {
  margin: 0;
  font-size: 28px; /* chữ to để nổi bật */
  font-weight: 700; /* đậm */
  color: #111827; /* màu tối, dễ đọc */
  line-height: 1.2; /* chiều cao dòng cân đối */
}

/* Nhãn (tiêu đề phụ) */
.content p {
  margin: 0;
  font-size: 16px; /* to vừa */
  font-weight: 500; /* đậm nhẹ */
  color: #374151; /* xám đậm, bớt nổi bật hơn value */
}
</style>
