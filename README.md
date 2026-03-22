# ClinicAI –  Appointment Managment System

## Overview

ClinicAI is a full-stack application designed to manage doctor availability efficiently.
It allows users to create, update, delete, and filter doctor schedules with a clean UI and responsive experience.

The system is built using **Angular (frontend)** and **.NET (backend)** following modern architecture practices like **CQRS with MediatR**.

---

## Features

### Doctor Availability

* Create doctor availability slots
* Edit existing schedules
* Delete availability records
* Prevent invalid time ranges (start < end)

### Filtering & Search

* Filter by doctor
* Filter by date range (From / To)
* View past and upcoming availability

### UI/UX

* Bootstrap-based responsive UI
* Toast notifications (top-right)
* Confirmation modal for delete actions
* Clean card-based layout

### Data Handling

* Real-time UI updates after create/update/delete
* Proper date and time formatting
* Backend validation support

---

## Tech Stack

### Frontend

* Angular 17+
* Bootstrap 5
* TypeScript
* RxJS

### Backend

* .NET 8 Web API
* MediatR (CQRS pattern)
* Entity Framework Core
* SQL Server

---

## Project Structure

### Frontend (Angular)

```
src/
 ├── app/
 │   ├── core/
 │   │   ├── services/
 │   ├── shared/
 │   │   ├── components/
 │   │   │   ├── toast/
 │   │   │   ├── confirm/
 │   ├── features/
 │   │   ├── doctor-availability/
```

### Backend (.NET)

```
Application/
 ├── Features/
 │   ├── Availability/
 │   │   ├── Commands/
 │   │   ├── Queries/

Domain/
Infrastructure/
API/
```

---

## API Endpoints

### Availability

* `GET /api/availability` → Get all / filtered records
* `POST /api/availability` → Create availability
* `PUT /api/availability/{id}` → Update availability
* `DELETE /api/availability/{id}` → Delete availability

---

## Setup Instructions

### Backend (.NET)

```bash
dotnet restore
dotnet build
dotnet run
```

### Frontend (Angular)

```bash
npm install
ng serve
```

App runs at:

```
http://localhost:4200
```

---

## Usage

1. Select a doctor
2. Choose date and time range
3. Save availability
4. Use filters to view records
5. Edit or delete records as needed

---

## Key Implementation Notes

* Uses **CQRS pattern** with MediatR for clean separation
* Avoids returning full entities in DTOs (optimized API responses)
* Handles time formatting issues between Angular and .NET
* Implements reusable UI components (Toast & Confirm Modal)

---

## Future Enhancements

* Calendar-based scheduling UI
* Overlapping availability validation
* Pagination & server-side filtering
* Role-based access control
* Dashboard analytics

---

## Author

Developed as part of a client Requirement

---

## License

Bhargav Patel 
