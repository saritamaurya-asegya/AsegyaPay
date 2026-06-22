import axios from 'axios';

const API_BASE_URL = process.env.NEXT_PUBLIC_API_URL || 'http://localhost:5000/api/v1';

export const apiClient = axios.create({
  baseURL: API_BASE_URL,
  headers: {
    'Content-Type': 'application/json',
  },
});

// Request interceptor to add auth token
apiClient.interceptors.request.use((config) => {
  const token = typeof window !== 'undefined' ? localStorage.getItem('access_token') : null;
  if (token) {
    config.headers.Authorization = `******;
  }
  return config;
});

// Response interceptor for error handling
apiClient.interceptors.response.use(
  (response) => response,
  (error) => {
    if (error.response?.status === 401) {
      // Handle token expiry
      if (typeof window !== 'undefined') {
        localStorage.removeItem('access_token');
        window.location.href = '/login';
      }
    }
    return Promise.reject(error);
  }
);

// Payment APIs
export const paymentApi = {
  create: (data: CreatePaymentParams) => apiClient.post('/payments', data),
  get: (id: string) => apiClient.get(`/payments/${id}`),
  list: (params?: ListPaymentsParams) => apiClient.get('/payments', { params }),
  capture: (id: string, amount?: number) => apiClient.post(`/payments/${id}/capture`, { amount }),
  refund: (id: string, amount: number, reason?: string) =>
    apiClient.post(`/payments/${id}/refund`, { amount, reason }),
};

// Merchant APIs
export const merchantApi = {
  getProfile: () => apiClient.get('/merchants/me'),
  updateProfile: (data: unknown) => apiClient.patch('/merchants/me', data),
  getApiKeys: () => apiClient.get('/merchants/me/api-keys'),
  createApiKey: (name: string) => apiClient.post('/merchants/me/api-keys', { name }),
  revokeApiKey: (keyId: string) => apiClient.delete(`/merchants/me/api-keys/${keyId}`),
};

// Settlement APIs
export const settlementApi = {
  list: (params?: PaginationParams) => apiClient.get('/settlements', { params }),
  get: (id: string) => apiClient.get(`/settlements/${id}`),
};

// Types
interface CreatePaymentParams {
  amount: number;
  currency: string;
  method: string;
  description?: string;
  order_id?: string;
  return_url?: string;
  callback_url?: string;
}

interface ListPaymentsParams {
  page?: number;
  page_size?: number;
  status?: string;
  method?: string;
  from?: string;
  to?: string;
}

interface PaginationParams {
  page?: number;
  page_size?: number;
}
