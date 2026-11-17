# iza shop — Customer Assignment

Project ASP.NET MVC 5 / .NET Framework 4.8 / Entity Framework 6, separated under `CustomerAssignment/` so the existing assignment on `main` is untouched.

The full ZIP attached in this conversation contains the Visual Studio solution, product image assets and SQL script. Features: customer order history, search by order ID/product name, status filters, order details and buy-again via Session cart.

Setup: run `database/MyStoreCustomer.sql` in SQL Server, adjust `Web.config` connection string, open `MyStoreCustomer.sln` in Visual Studio 2022 and restore NuGet packages.

Note: this customer-only project expects the site's authentication to set `User.Identity.Name` to the matching `Customer.Username`; a standalone login/register flow is not included. Build/runtime validation remains to be done in Visual Studio + SQL Server.