using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.Json;
using Labb_3.Models;

namespace Labb_3.Services;

internal class JsonService
{
    public async Task SaveAsync(List<QuestionPack> packs, string filePath)
    {
        string json = JsonSerializer.Serialize(packs, new JsonSerializerOptions { WriteIndented = true });

        await File.WriteAllTextAsync(filePath, json);
    }
    
    public async Task<List<QuestionPack>> LoadAsync(string filePath)
    {
        if (!File.Exists(filePath))
        {
            return new List<QuestionPack>();
        }

        string json = await File.ReadAllTextAsync(filePath);
        
        return JsonSerializer.Deserialize<List<QuestionPack>>(json) 
            ?? new List<QuestionPack>();
    }

    
}
