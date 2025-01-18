using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using AuthDemo.API.Models;

namespace AuthDemo.BlazorWebApplication.Services
{
    /* Сервис для работы с данными через API */
    public class DataService
    {
        private readonly HttpClient _httpClient;
        private List<DataModel> _items = new List<DataModel>();
        private List<string> _categories = new List<string>();
        private DataModel? _selectedItem;
        private bool _isApiAvailable;

        /* Событие для уведомления об обновлении данных */
        public event Action? OnDataChanged;

        public DataService(HttpClient httpClient)
        {
            _httpClient = httpClient;
            _httpClient.BaseAddress = new Uri("https://localhost:7777/");
        }

        /* Получение списка всех объектов */
        public async Task GetItemsAsync()
        {
            try
            {
                var result = await _httpClient.GetFromJsonAsync<List<DataModel>>("api/Data");
                if (result != null)
                {
                    _items = result;
                    _categories = result.Select(x => x.Description ?? "Без категории")
                                     .Distinct()
                                     .OrderBy(x => x)
                                     .ToList();
                    _isApiAvailable = true;
                }
            }
            catch (Exception)
            {
                _isApiAvailable = false;
                /* Загружаем тестовые данные при недоступности API */
                _items = TestData.Items;
                _categories = TestData.Categories;
            }
            finally
            {
                OnDataChanged?.Invoke();
            }
        }

        /* Проверка доступности API */
        public bool IsApiAvailable => _isApiAvailable;

        /* Получение списка категорий */
        public List<string> GetCategories()
        {
            return _categories;
        }

        /* Получение элементов для текущей страницы с учетом фильтрации */
        public List<DataModel> GetFilteredItems(string? category, int page, int pageSize)
        {
            var query = _items.AsQueryable();

            /* Применяем фильтр по категории если она выбрана */
            if (!string.IsNullOrEmpty(category))
            {
                query = query.Where(item => item.Description == category);
            }

            /* Применяем пагинацию */
            return query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();
        }

        /* Получение общего количества элементов с учетом фильтрации */
        public int GetTotalItems(string? category)
        {
            var query = _items.AsQueryable();
            if (!string.IsNullOrEmpty(category))
            {
                query = query.Where(item => item.Description == category);
            }
            return query.Count();
        }

        /* Установка выбранного элемента */
        public void SelectItem(DataModel item)
        {
            _selectedItem = item;
            OnDataChanged?.Invoke();
        }

        /* Получение выбранного элемента */
        public DataModel? GetSelectedItem()
        {
            return _selectedItem;
        }
    }
}