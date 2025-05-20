# Digital Library Web Application

## Overview

This project is a fully functional ASP.NET MVC-based Digital Library Web Application developed with C#, Entity Framework, and SQL Server. It allows users to register, log in, browse books, purchase or borrow digital books, and manage their own personal library. The system integrates comprehensive user interaction flows, dynamic inventory management, and robust admin capabilities.

## Technologies Used

* ASP.NET MVC 5
* C#
* Entity Framework 6
* SQL Server
* Razor Views + Bootstrap
* JavaScript (form validation, animation)
* SMTP (Gmail) Email Service

## Database Design Highlights

* **Users**: Includes roles, hashed passwords, password reset tokens
* **Books**: With inventory (stock), sale & borrow prices, and formats
* **Borrows**: Tracks start and return dates
* **WaitingList**: Position-based queue per book
* **Purchases / Orders / Payments**: Transactional records

## Project Highlights

* Advanced waitlist handling with auto-promotion logic
* Real-time borrowing eligibility checks at multiple flow points
* Separation of concerns between user/admin flows
* Robust cart logic with fallback and temporary cart restoration
## Features

### User Functionality

* **Authentication**: Secure login and registration system with hashed passwords
* **Password Reset Flow**: Email-based reset via token with expiration
* **User Dashboard**: View purchased and borrowed books, download in supported formats
* **Borrowing Restrictions**:

  * Limit of 3 concurrent borrowed books
  * Borrowing governed by active waitlist and inventory availability
  * Time-limited access with automatic expiration and inventory update
* **Waitlist System**:

  * Automatically notifies top 3 users when a book becomes available
  * Users removed after 30 days of inactivity
  * Auto-promotion of users in queue

### Book Store

* **Browse Books**: Paginated list with search and filters
* **Book Details**: Detailed view with borrow/purchase options based on stock and waitlist status
* **Cart System**: Add, remove, and update items with type switching (buy/borrow) validations
* **Buy Now**: Instant checkout flow bypassing cart

### Checkout and Payments

* **Credit Card Validation**: Built-in CVC, date, and Luhn checks
* **PayPal Integration**: Secure redirection and transaction confirmation
* **Order Summary**: Confirmation with email notification

### Admin Panel

* **Book Management**: Add, edit, delete books with validations
* **User Management**:

  * View all non-admin users
  * See borrowing/purchase history
  * Delete users with inventory rollback and waitlist updates

### Additional Functionalities

* **Responsive Alerts**: System messages for errors/success via TempData
* **Auto Email Notifications**:

  * Book due reminders (5 days before return date)
  * Password reset
  * Order confirmations
  * Waitlist availability
* **Format-Based Downloads**:

  * Users can select book format (PDF, EPUB, MOBI, etc.) during download
  * Availability based on format field

## Getting Started

1. Clone the repository
2. Set your local DB connection in `web.config`
3. Update email credentials in `EmailService.cs`
4. Build and run the solution in Visual Studio

## Folder Structure

* **Controllers**: MVC Controllers (Account, Cart, Admin, Library)
* **Models**: EF entities, ViewModels
* **Views**: Razor pages per controller
* **App\_Data**: Sample files for format downloads
* **Content**: CSS
* **Scripts**: JS for client-side behaviors

## Author

Developed by \Roy Boker, as part of a student final project to demonstrate backend engineering capabilities and software design with real-world constraints.

---

**Note**: This project was created for educational and portfolio purposes to showcase full-stack ASP.NET MVC web application development including business logic, data modeling, and UI integration.
