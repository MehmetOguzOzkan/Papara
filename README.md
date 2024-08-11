
# Papara E-Commerce Web API

This project is an advanced .NET 8 Web API for an e-commerce platform. It is designed with a clean, modular architecture to ensure scalability, maintainability, and performance. The system integrates various modern technologies like Hangfire, RabbitMQ, and Docker to support robust operations, including background processing, messaging, and containerization.

## Table of Contents
- [Architecture](#architecture)
- [Technologies Used](#technologies-used)
- [Core Features](#core-features)
- [Modules and Components](#modules-and-components)
- [Data Flow](#data-flow)
- [Business Logic](#business-logic)
- [Database Management](#database-management)
- [Background Jobs](#background-jobs)
- [Messaging](#messaging)
- [Deployment](#deployment)
- [API Endpoints](#api-endpoints)

## Architecture

The solution is structured in three primary layers:
- **Representation Layer**: Manages the API endpoints and HTTP requests.
- **Business Layer**: Implements the core business logic using the CQRS pattern.
- **Data Layer**: Handles database interactions through UnitOfWork and GenericRepository patterns.

![Architecture](https://github.com/MehmetOguzOzkan/Papara/blob/master/Images/Architecture.png)

## Technologies Used

- **ASP.NET Core 8**: For building the Web API.
- **MediatR**: To implement the CQRS pattern.
- **AutoMapper**: For object mapping between DTOs and entities.
- **FluentValidation**: For input validation in the business layer.
- **Entity Framework Core**: For ORM and database management.
- **Hangfire**: To manage background jobs and tasks.
- **RabbitMQ**: For messaging and communication between microservices.
- **MSSQL & PostgreSQL**: Dual database setup for relational data storage.
- **Docker**: Containerization of the application and its dependencies.
- **SMTP**: For sending emails within the system.

## Core Features

- **User Management**: Securely manage user data with ASP.NET Core Identity.
- **Product Management**: CRUD operations for products with category assignments.
- **Order Processing**: Handle order placements, including payment processing.
- **Coupon Management**: Apply discount coupons during checkout.
- **Background Jobs**: Schedule and execute tasks like sending emails using Hangfire.
- **Messaging**: Use RabbitMQ for asynchronous communication between services.
- **Multi-Database Support**: Seamlessly switch between MSSQL and PostgreSQL.

## Modules and Components

### 1. Representation Layer
Handles incoming API requests and returns the appropriate responses. This layer uses controllers to define the endpoints.

### 2. Business Layer
Implements the business rules, logic, and data processing. It uses CQRS for separating the write and read operations, ensuring clear and maintainable code.

### 3. Data Layer
Manages data persistence using the UnitOfWork and GenericRepository patterns. This layer interacts with the databases via Entity Framework Core.

## Data Flow

```plaintext
[Client] --> [API Gateway] --> [Controllers] --> [Business Layer]
                                            \--> [Data Layer] --> [Database]
```

- **Request Handling**: Client requests are received by controllers, which route them to the appropriate business logic.
- **Business Logic Execution**: Commands and queries are processed, validated, and mapped to entities.
- **Data Persistence**: The Data Layer manages CRUD operations and interacts with the databases.

## Business Logic

The business logic is organized using the CQRS pattern. This structure allows clear separation of read (query) and write (command) operations, making the system easier to maintain and extend.

### Example Command Flow
1. **Command Initiation**: A command is initiated from the API controller.
2. **Validation**: The command data is validated using FluentValidation.
3. **Mapping**: Valid data is mapped to an entity using AutoMapper.
4. **Persistence**: The entity is saved in the database via the UnitOfWork.
5. **Response**: A response is sent back to the client.

### Example Query Flow
1. **Query Initiation**: A query is initiated from the API controller.
2. **Data Retrieval**: Data is fetched from the database using the GenericRepository.
3. **Mapping**: Retrieved data is mapped to a DTO.
4. **Response**: The DTO is returned to the client.

## Database Management

- **Entity Framework Core**: Used for database interactions. It supports migrations, seeding, and complex querying.
- **Multi-Database Support**: The application can switch between MSSQL and PostgreSQL based on configuration settings.
- **Database Diagram**:

![Database Diagram](https://github.com/MehmetOguzOzkan/Papara/blob/master/Images/DatabaseDiagram.png)

## API Endpoints

### Authorization

- **POST /api/Authorization/Login**: Authenticates the user based on the provided credentials.
- **POST /api/Authorization/Logout**: Logs out the authenticated user.
- **POST /api/Authorization/Register**: Registers a new user with the provided details.
- **POST /api/Authorization/ChangePassword**: Changes the password for the authenticated user.

![AuthorizationEndpoints](https://github.com/MehmetOguzOzkan/Papara/blob/master/Images/AuthorizationEndpoints.png)

### Categories

- **GET /api/Categories**: Retrieves all categories from the database.
- **GET /api/Categories/{id}**: Retrieves a specific category by its unique identifier.
- **POST /api/Categories**: Creates a new category.
- **PUT /api/Categories/{id}**: Updates an existing category.
- **DELETE /api/Categories/{id}**: Deletes an existing category.

![CategoriesEndpoints](https://github.com/MehmetOguzOzkan/Papara/blob/master/Images/CategoriesEndpoints.png)

### Coupons

- **GET /api/Coupons**: Retrieves all coupons.
- **GET /api/Coupons/{id}**: Retrieves a coupon by its unique identifier.
- **GET /api/Coupons/Users**: Retrieves all coupons available for the current user.
- **GET /api/Coupons/Codes/{code}**: Retrieves a coupon by its unique code.
- **POST /api/Coupons**: Creates a new coupon.
- **PUT /api/Coupons/{id}**: Updates an existing coupon.
- **DELETE /api/Coupons/{id}**: Deletes a coupon.

![CouponsEndpoints](https://github.com/MehmetOguzOzkan/Papara/blob/master/Images/CouponsEndpoints.png)

### Jobs

- **GET /api/Jobs/FireAndForget**: Enqueues a fire-and-forget job.
- **GET /api/Jobs/Delayed**: Schedules a delayed job.
- **GET /api/Jobs/Recurring**: Adds or updates a recurring job.
- **GET /api/Jobs/Continuations**: Schedules a continuation job.

![JobsEndpoints](https://github.com/MehmetOguzOzkan/Papara/blob/master/Images/JobsEndpoints.png)

### Orders And Payment

- **GET /api/Orders**: Retrieves all orders.
- **GET /api/Orders/Users**: Retrieves all orders placed by the current user.
- **GET /api/Orders/{id}**: Retrieves an order by its unique identifier.
- **GET /api/Orders/Codes/{code}**: Retrieves an order by its unique code.
- **POST /api/Orders**: Creates a new order.
- **DELETE /api/Orders/{id}**: Deletes an order.

- **POST /api/Payment/Card**: Processes a payment using a credit or debit card.
- **POST /api/Payment/Wallet**: Processes a payment using wallet balance without a card.

![OrdersPaymentEndpoints](https://github.com/MehmetOguzOzkan/Papara/blob/master/Images/OrdersPaymentEndpoints.png)

## Background Jobs

- **Hangfire**: Manages recurring jobs like sending email notifications or processing large datasets in the background.
- **Job Management**: Jobs can be monitored, retried, and scheduled using Hangfire's dashboard.

## Messaging

- **RabbitMQ**: Implements a robust messaging system for inter-service communication. It is used for handling events, notifications, and other asynchronous tasks.

## Deployment

The entire system can be containerized using Docker, making it easy to deploy across different environments. The Docker setup includes containers for the application, RabbitMQ, MSSQL, PostgreSQL, and other dependencies.

---

This project is a scalable and well-architected e-commerce API that incorporates modern software development practices and technologies. Its modular design ensures flexibility, allowing it to be easily extended or modified to suit different business needs.
