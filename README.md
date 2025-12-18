# Cundi.XAF

This solution provides a collection of extended modules for the DevExpress eXpressApp Framework (XAF), aiming to enhance system capabilities and development efficiency.

## Modules

Core functionalities are located in the `Modules` directory:

*   **Cundi.XAF.Triggers**: Core Triggers module, providing trigger-related logic and webhook execution for CRUD operations.
*   **Cundi.XAF.Triggers.Api**: API integration module for Triggers, supporting trigger operations in Web API environments.
*   **Cundi.XAF.Metadata**: Module for scanning and recording system metadata (Types and Properties).
*   **Cundi.XAF.Metadata.Api**: REST API extensions for the Metadata module.
*   **Cundi.XAF.FullTextSearch**: Global full-text search module, enabling search across all business objects from a single entry point.

## Samples

Implementation examples for each platform are located in the `Samples` directory:

*   **Sample.Win**: Windows Forms application example.
*   **Sample.Blazor.Server**: Blazor Server application example.
*   **Sample.WebApi**: Web API application example.

## Getting Started

1.  Open the `Cundi.XAF.sln` solution file with Visual Studio.
2.  Restore NuGet packages.
3.  Set the example project you want to run (e.g., `Sample.Blazor.Server`) as the startup project.
4.  Build and run the solution.

## Module Documentation

- [Cundi.XAF.Triggers](Modules/Cundi.XAF.Triggers/README.md)
- [Cundi.XAF.Metadata](Modules/Cundi.XAF.Metadata/README.md)
- [Cundi.XAF.FullTextSearch](Modules/Cundi.XAF.FullTextSearch/README.md)
