using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using FitFanShop.Domain.Entities.Catalog;
using FitFanShop.Domain.Entities.Identity;
using FitFanShop.Domain.Entities.Memberships;
using FitFanShop.Domain.Entities.Discounts;
using FitFanShop.Domain.Entities.Tickets;
using FitFanShop.Domain.Entities.Sales;

namespace FitFanShop.Infrastructure.Database.Seeders;

/// <summary>
/// Dynamic seeder for demo/test data.
/// Runs at application startup in Development environment.
/// </summary>
public static class DynamicDataSeeder
{
    public static async Task SeedAsync(DatabaseContext context)
    {
        if (await context.Users.AnyAsync())
        {
            Console.WriteLine("??  Database already contains data. Skipping seeder.");
            return;
        }

        Console.WriteLine("?? Starting Dynamic Data Seeder...");

        var users = await SeedUsersAsync(context);
        var categories = await SeedCategoriesAsync(context);
        var (products, variants) = await SeedProductsAsync(context, categories);
        await SeedMembersAsync(context, users);
        await SeedDiscountsAsync(context, products);
        var (events, ticketTypes) = await SeedEventsAsync(context);
        await SeedOrdersAsync(context, users, variants);
        await SeedTicketsAsync(context, users, events, ticketTypes);

        Console.WriteLine("? Dynamic Data Seeder completed!");
    }

    private static async Task<List<FitFanShopUserEntity>> SeedUsersAsync(DatabaseContext context)
    {
        var hasher = new PasswordHasher<FitFanShopUserEntity>();
        var users = new List<FitFanShopUserEntity>
        {
            // Admin user
            new() { Email = "admin@fitfanshop.com", PasswordHash = hasher.HashPassword(null!, "admin123"), FirstName = "Admin", LastName = "User", RoleId = 2, RegistrationDate = DateTime.UtcNow.AddDays(-90), IsEnabled = true, CreatedAtUtc = DateTime.UtcNow.AddDays(-90) },
            
            // Demo customers
            new() { Email = "user1@example.com", PasswordHash = hasher.HashPassword(null!, "demouser1"), FirstName = "John", LastName = "Smith", RoleId = 1, RegistrationDate = DateTime.UtcNow.AddDays(-60), IsEnabled = true, CreatedAtUtc = DateTime.UtcNow.AddDays(-60) },
            new() { Email = "user2@example.com", PasswordHash = hasher.HashPassword(null!, "demouser2"), FirstName = "Emma", LastName = "Johnson", RoleId = 1, RegistrationDate = DateTime.UtcNow.AddDays(-45), IsEnabled = true, CreatedAtUtc = DateTime.UtcNow.AddDays(-45) },
            new() { Email = "user3@example.com", PasswordHash = hasher.HashPassword(null!, "demouser3"), FirstName = "Michael", LastName = "Williams", RoleId = 1, RegistrationDate = DateTime.UtcNow.AddDays(-30), IsEnabled = true, CreatedAtUtc = DateTime.UtcNow.AddDays(-30) },
            
            // Swagger test user
            new() { Email = "string", PasswordHash = hasher.HashPassword(null!, "string"), FirstName = "Swagger", LastName = "Test", RoleId = 1, RegistrationDate = DateTime.UtcNow.AddDays(-1), IsEnabled = true, CreatedAtUtc = DateTime.UtcNow.AddDays(-1) }
        };
        
        context.Users.AddRange(users);
        await context.SaveChangesAsync();
        Console.WriteLine($"? Seeded {users.Count} users");
        return users;
    }

    private static async Task<List<CategoryEntity>> SeedCategoriesAsync(DatabaseContext context)
    {
        var categories = new List<CategoryEntity>
        {
            new() { Name = "Jerseys", Description = "Official FC Fit club jerseys", IsEnabled = true, CreatedAtUtc = DateTime.UtcNow },
            new() { Name = "Shorts", Description = "FC Fit football shorts", IsEnabled = true, CreatedAtUtc = DateTime.UtcNow },
            new() { Name = "Scarves", Description = "FC Fit fan scarves", IsEnabled = true, CreatedAtUtc = DateTime.UtcNow },
            new() { Name = "Caps", Description = "FC Fit caps and beanies", IsEnabled = true, CreatedAtUtc = DateTime.UtcNow },
            new() { Name = "Flags", Description = "FC Fit fan flags and banners", IsEnabled = true, CreatedAtUtc = DateTime.UtcNow },
            new() { Name = "Mugs", Description = "FC Fit coffee mugs", IsEnabled = true, CreatedAtUtc = DateTime.UtcNow },
            new() { Name = "Bags", Description = "FC Fit backpacks and sports bags", IsEnabled = true, CreatedAtUtc = DateTime.UtcNow },
            new() { Name = "Balls", Description = "FC Fit footballs", IsEnabled = true, CreatedAtUtc = DateTime.UtcNow }
        };
        
        context.Categories.AddRange(categories);
        await context.SaveChangesAsync();
        Console.WriteLine($"? Seeded {categories.Count} categories");
        return categories;
    }

    private static async Task<(List<ProductEntity>, List<ProductVariantEntity>)> SeedProductsAsync(DatabaseContext context, List<CategoryEntity> categories)
    {
        var products = new List<ProductEntity>
        {
            // Jerseys (6 products)
            new() { Name = "FC Fit Home Jersey 2024/25", Description = "Official home jersey with adidas logo", Price = 149.99m, IsEnabled = true, Exclusive = false, CreatedAtUtc = DateTime.UtcNow },
            new() { Name = "FC Fit Away Jersey 2024/25", Description = "Modern away jersey design", Price = 149.99m, IsEnabled = true, Exclusive = false, CreatedAtUtc = DateTime.UtcNow },
            new() { Name = "FC Fit Third Jersey 2024/25", Description = "Third jersey for special matches", Price = 139.99m, IsEnabled = true, Exclusive = false, CreatedAtUtc = DateTime.UtcNow },
            new() { Name = "FC Fit Goalkeeper Jersey 2024/25", Description = "Goalkeeper jersey with reinforced elbows", Price = 129.99m, IsEnabled = true, Exclusive = false, CreatedAtUtc = DateTime.UtcNow },
            new() { Name = "FC Fit Retro Jersey 1990s", Description = "Retro edition from golden years", Price = 159.99m, IsEnabled = true, Exclusive = false, CreatedAtUtc = DateTime.UtcNow },
            new() { Name = "FC Fit Limited Edition Gold Jersey", Description = "Limited gold edition - members only", Price = 199.99m, IsEnabled = true, Exclusive = true, CreatedAtUtc = DateTime.UtcNow },
            
            // Shorts (3 products)
            new() { Name = "FC Fit Home Shorts 2024/25", Description = "Home shorts with breathable material", Price = 59.99m, IsEnabled = true, Exclusive = false, CreatedAtUtc = DateTime.UtcNow },
            new() { Name = "FC Fit Away Shorts 2024/25", Description = "Away shorts with modern cut", Price = 59.99m, IsEnabled = true, Exclusive = false, CreatedAtUtc = DateTime.UtcNow },
            new() { Name = "FC Fit Training Shorts", Description = "Training shorts for practice", Price = 49.99m, IsEnabled = true, Exclusive = false, CreatedAtUtc = DateTime.UtcNow },
            
            // Scarves (3 products)
            new() { Name = "FC Fit Official Scarf", Description = "Official fan scarf with club colors", Price = 49.99m, IsEnabled = true, Exclusive = false, CreatedAtUtc = DateTime.UtcNow },
            new() { Name = "FC Fit Ultras Edition Scarf", Description = "Special edition for ultras group", Price = 59.99m, IsEnabled = true, Exclusive = false, CreatedAtUtc = DateTime.UtcNow },
            new() { Name = "FC Fit Winter Knit Scarf", Description = "Winter knitted scarf", Price = 44.99m, IsEnabled = true, Exclusive = false, CreatedAtUtc = DateTime.UtcNow },
            
            // Caps (2 products)
            new() { Name = "FC Fit Snapback Cap", Description = "Snapback cap with embroidered logo", Price = 39.99m, IsEnabled = true, Exclusive = false, CreatedAtUtc = DateTime.UtcNow },
            new() { Name = "FC Fit Beanie", Description = "Winter beanie for cold days", Price = 29.99m, IsEnabled = true, Exclusive = false, CreatedAtUtc = DateTime.UtcNow },
            
            // Flags (3 products)
            new() { Name = "FC Fit Flag 150x100cm", Description = "Large stadium fan flag", Price = 79.99m, IsEnabled = true, Exclusive = false, CreatedAtUtc = DateTime.UtcNow },
            new() { Name = "FC Fit Mini Flag", Description = "Small hand flag", Price = 19.99m, IsEnabled = true, Exclusive = false, CreatedAtUtc = DateTime.UtcNow },
            new() { Name = "FC Fit Champions Edition Flag", Description = "Exclusive flag for special occasions", Price = 99.99m, IsEnabled = true, Exclusive = true, CreatedAtUtc = DateTime.UtcNow },
            
            // Mugs (2 products)
            new() { Name = "FC Fit Official Mug", Description = "Ceramic mug with club crest", Price = 29.99m, IsEnabled = true, Exclusive = false, CreatedAtUtc = DateTime.UtcNow },
            new() { Name = "FC Fit VIP Mug Set (3pc)", Description = "Premium 3-piece mug set - exclusive", Price = 79.99m, IsEnabled = true, Exclusive = true, CreatedAtUtc = DateTime.UtcNow },
            
            // Bags (3 products)
            new() { Name = "FC Fit Official Backpack", Description = "Sports backpack with multiple pockets", Price = 89.99m, IsEnabled = true, Exclusive = false, CreatedAtUtc = DateTime.UtcNow },
            new() { Name = "FC Fit Sports Bag", Description = "Large equipment bag", Price = 69.99m, IsEnabled = true, Exclusive = false, CreatedAtUtc = DateTime.UtcNow },
            new() { Name = "FC Fit Gym Bag", Description = "Compact gym bag", Price = 74.99m, IsEnabled = true, Exclusive = false, CreatedAtUtc = DateTime.UtcNow },
            
            // Balls (2 products)
            new() { Name = "FC Fit Official Match Ball", Description = "Official club match ball", Price = 149.99m, IsEnabled = true, Exclusive = false, CreatedAtUtc = DateTime.UtcNow },
            new() { Name = "FC Fit Training Ball Size 5", Description = "Training ball for practice", Price = 79.99m, IsEnabled = true, Exclusive = false, CreatedAtUtc = DateTime.UtcNow }
        };

        // Set up ProductCategories collection BEFORE saving products
        for (int i = 0; i < 6; i++) products[i].ProductCategories.Add(new ProductCategoryEntity { CategoryId = categories[0].Id, CreatedAtUtc = DateTime.UtcNow });
        for (int i = 6; i < 9; i++) products[i].ProductCategories.Add(new ProductCategoryEntity { CategoryId = categories[1].Id, CreatedAtUtc = DateTime.UtcNow });
        for (int i = 9; i < 12; i++) products[i].ProductCategories.Add(new ProductCategoryEntity { CategoryId = categories[2].Id, CreatedAtUtc = DateTime.UtcNow });
        for (int i = 12; i < 14; i++) products[i].ProductCategories.Add(new ProductCategoryEntity { CategoryId = categories[3].Id, CreatedAtUtc = DateTime.UtcNow });
        for (int i = 14; i < 17; i++) products[i].ProductCategories.Add(new ProductCategoryEntity { CategoryId = categories[4].Id, CreatedAtUtc = DateTime.UtcNow });
        for (int i = 17; i < 19; i++) products[i].ProductCategories.Add(new ProductCategoryEntity { CategoryId = categories[5].Id, CreatedAtUtc = DateTime.UtcNow });
        for (int i = 19; i < 22; i++) products[i].ProductCategories.Add(new ProductCategoryEntity { CategoryId = categories[6].Id, CreatedAtUtc = DateTime.UtcNow });
        for (int i = 22; i < 24; i++) products[i].ProductCategories.Add(new ProductCategoryEntity { CategoryId = categories[7].Id, CreatedAtUtc = DateTime.UtcNow });

        context.Products.AddRange(products);
        await context.SaveChangesAsync();

        // Add variants
        var variants = new List<ProductVariantEntity>();
        for (int i = 0; i < 9; i++)
        {
            foreach (var size in new[] { "S", "M", "L", "XL", "XXL" })
                variants.Add(new() { ProductId = products[i].Id, Size = size, StockQuantity = products[i].Exclusive ? 5 : 15, Sku = $"FCFIT-{i + 1:D3}-{size}", CreatedAtUtc = DateTime.UtcNow });
        }
        
        int[] stocks = { 100, 75, 80, 120, 150, 50, 200, 25, 150, 30, 60, 70, 50, 40, 80 };
        for (int i = 9; i < 24; i++)
            variants.Add(new() { ProductId = products[i].Id, Size = "One Size", StockQuantity = stocks[i - 9], Sku = $"FCFIT-{i + 1:D3}-ONE", CreatedAtUtc = DateTime.UtcNow });

        context.ProductVariants.AddRange(variants);
        await context.SaveChangesAsync();
        Console.WriteLine($"? Seeded {products.Count} products with {variants.Count} variants");
        return (products, variants);
    }

    private static async Task SeedMembersAsync(DatabaseContext context, List<FitFanShopUserEntity> users)
    {
        var members = new List<MemberEntity>
        {
            new() { UserId = users[1].Id, StartDate = DateTime.UtcNow.AddDays(-30), EndDate = DateTime.UtcNow.AddDays(335), PricePaid = 149.99m, CreatedAtUtc = DateTime.UtcNow.AddDays(-30) },
            new() { UserId = users[2].Id, StartDate = DateTime.UtcNow.AddDays(-15), EndDate = DateTime.UtcNow.AddDays(350), PricePaid = 149.99m, CreatedAtUtc = DateTime.UtcNow.AddDays(-15) },
            new() { UserId = users[3].Id, StartDate = DateTime.UtcNow.AddDays(-400), EndDate = DateTime.UtcNow.AddDays(-35), PricePaid = 149.99m, CreatedAtUtc = DateTime.UtcNow.AddDays(-400) }
        };
        
        context.Members.AddRange(members);
        await context.SaveChangesAsync();
        Console.WriteLine($"? Seeded {members.Count} members (2 active, 1 expired)");
    }

    private static async Task SeedDiscountsAsync(DatabaseContext context, List<ProductEntity> products)
    {
        var discounts = new List<DiscountEntity>
        {
            new() { Name = "Summer Sale 2025", Percentage = 15m, StartDate = DateTime.UtcNow.AddDays(-10), EndDate = DateTime.UtcNow.AddDays(20), MembersOnly = false, CreatedAtUtc = DateTime.UtcNow.AddDays(-10) },
            new() { Name = "VIP Member Exclusive", Percentage = 25m, StartDate = DateTime.UtcNow.AddDays(-30), EndDate = DateTime.UtcNow.AddDays(335), MembersOnly = true, CreatedAtUtc = DateTime.UtcNow.AddDays(-30) },
            new() { Name = "Black Friday", Percentage = 30m, StartDate = DateTime.UtcNow.AddDays(-5), EndDate = DateTime.UtcNow.AddDays(2), MembersOnly = false, CreatedAtUtc = DateTime.UtcNow.AddDays(-5) }
        };
        
        context.Discounts.AddRange(discounts);
        await context.SaveChangesAsync();

        var discountProducts = new List<DiscountProductEntity>
        {
            // Summer Sale (15%) - Jerseys
            new() { DiscountId = discounts[0].Id, ProductId = products[0].Id, CreatedAtUtc = DateTime.UtcNow },
            new() { DiscountId = discounts[0].Id, ProductId = products[1].Id, CreatedAtUtc = DateTime.UtcNow },
            
            // VIP Member (25%) - Exclusive products
            new() { DiscountId = discounts[1].Id, ProductId = products[5].Id, CreatedAtUtc = DateTime.UtcNow },
            new() { DiscountId = discounts[1].Id, ProductId = products[16].Id, CreatedAtUtc = DateTime.UtcNow },
            new() { DiscountId = discounts[1].Id, ProductId = products[18].Id, CreatedAtUtc = DateTime.UtcNow },
            
            // Black Friday (30%) - Balls and bags
            new() { DiscountId = discounts[2].Id, ProductId = products[22].Id, CreatedAtUtc = DateTime.UtcNow },
            new() { DiscountId = discounts[2].Id, ProductId = products[23].Id, CreatedAtUtc = DateTime.UtcNow },
            new() { DiscountId = discounts[2].Id, ProductId = products[20].Id, CreatedAtUtc = DateTime.UtcNow }
        };
        
        context.DiscountProducts.AddRange(discountProducts);
        await context.SaveChangesAsync();
        Console.WriteLine($"? Seeded {discounts.Count} discounts with {discountProducts.Count} products");
    }

    private static async Task<(List<EventEntity>, List<TicketTypeEntity>)> SeedEventsAsync(DatabaseContext context)
    {
        var events = new List<EventEntity>
        {
            new() { Name = "FC Fit vs Rival FC", Description = "Premier League - Round 15", EventDate = DateTime.UtcNow.AddDays(15).AddHours(20).AddMinutes(45), Location = "FC Fit Stadium, City", CreatedAtUtc = DateTime.UtcNow },
            new() { Name = "FC Fit vs United FC", Description = "Premier League - Round 18", EventDate = DateTime.UtcNow.AddDays(30).AddHours(19), Location = "FC Fit Stadium, City", CreatedAtUtc = DateTime.UtcNow },
            new() { Name = "FC Fit vs City FC", Description = "National Cup - Semi-final", EventDate = DateTime.UtcNow.AddDays(45).AddHours(21), Location = "FC Fit Stadium, City", CreatedAtUtc = DateTime.UtcNow }
        };
        
        context.Events.AddRange(events);
        await context.SaveChangesAsync();

        var ticketTypes = new List<TicketTypeEntity>
        {
            // Event 1: FC Fit vs Rival FC
            new() { EventId = events[0].Id, Name = "VIP Box", Price = 150m, TotalAvailable = 50, Description = "Premium VIP box with catering", CreatedAtUtc = DateTime.UtcNow },
            new() { EventId = events[0].Id, Name = "North Stand", Price = 80m, TotalAvailable = 300, Description = "North stand", CreatedAtUtc = DateTime.UtcNow },
            new() { EventId = events[0].Id, Name = "South Stand", Price = 80m, TotalAvailable = 300, Description = "South stand", CreatedAtUtc = DateTime.UtcNow },
            new() { EventId = events[0].Id, Name = "East Stand", Price = 50m, TotalAvailable = 500, Description = "East stand", CreatedAtUtc = DateTime.UtcNow },
            new() { EventId = events[0].Id, Name = "West Stand", Price = 50m, TotalAvailable = 500, Description = "West stand", CreatedAtUtc = DateTime.UtcNow },
            
            // Event 2: FC Fit vs United FC
            new() { EventId = events[1].Id, Name = "VIP Box", Price = 120m, TotalAvailable = 50, Description = "Premium VIP box", CreatedAtUtc = DateTime.UtcNow },
            new() { EventId = events[1].Id, Name = "North Stand", Price = 60m, TotalAvailable = 300, Description = "North stand", CreatedAtUtc = DateTime.UtcNow },
            new() { EventId = events[1].Id, Name = "South Stand", Price = 60m, TotalAvailable = 300, Description = "South stand", CreatedAtUtc = DateTime.UtcNow },
            new() { EventId = events[1].Id, Name = "East Stand", Price = 40m, TotalAvailable = 500, Description = "East stand", CreatedAtUtc = DateTime.UtcNow },
            new() { EventId = events[1].Id, Name = "West Stand", Price = 40m, TotalAvailable = 500, Description = "West stand", CreatedAtUtc = DateTime.UtcNow },
            
            // Event 3: FC Fit vs City FC (Cup - higher prices)
            new() { EventId = events[2].Id, Name = "VIP Box", Price = 180m, TotalAvailable = 50, Description = "Premium VIP box for Cup", CreatedAtUtc = DateTime.UtcNow },
            new() { EventId = events[2].Id, Name = "North Stand", Price = 100m, TotalAvailable = 300, Description = "North stand", CreatedAtUtc = DateTime.UtcNow },
            new() { EventId = events[2].Id, Name = "South Stand", Price = 100m, TotalAvailable = 300, Description = "South stand", CreatedAtUtc = DateTime.UtcNow },
            new() { EventId = events[2].Id, Name = "East Stand", Price = 70m, TotalAvailable = 500, Description = "East stand", CreatedAtUtc = DateTime.UtcNow },
            new() { EventId = events[2].Id, Name = "West Stand", Price = 70m, TotalAvailable = 500, Description = "West stand", CreatedAtUtc = DateTime.UtcNow }
        };
        
        context.TicketTypes.AddRange(ticketTypes);
        await context.SaveChangesAsync();
        Console.WriteLine($"? Seeded {events.Count} events with {ticketTypes.Count} ticket types");
        return (events, ticketTypes);
    }

    private static async Task SeedOrdersAsync(DatabaseContext context, List<FitFanShopUserEntity> users, List<ProductVariantEntity> variants)
    {
        var orders = new List<OrderEntity>
        {
            new() { UserId = users[1].Id, OrderDate = DateTime.UtcNow.AddDays(-10), StatusId = 3, TotalAmount = 299.98m, Note = "Fast delivery please", CreatedAtUtc = DateTime.UtcNow.AddDays(-10) },
            new() { UserId = users[2].Id, OrderDate = DateTime.UtcNow.AddDays(-5), StatusId = 2, TotalAmount = 189.99m, Note = "Member discount applied", CreatedAtUtc = DateTime.UtcNow.AddDays(-5) },
            new() { UserId = users[3].Id, OrderDate = DateTime.UtcNow.AddDays(-2), StatusId = 1, TotalAmount = 149.99m, Note = "Awaiting payment", CreatedAtUtc = DateTime.UtcNow.AddDays(-2) }
        };
        
        context.Orders.AddRange(orders);
        await context.SaveChangesAsync();

        var orderItems = new List<OrderItemEntity>
        {
            // Order 1 (John) - Jersey + Mugs
            new() { OrderId = orders[0].Id, ProductVariantId = variants[0].Id, Quantity = 1, UnitPrice = 149.99m, CreatedAtUtc = DateTime.UtcNow.AddDays(-10) },
            new() { OrderId = orders[0].Id, ProductVariantId = variants[47].Id, Quantity = 5, UnitPrice = 29.99m, CreatedAtUtc = DateTime.UtcNow.AddDays(-10) },
            
            // Order 2 (Emma) - Jersey + Shorts (with Summer Sale discount)
            new() { OrderId = orders[1].Id, ProductVariantId = variants[7].Id, Quantity = 1, UnitPrice = 127.49m, CreatedAtUtc = DateTime.UtcNow.AddDays(-5) },
            new() { OrderId = orders[1].Id, ProductVariantId = variants[31].Id, Quantity = 1, UnitPrice = 59.99m, CreatedAtUtc = DateTime.UtcNow.AddDays(-5) },
            
            // Order 3 (Michael) - Jersey + Scarf
            new() { OrderId = orders[2].Id, ProductVariantId = variants[12].Id, Quantity = 1, UnitPrice = 139.99m, CreatedAtUtc = DateTime.UtcNow.AddDays(-2) },
            new() { OrderId = orders[2].Id, ProductVariantId = variants[45].Id, Quantity = 1, UnitPrice = 49.99m, CreatedAtUtc = DateTime.UtcNow.AddDays(-2) }
        };
        
        context.OrderItems.AddRange(orderItems);
        await context.SaveChangesAsync();
        Console.WriteLine($"? Seeded {orders.Count} orders with {orderItems.Count} items");
    }

    private static async Task SeedTicketsAsync(DatabaseContext context, List<FitFanShopUserEntity> users, List<EventEntity> events, List<TicketTypeEntity> ticketTypes)
    {
        var tickets = new List<TicketEntity>
        {
            // John bought 2 tickets for Event 1 (North Stand)
            new() { TicketTypeId = ticketTypes[1].Id, EventId = events[0].Id, UserId = users[1].Id, PricePaid = 80m, QRCode = $"QR-FCFIT-{Guid.NewGuid():N}", SeatNumber = "N-15", Status = "Valid", PurchaseDate = DateTime.UtcNow.AddDays(-7), CreatedAtUtc = DateTime.UtcNow.AddDays(-7) },
            new() { TicketTypeId = ticketTypes[1].Id, EventId = events[0].Id, UserId = users[1].Id, PricePaid = 80m, QRCode = $"QR-FCFIT-{Guid.NewGuid():N}", SeatNumber = "N-16", Status = "Valid", PurchaseDate = DateTime.UtcNow.AddDays(-7), CreatedAtUtc = DateTime.UtcNow.AddDays(-7) },
            
            // Emma bought VIP ticket for Event 2
            new() { TicketTypeId = ticketTypes[5].Id, EventId = events[1].Id, UserId = users[2].Id, PricePaid = 120m, QRCode = $"QR-FCFIT-{Guid.NewGuid():N}", SeatNumber = "VIP-A12", Status = "Valid", PurchaseDate = DateTime.UtcNow.AddDays(-3), CreatedAtUtc = DateTime.UtcNow.AddDays(-3) },
            
            // Michael bought 2 tickets for Event 3 (East Stand)
            new() { TicketTypeId = ticketTypes[13].Id, EventId = events[2].Id, UserId = users[3].Id, PricePaid = 70m, QRCode = $"QR-FCFIT-{Guid.NewGuid():N}", SeatNumber = "E-28", Status = "Valid", PurchaseDate = DateTime.UtcNow.AddDays(-1), CreatedAtUtc = DateTime.UtcNow.AddDays(-1) },
            new() { TicketTypeId = ticketTypes[13].Id, EventId = events[2].Id, UserId = users[3].Id, PricePaid = 70m, QRCode = $"QR-FCFIT-{Guid.NewGuid():N}", SeatNumber = "E-29", Status = "Valid", PurchaseDate = DateTime.UtcNow.AddDays(-1), CreatedAtUtc = DateTime.UtcNow.AddDays(-1) }
        };
        
        context.Tickets.AddRange(tickets);
        await context.SaveChangesAsync();
        Console.WriteLine($"? Seeded {tickets.Count} tickets");
    }
}
