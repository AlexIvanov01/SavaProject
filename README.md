# Microservices-Based Platform for Distribution Company Systems

A scalable and modern platform designed for distribution companies, built using a microservices architecture. The system includes independent services for managing inventory, orders, and customers, ensuring flexibility and efficiency.

## Features

- **Inventory Service**: Manages product and batch information.
- **Order Service**: Handles customer orders and associated order items.
- **Customer Service**: Maintains customer data and interactions.
- **Authentication Service**: Secures the platform with user authentication and authorization.
- **Centralized Logging**: Uses Serilog for structured and asynchronous logging.
- **API Gateway**: Ocelot API Gateway for routing and aggregating service calls.
- **Inter-Service Communication**: gRPC and RabbitMQ for asynchronous and synchronous communication.

## Technologies Used

- **ASP.NET Core**: Framework for building web APIs and services.
- **Entity Framework Core**: ORM for database access and management.
- **MySQL**: Relational database for storing structured data.
- **RabbitMQ**: Message broker for event-driven communication.
- **gRPC**: High-performance RPC framework for inter-service communication.
- **Docker**: Containerization for consistent environments.
- **Kubernetes**: Orchestration for scaling and managing containerized services.
- **Serilog**: Logging for tracking application behavior and issues.

## System Architecture

The platform follows a microservices architecture, with each service having its own database and clear responsibilities. Communication between services is achieved through gRPC for synchronous operations and RabbitMQ for asynchronous messaging.


## Usage

1. Register and authenticate users via the Authentication Service.
2. Use the API Gateway to interact with inventory, order, and customer services.
3. Monitor logs and events using Serilog and RabbitMQ.


