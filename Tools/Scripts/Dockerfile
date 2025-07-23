# Dockerfile
FROM ubuntu:20.04

# Install dependencies
RUN apt-get update && apt-get install -y \
    wget \
    unzip \
    xvfb

# Create app directory
WORKDIR /app

# Copy Unity server build
COPY ./ServerBuild/ ./

# Make executable
RUN chmod +x ./CafeConnect3D.x86_64

# Expose port
EXPOSE 7777

# Start server with virtual display
CMD ["xvfb-run", "-a", "./CafeConnect3D.x86_64", "-batchmode", "-nographics"]
