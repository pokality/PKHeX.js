FROM mcr.microsoft.com/dotnet/sdk:9.0

RUN dotnet workload install wasm-tools
RUN apt-get update && apt-get install -y nodejs npm && rm -rf /var/lib/apt/lists/*

WORKDIR /app
COPY . /app/

RUN npm install
RUN chmod +x scripts/*.sh && ./scripts/build.sh
