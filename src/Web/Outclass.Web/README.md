# Outclass Web Frontend

## Purpose
The **Outclass Web** project is the "Face" of the platform. It is the website that users interact with to manage their data, workflows, and settings.

## What it does (Plain English)
- **Visual Interface**: Provides the dashboard, forms, and pages you see in your browser.
- **Dynamic Rendering**: Because Outclass is "Metadata-Driven," this frontend is smart. It asks the Metadata Service what a "Student" looks like and then automatically builds a form with the right text boxes and buttons.
- **Tenant Awareness**: It handles logging in and remembers which organization you belong to so it can show you the right data.

## Technology Stack
- **Next.js 16**: A modern framework for building fast web applications.
- **TypeScript**: Ensures the code is reliable and has fewer bugs.
- **Tailwind CSS**: Used for styling the beautiful and responsive user interface.

## How it fits in
This is the only part of the system that the end-user actually sees. It talks to the **API Gateway**, which then forwards the requests to the various backend services. 

## Getting Started
1. Install dependencies: `npm install`
2. Run locally: `npm run dev`
3. Visit: `http://localhost:3000`
