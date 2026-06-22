/** @type {import('next').NextConfig} */
const nextConfig = {
  reactStrictMode: true,
  output: 'standalone',
  images: {
    domains: ['assets.asegyapay.com'],
  },
};

module.exports = nextConfig;
