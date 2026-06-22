-- AsegyaPay Database Initialization Script
-- Creates separate databases for each microservice (database-per-service pattern)

CREATE DATABASE asegyapay_auth;
CREATE DATABASE asegyapay_payments;
CREATE DATABASE asegyapay_merchants;
CREATE DATABASE asegyapay_settlements;
CREATE DATABASE asegyapay_subscriptions;
CREATE DATABASE asegyapay_fraud;
CREATE DATABASE asegyapay_notifications;
CREATE DATABASE asegyapay_payouts;
CREATE DATABASE asegyapay_analytics;

-- Create application users with minimal privileges
CREATE USER auth_service WITH ENCRYPTED PASSWORD 'auth_svc_pwd';
CREATE USER payment_service WITH ENCRYPTED PASSWORD 'payment_svc_pwd';
CREATE USER merchant_service WITH ENCRYPTED PASSWORD 'merchant_svc_pwd';
CREATE USER settlement_service WITH ENCRYPTED PASSWORD 'settlement_svc_pwd';
CREATE USER subscription_service WITH ENCRYPTED PASSWORD 'subscription_svc_pwd';
CREATE USER fraud_service WITH ENCRYPTED PASSWORD 'fraud_svc_pwd';
CREATE USER notification_service WITH ENCRYPTED PASSWORD 'notification_svc_pwd';
CREATE USER payout_service WITH ENCRYPTED PASSWORD 'payout_svc_pwd';
CREATE USER analytics_service WITH ENCRYPTED PASSWORD 'analytics_svc_pwd';

GRANT ALL PRIVILEGES ON DATABASE asegyapay_auth TO auth_service;
GRANT ALL PRIVILEGES ON DATABASE asegyapay_payments TO payment_service;
GRANT ALL PRIVILEGES ON DATABASE asegyapay_merchants TO merchant_service;
GRANT ALL PRIVILEGES ON DATABASE asegyapay_settlements TO settlement_service;
GRANT ALL PRIVILEGES ON DATABASE asegyapay_subscriptions TO subscription_service;
GRANT ALL PRIVILEGES ON DATABASE asegyapay_fraud TO fraud_service;
GRANT ALL PRIVILEGES ON DATABASE asegyapay_notifications TO notification_service;
GRANT ALL PRIVILEGES ON DATABASE asegyapay_payouts TO payout_service;
GRANT ALL PRIVILEGES ON DATABASE asegyapay_analytics TO analytics_service;
