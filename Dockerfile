FROM mcr.microsoft.com/dotnet/sdk:10.0

RUN apt-get update && apt-get install -y nodejs npm && rm -rf /var/lib/apt/lists/*

WORKDIR /app
COPY . /app/

RUN npm ci
RUN chmod +x scripts/build.sh && scripts/build.sh
