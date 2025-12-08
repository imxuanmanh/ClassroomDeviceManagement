// src/composables/useSSE.js
import { ref, onUnmounted } from "vue";
// 👇 Đảm bảo bạn đã cài: npm install event-source-polyfill
import { EventSourcePolyfill } from "event-source-polyfill";

/**
 * Composable quản lý SSE có hỗ trợ Header Authorization (Bearer Token)
 */
export function useSSE(url, options = {}) {
  const {
    autoConnect = false,
    reconnect = true,
    reconnectDelay = 3000,
    maxReconnectAttempts = 5,
  } = options;

  // State
  const data = ref(null);
  const error = ref(null);
  const isConnected = ref(false);
  const reconnectAttempts = ref(0);

  let eventSource = null;
  let reconnectTimer = null;

  /**
   * Kết nối tới SSE server
   */
  const connect = () => {
    try {
      // Ngăn multiple connections
      if (eventSource) {
        disconnect();
      }

      // 👇 BƯỚC QUAN TRỌNG: Lấy token và cấu hình Header
      const token = localStorage.getItem("authToken");

      const esOptions = {
        headers: {
          Authorization: `Bearer ${token}`, // Gửi Token tại đây
        },
        heartbeatTimeout: 120000, // Tăng thời gian timeout để tránh ngắt kết nối sớm
      };

      // 👇 Dùng EventSourcePolyfill thay vì EventSource gốc
      eventSource = new EventSourcePolyfill(url, esOptions);

      // Event: Connection opened
      eventSource.onopen = () => {
        isConnected.value = true;
        reconnectAttempts.value = 0;
        error.value = null;
        console.log("✅ SSE Connected to:", url);
      };

      // Event: Message received
      eventSource.onmessage = (event) => {
        try {
          data.value = JSON.parse(event.data);
        } catch (e) {
          data.value = event.data;
        }
      };

      // Event: Error occurred
      eventSource.onerror = (err) => {
        // Log lỗi chi tiết để debug
        console.error("❌ SSE Error Details:", err);

        error.value = err;
        isConnected.value = false;

        // 🛑 Nếu lỗi là 401 (Hết hạn Token), DỪNG reconnect ngay lập tức
        if (err && err.status === 401) {
          console.error("⛔ Token hết hạn hoặc không hợp lệ. Dừng Reconnect.");
          disconnect();
          return;
        }

        // Auto reconnect logic
        if (reconnect && reconnectAttempts.value < maxReconnectAttempts) {
          handleReconnect();
        } else if (reconnectAttempts.value >= maxReconnectAttempts) {
          console.error("🚫 Max reconnect attempts reached");
        }
      };
    } catch (err) {
      error.value = err;
      console.error("❌ Failed to create SSE connection:", err);
    }
  };

  /**
   * Ngắt kết nối SSE
   */
  const disconnect = () => {
    if (reconnectTimer) {
      clearTimeout(reconnectTimer);
      reconnectTimer = null;
    }

    if (eventSource) {
      eventSource.close();
      eventSource = null;
      isConnected.value = false;
      console.log("🔌 SSE Disconnected");
    }
  };

  /**
   * Xử lý reconnect logic
   */
  const handleReconnect = () => {
    reconnectAttempts.value++;
    console.log(
      `🔄 Attempting to reconnect... (${reconnectAttempts.value}/${maxReconnectAttempts})`
    );

    reconnectTimer = setTimeout(() => {
      connect();
    }, reconnectDelay);
  };

  /**
   * Lắng nghe custom event từ SSE
   */
  const addEventListener = (eventName, callback) => {
    if (eventSource) {
      eventSource.addEventListener(eventName, (event) => {
        try {
          const parsedData = JSON.parse(event.data);
          callback(parsedData, event);
        } catch (e) {
          callback(event.data, event);
        }
      });
    } else {
      console.warn("⚠️ EventSource not initialized. Call connect() first.");
    }
  };

  const resetError = () => {
    error.value = null;
  };

  const reconnectManual = () => {
    reconnectAttempts.value = 0;
    connect();
  };

  // Auto connect nếu được bật
  if (autoConnect) {
    connect();
  }

  // Cleanup khi component unmount
  onUnmounted(() => {
    disconnect();
  });

  return {
    data,
    error,
    isConnected,
    reconnectAttempts,
    connect,
    disconnect,
    addEventListener,
    resetError,
    reconnect: reconnectManual,
  };
}
