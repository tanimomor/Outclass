# FileStorage Service

## Purpose
The **FileStorage Service** is the "Digital Filing Cabinet." It handles all the "heavy" files that don't fit well inside a standard database.

## What it does (Plain English)
- **File Uploads**: Handles the uploading of images, PDFs, spreadsheets, and more.
- **Organization**: Keeps track of which file belongs to which record (e.g., "This PDF is the attachment for Invoice #123").
- **Security**: Ensures that only authorized users can download or view specific files.
- **Storage Abstraction**: It can save files locally on a disk or in the cloud (like Amazon S3), but it hides that complexity from the rest of the application.

## Key Concepts
- **Blob**: A fancy word for a file (Binary Large Object).
- **Metadata**: Information about the file (like its size, name, and type).

## How it fits in
While the Document Service stores text and numbers, the FileStorage Service stores the actual files. It provides a simple URL or ID that other services can use to "point" to a file.
