# Automation Service

## Purpose
The **Automation Service** is the "Brain" or "Robot" of the platform. It automatically performs tasks when certain things happen in the system.

## What it does (Plain English)
- **Event Listener**: It constantly listens to what's happening in other services (like *"A new document was created"* or *"A user logged in"*).
- **Rule Engine**: It checks a list of "If-Then" rules. (e.g., *"IF a new Document is created, THEN send an email notification"*).
- **Background Worker**: It performs the work in the background so the user doesn't have to wait for the page to load.

## Key Concepts
- **Trigger**: The event that starts the automation (e.g., "Invoice Saved").
- **Action**: The work that the robot does (e.g., "Generate PDF").
- **Rule**: The logic that connects a Trigger to an Action.

## Automation & Documents
One of the most powerful features of this service is its connection to the **Document Service**.
- Whenever a **Document** is created, updated, or deleted, this service is notified.
- It can then automatically update other documents, send messages, or trigger external APIs.
- Think of it as a "Hook" system that makes your data alive and reactive.

## How it fits in
This service makes the platform smart. Instead of you manually doing repetitive work, you set up a rule once, and the Automation Service handles it every time.
