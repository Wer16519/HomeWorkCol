using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using System.Text;

namespace pz6
{
    public class StudentService
    {
        private readonly HttpClient _httpClient;
        private readonly string _baseUrl = "https://vic-site.ru/api";

        public StudentService()
        {
            _httpClient = new HttpClient();
        }

        // 1. Получить всех студентов 
        public async Task<List<Student>> GetAllStudentsAsync()
        {
            var response = await _httpClient.GetAsync($"{_baseUrl}/students");
            if (response.IsSuccessStatusCode)
            {
                string content = await response.Content.ReadAsStringAsync();
                List<Student> students = JsonConvert.DeserializeObject<List<Student>>(content);
                return students;
            }
            return new List<Student>(); // Возвращаем пустой список в случае ошибки 
        }

    }
}