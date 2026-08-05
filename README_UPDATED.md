# Less Jackson Microservices (Updated)

Course: https://youtu.be/DgVjEo3OGBI?si=-7wme82-oG4BsvxJ

Lightweight .NET 10 example showing two cooperating microservices: a Platform Service and a Command Service. The repo includes Dockerfiles, optional RabbitMQ messaging, and Kubernetes manifests for local or cluster deployments.

## Quick architecture

```mermaid
flowchart LR
    Client -->|HTTP| PlatformAPI[Platform Service]
    PlatformAPI -->|POST| CommandAPI[Command Service]
    PlatformAPI -->|AMQP (optional)| RabbitMQ[(RabbitMQ)]
    RabbitMQ -->|fanout| CommandAPI
    Ingress[NGINX Ingress] --> PlatformAPI
    Ingress --> CommandAPI
```

## Services overview

- `PlatformService` (PlatformService)
  - API: `GET/POST /api/platforms`
  - Local dev ports: HTTP 5054 (see launch profiles)
  - Container port: `8080`
  - Data: EF Core (InMemory in development, SQL Server in production)
  - Publishes events via HTTP sync (`ICommandDataClient`) and optionally via RabbitMQ (`IMessageBusClient`).

- `CommandService` (CommandService)
  - API: `POST /api/command/platforms`
  - Local dev ports: HTTP 5073 (see launch profiles)
  - Container port: `8080`
  - Subscribes to RabbitMQ when `MessageBusSubscriber` hosted service is enabled.

Project layout (top-level folders): `PlatformService`, `CommandService`, `K8S`.

## Prerequisites

- .NET 10 SDK
- Docker (Docker Desktop recommended)
- `kubectl` for Kubernetes operations
- (Optional) A running SQL Server for `PlatformService` migrations/production mode

## Local development (dotnet run)

1. Run `PlatformService`:

```powershell
cd PlatformService
dotnet restore
dotnet run
```

2. In a second terminal, run `CommandService`:

```powershell
cd CommandService
dotnet restore
dotnet run
```

3. Verify endpoints:

```powershell
curl http://localhost:5054/api/platforms
curl http://localhost:5073/api/command/platforms
```

## Docker (build and run)

Each service contains a multi-stage `Dockerfile` for small images. Example builds:

```powershell
# Platform
cd PlatformService
docker build -t platformservice:local .

docker run -d -p 8080:8080 --name platformservice platformservice:local

# Command
cd ../CommandService
docker build -t commandservice:local .

docker run -d -p 8081:8080 --name commandservice commandservice:local
```

Logs and checks:

```powershell
docker ps
docker logs platformservice
docker logs commandservice
```

Optional: add `-e RabbitMQHost=... -e RabbitMQPort=...` to the `docker run` commands.

### Suggested docker-compose (local RabbitMQ + services)

You can use this snippet in a `docker-compose.yml` for local end-to-end testing:

```yaml
version: '3.8'
services:
  rabbitmq:
    image: rabbitmq:3-management
    ports:
      - '5672:5672'
      - '15672:15672'
  platformservice:
    build: ./PlatformService
    ports:
      - '8080:8080'
    environment:
      - RabbitMQHost=rabbitmq
      - RabbitMQPort=5672
  commandservice:
    build: ./CommandService
    ports:
      - '8081:8080'
    environment:
      - RabbitMQHost=rabbitmq
      - RabbitMQPort=5672
```

Run it locally:

```powershell
# from repo root
docker compose up --build
```

## RabbitMQ messaging (optional)

- K8s manifest: `K8S/rabbitmq-depl.yaml`
- Exchange: `trigger` (fanout)
- Queue: `commandqueue` (bound to `trigger`)
- Config keys: `RabbitMQHost` and `RabbitMQPort` (see `PlatformService/appsettings.Development.json` and `CommandService/appsettings.Development.json`)

Local RabbitMQ options:

```powershell
# run local rabbitmq with management UI
docker run -d --hostname rabbitmq --name rabbitmq -p 5672:5672 -p 15672:15672 rabbitmq:3-management
# or use included k8s manifest and port-forward
kubectl apply -f .\K8S\rabbitmq-depl.yaml
kubectl port-forward deploy/rabbitmq-depl 5672:5672 15672:15672
# Management UI: http://localhost:15672 (default guest/guest for official image)
```

How services wire to RabbitMQ:

- `PlatformService` registers `RabbitMqMessageBusClient` as `IMessageBusClient` in `PlatformService/Program.cs` and publishes serialized platform events.
- `CommandService` registers `MessageBusSubscriber` as a hosted background service in `CommandService/Program.cs` to consume messages and pass them to `IEventProcessor`.

Production note: the included RabbitMQ manifests and examples are intended for development. For production use, enable authentication, TLS, persistent volumes, and proper user management.

## Database & EF Core migrations (PlatformService)

`PlatformService` uses EF Core. In development it defaults to an InMemory DB; in non-development it uses the `PlatformConn` SQL Server connection string.

Create and apply migrations from the `PlatformService` folder:

```powershell
dotnet tool install --global dotnet-ef
cd PlatformService
dotnet ef migrations add initialmigration
dotnet ef database update
```

Make sure `appsettings.Development.json` or your environment variables point to a reachable SQL Server when running migration commands.

## Kubernetes (manifests)

The `K8S` folder contains manifests for deployments, ClusterIP/NodePort services, ingress, and RabbitMQ. Apply core services:

```powershell
kubectl apply -f .\K8S\platforms-depl.yaml
kubectl apply -f .\K8S\commands-depl.yaml
```

Optional manifests: `platforms-np-srv.yaml`, `ingress-srv.yaml`, `rabbitmq-depl.yaml`, `mssql-plat-depl.yaml`.

## Troubleshooting & tips

- If RabbitMQ fails to connect, check `RabbitMQHost`/`RabbitMQPort` in service config or environment variables.
- For local debugging use `kubectl port-forward` to expose ClusterIP services to localhost.
- Check logs with `kubectl logs` or `docker logs` depending on your run mode.

## Next steps

- Add a `docker-compose.yml` file to the repo (I can create one).
- Add CI workflow to build images and run tests.
- Harden RabbitMQ and add PVC-based stateful deployment for production.

---

If you'd like, I can now create a `docker-compose.yml` in the repo and a short `run-local.ps1` script — tell me to proceed and I'll add them.
