// ============================================================================
// Infrastructure/Data/DbInitializer.cs - تصحيح الـ Namespace
// ============================================================================
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Restaurant.Domain.Constants;
using Restaurant.Domain.Entities;
using Restaurant.Domain.Enums;
using Restaurant.Infrastructure.DbContext;

namespace Restaurant.Infrastructure.Data  // ✅ صح
{
    public static class DbInitializer
    {
        public static async Task SeedAsync(
            ApplicationDbContext context,
            UserManager<ApplicationUser> userManager,
            RoleManager<IdentityRole> roleManager)
        {
            // Ensure database is created
            await context.Database.MigrateAsync();

            // Seed Roles
            await SeedRolesAsync(roleManager);

            // Seed Users
            await SeedUsersAsync(userManager);

            // Seed Categories & Menu Items
            await SeedMenuDataAsync(context);

            // Seed Tables
            await SeedTablesAsync(context);

            // Seed Sample Orders
            await SeedOrdersAsync(context, userManager);

            await context.SaveChangesAsync();
        }

        // ========================================================================
        // 1. Seed Roles
        // ========================================================================
        private static async Task SeedRolesAsync(RoleManager<IdentityRole> roleManager)
        {
            string[] roleNames = { Roles.Admin, Roles.Manager, Roles.Staff, Roles.Customer };

            foreach (var roleName in roleNames)
            {
                if (!await roleManager.RoleExistsAsync(roleName))
                {
                    await roleManager.CreateAsync(new IdentityRole(roleName));
                }
            }
        }

        // ========================================================================
        // 2. Seed Users
        // ========================================================================
        private static async Task SeedUsersAsync(UserManager<ApplicationUser> userManager)
        {
            // Admin User
            if (await userManager.FindByEmailAsync("admin@restaurant.com") == null)
            {
                var admin = new ApplicationUser
                {
                    UserName = "admin@restaurant.com",
                    Email = "admin@restaurant.com",
                    FirstName = "System",
                    LastName = "Administrator",
                    PreferredLanguage = Languages.English,
                    EmailConfirmed = true,
                    PhoneNumber = "01000000001",
                    PhoneNumberConfirmed = true
                };

                var result = await userManager.CreateAsync(admin, "Admin@123");
                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(admin, Roles.Admin);
                }
            }

            // Manager User
            if (await userManager.FindByEmailAsync("manager@restaurant.com") == null)
            {
                var manager = new ApplicationUser
                {
                    UserName = "manager@restaurant.com",
                    Email = "manager@restaurant.com",
                    FirstName = "Ahmed",
                    LastName = "Hassan",
                    PreferredLanguage = Languages.Arabic,
                    EmailConfirmed = true,
                    PhoneNumber = "01000000002",
                    PhoneNumberConfirmed = true
                };

                var result = await userManager.CreateAsync(manager, "Manager@123");
                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(manager, Roles.Manager);
                }
            }

            // Staff User
            if (await userManager.FindByEmailAsync("staff@restaurant.com") == null)
            {
                var staff = new ApplicationUser
                {
                    UserName = "staff@restaurant.com",
                    Email = "staff@restaurant.com",
                    FirstName = "Mohamed",
                    LastName = "Ali",
                    PreferredLanguage = Languages.Arabic,
                    EmailConfirmed = true,
                    PhoneNumber = "01000000003",
                    PhoneNumberConfirmed = true
                };

                var result = await userManager.CreateAsync(staff, "Staff@123");
                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(staff, Roles.Staff);
                }
            }

            // Customer Users
            var customers = new[]
            {
                new { Email = "customer1@gmail.com", FirstName = "Omar", LastName = "Khaled", Lang = Languages.English },
                new { Email = "customer2@gmail.com", FirstName = "Fatma", LastName = "Ahmed", Lang = Languages.Arabic },
                new { Email = "customer3@gmail.com", FirstName = "Sara", LastName = "Mohamed", Lang = Languages.English }
            };

            int phoneCounter = 4;
            foreach (var customerData in customers)
            {
                if (await userManager.FindByEmailAsync(customerData.Email) == null)
                {
                    var customer = new ApplicationUser
                    {
                        UserName = customerData.Email,
                        Email = customerData.Email,
                        FirstName = customerData.FirstName,
                        LastName = customerData.LastName,
                        PreferredLanguage = customerData.Lang,
                        EmailConfirmed = true,
                        PhoneNumber = $"0100000000{phoneCounter}",
                        PhoneNumberConfirmed = true,
                        Address = "Cairo, Egypt"
                    };

                    var result = await userManager.CreateAsync(customer, "Customer@123");
                    if (result.Succeeded)
                    {
                        await userManager.AddToRoleAsync(customer, Roles.Customer);
                    }
                    phoneCounter++;
                }
            }
        }

        // ========================================================================
        // 3. Seed Menu Categories & Items
        // ========================================================================
        private static async Task SeedMenuDataAsync(ApplicationDbContext context)
        {
            if (await context.MenuCategories.AnyAsync())
                return; // Already seeded

            var categories = new List<MenuCategory>
            {
                new MenuCategory
                {
                    NameEn = "Appetizers",
                    NameAr = "المقبلات",
                    DescriptionEn = "Start your meal with our delicious appetizers",
                    DescriptionAr = "ابدأ وجبتك بمقبلاتنا اللذيذة",
                    DisplayOrder = 1,
                    IsActive = true
                },
                new MenuCategory
                {
                    NameEn = "Main Courses",
                    NameAr = "الأطباق الرئيسية",
                    DescriptionEn = "Enjoy our signature main dishes",
                    DescriptionAr = "استمتع بأطباقنا الرئيسية المميزة",
                    DisplayOrder = 2,
                    IsActive = true
                },
                new MenuCategory
                {
                    NameEn = "Desserts",
                    NameAr = "الحلويات",
                    DescriptionEn = "Sweet treats to end your meal",
                    DescriptionAr = "حلويات لذيذة لإنهاء وجبتك",
                    DisplayOrder = 3,
                    IsActive = true
                },
                new MenuCategory
                {
                    NameEn = "Beverages",
                    NameAr = "المشروبات",
                    DescriptionEn = "Refreshing drinks and hot beverages",
                    DescriptionAr = "مشروبات منعشة وساخنة",
                    DisplayOrder = 4,
                    IsActive = true
                }
            };

            context.MenuCategories.AddRange(categories);
            await context.SaveChangesAsync();

            // Get category IDs
            var appetizersCategory = categories[0];
            var mainCoursesCategory = categories[1];
            var dessertsCategory = categories[2];
            var beveragesCategory = categories[3];

            var menuItems = new List<MenuItem>
            {
                // Appetizers
                new MenuItem
                {
                    NameEn = "Caesar Salad",
                    NameAr = "سلطة سيزر",
                    DescriptionEn = "Fresh romaine lettuce with parmesan cheese and croutons",
                    DescriptionAr = "خس طازج مع جبن بارميزان وخبز محمص",
                    Price = 45.00m,
                    PreparationTime = 10,
                    CategoryId = appetizersCategory.Id,
                    IsAvailable = true
                },
                new MenuItem
                {
                    NameEn = "Garlic Bread",
                    NameAr = "خبز بالثوم",
                    DescriptionEn = "Toasted bread with garlic butter and herbs",
                    DescriptionAr = "خبز محمص بالزبدة والثوم والأعشاب",
                    Price = 25.00m,
                    PreparationTime = 8,
                    CategoryId = appetizersCategory.Id,
                    IsAvailable = true
                },
                new MenuItem
                {
                    NameEn = "French Fries",
                    NameAr = "بطاطس مقلية",
                    DescriptionEn = "Crispy golden french fries",
                    DescriptionAr = "بطاطس مقلية ذهبية مقرمشة",
                    Price = 20.00m,
                    PreparationTime = 12,
                    CategoryId = appetizersCategory.Id,
                    IsAvailable = true
                },

                // Main Courses
                new MenuItem
                {
                    NameEn = "Grilled Chicken",
                    NameAr = "دجاج مشوي",
                    DescriptionEn = "Tender grilled chicken breast with vegetables",
                    DescriptionAr = "صدر دجاج مشوي طري مع الخضار",
                    Price = 120.00m,
                    PreparationTime = 25,
                    CategoryId = mainCoursesCategory.Id,
                    IsAvailable = true
                },
                new MenuItem
                {
                    NameEn = "Beef Steak",
                    NameAr = "ستيك لحم",
                    DescriptionEn = "Premium beef steak cooked to perfection",
                    DescriptionAr = "ستيك لحم فاخر مطبوخ بإتقان",
                    Price = 180.00m,
                    PreparationTime = 30,
                    CategoryId = mainCoursesCategory.Id,
                    IsAvailable = true
                },
                new MenuItem
                {
                    NameEn = "Pasta Carbonara",
                    NameAr = "باستا كاربونارا",
                    DescriptionEn = "Creamy pasta with bacon and parmesan",
                    DescriptionAr = "باستا كريمية مع لحم مقدد وجبن بارميزان",
                    Price = 90.00m,
                    PreparationTime = 20,
                    CategoryId = mainCoursesCategory.Id,
                    IsAvailable = true
                },
                new MenuItem
                {
                    NameEn = "Margherita Pizza",
                    NameAr = "بيتزا مارجريتا",
                    DescriptionEn = "Classic pizza with tomato sauce, mozzarella and basil",
                    DescriptionAr = "بيتزا كلاسيكية بصلصة الطماطم والموتزاريلا والريحان",
                    Price = 85.00m,
                    PreparationTime = 18,
                    CategoryId = mainCoursesCategory.Id,
                    IsAvailable = true
                },

                // Desserts
                new MenuItem
                {
                    NameEn = "Chocolate Cake",
                    NameAr = "كيكة الشوكولاتة",
                    DescriptionEn = "Rich chocolate cake with chocolate ganache",
                    DescriptionAr = "كيكة شوكولاتة غنية مع صوص الشوكولاتة",
                    Price = 50.00m,
                    PreparationTime = 5,
                    CategoryId = dessertsCategory.Id,
                    IsAvailable = true
                },
                new MenuItem
                {
                    NameEn = "Tiramisu",
                    NameAr = "تيراميسو",
                    DescriptionEn = "Italian classic dessert with coffee and mascarpone",
                    DescriptionAr = "حلوى إيطالية كلاسيكية بالقهوة والماسكاربوني",
                    Price = 60.00m,
                    PreparationTime = 5,
                    CategoryId = dessertsCategory.Id,
                    IsAvailable = true
                },
                new MenuItem
                {
                    NameEn = "Ice Cream",
                    NameAr = "آيس كريم",
                    DescriptionEn = "Three scoops of premium ice cream",
                    DescriptionAr = "ثلاث كرات من الآيس كريم الفاخر",
                    Price = 35.00m,
                    PreparationTime = 3,
                    CategoryId = dessertsCategory.Id,
                    IsAvailable = true
                },

                // Beverages
                new MenuItem
                {
                    NameEn = "Fresh Orange Juice",
                    NameAr = "عصير برتقال طازج",
                    DescriptionEn = "Freshly squeezed orange juice",
                    DescriptionAr = "عصير برتقال طازج معصور",
                    Price = 30.00m,
                    PreparationTime = 5,
                    CategoryId = beveragesCategory.Id,
                    IsAvailable = true
                },
                new MenuItem
                {
                    NameEn = "Cappuccino",
                    NameAr = "كابتشينو",
                    DescriptionEn = "Italian coffee with steamed milk",
                    DescriptionAr = "قهوة إيطالية مع حليب مبخر",
                    Price = 25.00m,
                    PreparationTime = 5,
                    CategoryId = beveragesCategory.Id,
                    IsAvailable = true
                },
                new MenuItem
                {
                    NameEn = "Soft Drink",
                    NameAr = "مشروب غازي",
                    DescriptionEn = "Coca Cola, Sprite, Fanta",
                    DescriptionAr = "كوكاكولا، سبرايت، فانتا",
                    Price = 15.00m,
                    PreparationTime = 2,
                    CategoryId = beveragesCategory.Id,
                    IsAvailable = true
                }
            };

            context.MenuItems.AddRange(menuItems);
            await context.SaveChangesAsync();
        }

        // ========================================================================
        // 4. Seed Tables
        // ========================================================================
        private static async Task SeedTablesAsync(ApplicationDbContext context)
        {
            if (await context.Tables.AnyAsync())
                return; // Already seeded

            var tables = new List<Table>();

            // Ground Floor Tables (1-10)
            for (int i = 1; i <= 10; i++)
            {
                tables.Add(new Table
                {
                    TableNumber = $"T{i:D2}",
                    Capacity = i % 3 == 0 ? 6 : (i % 2 == 0 ? 4 : 2),
                    Location = "Ground Floor",
                    IsAvailable = true
                });
            }

            // First Floor Tables (11-20)
            for (int i = 11; i <= 20; i++)
            {
                tables.Add(new Table
                {
                    TableNumber = $"T{i:D2}",
                    Capacity = i % 3 == 0 ? 6 : (i % 2 == 0 ? 4 : 2),
                    Location = "First Floor",
                    IsAvailable = true
                });
            }

            // Outdoor Tables (21-25)
            for (int i = 21; i <= 25; i++)
            {
                tables.Add(new Table
                {
                    TableNumber = $"T{i:D2}",
                    Capacity = 4,
                    Location = "Outdoor",
                    IsAvailable = true
                });
            }

            context.Tables.AddRange(tables);
            await context.SaveChangesAsync();
        }

        // ========================================================================
        // 5. Seed Sample Orders
        // ========================================================================
        private static async Task SeedOrdersAsync(
            ApplicationDbContext context,
            UserManager<ApplicationUser> userManager)
        {
            if (await context.Orders.AnyAsync())
                return; // Already seeded

            var customer = await userManager.FindByEmailAsync("customer1@gmail.com");
            if (customer == null) return;

            var menuItems = await context.MenuItems.ToListAsync();
            var table = await context.Tables.FirstAsync();

            // Order 1 - Dine In (Completed)
            var order1 = new Order
            {
                OrderNumber = GenerateOrderNumber(),
                CustomerId = customer.Id,
                TableId = table.Id,
                OrderType = OrderType.DineIn,
                Status = OrderStatus.Delivered,
                OrderDate = DateTime.UtcNow.AddDays(-2),
                SpecialInstructions = "Please serve hot",
                CompletedAt = DateTime.UtcNow.AddDays(-2).AddHours(1)
            };

            var orderItems1 = new List<OrderItem>
            {
                new OrderItem
                {
                    Order = order1,
                    MenuItemId = menuItems[3].Id,
                    Quantity = 1,
                    UnitPrice = menuItems[3].Price
                },
                new OrderItem
                {
                    Order = order1,
                    MenuItemId = menuItems[0].Id,
                    Quantity = 1,
                    UnitPrice = menuItems[0].Price
                },
                new OrderItem
                {
                    Order = order1,
                    MenuItemId = menuItems[10].Id,
                    Quantity = 2,
                    UnitPrice = menuItems[10].Price
                }
            };

            order1.OrderItems = orderItems1;
            order1.CalculateTotal();

            // Order 2 - Delivery (Preparing)
            var order2 = new Order
            {
                OrderNumber = GenerateOrderNumber(),
                CustomerId = customer.Id,
                OrderType = OrderType.Delivery,
                Status = OrderStatus.Preparing,
                OrderDate = DateTime.UtcNow.AddHours(-1),
                DeliveryAddress = "123 Main Street, Cairo, Egypt",
                SpecialInstructions = "Call before delivery"
            };

            var orderItems2 = new List<OrderItem>
            {
                new OrderItem
                {
                    Order = order2,
                    MenuItemId = menuItems[6].Id,
                    Quantity = 2,
                    UnitPrice = menuItems[6].Price
                },
                new OrderItem
                {
                    Order = order2,
                    MenuItemId = menuItems[2].Id,
                    Quantity = 1,
                    UnitPrice = menuItems[2].Price
                },
                new OrderItem
                {
                    Order = order2,
                    MenuItemId = menuItems[12].Id,
                    Quantity = 3,
                    UnitPrice = menuItems[12].Price
                }
            };

            order2.OrderItems = orderItems2;
            order2.CalculateTotal();

            context.Orders.AddRange(order1, order2);
            await context.SaveChangesAsync();
        }

        // ========================================================================
        // Helper Methods
        // ========================================================================
        private static string GenerateOrderNumber()
        {
            return $"ORD-{DateTime.UtcNow:yyyyMMdd}-{Guid.NewGuid().ToString("N").Substring(0, 6).ToUpper()}";
        }
    }
}