# =========================
# Etapa de build
# =========================
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copiar todo el proyecto
COPY . .

# Publicar la aplicación en modo Release
RUN dotnet publish -c Release -o /app/publish


# =========================
# Etapa final (runtime)
# =========================
FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app

# Instalar herramientas necesarias para reemplazar variables en JSON
# (envsubst viene del paquete gettext-base)
RUN apt-get update && apt-get install -y gettext-base && rm -rf /var/lib/apt/lists/*

# Copiar la aplicación publicada
COPY --from=build /app/publish .

# Copiar la plantilla de Ocelot
COPY ocelot.example.json.tmpl /app/ocelot.json


# Configurar la URL de escucha de ASP.NET
ENV ASPNETCORE_URLS=http://+:8080

# Al iniciar el contenedor:
# 1. Sustituye variables de entorno en ocelot.example.json -> ocelot.json
# 2. Lanza la app
CMD envsubst < /app/ocelot.json > /app/ocelot.json && dotnet cer-gateway.dll
