using Labb_3.Models;
using System.Collections.Generic;
using System.Net.Http;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Windows;


namespace Labb_3.Services;

public class OpenTriviaService
{
    private readonly HttpClient _httpClient = new();

    public async Task<List<TriviaQuestion>> GetQuestionsAsync(int amount, int categoryId)
    {
        try
        {
            string url =
                $"https://opentdb.com/api.php?amount={amount}&category={categoryId}&type=multiple";

            string json =
                await _httpClient.GetStringAsync(url);

            QuestionResponse? response =
                JsonSerializer.Deserialize<QuestionResponse>(
                    json,
                    new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    });

            return response?.Results.Select(q => new TriviaQuestion
            {
                Question = q.Question,
                CorrectAnswer = q.CorrectAnswer,
                IncorrectAnswers = q.IncorrectAnswers
            }).ToList() ?? new();
        }
        catch (Exception ex)
        {
            MessageBox.Show(
                $"Failed to load questions.\n\n{ex.Message}");

            return new List<TriviaQuestion>();
        }
    }

    public async Task<List<TriviaCategory>> GetCategoriesAsync()
    {
        try
        {
            string url =
                "https://opentdb.com/api_category.php";

            string json =
                await _httpClient.GetStringAsync(url);

            CategoryResponse? response =
                JsonSerializer.Deserialize<CategoryResponse>(
                    json,
                    new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    });

            return response?.TriviaCategories ?? new();
        }
        catch (Exception ex)
        {
            MessageBox.Show(
                $"Failed to load categories.\n\n{ex.Message}");

            return new List<TriviaCategory>();
        }
    }

    private class CategoryResponse
    {
        [JsonPropertyName("trivia_categories")]
        public List<TriviaCategory> TriviaCategories { get; set; } = new();
    }

    private class QuestionResponse
    {
        [JsonPropertyName("results")]
        public List<QuestionDto> Results { get; set; } = new();
    }

    private class QuestionDto
    {
        [JsonPropertyName("question")]
        public string Question { get; set; } = "";

        [JsonPropertyName("correct_answer")]
        public string CorrectAnswer { get; set; } = "";

        [JsonPropertyName("incorrect_answers")]
        public List<string> IncorrectAnswers { get; set; } = new();
    }
}

