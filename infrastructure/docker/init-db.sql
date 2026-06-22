-- Database initialization script for Docker
-- Creates all required databases

CREATE DATABASE asegyapay_payments;
CREATE DATABASE asegyapay_merchants;
CREATE DATABASE asegyapay_auth;
CREATE DATABASE asegyapay_settlements;
CREATE DATABASE asegyapay_subscriptions;
CREATE DATABASE asegyapay_notifications;
CREATE DATABASE asegyapay_fraud;
CREATE DATABASE asegyapay_analytics;
CREATE DATABASE asegyapay_audit;

-- Grant permissions
GRANT ALL PRIVILEGES ON DATABASE asegyapay_payments TO asegyapay;
GRANT ALL PRIVILEGES ON DATABASE asegyapay_merchants TO asegyapay;
GRANT ALL PRIVILEGES ON DATABASE asegyapay_auth TO asegyapay;
GRANT ALL PRIVILEGES ON DATABASE asegyapay_settlements TO asegyapay;
GRANT ALL PRIVILEGES ON DATABASE asegyapay_subscriptions TO asegyapay;
GRANT ALL PRIVILEGES ON DATABASE asegyapay_notifications TO asegyapay;
GRANT ALL PRIVILEGES ON DATABASE asegyapay_fraud TO asegyapay;
GRANT ALL PRIVILEGES ON DATABASE asegyapay_analytics TO asegyapay;
GRANT ALL PRIVILEGES ON DATABASE asegyapay_audit TO asegyapay;
