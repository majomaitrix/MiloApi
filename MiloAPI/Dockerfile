## Etapa de build
FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src

# IMPORTANTE: En Render el contexto de build es la RAIZ del repo cuando
# dejas Root directory vacío. Por eso, las rutas de COPY deben ser
# relativas a la raíz del repo (no usar ../).

# Copiar csproj de todas las capas para restaurar correctamente
COPY Milo.Domain/Milo.Domain.csproj Milo.Domain/
COPY Milo.Infrastructure/Milo.Infrastructure.csproj Milo.Infrastructure/
COPY Milo.Application/Milo.Application.csproj Milo.Application/
COPY MiloAPI/MiloAPI.csproj MiloAPI/

RUN dotnet restore MiloAPI/MiloAPI.csproj

# Copiar todo el repositorio al contexto de build
COPY . .

# Publicar la API
RUN dotnet publish MiloAPI/MiloAPI.csproj -c Release -o /app/out

## Etapa de runtime (imagen ligera)
FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS runtime
WORKDIR /app
COPY --from=build /app/out ./

# Render inyecta $PORT. .NET debe escuchar en 0.0.0.0:$PORT
ENV ASPNETCORE_URLS=http://0.0.0.0:${PORT}
EXPOSE 8080

ENTRYPOINT ["dotnet", "MiloAPI.dll"]


