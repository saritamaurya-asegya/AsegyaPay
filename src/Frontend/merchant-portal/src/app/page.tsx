import { DashboardOverview } from '@/components/dashboard/DashboardOverview';

export default function HomePage() {
  return (
    <main className="flex min-h-screen">
      {/* Sidebar */}
      <aside className="hidden w-64 border-r border-border bg-card lg:block">
        <div className="flex h-16 items-center gap-2 border-b border-border px-6">
          <div className="h-8 w-8 rounded-lg bg-primary" />
          <span className="text-lg font-bold">AsegyaPay</span>
        </div>
        <nav className="space-y-1 p-4">
          <NavItem href="/" label="Dashboard" active />
          <NavItem href="/payments" label="Payments" />
          <NavItem href="/settlements" label="Settlements" />
          <NavItem href="/refunds" label="Refunds" />
          <NavItem href="/subscriptions" label="Subscriptions" />
          <NavItem href="/payouts" label="Payouts" />
          <NavItem href="/analytics" label="Analytics" />
          <NavItem href="/settings" label="Settings" />
        </nav>
      </aside>

      {/* Main Content */}
      <div className="flex-1">
        <header className="flex h-16 items-center justify-between border-b border-border px-6">
          <h1 className="text-xl font-semibold">Dashboard</h1>
          <div className="flex items-center gap-4">
            <span className="text-sm text-muted-foreground">Test Mode</span>
            <div className="h-8 w-8 rounded-full bg-muted" />
          </div>
        </header>
        <div className="p-6">
          <DashboardOverview />
        </div>
      </div>
    </main>
  );
}

function NavItem({ href, label, active = false }: { href: string; label: string; active?: boolean }) {
  return (
    <a
      href={href}
      className={`block rounded-lg px-3 py-2 text-sm font-medium transition-colors ${
        active
          ? 'bg-primary/10 text-primary'
          : 'text-muted-foreground hover:bg-muted hover:text-foreground'
      }`}
    >
      {label}
    </a>
  );
}
