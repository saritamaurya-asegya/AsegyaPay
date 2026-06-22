import axios from "axios";

const API_BASE_URL = process.env.NEXT_PUBLIC_API_URL || "http://localhost:5000";

export const apiClient = axios.create({
  baseURL: `${API_BASE_URL}/api/v1`,
  headers: {
    "Content-Type": "application/json",
  },
});

// Request interceptor — attach JWT token
apiClient.interceptors.request.use((config) => {
  if (typeof window !== "undefined") {
    const token = localStorage.getItem("access_token");
    if (token) {
      config.headers.Authorization = `******;
    }
  }
  return config;
});

// Response interceptor — handle 401
apiClient.interceptors.response.use(
  (response) => response,
  async (error) => {
    const originalRequest = error.config;
    if (error.response?.status === 401 && !originalRequest._retry) {
      originalRequest._retry = true;
      try {
        const refreshToken = localStorage.getItem("refresh_token");
        const res = await axios.post(`${API_BASE_URL}/api/v1/auth/refresh`, { refreshToken });
        const { accessToken } = res.data;
        localStorage.setItem("access_token", accessToken);
        originalRequest.headers.Authorization = `******;
        return apiClient(originalRequest);
      } catch {
        localStorage.removeItem("access_token");
        localStorage.removeItem("refresh_token");
        window.location.href = "/auth/login";
      }
    }
    return Promise.reject(error);
  }
);

// Payment API
export const paymentApi = {
  create: (data: CreatePaymentRequest) =>
    apiClient.post("/payments", data).then((r) => r.data),
  get: (id: string) =>
    apiClient.get(`/payments/${id}`).then((r) => r.data),
  list: (params?: { page?: number; pageSize?: number; status?: string }) =>
    apiClient.get("/payments", { params }).then((r) => r.data),
  capture: (id: string, amount?: number) =>
    apiClient.post(`/payments/${id}/capture`, { amount }).then((r) => r.data),
  refund: (id: string, amount: number, reason: string) =>
    apiClient.post(`/payments/${id}/refund`, { amount, reason }).then((r) => r.data),
};

// Merchant API
export const merchantApi = {
  getProfile: () => apiClient.get("/merchants/me").then((r) => r.data),
  updateProfile: (data: UpdateMerchantRequest) =>
    apiClient.patch("/merchants/me", data).then((r) => r.data),
  generateApiKey: (name: string, type: "test" | "live") =>
    apiClient.post("/merchants/api-keys", { name, type }).then((r) => r.data),
  listApiKeys: () => apiClient.get("/merchants/api-keys").then((r) => r.data),
  revokeApiKey: (id: string) =>
    apiClient.delete(`/merchants/api-keys/${id}`).then((r) => r.data),
};

// Settlement API
export const settlementApi = {
  list: (params?: { page?: number; pageSize?: number }) =>
    apiClient.get("/settlements", { params }).then((r) => r.data),
  get: (id: string) =>
    apiClient.get(`/settlements/${id}`).then((r) => r.data),
};

// Analytics API
export const analyticsApi = {
  getSummary: (period: string) =>
    apiClient.get("/analytics/summary", { params: { period } }).then((r) => r.data),
  getRevenueByDay: (from: string, to: string) =>
    apiClient.get("/analytics/revenue", { params: { from, to } }).then((r) => r.data),
};

// Type definitions
interface CreatePaymentRequest {
  orderId: string;
  amount: number;
  currency: string;
  method: string;
  description: string;
  customerId?: string;
  callbackUrl?: string;
  redirectUrl?: string;
  metadata?: Record<string, string>;
}

interface UpdateMerchantRequest {
  businessName?: string;
  website?: string;
  description?: string;
}
