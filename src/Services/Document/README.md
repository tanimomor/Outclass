# Document Service

## Purpose
The **Document Service** is the "Library" or "Warehouse" of the platform. It stores the **actual data** that users create.

## What it does (Plain English)
- **Data Storage**: Stores the records created by users (the actual Student names, the actual Invoices, etc.).
- **Flexible Records**: Uses a technology called "JSONB" which allows it to store any kind of data specified by the blueprints in the Metadata Service.
- **Search & Retrieval**: Allows you to find, edit, and delete your records.

## Key Concepts
- **Document**: A single record of data (e.g., one specific Invoice).
- **JSONB**: A flexible way to store data that can change its shape without breaking the database.

## How it fits in
If the Metadata Service is the *blueprint*, the Document Service is the *actual building*. When you save a form on the website, the data is sent here to be tucked away safely in the database.
