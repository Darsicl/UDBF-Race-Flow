# UDBF Race Flow 🛶

[![License: MIT](https://img.shields.io/badge/License-MIT-yellow.svg)](https://opensource.org/licenses/MIT)

**UDBF Race Flow** is a specialized race management system designed for dragon boat competitions. Built to adhere to the international standards, it ensures precise lane seeding, automated timing, and official race protocol generation.

## 🌟 Key Features

- 🏁 **Race Management:** Automated generation of brackets (Heats, Repechages, Semi-finals, Finals).
- ⏱️ **Timing Integration:** Real-time result processing and synchronization with timing hardware.
- 👥 **Roster Management:** Team registration, category tracking (Open, Women, Mixed), and crew composition.
- 📊 **Official Protocols:** Exporting race results to PDF/Excel according to international standards.

## 🏗 Architecture & Tech Stack

The project is built using **Clean Architecture** principles to keep the business logic independent and testable.

- **Backend:** .NET 10 (ASP.NET Core Web API)
- **Logic:** Clean Architecture.
- **Persistence:** PostgreSQL + Entity Framework Core.
- **CI/CD:** GitHub Actions for automated builds, testing, and linting.
- **Frontend:** React + TypeScript (located in a separate repository).
