import { Sidebar } from "@/components/layout/Sidebar";
import { Header } from "@/components/layout/Header";

export default function DashboardLayout({
    children,
}: {
    children: React.ReactNode;
}) {
    return (
        <div className="min-h-screen bg-muted/40 font-sans">
            <Sidebar />
            <Header />
            <main className="pt-16 md:pl-64 min-h-screen transition-all duration-300">
                <div className="container mx-auto p-6 md:p-8 max-w-7xl animate-in fade-in slide-in-from-bottom-4 duration-500">
                    {children}
                </div>
            </main>
        </div>
    );
}
