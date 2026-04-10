import Sidebar from '@/components/Sidebar';
import { ThemeColors } from '@/components/ThemeColors';

export default function DashboardLayout({ children }) {
  return (
    <main className="flex min-h-screen">
      <aside className="w-[250px] shrink-0 h-screen sticky top-0 hidden md:block">
        <Sidebar />
      </aside>

      <section 
        style={{ backgroundColor: ThemeColors.bgBlue }} 
        className="min-w-0 flex-1 min-h-screen text-zinc-300 font-sans"
      >
        {children}
      </section>
    </main>
  );
}