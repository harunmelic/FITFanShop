# FitFanShop Frontend - Inicijalizacija sa Backend API

## 📋 Šta je urađeno?

### 1. **Backend Konekcija**
- ✅ **Environment podesavanje**: Postavio sam `environment.ts` da pokazuje na **`http://localhost:7260`**
- ✅ **API Putanja ispravljena**: Promenio sam putanju sa `/api1/auth` na `/api/auth` da odgovara tvom Swagger API-ju

### 2. **Očišćena Struktura Projekta**

#### **Uklonjeni Moduli:**
- ❌ `modules/admin` - Kompletan admin modul (proizvodi, kategorije, narudžbine admin panel)
- ❌ `modules/client` - Client modul
- ❌ `modules/public` - Public modul (search products)

#### **Uklonjeni API Servisi:**
- ❌ `api-services/products` - Products API
- ❌ `api-services/product-categories` - Product Categories API
- ❌ `api-services/orders` - Orders API

#### **Uklonjeni Lokalizacija/i18n:**
- ❌ `public/i18n` folder (bs.json, en.json)
- ❌ `TranslateModule` iz `app-module.ts`
- ❌ `CustomTranslateLoader` servis

### 3. **Šta je OSTALO (Minimalna Auth Funkcionalnost)**

#### **Struktura:**
```
src/app/
├── api-services/
│   └── auth/                    ✅ Auth API (login, logout, refresh)
│       ├── auth-api.service.ts
│       └── auth-api.model.ts
├── core/
│   ├── components/              ✅ Base komponente
│   ├── guards/                  ✅ Auth guard
│   ├── interceptors/            ✅ HTTP interceptori (auth, loading, error)
│   ├── models/                  ✅ Paging, HTTP params
│   └── services/
│       ├── auth/                ✅ Auth servisi (facade, storage, current-user)
│       ├── loading-bar.service.ts
│       └── toaster.service.ts
├── modules/
│   ├── auth/                    ✅ Login, Register, Logout komponente
│   └── shared/                  ✅ Deljene komponente (loading bar, dialog)
└── environments/
    └── environment.ts           ✅ Backend URL: http://localhost:7260
```

## 🔌 Backend API Konekcija

### **Auth API Endpoints:**
Tvoj frontend je sada povezan na:

```typescript
BASE URL: http://localhost:7260/api/auth

POST /api/auth/login    - Prijava korisnika
POST /api/auth/refresh  - Refresh token
POST /api/auth/logout   - Odjava korisnika
```

### **Auth Models (sa Swaggera):**

**LoginCommand:**
```typescript
{
  email: string;
  password: string;
  fingerprint?: string | null;
}
```

**LoginCommandDto (Response):**
```typescript
{
  accessToken: string;
  refreshToken: string;
  expiresAtUtc: string;
}
```

## 🚀 Kako pokrenuti?

### 1. Instaliraj dependencies:
```bash
cd "c:\Users\danis\Desktop\FitFanshop\Frontend\FITFanShop"
npm install
```

### 2. Pokreni backend:
Proveri da ti backend radi na `http://localhost:7260`

### 3. Pokreni frontend:
```bash
npm start
```

Frontend će biti dostupan na: **http://localhost:4200**

## 📂 Routing

Pojednostavljen routing:
- `/auth/login` - Login stranica (default)
- `/auth/register` - Registracija
- `/auth/logout` - Logout

## 🔑 Kako funkcioniše Auth?

1. **Login:** Korisnik unese email/password → šalje se na `/api/auth/login` → dobija `accessToken` i `refreshToken`
2. **Auth Interceptor:** Automatski dodaje `Authorization: Bearer {accessToken}` header na sve HTTP zahteve
3. **Refresh Token:** Kada access token istekne, automatski se zove `/api/auth/refresh`
4. **Auth Guard:** Štiti rute koje zahtevaju autentifikaciju
5. **Logout:** Zove `/api/auth/logout` i briše tokene iz localStorage-a

## 📝 Važne napomene

### **Auth Storage:**
- Tokeni se čuvaju u **localStorage**
- `accessToken` - za autorizaciju
- `refreshToken` - za refresh access tokena
- `currentUser` - dekodiran JWT payload

### **HTTP Interceptori:**
1. **Loading Bar Interceptor** - Prikazuje loading bar tokom HTTP zahteva
2. **Auth Interceptor** - Dodaje Bearer token
3. **Error Logging Interceptor** - Loguje HTTP greške

### **Servisi:**
- `AuthApiService` - Komunicira sa backend API-jem
- `AuthFacadeService` - Glavni auth servis (login, logout, refresh)
- `AuthStorageService` - localStorage operacije
- `CurrentUserService` - Informacije o trenutnom korisniku

## 🎯 Sledeći Koraci

Sada možeš:
1. Dodati nove API servise za svoje backend endpoints
2. Kreirati nove komponente/module po potrebi
3. Prilagoditi login/register forme
4. Dodati dodatne rute

## 💡 Test Kredencijali

Prema login komponenti, default kredencijali su:
```
Email: admin@market.local
Password: Admin123!
```
(Možeš ih promeniti u `login.component.ts`)

---

✅ **Frontend je spreman za rad sa tvojim backendom!**
