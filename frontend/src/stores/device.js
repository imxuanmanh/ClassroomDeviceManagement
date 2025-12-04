import { defineStore } from 'pinia'
import { ref, computed } from 'vue'
import { deviceApi, categoryApi, instanceApi } from '@/config/apiWrapper'

export const useDeviceStore = defineStore('device', () => {
  const devices = ref([])
  const categories = ref([])
  const modelsByCategory = ref({})
  const allInstances = ref([]) // Lưu tất cả instances
  const loading = ref(false)
  const error = ref('')

  async function refreshInstances() {
    allInstances.value = []

    for (const categoryId in modelsByCategory.value) {
      const models = modelsByCategory.value[categoryId] || []

      for (const model of models) {
        try {
          const instances = await instanceApi.getByModelId(model.modelId)
          allInstances.value.push(...instances)
        } catch (err) {
          console.log('Lỗi fetch instance:', err)
        }
      }
    }
  }

  // Fetch tất cả thiết bị
  async function fetchDevices() {
    loading.value = true
    error.value = ''
    try {
      const data = await deviceApi.getAll()
      devices.value = Array.isArray(data) ? data : []
    } catch (err) {
      console.error('Lỗi tải thiết bị:', err)
      error.value = 'Không thể tải danh sách thiết bị'
      devices.value = []
    } finally {
      loading.value = false
    }
  }

  // Fetch tất cả categories
  async function fetchCategories() {
    loading.value = true
    error.value = ''
    try {
      categories.value = await categoryApi.getAll()
      console.log('✅ Đã fetch categories:', categories.value.length)
    } catch (err) {
      console.error('Lỗi tải category:', err)
      error.value = 'Không thể tải danh sách loại thiết bị'
      categories.value = []
    } finally {
      loading.value = false
    }
  }

  // 🔥 FIXED: Fetch models theo category và instances của chúng (LUÔN FETCH MỚI)
  async function fetchModelsByCategory(categoryId) {
    try {
      console.log(`🔄 Fetching models cho category ${categoryId}...`)

      // Fetch models mới
      const models = await categoryApi.getModelsByCategory(categoryId)
      modelsByCategory.value[categoryId] = models
      console.log(`✅ Đã fetch ${models.length} models cho category ${categoryId}`)

      // 🔥 XÓA instances cũ của category này bằng instanceId
      const modelIdsInCategory = models.map((m) => m.modelId)
      const oldInstanceIds = allInstances.value
        .filter((i) => modelIdsInCategory.includes(i.modelId))
        .map((i) => i.instanceId)

      allInstances.value = allInstances.value.filter((instance) => {
        return !oldInstanceIds.includes(instance.instanceId)
      })
      console.log(`🧹 Đã xóa ${oldInstanceIds.length} instances cũ của category ${categoryId}`)

      // Fetch instances MỚI cho từng model
      for (const model of models) {
        if (instanceApi && instanceApi.getByModelId) {
          try {
            const instances = await instanceApi.getByModelId(model.modelId)
            console.log(`  ↳ Model ${model.modelId}: ${instances.length} instances`)

            // 🔥 CHỈ THÊM instances chưa có trong allInstances
            instances.forEach((newInstance) => {
              const exists = allInstances.value.some((i) => i.instanceId === newInstance.instanceId)
              if (!exists) {
                allInstances.value.push(newInstance)
              }
            })
          } catch (err) {
            console.log(`  ↳ Lỗi tải instances cho model ${model.modelId}:`, err)
          }
        }
      }

      console.log(`📊 Tổng instances hiện tại: ${allInstances.value.length}`)

      // 🔥 LOG CHI TIẾT để debug
      const statusCount = {
        available: allInstances.value.filter((i) => i.statusId === 1).length,
        borrowed: allInstances.value.filter((i) => i.statusId === 2).length,
        maintenance: allInstances.value.filter((i) => i.statusId === 3).length,
        broken: allInstances.value.filter((i) => i.statusId === 4).length,
      }
      console.log('📊 Phân loại instances:', statusCount)

      return modelsByCategory.value[categoryId]
    } catch (err) {
      console.error('Lỗi tải models:', err)
      modelsByCategory.value[categoryId] = []
      return []
    }
  }

  // Tính toán stats từ instances
  const stats = computed(() => {
    let total = 0
    let available = 0
    let borrowed = 0
    let maintenance = 0
    let broken = 0

    // Tính từ all instances
    allInstances.value.forEach((instance) => {
      total += 1
      if (instance.statusId === 1)
        available += 1 // Khả dụng
      else if (instance.statusId === 2)
        borrowed += 1 // Đang mượn
      else if (instance.statusId === 3)
        maintenance += 1 // Bảo trì
      else if (instance.statusId === 4) broken += 1 // Hỏng
    })

    console.log('📊 Stats computed:', { total, available, borrowed, maintenance, broken })

    return {
      total,
      borrowed,
      available,
      maintenance,
      broken,
      recentBorrows: 0,
    }
  })

  // Tính toán theo category
  const categoriesStats = computed(() => {
    const stats = {}
    categories.value.forEach((cat) => {
      const models = modelsByCategory.value[cat.id] || []
      const totalQuantity = models.reduce((sum, m) => sum + (m.totalQuantity || 0), 0)
      const availableQuantity = models.reduce((sum, m) => sum + (m.availableQuantity || 0), 0)
      const borrowedQuantity = totalQuantity - availableQuantity

      stats[cat.id] = {
        total: totalQuantity,
        borrowed: borrowedQuantity,
        available: availableQuantity,
      }
    })
    return stats
  })

  return {
    devices,
    categories,
    modelsByCategory,
    allInstances,
    loading,
    error,
    stats,
    categoriesStats,
    fetchDevices,
    fetchCategories,
    fetchModelsByCategory,
    refreshInstances,
  }
})
