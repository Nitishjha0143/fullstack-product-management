# E-Commerce Microservices Solution

This solution consists of multiple microservices built with .NET Core for an e-commerce application, orchestrated via an API Gateway.

## Services Overview

1. **Auth Service**
   - Handles user registration and login
   - Generates and validates JWT tokens

2. **Product Service**
   - Manages product catalog
   - Handles product CRUD operations

3. **Order Service**
   - Manages customer orders
   - Handles order creation and retrieval

4. **API Gateway**
   - Routes requests to the appropriate microservice
   - Central entry point for the frontend

---

## Setup Instructions

### Prerequisites
- .NET 7.0 SDK or later
- SQL Server
- Visual Studio 2022 or VS Code

---

### Getting Started

1. **Clone the repository**
```bash
git clone <repository-url>
cd Microservices
cd AuthService
dotnet ef database update

cd ../ProductService
dotnet ef database update

cd ../OrderService
dotnet ef database update
cd AuthService
dotnet run

cd ProductService
dotnet run

cd OrderService
dotnet run

cd ApiGateway
dotnet run
API Endpoints
Product Service

POST api/product - Add new product

GET api/product - Get all products

GET api/product/{id} - Get product by ID

PUT api/product/{id} - Update product

Auth Service

POST api/auth/register - Register new user

POST api/auth/login - User login

Order Service

POST api/order - Create new order

GET api/order - Get all orders

GET api/order/{id} - Get order by ID

Microservices/
├── AuthService/
│   ├── Controllers/
│   ├── Models/
│   └── Data/
├── ProductService/
│   ├── Controllers/
│   ├── Models/
│   └── Data/
└── OrderService/
    ├── Controllers/
    ├── Models/
    └── Data/
Features

Separate microservices for authentication, products, and orders

Centralized API Gateway

Database persistence for all services

RESTful API design

Development Guidelines

Always run migrations before starting the services

Use appropriate HTTP methods for CRUD operations

Follow RESTful conventions

Contributing

Create a feature branch

Commit your changes

Push the branch

Create a pull request