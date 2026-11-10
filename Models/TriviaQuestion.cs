using System;
using System.Collections.Generic;
using System.Text;

namespace Labb_3.Models;

public class TriviaQuestion
{
    public string Question { get; set; } = "";

    public string CorrectAnswer { get; set; } = "";

    public List<string> IncorrectAnswers { get; set; } = new();
}

