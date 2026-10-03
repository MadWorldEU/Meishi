# syntax=docker/dockerfile:1
# Build context is the repository root, e.g.:
#   docker build -f Sources/MadWorldEU.Meishi.Controllers.Portal/Dockerfile .

FROM node:24-alpine AS build
WORKDIR /src
COPY ["Sources/MadWorldEU.Meishi.Controllers.Portal/package.json", "Sources/MadWorldEU.Meishi.Controllers.Portal/package-lock.json", "./"]
RUN npm ci --ignore-scripts
COPY ["Sources/MadWorldEU.Meishi.Controllers.Portal/angular.json", "Sources/MadWorldEU.Meishi.Controllers.Portal/tsconfig*.json", "./"]
COPY ["Sources/MadWorldEU.Meishi.Controllers.Portal/public", "./public"]
COPY ["Sources/MadWorldEU.Meishi.Controllers.Portal/src", "./src"]
RUN npm run build

FROM nginxinc/nginx-unprivileged:alpine AS final
COPY <<'EOF' /etc/nginx/conf.d/default.conf
server {
    listen 8080;
    root /usr/share/nginx/html;
    index index.html;

    # Hashed bundles never change, so they can be cached forever.
    location ~* \.(?:js|css|woff2?|ico|png|svg|jpg|jpeg|webp)$ {
        expires 1y;
        add_header Cache-Control "public, immutable";
        try_files $uri =404;
    }

    # Serve index.html for client-side routes.
    location / {
        add_header Cache-Control "no-cache";
        try_files $uri $uri/ /index.html;
    }
}
EOF
COPY --from=build /src/dist/MadWorldEU.Meishi.Controllers.Portal/browser /usr/share/nginx/html
EXPOSE 8080
