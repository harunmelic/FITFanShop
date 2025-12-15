using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using FitFanShop.Domain.Entities.Catalog;
using FitFanShop.Domain.Entities.Sales;
using FitFanShop.Domain.Entities.Identity;

namespace FitFanShop.Infrastructure.Database.Seeders;

/// <summary>
/// Dynamic seeder koji se pokreće u runtime-u,
/// obično pri startu aplikacije (npr. u Program.cs).
/// Koristi se za unos demo/test podataka koji nisu dio migracije.
/// 
/// NOTE: OLD SEEDER CODE IS COMMENTED OUT - Written for OLD entity structure.
/// TODO: Rewrite this seeder for NEW entity structure after migration is complete.
/// </summary>
public static class DynamicDataSeeder
{
    public static async Task SeedAsync(DatabaseContext context)
    {
        // TODO: Implement new seeding logic for new entity structure
        await Task.CompletedTask;
    }

    /* ==================== OLD SEEDER CODE - KEPT FOR REFERENCE ====================
    
    public static async Task SeedAsync_OLD(DatabaseContext context)
    {
        // Osiguraj da baza postoji (bez migracija)
        await context.Database.EnsureCreatedAsync();

        await SeedProductCategoriesAsync(context);
        await SeedUsersAsync(context);
        await SeedProductsAsync(context);
        await SeedOrdersAsync(context);
    }

    private static async Task SeedProductCategoriesAsync(DatabaseContext context)
    {
        if (!await context.ProductCategories.AnyAsync())
        {
            context.ProductCategories.AddRange(
                new ProductCategoryEntity
                {
                    Name = "Računari",
                    IsEnabled = true,
                    CreatedAtUtc = DateTime.UtcNow
                },
                new ProductCategoryEntity
                {
                    Name = "Mobilni uređaji",
                    IsEnabled = true,
                    CreatedAtUtc = DateTime.UtcNow
                },
                new ProductCategoryEntity
                {
                    Name = "Periferija",
                    IsEnabled = true,
                    CreatedAtUtc = DateTime.UtcNow
                },
                new ProductCategoryEntity
                {
                    Name = "Komponente",
                    IsEnabled = true,
                    CreatedAtUtc = DateTime.UtcNow
                },
                new ProductCategoryEntity
                {
                    Name = "Audio oprema",
                    IsEnabled = false,
                    CreatedAtUtc = DateTime.UtcNow
                }
            );
            await context.SaveChangesAsync();
            Console.WriteLine("✅ Dynamic seed: product categories added.");
        }
    }

    private static async Task SeedUsersAsync(DatabaseContext context)
    {
        if (await context.Users.AnyAsync())
            return;

        var hasher = new PasswordHasher<FitFanShopUserEntity>();

        var admin = new FitFanShopUserEntity
        {
            Email = "admin@market.local",
            PasswordHash = hasher.HashPassword(null!, "Admin123!"),
            IsAdmin = true,
            IsEnabled = true,
            CreatedAtUtc = DateTime.UtcNow
        };

        var manager = new FitFanShopUserEntity
        {
            Email = "manager@market.local",
            PasswordHash = hasher.HashPassword(null!, "Manager123!"),
            IsManager = true,
            IsEnabled = true,
            CreatedAtUtc = DateTime.UtcNow
        };

        var employee = new FitFanShopUserEntity
        {
            Email = "employee@market.local",
            PasswordHash = hasher.HashPassword(null!, "Employee123!"),
            IsEmployee = true,
            IsEnabled = true,
            CreatedAtUtc = DateTime.UtcNow
        };

        var dummyForSwagger = new FitFanShopUserEntity
        {
            Email = "string",
            PasswordHash = hasher.HashPassword(null!, "string"),
            IsEmployee = true,
            IsEnabled = true,
            CreatedAtUtc = DateTime.UtcNow
        };

        var dummyForTests = new FitFanShopUserEntity
        {
            Email = "test",
            PasswordHash = hasher.HashPassword(null!, "test123"),
            IsEmployee = true,
            IsEnabled = true,
            CreatedAtUtc = DateTime.UtcNow
        };

        // Demo customers for orders
        var customer1 = new FitFanShopUserEntity
        {
            Email = "nina.bijedic@email.com",
            PasswordHash = hasher.HashPassword(null!, "Customer123!"),
            IsEnabled = true,
            CreatedAtUtc = DateTime.UtcNow.AddDays(-30)
        };

        var customer2 = new FitFanShopUserEntity
        {
            Email = "iris.memic@email.com",
            PasswordHash = hasher.HashPassword(null!, "Customer123!"),
            IsEnabled = true,
            CreatedAtUtc = DateTime.UtcNow.AddDays(-25)
        };

        var customer3 = new FitFanShopUserEntity
        {
            Email = "azra.smajic@email.com",
            PasswordHash = hasher.HashPassword(null!, "Customer123!"),
            IsEnabled = true,
            CreatedAtUtc = DateTime.UtcNow.AddDays(-20)
        };

        context.Users.AddRange(
            admin,
            manager,
            employee,
            dummyForSwagger,
            dummyForTests,
            customer1,
            customer2,
            customer3
        );

        await context.SaveChangesAsync();
        Console.WriteLine("✅ Dynamic seed: demo users added.");
    }

    /// <summary>
    /// Kreira demo proizvode ako ih još nema u bazi.
    /// </summary>
    private static async Task SeedProductsAsync(DatabaseContext context)
    {
        if (await context.Products.AnyAsync())
            return;

        // Učitaj kategorije
        var categories = await context.ProductCategories
            .Where(c => c.IsEnabled)
            .ToListAsync();

        if (!categories.Any())
        {
            Console.WriteLine("⚠️  No categories found. Skipping product seed.");
            return;
        }

        var racunariCat = categories.FirstOrDefault(c => c.Name.Contains("Računari"));
        var mobilniCat = categories.FirstOrDefault(c => c.Name.Contains("Mobilni"));
        var periferijaCat = categories.FirstOrDefault(c => c.Name.Contains("Periferija"));
        var komponenteCat = categories.FirstOrDefault(c => c.Name.Contains("Komponente"));

        var products = new List<ProductEntity>();

        // Računari
        if (racunariCat != null)
        {
            products.AddRange(new[]
            {
                new ProductEntity
                {
                    Name = "Lenovo ThinkPad X1 Carbon Gen 11",
                    Description = "14\" FHD+ IPS, Intel i7-1365U, 16GB RAM, 512GB SSD, Windows 11 Pro",
                    Price = 2899.00m,
                    StockQuantity = 15,
                    CategoryId = racunariCat.Id,
                    IsEnabled = true,
                    CreatedAtUtc = DateTime.UtcNow.AddDays(-20)
                },
                new ProductEntity
                {
                    Name = "Dell XPS 15 9530",
                    Description = "15.6\" 4K OLED Touch, Intel i9-13900H, 32GB RAM, 1TB SSD, RTX 4070",
                    Price = 3499.00m,
                    StockQuantity = 8,
                    CategoryId = racunariCat.Id,
                    IsEnabled = true,
                    CreatedAtUtc = DateTime.UtcNow.AddDays(-18)
                },
                new ProductEntity
                {
                    Name = "HP Pavilion Gaming Desktop",
                    Description = "AMD Ryzen 7 5700G, 16GB RAM, 512GB SSD, RTX 3060 Ti, Windows 11",
                    Price = 1899.00m,
                    StockQuantity = 12,
                    CategoryId = racunariCat.Id,
                    IsEnabled = true,
                    CreatedAtUtc = DateTime.UtcNow.AddDays(-15)
                },
                new ProductEntity
                {
                    Name = "ASUS ROG Zephyrus G14",
                    Description = "14\" QHD 165Hz, AMD Ryzen 9 7940HS, 32GB RAM, 1TB SSD, RTX 4060",
                    Price = 2699.00m,
                    StockQuantity = 6,
                    CategoryId = racunariCat.Id,
                    IsEnabled = true,
                    CreatedAtUtc = DateTime.UtcNow.AddDays(-12)
                }
            });
        }

        // Mobilni uređaji
        if (mobilniCat != null)
        {
            products.AddRange(new[]
            {
                new ProductEntity
                {
                    Name = "Samsung Galaxy S24 Ultra",
                    Description = "6.8\" AMOLED 2X, 12GB RAM, 512GB, Snapdragon 8 Gen 3, S Pen",
                    Price = 1699.00m,
                    StockQuantity = 25,
                    CategoryId = mobilniCat.Id,
                    IsEnabled = true,
                    CreatedAtUtc = DateTime.UtcNow.AddDays(-10)
                },
                new ProductEntity
                {
                    Name = "Apple iPhone 15 Pro Max",
                    Description = "6.7\" Super Retina XDR, A17 Pro, 256GB, Titanium Design",
                    Price = 1899.00m,
                    StockQuantity = 18,
                    CategoryId = mobilniCat.Id,
                    IsEnabled = true,
                    CreatedAtUtc = DateTime.UtcNow.AddDays(-9)
                },
                new ProductEntity
                {
                    Name = "Google Pixel 8 Pro",
                    Description = "6.7\" LTPO OLED, Google Tensor G3, 12GB RAM, 256GB",
                    Price = 1299.00m,
                    StockQuantity = 20,
                    CategoryId = mobilniCat.Id,
                    IsEnabled = true,
                    CreatedAtUtc = DateTime.UtcNow.AddDays(-8)
                },
                new ProductEntity
                {
                    Name = "Xiaomi 14 Pro",
                    Description = "6.73\" AMOLED, Snapdragon 8 Gen 3, 12GB RAM, 512GB, Leica Camera",
                    Price = 1099.00m,
                    StockQuantity = 30,
                    CategoryId = mobilniCat.Id,
                    IsEnabled = true,
                    CreatedAtUtc = DateTime.UtcNow.AddDays(-7)
                }
            });
        }

        // Periferija
        if (periferijaCat != null)
        {
            products.AddRange(new[]
            {
                new ProductEntity
                {
                    Name = "Logitech MX Master 3S",
                    Description = "Wireless Performance Mouse, 8K DPI, USB-C, Bluetooth",
                    Price = 149.00m,
                    StockQuantity = 50,
                    CategoryId = periferijaCat.Id,
                    IsEnabled = true,
                    CreatedAtUtc = DateTime.UtcNow.AddDays(-6)
                },
                new ProductEntity
                {
                    Name = "Keychron K8 Pro Mechanical Keyboard",
                    Description = "Wireless TKL, Hot-swappable, RGB Backlight, Gateron Switch",
                    Price = 189.00m,
                    StockQuantity = 35,
                    CategoryId = periferijaCat.Id,
                    IsEnabled = true,
                    CreatedAtUtc = DateTime.UtcNow.AddDays(-5)
                },
                new ProductEntity
                {
                    Name = "Dell UltraSharp U2723DE",
                    Description = "27\" QHD IPS Monitor, USB-C Hub, 90W Power Delivery",
                    Price = 749.00m,
                    StockQuantity = 22,
                    CategoryId = periferijaCat.Id,
                    IsEnabled = true,
                    CreatedAtUtc = DateTime.UtcNow.AddDays(-4)
                },
                new ProductEntity
                {
                    Name = "Blue Yeti USB Microphone",
                    Description = "Professional USB Condenser Microphone, Multiple Patterns",
                    Price = 169.00m,
                    StockQuantity = 40,
                    CategoryId = periferijaCat.Id,
                    IsEnabled = true,
                    CreatedAtUtc = DateTime.UtcNow.AddDays(-3)
                }
            });
        }

        // Komponente
        if (komponenteCat != null)
        {
            products.AddRange(new[]
            {
                new ProductEntity
                {
                    Name = "AMD Ryzen 9 7950X",
                    Description = "16-Core, 32-Thread Desktop Processor, 5.7 GHz Max Boost",
                    Price = 899.00m,
                    StockQuantity = 15,
                    CategoryId = komponenteCat.Id,
                    IsEnabled = true,
                    CreatedAtUtc = DateTime.UtcNow.AddDays(-2)
                },
                new ProductEntity
                {
                    Name = "NVIDIA GeForce RTX 4080",
                    Description = "16GB GDDR6X, Ray Tracing, DLSS 3, Ada Lovelace Architecture",
                    Price = 1899.00m,
                    StockQuantity = 10,
                    CategoryId = komponenteCat.Id,
                    IsEnabled = true,
                    CreatedAtUtc = DateTime.UtcNow.AddDays(-1)
                },
                new ProductEntity
                {
                    Name = "Corsair Vengeance DDR5 32GB",
                    Description = "2x16GB DDR5-6000 CL30, Intel XMP 3.0, AMD EXPO",
                    Price = 249.00m,
                    StockQuantity = 45,
                    CategoryId = komponenteCat.Id,
                    IsEnabled = true,
                    CreatedAtUtc = DateTime.UtcNow
                },
                new ProductEntity
                {
                    Name = "Samsung 990 PRO NVMe SSD 2TB",
                    Description = "PCIe 4.0, 7450/6900 MB/s Read/Write, V-NAND Technology",
                    Price = 299.00m,
                    StockQuantity = 38,
                    CategoryId = komponenteCat.Id,
                    IsEnabled = true,
                    CreatedAtUtc = DateTime.UtcNow
                }
            });
        }

        context.Products.AddRange(products);
        await context.SaveChangesAsync();

        Console.WriteLine($"✅ Dynamic seed: {products.Count} products added.");
    }

    /// <summary>
    /// Kreira demo narudžbe (orders) sa order items.
    /// </summary>
    private static async Task SeedOrdersAsync(DatabaseContext context)
    {
        if (await context.Orders.AnyAsync())
            return;

        // Učitaj korisnike i proizvode
        var customers = await context.Users
            .ToListAsync();

        var products = await context.Products
            .Where(p => p.IsEnabled)
            .ToListAsync();

        if (!customers.Any() || !products.Any())
        {
            Console.WriteLine("⚠️  No customers or products found. Skipping orders seed.");
            return;
        }

        var orders = new List<OrderEntity>();
        var orderCounter = 1;

        // Order 1 - Completed (customer1, 15 dana prije)
        var customer1 = customers[0];
        var order1Date = DateTime.UtcNow.AddDays(-15);
        var order1Products = products.Take(3).ToList();

        var order1Items = new List<OrderItemEntity>
        {
            CreateOrderItem(order1Products[0], 1, 0m), // Laptop
            CreateOrderItem(order1Products[1], 1, 10m), // Desktop sa 10% popustom
            CreateOrderItem(order1Products[2], 2, 0m)  // 2x monitor
        };

        var order1 = new OrderEntity
        {
            ReferenceNumber = $"ORD-{orderCounter++:D4}",
            FitFanShopUserId = customer1.Id,
            OrderedAtUtc = order1Date,
            PaidAtUtc = order1Date.AddHours(2),
            Status = OrderStatusType.Completed,
            TotalAmount = order1Items.Sum(i => i.Total),
            Note = "Brza dostava molim.",
            Items = order1Items,
            CreatedAtUtc = order1Date
        };

        orders.Add(order1);

        // Order 2 - Paid (customer2, 10 dana prije)
        var customer2 = customers[1];
        var order2Date = DateTime.UtcNow.AddDays(-10);
        var order2Products = products.Skip(4).Take(2).ToList(); // Phone + accessories

        var order2Items = new List<OrderItemEntity>
        {
            CreateOrderItem(order2Products[0], 1, 5m), // Phone sa 5% popustom
            CreateOrderItem(order2Products[1], 1, 0m)  // Accessories
        };

        var order2 = new OrderEntity
        {
            ReferenceNumber = $"ORD-{orderCounter++:D4}",
            FitFanShopUserId = customer2.Id,
            OrderedAtUtc = order2Date,
            PaidAtUtc = order2Date.AddMinutes(30),
            Status = OrderStatusType.Paid,
            TotalAmount = order2Items.Sum(i => i.Total),
            Note = null,
            Items = order2Items,
            CreatedAtUtc = order2Date
        };

        orders.Add(order2);

        // Order 3 - Confirmed (customer3, 5 dana prije)
        var customer3 = customers[2];
        var order3Date = DateTime.UtcNow.AddDays(-5);
        var order3Products = products.Skip(8).Take(4).ToList(); // PC components

        var order3Items = new List<OrderItemEntity>
        {
            CreateOrderItem(order3Products[0], 1, 0m), // CPU
            CreateOrderItem(order3Products[1], 1, 0m), // GPU
            CreateOrderItem(order3Products[2], 2, 15m), // 2x RAM sa 15% popustom
            CreateOrderItem(order3Products[3], 1, 0m)  // SSD
        };

        var order3 = new OrderEntity
        {
            ReferenceNumber = $"ORD-{orderCounter++:D4}",
            FitFanShopUserId = customer3.Id,
            OrderedAtUtc = order3Date,
            PaidAtUtc = null,
            Status = OrderStatusType.Confirmed,
            TotalAmount = order3Items.Sum(i => i.Total),
            Note = "Kontaktirajte me prije dostave.",
            Items = order3Items,
            CreatedAtUtc = order3Date
        };

        orders.Add(order3);

        // Order 4 - Draft (customer1, 2 dana prije)
        var order4Date = DateTime.UtcNow.AddDays(-2);
        var order4Products = products.Skip(6).Take(2).ToList();

        var order4Items = new List<OrderItemEntity>
        {
            CreateOrderItem(order4Products[0], 1, 0m),
            CreateOrderItem(order4Products[1], 3, 20m) // 3x keyboards sa 20% popustom
        };

        var order4 = new OrderEntity
        {
            ReferenceNumber = $"ORD-{orderCounter++:D4}",
            FitFanShopUserId = customer1.Id,
            OrderedAtUtc = order4Date,
            PaidAtUtc = null,
            Status = OrderStatusType.Draft,
            TotalAmount = order4Items.Sum(i => i.Total),
            Note = "Još razmišljam o boji...",
            Items = order4Items,
            CreatedAtUtc = order4Date
        };

        orders.Add(order4);

        // Order 5 - Cancelled (customer2, 7 dana prije)
        var order5Date = DateTime.UtcNow.AddDays(-7);
        var order5Products = products.Skip(3).Take(1).ToList();

        var order5Items = new List<OrderItemEntity>
        {
            CreateOrderItem(order5Products[0], 1, 0m)
        };

        var order5 = new OrderEntity
        {
            ReferenceNumber = $"ORD-{orderCounter++:D4}",
            FitFanShopUserId = customer2.Id,
            OrderedAtUtc = order5Date,
            PaidAtUtc = null,
            Status = OrderStatusType.Cancelled,
            TotalAmount = order5Items.Sum(i => i.Total),
            Note = "Otkazan na zahtjev korisnika.",
            Items = order5Items,
            CreatedAtUtc = order5Date
        };

        orders.Add(order5);

        // Order 6 - Recent Paid (customer3, 1 dan prije)
        var order6Date = DateTime.UtcNow.AddDays(-1);
        var order6Products = products.Skip(10).Take(2).ToList();

        var order6Items = new List<OrderItemEntity>
        {
            CreateOrderItem(order6Products[0], 2, 0m),
            CreateOrderItem(order6Products[1], 1, 5m)
        };

        var order6 = new OrderEntity
        {
            ReferenceNumber = $"ORD-{orderCounter++:D4}",
            FitFanShopUserId = customer3.Id,
            OrderedAtUtc = order6Date,
            PaidAtUtc = order6Date.AddHours(1),
            Status = OrderStatusType.Paid,
            TotalAmount = order6Items.Sum(i => i.Total),
            Note = "Express dostava.",
            Items = order6Items,
            CreatedAtUtc = order6Date
        };

        orders.Add(order6);

        context.Orders.AddRange(orders);
        await context.SaveChangesAsync();

        Console.WriteLine($"✅ Dynamic seed: {orders.Count} orders added with {orders.Sum(o => o.Items.Count)} total items.");
    }

    /// <summary>
    /// Helper metoda za kreiranje order itema sa kalkulacijama.
    /// </summary>
    private static OrderItemEntity CreateOrderItem(
        ProductEntity product,
        decimal quantity,
        decimal discountPercent)
    {
        var unitPrice = product.Price;
        var subtotal = quantity * unitPrice;
        var discountAmount = discountPercent > 0
            ? Math.Round(subtotal * (discountPercent / 100), 2)
            : 0m;
        var total = subtotal - discountAmount;

        return new OrderItemEntity
        {
            ProductId = product.Id,
            Quantity = quantity,
            UnitPrice = unitPrice,
            Subtotal = subtotal,
            DiscountPercent = discountPercent > 0 ? discountPercent : null,
            DiscountAmount = discountAmount > 0 ? discountAmount : null,
            Total = total,
            Order = null!,
            CreatedAtUtc = DateTime.UtcNow
        };
    }
    
    ==================== END OF OLD SEEDER CODE ====================
    */
}