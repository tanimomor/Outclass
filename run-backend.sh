#!/bin/bash

# Outclass Backend Runner (No Docker)
# This script starts all microservices and the gateway locally using dotnet run.

# Colors for logging
GREEN='\033[0;32m'
BLUE='\033[0;34m'
RED='\033[0;31m'
NC='\033[0m' # No Color

echo -e "${BLUE}==================================================${NC}"
echo -e "${BLUE}   Outclass Platform - Local Backend Runner       ${NC}"
echo -e "${BLUE}==================================================${NC}"

# 1. Check for .NET SDK
if ! command -v dotnet &> /dev/null; then
    echo -e "${RED}Error: .NET SDK not found. Please install .NET 10.${NC}"
    exit 1
fi

# 2. Define projects
PROJECTS=(
    "src/Services/Identity/Outclass.Identity.API"
    "src/Services/Tenant/Outclass.Tenant.API"
    "src/Services/Metadata/Outclass.Metadata.API"
    "src/Services/Document/Outclass.Document.API"
    "src/Services/Workflow/Outclass.Workflow.API"
    "src/Services/Automation/Outclass.Automation.API"
    "src/Services/FileStorage/Outclass.FileStorage.API"
    "src/Gateway/Outclass.Gateway"
)

# 3. Cleanup function to stop all processes on exit
cleanup() {
    echo -e "\n${RED}Stopping all services...${NC}"
    # Kill all background jobs started by this script
    pkill -P $$
    echo -e "${GREEN}Done.${NC}"
    exit
}

trap cleanup SIGINT SIGTERM

# 4. Start Infrastructure Warnings
echo -e "${BLUE}Starting services...${NC}"
echo -e "Note: Ensure Postgres, Redis, and RabbitMQ are running locally on default ports."

# 5. Launch each project
for project in "${PROJECTS[@]}"; do
    service_name=$(basename "$project")
    echo -e "${GREEN}Launching ${service_name}...${NC}"
    
    # Run dotnet in the background and redirect output to a log file if needed, 
    # or just let them stream to console with a prefix
    dotnet run --project "$project" --no-launch-profile & 
    
    # Minimal sleep to prevent simultaneous DB migration race conditions
    sleep 2
done

echo -e "${BLUE}--------------------------------------------------${NC}"
echo -e "${GREEN}All services are starting up!${NC}"
echo -e "Gateway: http://localhost:5000"
echo -e "Press ${RED}Ctrl+C${NC} to stop all services."
echo -e "${BLUE}--------------------------------------------------${NC}"

# Keep the script alive so it can trap Ctrl+C
wait
