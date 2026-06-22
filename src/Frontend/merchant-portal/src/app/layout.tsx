import type { Metadata } from 'next';
import './globals.css';

export const metadata: Metadata = {
  title: 'AsegyaPay - Merchant Dashboard',
  description: 'Manage your payments, settlements, and business with AsegyaPay',
};

export default function RootLayout({
  children,
}: {
  children: React.ReactNode;
}) {
  return (
    <html lang="en" className="dark">
      <body className="min-h-screen bg-background text-foreground antialiased">
        {children}
      </body>
    </html>
  );
}
