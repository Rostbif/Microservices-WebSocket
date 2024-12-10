# Microservices-WebSocket

A project to practice microservices, web sockets, and graph manipulations.

## Project Structure

- **ServiceA**: Acts as the client-facing service, handling HTTP requests and communicating with ServiceB via WebSockets.
- **ServiceB**: Manages the core business logic related to graph data and handles WebSocket connections.
- **SharedLibrary**: Contains shared models and utilities used by both services.

## Prerequisites

- Docker and Docker Compose installed on your machine.
- .NET SDK 8.0 installed on your machine (for local development).

## Cloning the repository

```sh
   git clone https://github.com/Rostbif/Microservices-WebSocket.git
```

## Running the Project

### Using Docker Compose

1. **Build and Run the Containers:**
   From the root directory (Microservices-WebSocket)

```sh
   docker-compose build
   docker-compose up
```

2. **Access the Service:**

```sh
    ServiceA: http://localhost:5000 (mapped to 8080)
    ServiceA Swagger: http://localhost:5000/swagger/
    ServiceB: http://localhost:5001 (mapped to 5297)
    ServiceB WebSocket: ws://serviceb:5297/ws
```

### Running Localy

From the root directory (Microservices-WebSocket):

1. **Navigate to ServiceA, Restore Dependencies and Run:**:

```sh
    cd ServiceA
    dotnet restore
    dotnet run
```

2. **Navigate to ServiceB, Restore Dependencies and Run:**:

```sh
    cd ../ServiceB
    dotnet restore
    dotnet run
```

3. **Access the Services:**

```sh
    ServiceA: http://localhost:5000
    ServiceA Swagger: http://localhost:5054/swagger/
    ServiceB: http://localhost:5001
    ServiceB WebSocket: ws://localhost:5297/ws
```
