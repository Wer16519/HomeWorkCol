using System;
using System.Collections.Generic;
using System.Text;

namespace pz5.Models
{
    public class User
    {
        private string _name;
        private string _phone;
        private string _email;

        public int UserId { get; set; }

        public string Name
        {
            get => _name;
            set
            {
                if (string.IsNullOrWhiteSpace(value))
                    throw new ArgumentException("Имя не может быть пустым.");
                _name = value;
            }
        }

        public string Phone
        {
            get => _phone;
            set
            {
                if (string.IsNullOrWhiteSpace(value) || !System.Text.RegularExpressions.Regex.IsMatch(value, @"^\+?[0-9\s\-\(\)]+$"))
                    throw new ArgumentException("Телефон должен быть корректным.");
                _phone = value;
            }
        }

        public string Email
        {
            get => _email;
            set
            {
                if (string.IsNullOrWhiteSpace(value) || !value.Contains('@'))
                    throw new ArgumentException("Email должен быть корректным.");
                _email = value;
            }
        }

        public User() { }

        public User(int userId, string name, string phone, string email)
        {
            UserId = userId;
            Name = name;
            Phone = phone;
            Email = email;
        }

        public override string ToString()
        {
            return $"[{UserId}] {Name} | Тел: {Phone} | Email: {Email}";
        }
    }
}