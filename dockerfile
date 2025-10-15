
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src


COPY TodoApi/TodoApi.csproj TodoApi/
COPY TodoBlazor/TodoBlazor.csproj TodoBlazor/
RUN dotnet restore TodoApi/TodoApi.csproj
RUN dotnet restore TodoBlazor/TodoBlazor.csproj


COPY TodoApi/ TodoApi/
COPY TodoBlazor/ TodoBlazor/

# Build backend
RUN dotnet publish TodoApi/TodoApi.csproj -c Release -o /app/publish

# Stage 2: Build the Blazor WebAssembly frontend
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS blazor-build
WORKDIR /src
COPY TodoBlazor/ TodoBlazor/
RUN dotnet publish TodoBlazor/TodoBlazor.csproj -c Release -o /blazor/publish

FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS runtime
WORKDIR /app

# Copy backend
COPY --from=build /app/publish ./

# Copy Blazor build output into wwwroot of API
COPY --from=blazor-build /blazor/publish/wwwroot ./wwwroot

EXPOSE 5000
ENV ASPNETCORE_URLS=http://+:5000
ENTRYPOINT ["dotnet", "TodoApi.dll"]
