# Metadata Service

## Purpose
The **Metadata Service** is the "Architect" of the platform. It defines the **blueprints** for all the data in the system.

## What it does (Plain English)
- **Schema Builder**: Instead of writing code to create a "Student" table, you tell this service: *"I want a Student object with a name (text) and an age (number)."*
- **Dynamic Fields**: Allows admins to add new fields to any entity (like "Phone Number") instantly without needing a developer.
- **UI Logic**: Tells the frontend how to draw forms based on these blueprints.

## Key Concepts
- **Entity Definition**: A blueprint for a type of data (e.g., "Invoice," "Lead," "Project").
- **Field Definition**: A specific piece of information inside an entity (e.g., "Due Date," "Price").

## How it fits in
This is the heart of the "Low-Code" part of Outclass. It doesn't store the actual data; it stores the **structure**. When you want to see a list of Students, the system asks this service: *"What does a Student look like?"* before fetching the actual records.
