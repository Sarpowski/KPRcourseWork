5130904/20102
Джэн Сарп Сакаоглу

📋 Overview
This URL Shortener application provides a simple way to create shortened URLs with customizable expiration times. The project consists of two main components:

Backend API: ASP.NET Core minimal API service with Entity Framework Core for data persistence
Frontend Client: Angular 19 application for user interface


📁 Project Structure
```
url-shortener/
├── .gitignore                     # Git ignore file
├── LICENSE                        # license
├── docker-compose.yml             # Main Docker Compose configuration
│
├── urlShorterAPI/                 # Backend API project
│   ├── docker-compose.yml         # API-specific Docker Compose config
│   ├── urlShorterAPI.sln          # Solution file
│   │
│   └── urlShorterAPI/             # API project root
│       ├── Controllers/           # API endpoints
│       │   └── UrlShortenerEndpoints.cs 
│       │
│       ├── Data/                  # Database context and migrations
│       │   ├── ApplicationDbContext.cs
│       │   └── Migrations/        # EF Core migrations
│       │
│       ├── Models/                # Data models
│       │   └── ShortUrl.cs        # URL model and DTOs
│       │
│       ├── Program.cs             # Application entry point and configuration
│       ├── appsettings.json       
│       └── urlShorterAPI.csproj   # Project file
│
└── urlShorterClient/              # Frontend Angular project
    ├── Dockerfile                 # Angular Docker build file
    ├── default.conf               # Nginx configuration
    ├── package.json               # npm dependencies
    ├── angular.json               # Angular configuration
    │
    └── src/                       # Angular source code
        ├── app/                   
        │   ├── components/        
        │   │   └── url-shortener/ 
        │   │
        │   ├── models/            # TypeScript interfaces
        │   │   └── url.model.ts   # URL data models
        │   │
        │   ├── services/          
        │   │   └── url-shortener.service.ts  
        │   │
        │   ├── app.component.ts   
        │   ├── app.routes.ts      
        │   └── app.config.ts      
        │
        ├── environments/          
        ├── index.html             
        ├── main.ts                   
        └── styles.css             
```



✨ Features

Create shortened URLs from long URLs
Set custom expiration times (TTL - Time To Live) for each URL
Automatically expires links after the specified time
Copy shortened URLs to clipboard with one click
Containerized deployment with Docker and Docker Compose
SQLite database for simple, file-based persistence

🏗️ Architecture
The application follows a client-server architecture:
```
┌─────────────────┐      ┌─────────────────┐      ┌─────────────────┐
│                               │                 │      │                 │
│  Angular Client       │ ─────▶  ASP.NET Core   │ ─────▶  SQLite DB     │
│  (Frontend)             │      │  API (Backend)  │      │  (Data Storage) │
│                 │      │                 │      │                 │
└─────────────────┘      └─────────────────┘      └─────────────────┘
```
Backend API (urlShorterAPI)

Built with ASP.NET Core 8.0 minimal API
Uses Entity Framework Core with SQLite provider for data access
Provides endpoints for creating and retrieving shortened URLs
Implements URL expiration with TTL (Time To Live) functionality

Frontend Client (urlShorterClient)

Built with Angular 19
Reactive form for URL submission with validation
Responsive design with clean UI
Easy copy-to-clipboard functionality

🛠️ Prerequisites

Docker and Docker Compose
For local development:

.NET 8 SDK
Node.js (v18+)
Angular CLI (npm install -g @angular/cli)



🚀 Getting Started
Using Docker Compose (Recommended)

Clone the repository:
```
git clone https://github.com/Sarpowski/KPRcourseWork.git
cd url-shortener
```
Start the application with Docker Compose:
```
docker-compose up -d
```
Access the application at 
```
http://localhost:80
```
