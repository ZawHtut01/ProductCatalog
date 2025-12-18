# 🛍️ Product Catalog - ASP.NET Core MVC with Dapper & MSSQL

A production-ready product management system built with **Clean Architecture**, **ASP.NET Core MVC**, **Dapper ORM**, and **Microsoft SQL Server**. Features a modern, responsive UI with full CRUD operations, validation, and error handling.

## ✨ Features

- ✅ **Clean Architecture** - Domain, Application, Infrastructure, Presentation layers
- ✅ **High Performance** - Dapper ORM for fast database operations
- ✅ **Modern UI** - Bootstrap 5, DataTables, SweetAlert2
- ✅ **Full CRUD Operations** - Create, Read, Update, Delete products
- ✅ **Validation** - Server-side & client-side validation
- ✅ **Global Exception Handling** - Custom middleware with Result pattern
- ✅ **Logging** - Structured logging with Serilog (optional)
- ✅ **Responsive Design** - Mobile-friendly interface
- ✅ **Search & Pagination** - DataTables integration
- ✅ **Soft Delete** - Products are not physically deleted
- ✅ **Docker Support** - Containerized deployment

## 📸 Screenshots

| Products List | Create Product | Edit Product |
|--------------|---------------|--------------|
| ![List View](https://via.placeholder.com/400x250/4A6FA5/FFFFFF?text=Products+List) | ![Create View](https://via.placeholder.com/400x250/6A8E7F/FFFFFF?text=Create+Product) | ![Edit View](https://via.placeholder.com/400x250/D4A76A/FFFFFF?text=Edit+Product) |

## 🚀 Quick Start

### Prerequisites
- [.NET 8.0 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- [SQL Server 2022](https://www.microsoft.com/sql-server/sql-server-downloads) or Docker
- [Visual Studio 2022+](https://visualstudio.microsoft.com/) or [VS Code](https://code.visualstudio.com/)

### Option 1: Using Docker (Recommended)
```bash
# Clone the repository
git clone https://github.com/yourusername/product-catalog.git
cd product-catalog

# Start with Docker Compose
docker-compose up -d

# Access the application
http://localhost:8080
