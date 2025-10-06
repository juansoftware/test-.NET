# Stargate API - Astronaut Management System

A full-stack application for managing astronauts and their duties, built with ASP.NET Core Web API and Angular frontend.

##  Features

- **Person Management**: Create, read, and update astronaut personnel
- **Duty Assignment**: Assign and track astronaut duties with ranks and dates
- **Career Tracking**: Monitor career start/end dates and current assignments
- **Audit Logging**: Complete audit trail of all operations
- **RESTful API**: Clean, well-documented API endpoints

##  Architecture

### Backend (ASP.NET Core)
- **Clean Architecture** with separation of concerns
- **CQRS Pattern** using MediatR for command/query separation
- **Entity Framework Core** + **Dapper** for data access
- **Audit Service** for comprehensive logging
- **Pre-processors** for validation and business rules

### Frontend (Angular)
- **Standalone Components** (no NgModules)
- **Reactive Forms** for data validation
- **Service-based** architecture for API communication
- **TypeScript** for type safety

## 📋 Prerequisites

- .NET 8.0 SDK
- Node.js 18+ and npm
- SQLite (included)

## 🛠️ Setup & Installation

### Backend Setup
```bash
cd api
dotnet restore
dotnet build
dotnet run
```

The API will be available at `https://localhost:7000`

### Frontend Setup
```bash
cd stargate-frontend
npm install
ng serve
```

The frontend will be available at `http://localhost:4200`

##  API Endpoints

### Person Management
- `GET /Person` - Get all people
- `GET /Person/{name}` - Get person by name
- `POST /Person` - Create new person
- `PUT /Person/{name}` - Update person name

### Astronaut Duty Management
- `GET /AstronautDuty/{name}` - Get astronaut duties by person name
- `POST /AstronautDuty` - Create new astronaut duty

##  Database Schema

### Core Entities
- **Person**: Basic astronaut information
- **AstronautDetail**: Current rank, duty title, career dates
- **AstronautDuty**: Historical duty assignments
- **AuditLog**: System audit trail

### Key Business Rules
- Each person can have only one active duty at a time
- Career end date is automatically set when retiring
- Previous duty end date is set when assigning new duty
- All operations are audited for compliance

##  Configuration

### Backend Configuration
- Database connection strings in `appsettings.json`
- CORS policy configured for frontend communication
- Logging configured for development and production

### Frontend Configuration
- API base URL in `environment.ts`
- Service endpoints configured for backend communication

##  Testing

### Backend Tests
```bash
cd api/StargateAPI.Tests
dotnet test
```

### Frontend Tests
```bash
cd stargate-frontend
ng test
```

##  Development Notes

### Code Structure
- **Controllers**: Handle HTTP requests and responses
- **Commands/Queries**: Define business operations
- **Handlers**: Implement business logic
- **Pre-processors**: Handle validation
- **Services**: Cross-cutting concerns (audit, logging)

### Security Considerations
- Parameterized queries prevent SQL injection
- Input validation on all endpoints
- Comprehensive error handling
- Audit logging for compliance

##  Deployment

### Backend Deployment
1. Build the application: `dotnet publish -c Release`
2. Deploy to your preferred hosting platform
3. Configure connection strings and environment variables

### Frontend Deployment
1. Build for production: `ng build --configuration production`
2. Deploy the `dist/` folder to your web server
3. Configure API endpoints for production environment

##  Performance Considerations

- **Dapper** used for high-performance data access
- **Entity Framework** for complex queries and relationships
- **Database indexes** on frequently queried columns
- **Async/await** pattern throughout the application

##  Monitoring & Logging

- **Structured logging** with different log levels
- **Audit trail** for all business operations
- **Error tracking** with detailed exception information
- **Performance metrics** for database operations

##  Contributing

1. Fork the repository
2. Create a feature branch
3. Make your changes
4. Add tests for new functionality
5. Submit a pull request

## 📄 License

This project is part of a technical exercise and is for demonstration purposes.

---
