FROM mcr.microsoft.com/dotnet/sdk:10.0

RUN apt-get update && apt-get install -y nodejs npm && rm -rf /var/lib/apt/lists/*

WORKDIR /app
COPY . /app/

# Build WASI component
RUN dotnet restore src/PKHeX/PKHeX.csproj
RUN dotnet build src/PKHeX/PKHeX.csproj -c Release --nologo -v n

# Transpile WASI component to ES modules
RUN npm install --no-save @bytecodealliance/jco
RUN mkdir -p dist && \
    npx jco transpile src/PKHeX/bin/Release/net10.0/wasi-wasm/native/PKHeX.wasm \
      -o dist/ --name pkhex
