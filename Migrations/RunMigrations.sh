source ./.env && 
dotnet ef database update --connection "Server=localhost;Port=${POSTGRES_PORT};Database=${POSTGRES_DB};User Id=${POSTGRES_USER};Password=${POSTGRES_PASSWORD};"