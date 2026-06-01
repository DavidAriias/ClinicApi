# Clinic API - .NET REST API

API REST para la administración de citas médicas construida con ASP.NET Core Web API y Entity Framework Core.

---

## Tecnologías utilizadas

- .NET 8 (Web API)
- ASP.NET Core
- Entity Framework Core
- SQL Server (Docker)
- Swagger / OpenAPI

---

## Arquitectura

El proyecto está organizado en capas (basicamente utilizando arquitectura limpia):

- **Domain** → Entidades y contratos
- **App** → Servicios y reglas de negocio
- **Infrastructure** → Persistencia (EF Core)
- **Presentation** → Controllers (API REST)

---

## Modelos

### Doctor
- Id
- Name
- Specialty
- IsActive
- CreatedAt

### Appointment
- Id
- DoctorId
- PatientName
- AppointmentDate
- DurationMinutes
- Status (Scheduled, Completed, Cancelled)
- Notes
- CreatedAt

---

## Requisitos

- Docker Desktop instalado
- .NET 8 SDK (solo si corres fuera de Docker)
- Git

---

##  Cómo correr el proyecto con Docker

Desde la raíz del proyecto:

```bash
docker compose down -v
docker compose up --build
```

##  Acceso al swagger 

``` 
http://localhost:5001/swagger
```