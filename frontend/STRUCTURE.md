# 📂 Project Structure Guide

Hướng dẫn chi tiết về cấu trúc thư mục của project.

## Root Level Files

```
├── index.html              # HTML entry point
├── package.json            # Project metadata & dependencies
├── package-lock.json       # Dependency lock file
├── vite.config.js          # Vite configuration
├── eslint.config.js        # ESLint configuration
├── jsconfig.json           # JavaScript config (path aliases)
├── .env                    # Environment variables (local config)
├── .gitignore              # Git ignore rules
├── .gitattributes          # Git attributes
├── .vscode/                # VSCode workspace settings
├── README.md               # Project documentation
└── STRUCTURE.md            # This file
```

## Source Directory (`src/`)

### `assets/`
CSS files và static assets cho styling.

- `base.css` - Base styles
- `main.css` - Main application styles

### `components/`
Vue components tổ chức theo chức năng.

#### `Common/`
Components tái sử dụng trong toàn ứng dụng:
- `Pagination.vue` - Pagination component
- `SearchFilter.vue` - Search & filter component
- `StatusBadge.vue` - Status badge display

#### `Dashboard/`
Components cho Dashboard view:
- `DashboardView.vue` - Main dashboard container
- `StatCard.vue` - Statistics card component

#### `Device/`
Components cho Device Management:
- `DeviceForm.vue` - Form to create/edit device
- `DeviceCard.vue` - Device card display
- `DeviceModal.vue` - Device details modal
- `CategoryModal.vue` - Category management modal
- `InstanceModal.vue` - Device instance modal
- `ReportBrokenModal.vue` - Report broken device modal

#### `Layout/`
Layout components:
- `Header.vue` - Top header bar
- `Sidebar.vue` - Sidebar navigation
- `Layout.vue` - Main layout wrapper

#### `icons/`
Static icon assets (PNG, SVG)

#### `UserFormModal.vue`
User form modal component (top-level)

### `composables/`
Vue 3 Composition API hooks:

- `useSSE.js` - Server-Sent Events hook for real-time updates

### `config/`
Configuration files:

- `api.js` - API endpoints configuration
- `apiWrapper.js` - Axios wrapper with interceptors

### `mock/`
Mock data for development:

- `apimock.js` - Mock API responses

### `router/`
Vue Router configuration:

- `index.js` - Router setup & route definitions

### `stores/`
Pinia state management:

- `auth.js` - Authentication state
- `device.js` - Device management state
- `history.js` - History records state

### `utils/`
Utility functions & helpers:

- `toast.js` - Toast notification helper

### `views/`
Page-level components (routes):

- `Dashboard.vue` - Dashboard page
- `DeviceManagement.vue` - Device management page
- `History.vue` - Device history page
- `HistoryUser.vue` - User borrowing history
- `Login.vue` - Login page
- `Profile.vue` - User profile page
- `ReportManagement.vue` - Report management page
- `Requests.vue` - Borrow requests page
- `UserBorrow.vue` - User borrow page
- `UserManagement.vue` - User management page

### Root Files

- `App.vue` - Root component
- `main.js` - Application entry point

## Naming Conventions

### Files
- Components: **PascalCase** (e.g., `DeviceCard.vue`)
- Utilities: **camelCase** (e.g., `useSSE.js`)
- Directories: **kebab-case** (e.g., `device-management`)

### Components
- Container/Page components: End with name (e.g., `Dashboard.vue`)
- Reusable components: Descriptive noun (e.g., `StatCard.vue`)
- Modal components: End with `Modal` (e.g., `DeviceModal.vue`)

## Import Paths

Có thể cấu hình path aliases trong `jsconfig.json`:

```javascript
import { useDevice } from '@/stores/device'
import Component from '@/components/Common/Pagination.vue'
```

## Best Practices

1. **Component Organization**: Group related components in subdirectories
2. **Single Responsibility**: Mỗi component có một nhiệm vụ chính
3. **Naming**: Tên file phản ánh chức năng
4. **Composition**: Ưu tiên small, reusable components
5. **Store Usage**: Chỉ dùng Pinia cho shared state
6. **API Calls**: Centralize trong `config/apiWrapper.js`

## Common Tasks

### Thêm page mới
1. Tạo file `src/views/NewPage.vue`
2. Thêm route trong `src/router/index.js`
3. Thêm navigation trong layout

### Thêm component
1. Tạo file trong `src/components/`
2. Organize theo folder nếu cần
3. Export component trong file

### Thêm store
1. Tạo file trong `src/stores/`
2. Define state, getters, actions
3. Import và use trong components

### Thêm utility
1. Tạo file trong `src/utils/`
2. Export functions
3. Import khi cần
