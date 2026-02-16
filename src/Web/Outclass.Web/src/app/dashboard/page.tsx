import { Card, CardContent, CardHeader, CardTitle, CardDescription } from "@/components/ui/Card";
import { Button } from "@/components/ui/Button";
import { Users, FileText, Database, Activity } from "lucide-react";

export default function DashboardPage() {
    return (
        <div className="space-y-8">
            <div className="flex justify-between items-center">
                <div>
                    <h1 className="text-3xl font-bold tracking-tight bg-gradient-to-r from-foreground to-foreground/70 bg-clip-text text-transparent">Dashboard</h1>
                    <p className="text-muted-foreground mt-1">Overview of your platform activity.</p>
                </div>
                <Button>
                    Download Report
                </Button>
            </div>

            <div className="grid gap-4 md:grid-cols-2 lg:grid-cols-4">
                {[
                    { title: "Total Tenants", value: "12", desc: "+2 since last month", icon: Users },
                    { title: "Active Documents", value: "1,234", desc: "+180 today", icon: FileText },
                    { title: "Entity Definitions", value: "48", desc: "Across all tenants", icon: Database },
                    { title: "System Health", value: "99.9%", desc: "All systems operational", icon: Activity },
                ].map((stat, i) => {
                    const Icon = stat.icon;
                    return (
                        <Card key={i}>
                            <CardHeader className="flex flex-row items-center justify-between space-y-0 pb-2">
                                <CardTitle className="text-sm font-medium">{stat.title}</CardTitle>
                                <Icon className="h-4 w-4 text-muted-foreground" />
                            </CardHeader>
                            <CardContent>
                                <div className="text-2xl font-bold">{stat.value}</div>
                                <p className="text-xs text-muted-foreground">{stat.desc}</p>
                            </CardContent>
                        </Card>
                    )
                })}
            </div>

            <div className="grid gap-4 md:grid-cols-2 lg:grid-cols-7">
                <Card className="col-span-4">
                    <CardHeader>
                        <CardTitle>Overview</CardTitle>
                        <CardDescription>Document creation activity over the last 30 days.</CardDescription>
                    </CardHeader>
                    <CardContent className="pl-2">
                        <div className="h-[200px] flex items-center justify-center text-muted-foreground">
                            Chart Placeholder
                        </div>
                    </CardContent>
                </Card>
                <Card className="col-span-3">
                    <CardHeader>
                        <CardTitle>Recent Activity</CardTitle>
                        <CardDescription>Latest actions across the platform.</CardDescription>
                    </CardHeader>
                    <CardContent>
                        <div className="space-y-4">
                            {[1, 2, 3].map((_, i) => (
                                <div key={i} className="flex items-center gap-4">
                                    <div className="h-2 w-2 rounded-full bg-primary" />
                                    <div className="flex-1 space-y-1">
                                        <p className="text-sm font-medium leading-none">New tenant provisioned</p>
                                        <p className="text-xs text-muted-foreground">2 hours ago</p>
                                    </div>
                                </div>
                            ))}
                        </div>
                    </CardContent>
                </Card>
            </div>
        </div>
    );
}
