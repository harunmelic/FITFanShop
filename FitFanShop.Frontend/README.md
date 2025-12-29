# FITFanShop Frontend - Brzi Vodič

Angular e-commerce platforma za sportsku opremu, dresove i trenerke fudbalskih klubova.

---

## ⚡ Brzo Pokretanje

### Preduvjeti
- Node.js 18+
- npm 9+
- Angular CLI 21
- Backend API mora biti pokrenut na `https://localhost:7260`

### Instalacija i Pokretanje

```bash
# 1. Kloniraj projekat
git clone https://dev.azure.com/rs1-2025-26-FIT-FanShop/FITFanShop/_git/FitFanShop

# 2. Navigiraj u frontend folder
cd FitFanShop.Frontend

# 3. Instaliraj dependencies
npm install

# 4. Pokreni development server
npm start
```

**Aplikacija će se otvoriti na**: `http://localhost:4200`

---

## 📋 Pristupni Podaci

```
Email: admin@fitfanshop.ba
Lozinka: Admin123!
```

Ostali korisnici:
- `user@fitfanshop.ba` / `User123!`
- `member@fitfanshop.ba` / `Member123!`

---

## 🎯 Mogućnosti

### Landing Page
✅ **Loading Screen** - Animirani logo sa smooth fade-out efektom (3.5s)
✅ **Hero Slider** - Auto-rotating slajder sa slikama proizvoda
✅ **Features Section** - Brza dostava, Kvalitet, Super ponuda, Sigurno plaćanje
✅ **Products Slider** - 3 kategorije (Dresovi, Trenerke, Oprema) sa navigacijom
✅ **Membership Banner** - Promo sekcija za člansku karticu sa 10% popustom
✅ **Testimonials** - Korisničke recenzije sa rating sistemom
✅ **Newsletter** - Email subscription forma
✅ **Scroll to Top** - Floating dugme za brz povratak na vrh stranice

### Navigacija
✅ **Responsive Navbar** - Logo, navigacioni linkovi, search, cart, profile
✅ **Dropdown Menu** - KATALOG sa kategorizacijom proizvoda
✅ **Outside Click Detection** - Automatsko zatvaranje dropdown-a

### E-Commerce Funkcionalnosti (U razvoju)
⏳ **Katalog Proizvoda** - Pregled i filtriranje proizvoda
⏳ **Košarica** - Add to cart, update quantity, checkout
⏳ **Membership System** - Registracija i upravljanje članstvom
⏳ **Narudžbe** - Kreiranje i praćenje narudžbi
⏳ **User Profile** - Upravljanje nalogom i history narudžbi

### Tehnička Svojstva
✅ **Višejezičnost** - Priprema za BS/EN (ngx-translate)
✅ **Responsive Design** - Mobilni, tablet, desktop podržani
✅ **Material Design** - Angular Material komponente
✅ **State Management** - RxJS za reactive programming
✅ **HTTP Interceptors** - Auth, Loading bar, Error logging
✅ **Lazy Loading** - Optimizovano učitavanje modula

---

## 🛠️ Komande

```bash
npm install          # Instaliraj dependencies
npm start            # Pokreni dev server (port 4200)
ng build             # Build za produkciju
ng test              # Pokreni unit testove
ng lint              # Code linting
```

---

## 📁 Struktura Projekta

```
FitFanShop.Frontend/
├── src/
│   ├── app/
│   │   ├── modules/
│   │   │   ├── shared/          # Shared komponente (navbar, footer, sliders)
│   │   │   ├── admin/           # Admin panel moduli
│   │   │   ├── auth/            # Autentikacija (login, register, logout)
│   │   │   ├── client/          # Client-facing moduli
│   │   │   └── public/          # Public stranice (landing page)
│   │   ├── core/
│   │   │   ├── guards/          # Route guards za autorizaciju
│   │   │   ├── interceptors/    # HTTP interceptors
│   │   │   └── services/        # Core servisi (auth, storage)
│   │   ├── api-services/        # API integracija
│   │   └── app.component.ts     # Root komponenta
│   ├── environments/            # Environment konfiguracija
│   └── public/
│       └── images/              # Statički assets (logoi, slike)
├── angular.json                 # Angular konfiguracija
├── package.json                 # Dependencies
└── tsconfig.json                # TypeScript konfiguracija
```

---

## 🎨 Komponente Landing Page-a

### LoadingScreenComponent
- **Lokacija**: `shared/components/loading-screen/`
- **Funkcionalnost**: Početni loading screen sa rotating logom
- **Trajanje**: 3.5 sekundi sa fade-out animacijom

### NavbarComponent
- **Lokacija**: `shared/components/navbar/`
- **Funkcionalnost**: Glavna navigacija sa dropdown menijem i ikonama

### HeroSliderComponent
- **Lokacija**: `shared/components/hero-slider/`
- **Funkcionalnost**: Auto-rotating slider (5s interval) sa 2 slajda

### FeaturesSectionComponent
- **Lokacija**: `shared/components/features-section/`
- **Funkcionalnost**: 4 feature kartice sa Material ikonama

### ProductsSliderComponent
- **Lokacija**: `shared/components/products-slider/`
- **Funkcionalnost**: Slider sa 3 kategorije proizvoda (4 proizvoda po slajdu)

### MembershipBannerComponent
- **Lokacija**: `shared/components/membership-banner/`
- **Funkcionalnost**: Promo banner sa članskom karticom

### TestimonialsSectionComponent
- **Lokacija**: `shared/components/testimonials-section/`
- **Funkcionalnost**: 3 korisničke recenzije sa 5-star ratingom

### NewsletterSectionComponent
- **Lokacija**: `shared/components/newsletter-section/`
- **Funkcionalnost**: Email subscription forma

### FooterComponent
- **Lokacija**: `shared/components/footer/`
- **Funkcionalnost**: 4-column footer sa linkovima i kontakt informacijama

### ScrollToTopComponent
- **Lokacija**: `shared/components/scroll-to-top/`
- **Funkcionalnost**: Fixed dugme u donjem desnom uglu

---

## 🔧 Environment Konfiguracija

### Development (`environment.ts`)
```typescript
export const environment = {
  production: false,
  apiUrl: 'https://localhost:7260/api'
};
```

### Production (`environment.prod.ts`)
```typescript
export const environment = {
  production: true,
  apiUrl: 'https://api.fitfanshop.ba/api'
};
```

---

## 🎨 Design System

### Boje
- **Primary Blue**: `#4A90E2`
- **Dark Gray/Black**: `#333`, `#1a1a1a`, `#2c2c2c`
- **Light Gray**: `#f5f5f5`, `#f9f9f9`, `#fafafa`
- **Border Gray**: `#ddd`, `#ccc`
- **Warning Yellow**: `#ffc107` (rating stars)

### Font
- **Primary**: `Poppins`, sans-serif (Google Fonts)

### Spacing
- Section padding: `60px 120px` (desktop)
- Component gaps: `30px`, `40px`, `60px`
- Card padding: `20px`

### Breakpoints
- Desktop: `> 1024px`
- Tablet: `768px - 1024px`
- Mobile: `< 768px`

---

## 🚀 Deployment

### Build za Production
```bash
ng build --configuration production
```

Output folder: `dist/FitFanShop.Frontend/`

### Azure Deployment
```bash
# Commit i push na main branch
git add .
git commit -m "feat: your feature description"
git push origin main
```

---

## 📦 Dependencies

### Core
- **Angular**: 21.0.0
- **TypeScript**: 5.7.2
- **RxJS**: 7.8.1

### UI
- **Angular Material**: 21.0.0
- **Material Icons**: Included

### I18n
- **@ngx-translate/core**: 16.0.3
- **@ngx-translate/http-loader**: 16.0.0

### Build Tools
- **Angular CLI**: 21.0.0
- **Vite**: Latest
- **Sass**: Latest

---

## 🧪 Testing

```bash
# Unit testovi
ng test

# E2E testovi (kada se implementira)
ng e2e
```

---

## 📝 Git Workflow

### Branch Strategy
- **main** - Stabilna production-ready verzija
- **development** - Development branch za nove feature-e

### Commit Convention
```
feat: nova funkcionalnost
fix: bug fix
docs: dokumentacija
style: formatiranje koda
refactor: refactoring
test: testovi
chore: maintenance
```

### Primjer Commit-a
```bash
git commit -m "feat: implement hero slider with auto-rotation"
git commit -m "fix: resolve dropdown outside click detection"
git commit -m "docs: update README with deployment instructions"
```

---

## 🐛 Poznati Problemi

### NG6008 Error - Standalone Components
**Problem**: Angular CLI kreira komponente kao `standalone: true` po defaultu.
**Rješenje**: Eksplicitno postaviti `standalone: false` u `@Component` decorator-u.

```typescript
@Component({
  selector: 'app-component-name',
  standalone: false,  // VAŽNO!
  templateUrl: './component-name.component.html',
  styleUrls: ['./component-name.component.scss']
})
```

---

## 👥 Tim

**Development Team**: dule-tuli-nisda
- Danis Mameledžija
- Abdullah Musić
- Harun Melić

---

## 📄 Licenca

© 2025 FITFanShop. Sva prava zadržana.

---

## 📞 Kontakt

- **Email**: fitfanshop@gmail.com
- **Telefon**: +387 61 111 2222

---

## 🎯 Roadmap

### Q1 2026
- [ ] Implementacija shopping cart funkcionalnosti
- [ ] Backend integracija za proizvode
- [ ] Payment gateway integracija
- [ ] User registration i membership system

### Q2 2026
- [ ] Order management system
- [ ] Admin panel za upravljanje proizvodima
- [ ] Analytics i reporting
- [ ] Mobile aplikacija (opcional)

---

**Last Updated**: December 29, 2025
**Version**: 1.0.0
**Status**: In Development 🚧
