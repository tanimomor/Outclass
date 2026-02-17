# API Gateway

## Purpose
The **API Gateway** is the "Front Door" or "Receptionist" of the platform. It is the single point of entry for all requests coming from the internet.

## What it does (Plain English)
- **Routing**: When a request comes in for `/api/auth`, the Gateway knows to send it to the Identity Service. When it's for `/api/documents`, it sends it to the Document Service.
- **Security Check**: It can check if a user is logged in before even letting the request through to the microservices.
- **Simplification**: Instead of the browser having to remember 7 different URLs for 7 services, it only has to remember one: `http://localhost:5000`.

## Key Concepts
- **Reverse Proxy**: A middleman that takes a request and "proxies" (forwards) it to the right destination.
- **Routes**: The map that tells the Gateway how to handle different URLs.

## How it fits in
The Gateway is the first thing a request hits. It keeps the internal architecture (the microservices) hidden and secure, providing a clean and simple interface for the frontend to talk to.
