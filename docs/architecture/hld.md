# VulnWatch AI - High-Level Design Document (HLD)

## Document Information

| Property | Value |
|----------|-------|
| **Project** | VulnWatch AI |
| **Document Type** | High-Level Design (HLD) |
| **Version** | 1.0 |
| **Date** |        |
| **Status** | Approved |

---

## Table of Contents

1. [System Overview](#1-system-overview)
2. [Architecture Diagram](#2-architecture-diagram)
3. [Component Breakdown](#3-component-breakdown)
4. [Technology Stack](#4-technology-stack)
5. [Sequence Diagrams](#5-sequence-diagrams)
6. [Data Flow](#6-data-flow)
7. [Deployment Architecture](#7-deployment-architecture)

---

## 1. System Overview
### For a high-level summary of the project, see the [Architecture Overview](overview.md).

---
## 2. Architecture Diagram
The following diagram illustrates the high-level orchestration between the .NET API, the Java processing engine, and the shared data layers


![System Architecture](../puml/system_component.png)


### 🛠️ How to Update Diagrams

To ensure the documentation stays in sync with the codebase, please follow these steps if you make architectural changes:

1.  **Requirement**: Install the **PlantUML Integration** plugin in your IDE (IntelliJ or VS Code).
2.  **Edit Source**: Open the source file located at `docs/puml/system_component.puml` (or the specific `.puml` file for the flow you are changing).
3.  **Export**:
    *   **In IntelliJ**: Use the **Save Diagram** icon in the PlantUML tool window.
    *   **In VS Code**: Use `Alt + D` to preview and then export the file.
4.  **Overwrite**: Save the updated version as a `.png` (e.g., `system_component.png`) in the `docs/puml/` directory. Ensure the filename matches the existing one exactly to update the link.
5.  **Commit**: Include both the `.puml` source and the `.png` export in your Pull Request.


> If you add a **new** diagram, simply follow the steps above and reference it in this file using:
> `![Diagram Name](../puml/your_new_file.png)`

