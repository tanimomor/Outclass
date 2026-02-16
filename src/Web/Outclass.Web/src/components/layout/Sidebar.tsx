"use client";

import Link from "next/link";
import { usePathname } from "next/navigation";
import { LayoutDashboard, Users, Database, FileText, Settings, Workflow, Zap, FolderOpen } from "lucide-react";
import { cn } from "@/lib/utils";

const links = [
    { href: "/dashboard", label: "Dashboard", icon: LayoutDashboard },
    { href: "/tenants", label: "Tenants", icon: Users },
    { href: "/metadata", label: "Metadata", icon: Database },
    { href: "/documents", label: "Documents", icon: FileText },
    { href: "/workflows", label: "Workflows", icon: Workflow },
    { href: "/automation", label: "Automation", icon: Zap },
    { href: "/files", label: "Files", icon: FolderOpen },
    { href: "/settings", label: "Settings", icon: Settings },
];

export function Sidebar() {
    const pathname = usePathname();

    return (
        <aside className="w-64 border-r bg-card h-screen fixed left-0 top-0 flex flex-col z-30 hidden md:flex">
            <div className="h-16 flex items-center px-6 border-b">
                <div className="text-xl font-bold bg-gradient-to-r from-primary to-purple-600 bg-clip-text text-transparent">
                    Outclass
                </div>
            </div>
            <nav className="flex-1 p-4 space-y-1 overflow-y-auto">
                {links.map((link) => {
                    const Icon = link.icon;
                    const isActive = pathname.startsWith(link.href);
                    return (
                        <Link
                            key={link.href}
                            href={link.href}
                            className={cn(
                                "flex items-center gap-3 px-3 py-2 rounded-md transition-all duration-200 group text-sm font-medium",
                                isActive
                                    ? "bg-primary/10 text-primary hover:bg-primary/15"
                                    : "text-muted-foreground hover:bg-accent hover:text-accent-foreground"
                            )}
                        >
                            <Icon className={cn("h-4 w-4 transition-colors", isActive ? "text-primary" : "text-muted-foreground group-hover:text-foreground")} />
                            {link.label}
                        </Link>
                    );
                })}
            </nav>
            <div className="p-4 border-t">
                <div className="flex items-center gap-3">
                    <div className="h-9 w-9 rounded-full bg-secondary flex items-center justify-center text-xs font-medium">
                        JD
                    </div>
                    <div className="flex flex-col">
                        <span className="text-sm font-medium">John Doe</span>
                        <span className="text-xs text-muted-foreground">Admin</span>
                    </div>
                </div>
            </div>
        </aside>
    );
}
