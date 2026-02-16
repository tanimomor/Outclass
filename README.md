# Outclass Platform

Outclass is a production-grade, metadata-driven, multi-tenant low-code platform built with .NET 8, Next.js, and Cloud-Native technologies.

## Architecture

- **Microservices**: Identity, Tenant, Metadata, Document, Workflow, Automation, FileStorage.
- **Frontend**: Next.js 14 (App Router) with TypeScript and Tailwind CSS.
- **Gateway**: YARP Reverse Proxy.
- **Infrastructure**: PostgreSQL, Redis, RabbitMQ.
- **Communication**: Event-driven architecture using RabbitMQ and MassTransit stylistic patterns (raw RabbitMQ client implemented).

## Prerequisites

- [Docker Desktop](https://www.docker.com/products/docker-desktop/) or Docker Engine + Compose
- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0) (for local development)
- [Node.js 20+](https://nodejs.org/) (for frontend)

## Getting Started

### 1. Start Infrastructure & Services

Run the entire platform using Docker Compose:

```bash
docker-compose up --build -d
```

This will start:
- Postgres (Port 5432)
- Redis (Port 6379)
- RabbitMQ (Port 5672, Management UI on 15672)
- All Microservices
- API Gateway (Port 5000)

**Note:** The databases passed to connection strings are automatically created by the `infra/init-db/01-create-dbs.sh` script mounted in the Postgres container.

### 2. Run Frontend

The frontend is located in `src/Web/Outclass.Web`.

```bash
cd src/Web/Outclass.Web
npm install
npm run dev
```

Access the frontend at `http://localhost:3000`.

### 3. Default Credentials

- **Admin User**: `admin@outclass.com`
- **Password**: `Admin123!`
- **System Tenant ID**: `11111111-1111-1111-1111-111111111111` (used for system admin)

### 4. API Documentation

You can access the individual service APIs directly if exposed, but all traffic should go through the Gateway at `http://localhost:5000`.

Endpoints:
- Identity: `http://localhost:5000/api/auth`
- Tenants: `http://localhost:5000/api/tenants`
- Metadata: `http://localhost:5000/api/entitydefinitions`
- Documents: `http://localhost:5000/api/documents`

## Development

### Building Backend

```bash
dotnet build
```

### Running Tests

```bash
dotnet test
```

## License

Proprietary - Outclass Platform.
