using AuthDemo.API.Models;

namespace AuthDemo.BlazorWebApplication.Services
{
    /* Тестовые данные для отображения при недоступности API */
    public static class TestData
    {
        /* Список доступных категорий */
        public static List<string> Categories = new()
        {
            "Компьютеры",
            "Ноутбуки",
            "Планшеты",
            "Смартфоны",
            "Аксессуары"
        };

        /* Список тестовых элементов */
        public static List<DataModel> Items = new()
        {
            new DataModel { Id = 1, Name = "MacBook Pro", Description = "Ноутбуки", CreatedDate = DateTime.Now.AddDays(-10) },
            new DataModel { Id = 2, Name = "Dell XPS", Description = "Ноутбуки", CreatedDate = DateTime.Now.AddDays(-9) },
            new DataModel { Id = 3, Name = "iPhone 15", Description = "Смартфоны", CreatedDate = DateTime.Now.AddDays(-8) },
            new DataModel { Id = 4, Name = "Samsung S24", Description = "Смартфоны", CreatedDate = DateTime.Now.AddDays(-7) },
            new DataModel { Id = 5, Name = "iPad Pro", Description = "Планшеты", CreatedDate = DateTime.Now.AddDays(-6) },
            new DataModel { Id = 6, Name = "Surface Pro", Description = "Планшеты", CreatedDate = DateTime.Now.AddDays(-5) },
            new DataModel { Id = 7, Name = "Gaming PC", Description = "Компьютеры", CreatedDate = DateTime.Now.AddDays(-4) },
            new DataModel { Id = 8, Name = "Office PC", Description = "Компьютеры", CreatedDate = DateTime.Now.AddDays(-3) },
            new DataModel { Id = 9, Name = "AirPods Pro", Description = "Аксессуары", CreatedDate = DateTime.Now.AddDays(-2) },
            new DataModel { Id = 10, Name = "Magic Mouse", Description = "Аксессуары", CreatedDate = DateTime.Now.AddDays(-1) }
        };
    }
}