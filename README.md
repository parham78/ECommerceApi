# Supply — Full-Stack E-Commerce Application

Supply is a full-stack e-commerce application designed around a production-style architecture, with authentication, authorization, ownership validation, transactional checkout, inventory management, optimistic concurrency, historical order snapshots, centralized error handling, automated testing, and Angular integration.

The main customer shopping flow is implemented end to end, from account creation and product discovery through checkout, order history, order details, and cancellation.

The Angular application communicates with the real ASP.NET Core API rather than using mocked customer, product, basket, or order data.

---

## Tech Stack

### Backend

- C#
- .NET 10
- ASP.NET Core Web API
- Entity Framework Core
- SQL Server
- ASP.NET Core Identity
- JWT Authentication
- Role-Based Authorization
- Swagger / OpenAPI
- RFC 7807 ProblemDetails
- xUnit

### Frontend

- Angular
- TypeScript
- SCSS
- Angular Router
- Angular HttpClient
- Reactive Forms
- Angular Signals
- Vitest

### Development

- Git
- GitHub
- EF Core Migrations
- User Secrets
- REST APIs

---

## Current Customer Flow

Home
↓
Browse Products
↓
Product Details
↓
Register / Login
↓
Add Products to Basket
↓
Manage Shipping Addresses
↓
Checkout
↓
Order Created
↓
Order Confirmation
↓
My Orders
↓
Order Details
↓
Cancel Pending Order

---

## Authentication and Users

The application uses ASP.NET Core Identity together with JWT authentication.

Customers can:

- Register an account
- Login with email and password
- Receive a JWT access token
- Access authenticated customer routes
- Access customer-specific API endpoints

The application currently supports the following roles:

Customer
Admin

Customer-specific API endpoints use the `/api/me/...` route pattern.

The backend resolves the authenticated user from the JWT and links that Identity user to a customer profile.

Customer IDs are not trusted from the frontend for customer-specific operations.

Authorization is enforced by the backend even though Angular route guards are also used to improve the frontend user experience.

---

## Product Catalog

The public product catalog supports:

- Product listing
- Product details
- Search
- Minimum and maximum price filtering
- In-stock filtering
- Sorting
- Pagination
- SKU support
- Product stock information
- Product activation/deactivation

The Angular storefront includes:

- Responsive product catalog
- Product cards
- Product images
- Product detail pages
- Loading states
- Empty states
- Error states
- Retry handling
- URL-based pagination

Administrative backend endpoints also exist for product management.

Product updates use `RowVersion`-based optimistic concurrency to help prevent lost updates when multiple requests attempt to modify the same product.

---

## Customer Addresses

Authenticated customers can manage their own saved shipping addresses.

Supported operations include:

- Create an address
- View saved addresses
- Edit an address
- Delete an address
- Select a default address

The backend enforces the address rules.

For example:

- The first saved address automatically becomes the default.
- Selecting another address as default clears the previous default.
- The current default remains default until another address is selected.
- If the default address is deleted, another saved address is promoted when available.
- Customers cannot access another customer's addresses.

The Angular frontend provides a dedicated **My Addresses** page for managing these addresses.

---

## Shopping Basket

Each authenticated customer has their own basket.

Customers can:

- Add products
- Increase quantities
- Decrease quantities
- Remove individual items
- Clear the entire basket
- View current prices
- View current stock availability
- Proceed to checkout

The basket uses the current product price instead of storing a historical price.

Adding a product to the basket does not reserve inventory.

Product availability is checked again during checkout.

---

## Checkout

Checkout is performed through:

```http
POST /api/me/checkout
```

The frontend sends only the selected shipping address:

```json
{
  "addressId": 6
}
```

The client does not send values that should be controlled by the server, such as:

- Customer ID
- Basket ID
- Order total
- Product prices
- Product stock
- Order status

The backend resolves or calculates these values itself.

During checkout, the backend:

1. Resolves the authenticated customer.
2. Verifies that the customer exists and is active.
3. Verifies that the selected address belongs to the customer.
4. Loads the customer's basket.
5. Rejects an empty basket.
6. Loads the products referenced by the basket.
7. Verifies that each product is still active.
8. Verifies current inventory.
9. Uses the current product prices.
10. Creates the order.
11. Creates historical `OrderItem` snapshots.
12. Creates a shipping-address snapshot.
13. Decreases product inventory.
14. Calculates the order total.
15. Removes the basket items.
16. Saves the changes inside a database transaction.

If checkout succeeds, the basket itself remains available for future use but its items are cleared.

If checkout fails, the basket is not cleared.

---

## Checkout and Inventory Safety

The checkout flow handles invalid conditions such as:

- Empty basket
- Invalid customer
- Inactive customer
- Address belonging to another customer
- Inactive product
- Insufficient inventory
- Product stock changing during checkout
- Database concurrency conflicts

A successful checkout performs the important operations together:

Create Order +
Create OrderItems +
Snapshot Shipping Address +
Decrease Inventory +
Clear BasketItems

EF Core concurrency handling is used for inventory-sensitive operations.

---

## Orders

Customers can view their own order history through:

```http
GET /api/me/orders
```

Individual customer orders are available through:

```http
GET /api/me/orders/{id}
```

The Angular frontend currently includes:

- My Orders page
- Pagination
- Order date
- Order status
- Order item count
- Order total
- Order Details page
- Purchased product information
- Shipping-address snapshot
- Customer order cancellation

Only orders belonging to the authenticated customer can be retrieved through the customer endpoints.

---

## Historical Order Data

Orders keep historical information instead of depending entirely on the current product and address records.

Each `OrderItem` stores information such as:

- Product ID
- Product name
- SKU
- Quantity
- Unit price at the time of purchase

Orders also store a copy of the shipping address used during checkout.

This means that changing a product price later does not change an existing order.

Editing or deleting a saved customer address also does not change the shipping information stored on an old order.

---

## Order Status

The normal order lifecycle currently supports:

Pending
↓
Processing
↓
Shipped
↓
Completed

A pending order can also be cancelled:

Pending
↓
Cancelled

Invalid status transitions are rejected by the backend.

---

## Customer Order Cancellation

Customers can cancel their own pending orders through:

```http
PATCH /api/me/orders/{id}/cancel
```

Only `Pending` orders can be cancelled.

When an order is cancelled:

1. The backend verifies that the order belongs to the authenticated customer.
2. The backend verifies that the order can currently be cancelled.
3. The purchased product quantities are restored to inventory.
4. The order status becomes `Cancelled`.
5. The updated order is returned to the frontend.

The Angular Order Details page only displays the cancellation action for `Pending` orders.

The frontend also asks for confirmation before sending the cancellation request.

---

## Angular Storefront

The frontend is branded as **Supply**.

The current storefront includes:

- Branded homepage
- Responsive application shell
- Header navigation
- Footer
- Product catalog
- Product details
- Product imagery
- Customer registration
- Customer login
- JWT session handling
- HTTP authentication interceptor
- Guest route guard
- Customer route guard
- Shopping basket
- Address management
- Checkout
- Order confirmation
- My Orders
- Order Details
- Pending-order cancellation
- Loading states
- Empty states
- API error states
- Responsive layouts

Authenticated customers can currently navigate between:

Shop
Orders
Addresses
Basket

Frontend route guards are used for navigation and user experience, while the backend remains the security boundary.

---

## Backend Architecture

The backend currently follows a straightforward service-based architecture:

HTTP Request
↓
Controller
↓
Service
↓
Entity Framework Core
↓
SQL Server

Controllers remain relatively small while most business rules are implemented inside the service layer.

DTOs are used for API request and response contracts instead of exposing database entities directly.

---

## Main Database Relationships

ApplicationUser
│
│ 1:1
↓
Customer
│
├──── Orders
│ │
│ └──── OrderItems
│
├──── Addresses
│
└──── Basket
│
└──── BasketItems
│
↓
Product

Some important database rules include:

- Unique product SKU
- Unique customer email
- Indexed customer relationship on orders
- Decimal precision for prices
- Product `RowVersion` for optimistic concurrency
- Foreign-key relationships
- Controlled delete behavior

---

## API Examples

### Authentication

```http
POST /api/auth/register
POST /api/auth/login
```

### Products

```http
GET /api/products
GET /api/products/{id}
```

The product list also supports query parameters for search, price filtering, stock filtering, sorting, and pagination.

### Addresses

```http
GET    /api/me/addresses
GET    /api/me/addresses/{id}
POST   /api/me/addresses
PUT    /api/me/addresses/{id}
DELETE /api/me/addresses/{id}
```

### Basket

```http
GET    /api/me/basket
POST   /api/me/basket/items
PATCH  /api/me/basket/items/{itemId}
DELETE /api/me/basket/items/{itemId}
DELETE /api/me/basket/items
```

### Checkout

```http
POST /api/me/checkout
```

### Customer Orders

```http
GET   /api/me/orders
GET   /api/me/orders/{id}
PATCH /api/me/orders/{id}/cancel
```

Admin-protected backend endpoints also exist for product and order operations such as:

- Retrieving administrative product data
- Updating products
- Retrieving orders
- Changing order status
- Cancelling orders

A dedicated Angular Admin UI is intentionally deferred for now.

---

## Error Handling

The API uses centralized exception handling and returns standardized RFC 7807 `ProblemDetails` responses.

Handled application errors include scenarios such as:

- Product not found
- Customer not found
- Address not found
- Order not found
- Basket item not found
- Invalid requests
- Duplicate product SKU
- Concurrency conflicts

Problem responses also include a trace identifier to make debugging and request correlation easier.

---

## Pagination

Reusable pagination is implemented for API responses.

The general response shape is:

```json
{
  "items": [],
  "currentPage": 1,
  "pageSize": 10,
  "totalCount": 25,
  "totalPages": 3
}
```

The same generic pagination model can be reused across resources.

The frontend currently uses pagination for areas such as:

- Product catalog
- Customer order history

---

## Testing

The backend includes both unit tests and integration tests.

Testing covers important business and API behavior, including areas such as:

- Order status rules
- Order cancellation rules
- Authentication-related behavior
- API endpoints
- Error responses
- Business validation

The Angular application also has an automated test setup using Vitest.

In addition to automated testing, the complete customer purchase flow has been manually tested against the real backend.

The verified flow includes:

```text
Register / Login
      ↓
Add Product
      ↓
Basket
      ↓
Address
      ↓
Checkout
      ↓
Order Created
      ↓
Inventory Reduced
      ↓
Basket Cleared
      ↓
Order History
      ↓
Order Details
      ↓
Cancel Order
      ↓
Inventory Restored
```

Multiple separate orders have been successfully created through the Angular storefront.

---

## Project Structure

The repository contains both the ASP.NET Core backend and Angular frontend.

SupplyECommerce
│
├── Controllers/
├── Data/
├── Dtos/
├── Exceptions/
├── Models/
├── Services/
├── Migrations/
├── scripts/
│
├── OrderManagementApi.UnitTests/
├── OrderManagementApi.IntegrationTests/
│
├── OrderManagementApi.csproj
├── ECommerceApi.slnx
│
└── ECommerceWeb/
│
├── public/
│ └── images/
│
└── src/
└── app/
├── core/
├── features/
│ ├── addresses/
│ ├── auth/
│ ├── basket/
│ ├── checkout/
│ ├── home/
│ ├── orders/
│ └── products/
│
├── layout/
├── shared/
└── system/

The internal backend project still uses names from the earlier Order Management stage of the project, while the repository itself has grown into a full-stack e-commerce application.

---

## Running the Project Locally

### Requirements

You will need:

- .NET 10 SDK
- Node.js
- npm
- SQL Server
- EF Core tools

### Clone the Repository

git clone https://github.com/parham78/SupplyECommerce.git
cd SupplyECommerce

### Backend

Restore the .NET dependencies:

dotnet restore

Apply the EF Core migrations:

dotnet ef database update

Run the ASP.NET Core API:

dotnet run

The backend currently runs locally on:

http://localhost:5289

Sensitive configuration such as JWT settings and bootstrap credentials is kept outside source control using configuration sources such as User Secrets.

### Frontend

Open another terminal and navigate to the Angular application:

cd ECommerceWeb

Install the dependencies:

npm install

Start the Angular development server:

npm start

The frontend runs locally on:

http://localhost:4200

During local development, Angular requests under `/api` are proxied to the ASP.NET Core backend.

### Build the Frontend

From the `ECommerceWeb` directory:

npm run build

### Run Frontend Tests

From the `ECommerceWeb` directory:

npm test -- --watch=false

---

## Current Status

The main customer-facing e-commerce experience is implemented.

Completed areas currently include:

- Database schema and relationships
- Entity Framework Core migrations
- Authentication
- JWT authorization
- Customer and Admin roles
- Product catalog
- Product filtering
- Sorting
- Pagination
- Optimistic concurrency
- Shopping basket
- Address management
- Transactional checkout
- Inventory validation
- Inventory reduction
- Historical `OrderItem` snapshots
- Shipping-address snapshots
- Order history
- Order details
- Customer order cancellation
- Inventory restoration after cancellation
- Global exception handling
- ProblemDetails responses
- Angular storefront
- Responsive customer UI
- Backend unit tests
- Backend integration tests
- Initial frontend automated tests

---

## What I'm Working on Next

The next phase focuses mainly on production hardening and DevOps rather than adding more customer-facing screens.

Planned work includes:

- Structured application logging
- Production configuration
- Secrets and environment configuration
- Health checks
- Additional automated frontend testing
- Additional integration testing where useful
- Docker
- Docker Compose
- CI/CD
- Public deployment
- Deployment monitoring

---

## Possible Future Features

Features intentionally deferred until later include:

- Payment integration
- Checkout idempotency
- Refresh tokens
- Email confirmation
- Password reset
- Angular Admin UI
- Product categories
- Product image management
- Product reviews
- Background processing
- Caching
- Message brokers
- More advanced cloud infrastructure
- Kubernetes / AKS
- Helm

These features are not required for the current customer shopping flow.

---

## Project Goal

The purpose of Supply is not just to demonstrate individual CRUD endpoints.

The project is intended to demonstrate how different parts of a full-stack application work together:

Authentication +
Authorization +
Database Design +
Business Rules +
REST APIs +
Transactions +
Concurrency +
Testing +
Angular +
DevOps

The current focus is turning the working full-stack application into a production-ready and publicly deployable portfolio project.
