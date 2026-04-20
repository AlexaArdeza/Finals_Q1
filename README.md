# Finals_Q1 - Todo API (Backend)

A production-grade .NET Core Web API for managing tasks with built-in blockchain-style integrity verification.

## Features
- **CRUD Operations**: Complete task management (GET, POST, PUT, DELETE).
- **In-Memory Storage**: High-performance data handling using static collection.
- **Blockchain Verification**: SHA-256 integrity check for every task and the entire chain.
- **CORS Support**: Configured for seamless integration with React frontends.

## API Endpoints
- `GET /api/todos`: Retrieve all tasks.
- `POST /api/todos`: Create a new task (auto-hashes).
- `PUT /api/todos/{id}`: Update task details.
- `DELETE /api/todos/{id}`: Remove a task.
- `GET /api/todos/verify`: Verify the integrity of the data chain.

## Setup Instructions
1. Ensure the .NET SDK is installed.
2. Navigate to project directory.
3. Run: `dotnet run` or `dotnet watch run`
