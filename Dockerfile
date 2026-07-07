FROM mcr.microsoft.com/dotnet/sdk:10.0-preview AS restore

WORKDIR /src

COPY ExpenseControl.slnx ./
COPY ExpenseControl.Domain/ExpenseControl.Domain.csproj ExpenseControl.Domain/
COPY ExpenseControl.Application/ExpenseControl.Application.csproj ExpenseControl.Application/
COPY ExpenseControl.Infrastructure/ExpenseControl.Infrastructure.csproj ExpenseControl.Infrastructure/
COPY ExpenseControl.Tests/ExpenseControl.Tests.csproj ExpenseControl.Tests/

RUN dotnet restore ExpenseControl.slnx

FROM restore AS build

WORKDIR /src

COPY . .

RUN dotnet publish ExpenseControl.Infrastructure/ExpenseControl.Infrastructure.csproj \
    -c Release \
    -o /app/publish \
    --no-restore

FROM mcr.microsoft.com/dotnet/aspnet:10.0-preview AS runtime

WORKDIR /app

COPY --from=build /app/publish .

EXPOSE 8080

ENV ASPNETCORE_URLS=http://+:8080
ENV ASPNETCORE_ENVIRONMENT=Production

ENTRYPOINT ["dotnet", "ExpenseControl.Infrastructure.dll"]
