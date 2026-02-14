# FITFanShop Frontend

An Angular frontend application for an e-commerce platform focused on sports equipment, football jerseys, and tracksuits.

## Project Overview

The FITFanShop frontend provides a user interface for product browsing, user authentication, profiles, and orders.
The project is organized in a modular way (auth, catalog, profile, admin, events) with a focus on scalability and maintainability.

## Tech Stack

- Angular 21
- TypeScript
- Angular Material
- RxJS
- ngx-translate (i18n)

## Key Features

- Authentication and authorization (login/register/logout + route guards)
- Product catalog and product details
- User profile management and password change
- Order history view
- Admin section for content management
- HTTP interceptors (auth, loading, error logging)
- Multi-language support (EN/SR)

## Getting Started

### Prerequisites

- Node.js 18+
- npm 9+
- Angular CLI 21
- Running backend API (local or remote)

### Run locally

```bash
# 1) Clone repository
git clone https://github.com/harunmelic/FITFanShop.git

# 2) Go to frontend folder
cd FITFanShop.Frontend

# 3) Install dependencies
npm install

# 4) Start development server
npm start
```

The application is available at: `http://localhost:4200`

## Environment

The API URL is configured through Angular environment files:

- `src/environments/environment.ts`
- `src/environments/environment.staging.ts`
- `src/environments/environment.prod.ts`

Note: do not commit secret values (API keys, tokens, private connection strings).

## Scripts

```bash
npm start      # Start dev server
npm run build  # Build the application
npm test       # Run unit tests
```

## Project Structure

```text
src/
  app/
    api-services/
    core/
    modules/
      admin/
      auth/
      catalog/
      events/
      home/
      profile/
      shared/
  environments/
public/
  assets/
  images/
```

## Screenshots / Demo

- Add a landing page screenshot
- Add a catalog screenshot
- Add a profile or admin section screenshot
- (Optional) Add a live demo link

## Roadmap

- Further checkout flow improvements
- Improved test coverage
- Additional performance and UX optimizations

## Team

- Danis Mameledžija
- Abdullah Musić
- Harun Melić

## Status

In development.
