# Workflow Service

## Purpose
The **Workflow Service** is the "Traffic Controller" or "Process Manager." It ensures that data moves through the correct steps in a business process.

## What it does (Plain English)
- **State Tracking**: Remembers what "stage" a document is in (e.g., Is this invoice "Pending," "Paid," or "Overdue"?).
- **Rules of Movement**: Enforces rules like *"You cannot mark an invoice as Paid until it has been Approved."*
- **History Logging**: Keeps a record of who moved a document from one stage to another and when.

## Key Concepts
- **State**: A specific stage in a process (e.g., "Draft").
- **Transition**: The act of moving from one stage to another (e.g., "Submit for Review").
- **Workflow Instance**: One specific document currently moving through a process.

## How it fits in
This service adds "Logic" to your data. It turns a static document into a living business process. It coordinates with the Metadata and Document services to make sure everyone follows the rules of the company.
