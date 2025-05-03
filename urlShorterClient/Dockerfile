# Angular app build stage
FROM node:20-alpine AS build
WORKDIR /app

# 1. Paket dosyalarını yükle
COPY package*.json ./
RUN npm install

# 2. Angular kaynaklarını kopyala ve build et
COPY . .
RUN npm run build --prod

# Nginx aşaması
FROM nginx:alpine

# 3. Angular build çıktısını Nginx’e kopyala
COPY --from=build /app/dist/url-shorter-client/browser /usr/share/nginx/html

# 4. Özel Nginx yapılandırmanı ekle
COPY default.conf /etc/nginx/conf.d/default.conf
