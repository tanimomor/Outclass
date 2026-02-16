"use client";

import { Bell, Moon, Sun, Search } from "lucide-react";
import { useTheme } from "next-themes";
import { Button } from "@/components/ui/Button";
import { Input } from "@/components/ui/Input";

export function Header() {
    const { setTheme, theme } = useTheme();

    return (
        <header className="h-16 border-b bg-background/95 backdrop-blur supports-[backdrop-filter]:bg-background/60 fixed top-0 right-0 left-0 md:left-64 z-20 px-6 flex items-center justify-between">
            <div className="flex items-center w-full max-w-md">
                <div className="relative w-full">
                    <Search className="absolute left-2.5 top-2.5 h-4 w-4 text-muted-foreground" />
                    <Input
                        placeholder="Search..."
                        className="pl-9 bg-secondary/50 border-transparent focus:bg-background transition-colors"
                    />
                </div>
            </div>

            <div className="flex items-center gap-2">
                <Button variant="ghost" size="icon" className="text-muted-foreground">
                    <Bell className="h-4 w-4" />
                </Button>
                <Button
                    variant="ghost"
                    size="icon"
                    onClick={() => setTheme(theme === "dark" ? "light" : "dark")}
                >
                    <Sun className="h-4 w-4 rotate-0 scale-100 transition-all dark:-rotate-90 dark:scale-0" />
                    <Moon className="absolute h-4 w-4 rotate-90 scale-0 transition-all dark:rotate-0 dark:scale-100" />
                    <span className="sr-only">Toggle theme</span>
                </Button>
            </div>
        </header>
    );
}
