/**
 * MOCK API - Dữ liệu giả để test khi backend chưa sẵn sàng
 */

// Mock Data
const mockCategories = [
  { id: 1, name: 'Máy tính' },
  { id: 2, name: 'Máy chiếu' },
  { id: 3, name: 'Loa' },
]

const mockModels = {
  1: [
    {
      modelId: 101,
      modelName: 'Dell XPS 13',
      specifications: 'Intel i7, 16GB RAM',
      storageLocation: 'Tủ A1',
      totalQuantity: 5,
      availableQuantity: 3, // Sẽ được tính lại tự động
    },
    {
      modelId: 102,
      modelName: 'MacBook Pro',
      specifications: 'M1, 8GB RAM',
      storageLocation: 'Tủ A2',
      totalQuantity: 3,
      availableQuantity: 1, // Sẽ được tính lại tự động
    },
  ],
  2: [
    {
      modelId: 201,
      modelName: 'Epson EB-2250U',
      specifications: '5000 lumens, Full HD',
      storageLocation: 'Kho B',
      totalQuantity: 2,
      availableQuantity: 2, // Sẽ được tính lại tự động
    },
  ],
  3: [
    {
      modelId: 301,
      modelName: 'JBL Professional',
      specifications: '200W, Bluetooth',
      storageLocation: 'Kho C',
      totalQuantity: 4,
      availableQuantity: 4, // Sẽ được tính lại tự động
    },
  ],
}

const mockInstances = {
  101: [
    {
      instanceId: 1001,
      instanceCode: 'DELL-XPS-001',
      statusId: 3, // 🔥 ĐỔI statusId Ở ĐÂY
      currentLocation: 'Phòng học A',
      borrower: null,
      usageDuration: null,
    },
    {
      instanceId: 1002,
      instanceCode: 'DELL-XPS-002',
      statusId: 3,
      currentLocation: 'Nhà Nguyễn Văn A',
      borrower: 'Nguyễn Văn A',
      usageDuration: '2 tuần',
    },
    {
      instanceId: 1003,
      instanceCode: 'DELL-XPS-003',
      statusId: 3,
      currentLocation: 'Xưởng sửa chữa',
      borrower: null,
      usageDuration: null,
    },
    {
      instanceId: 1004,
      instanceCode: 'DELL-XPS-004',
      statusId: 4,
      currentLocation: 'Kho lưu trữ',
      borrower: null,
      usageDuration: null,
    },
    {
      instanceId: 1005,
      instanceCode: 'DELL-XPS-005',
      statusId: 1,
      currentLocation: 'Phòng học B',
      borrower: null,
      usageDuration: null,
    },
  ],
  102: [
    {
      instanceId: 1101,
      instanceCode: 'MAC-001',
      statusId: 1,
      currentLocation: 'Tủ A2',
      borrower: null,
      usageDuration: null,
    },
    {
      instanceId: 1102,
      instanceCode: 'MAC-002',
      statusId: 2,
      currentLocation: 'Nhà Trần Thị B',
      borrower: 'Trần Thị B',
      usageDuration: '1 tuần',
    },
    {
      instanceId: 1103,
      instanceCode: 'MAC-003',
      statusId: 4,
      currentLocation: 'Xưởng sửa chữa',
      borrower: null,
      usageDuration: null,
    },
  ],
  201: [
    {
      instanceId: 2001,
      instanceCode: 'EPSON-001',
      statusId: 1,
      currentLocation: 'Phòng họp',
      borrower: null,
      usageDuration: null,
    },
    {
      instanceId: 2002,
      instanceCode: 'EPSON-002',
      statusId: 4,
      currentLocation: 'Kho lưu trữ',
      borrower: null,
      usageDuration: null,
    },
  ],
  301: [
    {
      instanceId: 3001,
      instanceCode: 'JBL-001',
      statusId: 4,
      currentLocation: 'Kho',
      borrower: null,
      usageDuration: null,
    },
    {
      instanceId: 3002,
      instanceCode: 'JBL-002',
      statusId: 1,
      currentLocation: 'Kho',
      borrower: null,
      usageDuration: null,
    },
    {
      instanceId: 3003,
      instanceCode: 'JBL-003',
      statusId: 2,
      currentLocation: 'Nhà Lê Văn C',
      borrower: 'Lê Văn C',
      usageDuration: '3 ngày',
    },
    {
      instanceId: 3004,
      instanceCode: 'JBL-004',
      statusId: 1,
      currentLocation: 'Kho',
      borrower: null,
      usageDuration: null,
    },
  ],
}

const mockReports = {
  1: [
    {
      reportId: 1,
      reportDate: '2025-11-28',
      userFullName: 'Nguyễn Văn A',
      deviceName: 'Dell XPS 13',
      description: 'Màn hình bị sáng bất thường',
      imagePath: '/images/report1.jpg',
      status: 1,
    },
  ],
  2: [
    {
      reportId: 2,
      reportDate: '2025-11-27',
      userFullName: 'Trần Thị B',
      deviceName: 'MacBook Pro',
      description: 'Pin không sạc được',
      imagePath: null,
      status: 2,
    },
  ],
  3: [],
}

/**
 * Helper: Delay promise để mô phỏng network delay
 */
function delay(ms = 500) {
  return new Promise((resolve) => setTimeout(resolve, ms))
}

/**
 * 🔥 HÀM TỰ ĐỘNG TÍNH availableQuantity từ instances
 */
function recalculateModelStats(categoryId) {
  const models = mockModels[categoryId] || []

  models.forEach((model) => {
    const instances = mockInstances[model.modelId] || []

    // Tổng số thiết bị
    model.totalQuantity = instances.length

    // Đếm số thiết bị khả dụng (statusId === 1)
    model.availableQuantity = instances.filter((i) => i.statusId === 1).length

    console.log(
      `📊 Model ${model.modelId}: Total=${model.totalQuantity}, Available=${model.availableQuantity}`,
    )
  })
}

/**
 * Mock API Functions
 */

// Categories
export async function mockGetCategories() {
  await delay()
  return mockCategories
}

export async function mockCreateCategory(data) {
  await delay()
  const id = Math.max(...mockCategories.map((c) => c.id), 0) + 1
  const newCategory = { id, name: data.name }
  mockCategories.push(newCategory)
  mockModels[id] = []
  return newCategory
}

// Models
export async function mockGetModelsByCategory(categoryId) {
  await delay()

  // 🔥 TỰ ĐỘNG TÍNH LẠI availableQuantity trước khi trả về
  recalculateModelStats(categoryId)

  return mockModels[categoryId] || []
}

export async function mockCreateModel(data) {
  await delay()
  const categoryId = data.categoryId
  const modelId =
    Math.max(
      ...Object.values(mockModels)
        .flat()
        .map((m) => m.modelId),
      100,
    ) + 1

  const newModel = {
    modelId,
    modelName: data.modelName,
    specifications: data.specifications,
    storageLocation: data.storageLocation,
    totalQuantity: 1,
    availableQuantity: 1,
  }

  if (!mockModels[categoryId]) {
    mockModels[categoryId] = []
  }
  mockModels[categoryId].push(newModel)
  mockInstances[modelId] = []

  return newModel
}

// Instances
export async function mockGetInstances(modelId) {
  await delay()
  const instances = mockInstances[modelId] || []

  // 🔍 LOG RA ĐỂ KIỂM TRA
  console.log(`🔍 mockGetInstances(${modelId}):`, instances)
  instances.forEach((i) => {
    console.log(`  - ${i.instanceCode}: statusId=${i.statusId}`)
  })

  return instances
}

export async function mockCreateInstance(data) {
  await delay()
  const modelId = data.modelId
  const instanceId =
    Math.max(
      ...Object.values(mockInstances)
        .flat()
        .map((i) => i.instanceId),
      1000,
    ) + 1

  const newInstance = {
    instanceId,
    instanceCode: data.instanceCode || `INSTANCE-${instanceId}`,
    statusId: 1,
    currentLocation: data.currentLocation || 'Kho',
    borrower: null,
    usageDuration: null,
  }

  if (!mockInstances[modelId]) {
    mockInstances[modelId] = []
  }
  mockInstances[modelId].push(newInstance)

  // 🔥 Tự động cập nhật lại stats cho category
  const categoryId = data.categoryId
  if (categoryId) {
    recalculateModelStats(categoryId)
  }

  return newInstance
}

// Reports
export async function mockGetReportsByStatus(statusId) {
  await delay()
  return mockReports[statusId] || []
}

export async function mockProcessReport(reportId) {
  await delay()
  for (const statusReports of Object.values(mockReports)) {
    const report = statusReports.find((r) => r.reportId === reportId)
    if (report) {
      report.status = 2
      return report
    }
  }
  throw new Error('Report not found')
}

export async function mockCompleteReport(reportId, isSuccess) {
  await delay()
  for (const statusReports of Object.values(mockReports)) {
    const report = statusReports.find((r) => r.reportId === reportId)
    if (report) {
      report.status = 3
      return report
    }
  }
  throw new Error('Report not found')
}

export async function mockCancelReport(reportId) {
  await delay()
  for (const statusReports of Object.values(mockReports)) {
    const index = statusReports.findIndex((r) => r.reportId === reportId)
    if (index !== -1) {
      statusReports.splice(index, 1)
      return { success: true }
    }
  }
  throw new Error('Report not found')
}

// ... (Giữ nguyên code cũ của bạn ở trên)

// ---------------------------------------------------------
// 🔥 BỔ SUNG: MOCK USER & AUTH DATA
// ---------------------------------------------------------

const mockUsers = [
  { username: 'admin', fullname: 'Quản Trị Viên', email: 'admin@system.com', role: 1 },
  { username: 'gv001', fullname: 'Nguyễn Văn Giảng', email: 'gv1@school.edu.vn', role: 2 },
  { username: 'sv2024', fullname: 'Trần Học Sinh', email: 'sv@student.edu.vn', role: 3 },
  { username: 'sv2025', fullname: 'Lê Thị B', email: 'leb@student.edu.vn', role: 3 },
]

/**
 * User API Functions
 */
export async function mockGetUsers() {
  await delay(600)
  console.log('👥 [MOCK] Lấy danh sách users:', mockUsers.length)
  // Trả về bản sao để tránh tham chiếu
  return [...mockUsers]
}

/**
 * Auth API Functions
 */
export async function mockRegister(formData) {
  await delay(800)
  console.log('📝 [MOCK] Đăng ký user mới:', formData)

  // 1. Validate: Kiểm tra trùng username
  const exists = mockUsers.find((u) => u.username === formData.username)
  if (exists) {
    // Giả lập lỗi trả về từ server
    throw new Error('Tên đăng nhập đã tồn tại!')
  }

  // 2. Map dữ liệu: Form gửi roleId -> Database lưu role
  const newUser = {
    username: formData.username,
    fullname: formData.fullname,
    email: formData.email,
    role: parseInt(formData.roleId),
    // password: ... (thường backend không trả về password)
  }

  mockUsers.push(newUser)
  return { message: 'Đăng ký thành công', user: newUser }
}
