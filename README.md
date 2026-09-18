# ECommerceApi

ECommerceApi is an ecommerce backend that I’m building with ASP.NET Core, Entity Framework Core, SQL Server, Identity, and JWT authentication.

The project is still under development. Right now the main customer flow from account creation to checkout is working.

## Tech Stack

- C#
- .NET 10
- ASP.NET Core Web API
- Entity Framework Core
- SQL Server
- ASP.NET Core Identity
- JWT Authentication
- Swagger / OpenAPI
- Git / GitHub

## What is currently implemented

### Authentication and users

- Customer registration
- Login with JWT
- ASP.NET Core Identity
- Customer and Admin roles
- Role-based authorization
- Customer profile linked to the authenticated Identity user
- Customer-specific `/api/me/...` endpoints

### Product catalog

- Product creation and management
- Public product catalog
- Search
- Price filtering
- In-stock filtering
- Sorting
- Pagination
- SKU support
- Product activation/deactivation
- Stock management
- RowVersion concurrency protection

### Customer addresses

Customers can manage their own saved addresses.

- Create address
- View addresses
- Update address
- Delete address
- Default address handling
- Ownership protection

A customer cannot access another customer's addresses.

### Shopping basket

Each customer has one basket.

Customers can:

- Add products
- Change quantities
- Remove individual items
- Clear the basket
- View current prices and availability

The basket uses the current product price instead of storing a historical price.

Stock is not reserved when a product is added to the basket. Stock is checked again during checkout.

### Checkout

The checkout flow is now working.

A customer sends only the address they want to use:

```json
{
  "addressId": 6
}
```

The backend then:

1. Gets the customer from the JWT
2. Verifies the customer is active
3. Verifies the address belongs to that customer
4. Loads the customer's basket
5. Checks that every product is still active
6. Checks current stock
7. Uses current product prices
8. Creates the order
9. Decreases inventory
10. Creates OrderItem snapshots
11. Saves a shipping-address snapshot
12. Clears the basket items
13. Commits everything in a database transaction

The client does not send the customer ID, total price, basket ID, or product prices.

### Orders

Orders keep historical information instead of depending on the current product data.

Each `OrderItem` stores:

- Product name
- SKU
- Quantity
- Unit price at the time of purchase

Orders also store a copy of the shipping address used during checkout.

This means that changing a product price or editing/deleting a saved address later does not change old orders.

### Order status

The order lifecycle currently supports:

```text
Pending
   ↓
Processing
   ↓
Shipped
   ↓
Completed
```

Pending orders can also be cancelled.

When an order is cancelled, its product quantities are restored to inventory.

Invalid status changes are rejected.

## Checkout safety

Some of the checkout rules I have tested so far:

- Empty basket → rejected
- Address owned by another customer → rejected
- Inactive product → rejected
- Insufficient stock → rejected
- Failed checkout validation does not clear the basket
- Successful checkout decreases stock
- Successful checkout clears BasketItems but keeps the Basket
- Shipping information remains available when the order is retrieved later

EF Core concurrency handling is also used for product stock changes.

## Project structure

The API currently follows a simple layered structure:

```text
Controllers
    ↓
Services
    ↓
Entity Framework Core
    ↓
SQL Server
```

DTOs are used for API requests and responses instead of exposing everything directly from the database models.

Business logic is mainly kept inside the service layer, while controllers stay relatively small.

## Main customer flow

```text
Register
   ↓
Login
   ↓
Browse Products
   ↓
Add Products to Basket
   ↓
Manage Address
   ↓
Checkout
   ↓
Order Created
   ↓
View Order History
```

## Database relationships

The main data relationships are:

- A user account is linked to one customer profile.
- A customer can have multiple orders and addresses.
- Each customer has one basket.
- A basket contains multiple basket items.
- Each basket item points to a product.
- An order contains multiple order items.
- Each order item keeps a snapshot of the product information at the time of purchase.

## API examples

Some of the current endpoints include:

```text
POST   /api/auth/register
POST   /api/auth/login

GET    /api/products

GET    /api/me/addresses
POST   /api/me/addresses
PUT    /api/me/addresses/{id}
DELETE /api/me/addresses/{id}

GET    /api/me/basket
POST   /api/me/basket/items
PATCH  /api/me/basket/items/{itemId}
DELETE /api/me/basket/items/{itemId}
DELETE /api/me/basket/items

POST   /api/me/checkout

GET    /api/me/orders
GET    /api/me/orders/{id}
PATCH  /api/me/orders/{id}/cancel
```

There are also Admin-protected endpoints for managing products and orders.

## Running the project locally

The project requires:

- .NET 10 SDK
- SQL Server
- EF Core tools

Clone the repository:

```bash
git clone https://github.com/parham78/ECommerceApi.git
```

Go into the project:

```bash
cd ECommerceApi
```

Restore packages:

```bash
dotnet restore
```

Apply the EF Core migrations:

```bash
dotnet ef database update
```

Run the API:

```bash
dotnet run
```

The project also uses User Secrets for sensitive configuration such as JWT settings and bootstrap credentials, so secrets are not stored directly in the repository.

## What I'm working on next

The next parts of the project are:

- Checkout idempotency
- More concurrency testing
- Payment integration
- Admin operations
- Unit tests
- Integration tests
- Logging and production configuration
- Docker
- CI/CD
- Deployment
- Angular frontend

The Angular frontend will use this API for the customer ecommerce flow and later for the admin side as well.
