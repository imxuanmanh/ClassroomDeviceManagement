// src/composables/useSSE.js
import { ref, onUnmounted } from 'vue'

/**
 * Composable để quản lý Server-Sent Events (SSE) connection
 * @param {string} url - URL của SSE endpoint
 * @param {object} options - Các tùy chọn cấu hình
 * @returns {object} - SSE state và methods
 */
export function useSSE(url, options = {}) {
  const {
    autoConnect = false,
    withCredentials = false,
    reconnect = true,
    reconnectDelay = 3000,
    maxReconnectAttempts = 5,
  } = options

  // State
  const data = ref(null)
  const error = ref(null)
  const isConnected = ref(false)
  const reconnectAttempts = ref(0)

  let eventSource = null
  let reconnectTimer = null

  /**
   * Kết nối tới SSE server
   */
  const connect = () => {
    try {
      // Ngăn multiple connections
      if (eventSource) {
        disconnect()
      }

      // Tạo EventSource với options
      const eventSourceOptions = withCredentials ? { withCredentials: true } : {}

      eventSource = new EventSource(url, eventSourceOptions)

      // Event: Connection opened
      eventSource.onopen = () => {
        isConnected.value = true
        reconnectAttempts.value = 0
        error.value = null
        console.log('✅ SSE Connected to:', url)
      }

      // Event: Message received
      eventSource.onmessage = (event) => {
        try {
          // Parse JSON data
          data.value = JSON.parse(event.data)
        } catch (e) {
          // Nếu không phải JSON, lưu raw data
          data.value = event.data
        }
      }

      // Event: Error occurred
      eventSource.onerror = (err) => {
        error.value = err
        isConnected.value = false
        console.error('❌ SSE Error:', err)

        // Auto reconnect nếu được bật
        if (reconnect && reconnectAttempts.value < maxReconnectAttempts) {
          handleReconnect()
        } else if (reconnectAttempts.value >= maxReconnectAttempts) {
          console.error('🚫 Max reconnect attempts reached')
        }
      }
    } catch (err) {
      error.value = err
      console.error('❌ Failed to create SSE connection:', err)
    }
  }

  /**
   * Ngắt kết nối SSE
   */
  const disconnect = () => {
    if (reconnectTimer) {
      clearTimeout(reconnectTimer)
      reconnectTimer = null
    }

    if (eventSource) {
      eventSource.close()
      eventSource = null
      isConnected.value = false
      console.log('🔌 SSE Disconnected')
    }
  }

  /**
   * Xử lý reconnect logic
   */
  const handleReconnect = () => {
    reconnectAttempts.value++
    console.log(
      `🔄 Attempting to reconnect... (${reconnectAttempts.value}/${maxReconnectAttempts})`,
    )

    reconnectTimer = setTimeout(() => {
      connect()
    }, reconnectDelay)
  }

  /**
   * Lắng nghe custom event từ SSE
   * @param {string} eventName - Tên của custom event
   * @param {function} callback - Callback function
   */
  const addEventListener = (eventName, callback) => {
    if (eventSource) {
      eventSource.addEventListener(eventName, (event) => {
        try {
          const parsedData = JSON.parse(event.data)
          callback(parsedData, event)
        } catch (e) {
          callback(event.data, event)
        }
      })
    } else {
      console.warn('⚠️ EventSource not initialized. Call connect() first.')
    }
  }

  /**
   * Reset error state
   */
  const resetError = () => {
    error.value = null
  }

  /**
   * Manual reconnect
   */
  const reconnectManual = () => {
    reconnectAttempts.value = 0
    connect()
  }

  // Auto connect nếu được bật
  if (autoConnect) {
    connect()
  }

  // Cleanup khi component unmount
  onUnmounted(() => {
    disconnect()
  })

  return {
    // State
    data,
    error,
    isConnected,
    reconnectAttempts,

    // Methods
    connect,
    disconnect,
    addEventListener,
    resetError,
    reconnect: reconnectManual,
  }
}
