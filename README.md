﻿# Project Guide for Code Review

## Assumptions
This project has been designed to simulate a production environment where the `AssetPortfolio` class already exists and is actively used across multiple projects.
The  `Program.cs` in the former main application that is used for test, can been seen as a client that has been already used. All Refactory in the AssetPortfolio MUST be compatible with.

### Key Design Considerations
- All modifications have been implemented in a way that ensures backward compatibility, meaning existing applications that consume this version of PMS will not be affected.
- Some refactoring was necessary to maintain clean architecture and improve extensibility.

### Multi-Asset & Multi-Currency Portfolio
- A new class, `MultAssetPortfolio`, has been introduced, inheriting from `AssetPortfolio`. This allows the system to support multiple assets and currencies efficiently.

### Unit Testing
- A new unit test project has been added for development and validation purposes.


### Running Tests
- Open the **Test Explorer** in Visual Studio
- Run all unit tests to ensure the solution is stable
- Alternatively, use the command:
  ```sh
  dotnet test
  ```

## Author
**Erik Pimentel de Morais**  
✉️ Email: [erik.pimentel.morais@gmail.com](mailto:erik.pimentel.morais@gmail.com)  
📱 Mobile: **+44 7466 538584**

