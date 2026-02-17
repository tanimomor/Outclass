# Outclass Building Blocks

## Purpose
The **Building Blocks** are the "Shared Toolbox" of the entire platform. This is where we store common code that every service needs to use.

## What it does (Plain English)
- **Standardization**: It ensures that every service handles things like "Tenants" or "Events" in exactly the same way.
- **Efficiency**: Instead of writing the same code 7 times (once for each service), we write it once here and share it.
- **Foundation**: It provides the base classes (like `BaseEntity`) that all our data records are built on.

## Key Sub-modules
- **Contracts**: The shared "language" (interfaces) that services use to talk to each other.
- **Domain**: Core business logic and base classes for all our data models.
- **Infrastructure**: The plumbing code for things like Database connections, Redis caching, and RabbitMQ messaging.
- **Application**: Shared behaviors like automatic logging and validation.

## How it fits in
This is not a "service" that runs on its own. It is a set of libraries that are imported into every other service. If you want to change how the entire platform handles security or database connections, you usually change it here.
