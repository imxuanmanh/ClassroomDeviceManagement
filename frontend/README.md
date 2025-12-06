# 🎓 Classroom Device Management - Frontend

Ứng dụng quản lý thiết bị lớp học với giao diện Vue 3 và Vite. Hỗ trợ quản lý thiết bị, lịch sử, yêu cầu mượn và báo cáo hư hỏng với cập nhật real-time.

## 📋 Yêu cầu

- **Node.js**: ^20.19.0 hoặc >=22.12.0
- **npm**: Kèm theo Node.js

## 🚀 Bắt đầu nhanh

### 1. Cài đặt dependencies

```sh
npm install
```

### 2. Chạy development server

```sh
npm run dev
```

Ứng dụng sẽ chạy tại `http://localhost:5173`

### 3. Build cho production

```sh
npm run build
```

### 4. Xem preview build

```sh
npm run preview
```

### 5. Kiểm tra linting

```sh
npm run lint
```

### 6. Format code

```sh
npm run format
```

## 📁 Cấu trúc thư mục

```
src/
├── assets/              # Static assets (CSS, images)
│   ├── base.css
│   └── main.css
├── components/          # Vue components
│   ├── Common/          # Components dùng chung
│   │   ├── Pagination.vue
│   │   ├── SearchFilter.vue
│   │   └── StatusBadge.vue
│   ├── Dashboard/       # Dashboard components
│   │   ├── DashboardView.vue
│   │   └── StatCard.vue
│   ├── Device/          # Device management components
│   │   ├── CategoryModal.vue
│   │   ├── DeviceCard.vue
│   │   ├── DeviceForm.vue
│   │   ├── DeviceModal.vue
│   │   ├── InstanceModal.vue
│   │   └── ReportBrokenModal.vue
│   ├── Layout/          # Layout components
│   │   ├── Header.vue
│   │   ├── Layout.vue
│   │   └── Sidebar.vue
│   ├── icons/           # Icon assets
│   └── UserFormModal.vue
├── composables/         # Vue composables
│   └── useSSE.js       # Server-Sent Events hook
├── config/              # Configuration files
│   ├── api.js          # API endpoints
│   └── apiWrapper.js   # API wrapper
├── mock/                # Mock data
│   └── apimock.js
├── router/              # Vue Router configuration
│   └── index.js
├── stores/              # Pinia stores
│   ├── auth.js         # Authentication store
│   ├── device.js       # Device store
│   └── history.js      # History store
├── utils/               # Utility functions
│   └── toast.js        # Toast notifications
├── views/               # Page views
│   ├── Dashboard.vue
│   ├── DeviceManagement.vue
│   ├── History.vue
│   ├── HistoryUser.vue
│   ├── Login.vue
│   ├── Profile.vue
│   ├── ReportManagement.vue
│   ├── Requests.vue
│   ├── UserBorrow.vue
│   └── UserManagement.vue
├── App.vue              # Root component
└── main.js              # Entry point
```

## 🛠️ Công nghệ sử dụng

- **Vue 3** - Progressive JavaScript framework
- **Vite** - Next generation frontend tooling
- **Vue Router** - Official router for Vue.js
- **Pinia** - State management
- **Element Plus** - UI component library
- **Axios** - HTTP client
- **Chart.js** - JavaScript charting library
- **SweetAlert2** - Beautiful alerts
- **Font Awesome** - Icon library

## 🔧 Cấu hình

### Environment Variables

Tạo file `.env` tại root project:

```dotenv
# Sử dụng mock API (true) hoặc API thật (false)
VITE_USE_MOCK=true
```

### Editor Setup

**Khuyến khích sử dụng:**
- [VSCode](https://code.visualstudio.com/)
- [Volar](https://marketplace.visualstudio.com/items?itemName=Vue.volar) extension

**Vô hiệu hóa Vetur** nếu đã cài đặt trước đó.

## 🔄 Real-time Updates

Ứng dụng hỗ trợ cập nhật real-time cho các dữ liệu thiết bị, lịch sử và yêu cầu. Chi tiết xem [REALTIME_UPDATE_GUIDE.md](./REALTIME_UPDATE_GUIDE.md)

## 📚 Available Scripts

| Script | Mô tả |
|--------|-------|
| `npm run dev` | Chạy development server với hot reload |
| `npm run build` | Build ứng dụng cho production |
| `npm run preview` | Xem preview của production build |
| `npm run lint` | Kiểm tra linting và tự động fix |
| `npm run format` | Format code theo prettier rules |

## 📖 Tài liệu

- [Vite Documentation](https://vite.dev/)
- [Vue 3 Documentation](https://vuejs.org/)
- [Vue Router Documentation](https://router.vuejs.org/)
- [Pinia Documentation](https://pinia.vuejs.org/)
- [Element Plus Documentation](https://element-plus.org/)

## 🤝 Quy tắc Code

- **Indentation**: 2 spaces
- **Line length**: 100 characters
- **Quote style**: Single quotes
- **Semicolon**: Không sử dụng
- **Line ending**: LF

## 📝 License

MIT
