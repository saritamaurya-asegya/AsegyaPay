export interface Payment {
  id: string;
  orderId: string;
  merchantId: string;
  customerId?: string;
  amount: number;
  currency: string;
  status: PaymentStatus;
  method: PaymentMethod;
  description: string;
  gatewayTransactionId?: string;
  fraudScore: number;
  createdAt: string;
  updatedAt: string;
  capturedAt?: string;
  refunds: Refund[];
}

export type PaymentStatus =
  | "Pending"
  | "Authorized"
  | "Captured"
  | "PartiallyRefunded"
  | "Refunded"
  | "Failed"
  | "Cancelled"
  | "Expired";

export type PaymentMethod =
  | "Card"
  | "UPI"
  | "NetBanking"
  | "Wallet"
  | "EMI"
  | "BNPL"
  | "ApplePay"
  | "GooglePay"
  | "QRCode"
  | "TokenizedCard";

export interface Refund {
  id: string;
  amount: number;
  currency: string;
  reason: string;
  status: "Pending" | "Processing" | "Processed" | "Failed";
  createdAt: string;
}

export interface Merchant {
  id: string;
  businessName: string;
  businessEmail: string;
  businessType: string;
  website?: string;
  status: "Pending" | "Active" | "Suspended" | "Terminated";
  kycStatus: "NotSubmitted" | "Submitted" | "UnderReview" | "Verified" | "Rejected";
  mdrRate: number;
  settlementCycleInDays: number;
  country: string;
  createdAt: string;
}

export interface Settlement {
  id: string;
  merchantId: string;
  amount: number;
  currency: string;
  status: "Pending" | "Processing" | "Settled" | "Failed";
  settlementDate: string;
  utrNumber?: string;
  transactionCount: number;
  createdAt: string;
}

export interface ApiKey {
  id: string;
  name: string;
  prefix: string;
  type: "test" | "live";
  isRevoked: boolean;
  createdAt: string;
  lastUsedAt?: string;
}

export interface PaginatedResponse<T> {
  items: T[];
  totalCount: number;
  page: number;
  pageSize: number;
  totalPages: number;
}
