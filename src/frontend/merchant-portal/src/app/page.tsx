import Link from "next/link";

export default function HomePage() {
  return (
    <main className="flex min-h-screen flex-col items-center justify-center p-8">
      <div className="text-center animate-fade-in">
        <div className="mb-6 flex items-center justify-center gap-2">
          <div className="h-10 w-10 rounded-xl gradient-brand flex items-center justify-center">
            <span className="text-white font-bold text-lg">A</span>
          </div>
          <h1 className="text-3xl font-bold">AsegyaPay</h1>
        </div>
        <p className="text-gray-500 dark:text-gray-400 text-lg mb-8">
          Enterprise Payment Gateway Platform
        </p>
        <div className="flex gap-4 justify-center">
          <Link
            href="/dashboard"
            className="px-6 py-3 rounded-lg gradient-brand text-white font-medium hover:opacity-90 transition"
          >
            Go to Dashboard
          </Link>
          <Link
            href="/auth/login"
            className="px-6 py-3 rounded-lg border border-gray-200 dark:border-gray-700 font-medium hover:bg-gray-100 dark:hover:bg-gray-800 transition"
          >
            Sign In
          </Link>
        </div>
      </div>
    </main>
  );
}
