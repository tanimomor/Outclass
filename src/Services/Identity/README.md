# Identity Service

## Purpose
The **Identity Service** is the "Security Guard" of the platform. Its only job is to manage **who** can access the system and **what** they are allowed to do.

## What it does (Plain English)
- **Login & Registration**: Handles user accounts and passwords.
- **Security Keys (JWT)**: Issues digital "badges" (tokens) that users show to other services to prove who they are.
- **Permissions & Roles**: Remembers if a user is an "Admin" or a "Regular User."
- **Authentication**: Checks if a password is correct.

## Key Concepts
- **User**: A person who can log in.
- **Role**: A group of permissions (e.g., "Editor," "Viewer").
- **Token**: A temporary digital key that expires for security.

## How it fits in
Every time a user visits the platform, the frontend asks this service: *"Is this person who they say they are?"* If the answer is yes, this service provides a key that opens the doors to all other services.
