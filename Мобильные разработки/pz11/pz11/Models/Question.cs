using System;
using System.Collections.Generic;
using System.Text;

namespace pz11.Models
{
    public class Question
    {
        public string Text { get; set; } = string.Empty;
        public List<string> Options { get; set; } = new();
        public string CorrectAnswer { get; set; } = string.Empty;
    }
}
