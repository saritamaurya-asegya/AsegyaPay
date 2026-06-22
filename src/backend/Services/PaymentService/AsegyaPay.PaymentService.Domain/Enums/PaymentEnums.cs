namespace AsegyaPay.PaymentService.Domain.Enums;

public enum PaymentStatus
{
    Pending = 0,
    Authorized = 1,
    Captured = 2,
    PartiallyRefunded = 3,
    Refunded = 4,
    Failed = 5,
    Cancelled = 6,
    Expired = 7
}

public enum PaymentMethod
{
    Card = 0,
    UPI = 1,
    NetBanking = 2,
    Wallet = 3,
    EMI = 4,
    BNPL = 5,
    ApplePay = 6,
    GooglePay = 7,
    QRCode = 8,
    TokenizedCard = 9
}

public enum Currency
{
    INR = 0,
    USD = 1,
    EUR = 2,
    GBP = 3,
    AED = 4,
    SGD = 5,
    AUD = 6,
    CAD = 7,
    JPY = 8,
    MYR = 9
}

public enum PaymentGateway
{
    Internal = 0,
    Razorpay = 1,
    Stripe = 2,
    PayU = 3,
    Adyen = 4,
    PayPal = 5
}

public enum RefundStatus
{
    Pending = 0,
    Processing = 1,
    Processed = 2,
    Failed = 3
}
