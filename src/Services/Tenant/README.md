# Tenant Service

## Purpose
The **Tenant Service** is the "Landlord" of the platform. It manages different organizations (Tenants) that use the software.

## What it does (Plain English)
- **Organization Management**: Creates and manages separate "accounts" for different companies.
- **Data Isolation**: Ensures that Company A cannot see Company B's data. Everything is tagged with a `TenantId`.
- **Subscription Plans**: Keeps track of which organization is on which plan (e.g., Free vs. Enterprise).
- **Settings**: Stores global settings for a specific company (like their logo or brand colors).

## Key Concepts
- **Tenant**: An organization, company, or team using the platform.
- **Multi-tenancy**: The ability for many independent companies to use the same software while staying completely separated.

## How it fits in
When you log in, the system checks which "Tenant" you belong to. The Tenant Service tells the rest of the platform: *"This user belongs to Company X, so only show them Company X's data."*
