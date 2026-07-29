# =========================================================================
# STAGE 1: THE BUILD KITCHEN (SDK)
# Purpose: Open a heavy-duty workspace with all the tools (compilers)
# needed to turn raw .NET 10 code into a finished, runnable application.
# =========================================================================
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build

# Create and move into a working directory named '/app' inside the container
WORKDIR /app

# Copy ONLY the project file first (the shopping list of dependencies)
COPY *.csproj ./

# Download all required packages from NuGet (the internet)
# Done early so Docker can cache this step and skip it if packages haven't changed
RUN dotnet restore

# Copy all remaining source code files from your computer into the container
COPY . ./

# Compile, optimize, and package the Web API into a release-ready folder named 'out'
RUN dotnet publish -c Release -o out

# =========================================================================
# STAGE 2: THE RUNTIME LUNCHBOX (ASP.NET)
# Purpose: Create a tiny, secure, and fast container that ONLY contains
# the finished app. It leaves the heavy SDK tools behind.
# =========================================================================
FROM mcr.microsoft.com/dotnet/aspnet:10.0

# Create and move into a fresh '/app' directory inside this new, clean container
WORKDIR /app

# Reach back into the 'build' stage, grab ONLY the compiled files from '/app/out',
# and paste them right here into the runtime container
COPY --from=build /app/out .

# ASP.NET container default HTTP port documentation
EXPOSE 8080

# Explicitly tell the ASP.NET Core runtime to use port 8080 (fixes fallback bugs)
ENV ASPNETCORE_HTTP_PORTS=8080

# SECURITY BEST PRACTICE: Switch to the built-in non-root 'app' user
USER app

# Tell the container exactly what command to run when it starts up
ENTRYPOINT ["dotnet", "PlatformService.dll"]

# =========================================================================
# 🚀 HOW TO RUN AND TEST YOUR CONTAINER (INSTRUCTIONS)
# =========================================================================
#
# 1. BUILD THE IMAGE
# Run this in your terminal to build the Docker image template:
# 👉 docker build -t rahul1994jh/platformservice:latest .
#
# 2. RUN THE CONTAINER
# Run this to spin up a live, running instance of your Web API:
# 👉 docker run -d -p 8080:8080 --name platform-api rahul1994jh/platformservice:latest
#
# Run in Development environment (uses a unique name to avoid conflicts):
# 👉 docker run -d -p 8080:8080 -e ASPNETCORE_ENVIRONMENT=Development --name platform-api-dev rahul1994jh/platformservice:latest
#
# 3. HOW TO TEST IT
# Once running, open your web browser or terminal and test these links:
# 👉 http://localhost:8080/api/platforms
# 👉 http://localhost:8080/swagger (Note: .NET 10 uses Scalar by default instead of Swagger if using native OpenAPI tools)
# 👉 Or use curl in terminal: curl http://localhost:8080/api/platforms
#
# 4. USEFUL MANAGEMENT COMMANDS
# 👉 Check if it's running: docker ps
# 👉 View application logs: docker logs platform-api
# 👉 Stop the container: docker stop platform-api
# 👉 Start the container: docker start platform-api
# 👉 Delete the container: docker rm platform-api
# 👉 Push the container to docker hub: docker push rahul1994jh/platformservice:latest
#
# =========================================================================
