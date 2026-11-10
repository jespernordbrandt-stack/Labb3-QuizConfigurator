using Labb_3.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.Json;
using System.Windows;

namespace Labb_3.Services;

internal class JsonService
{
    public async Task SaveAsync(List<QuestionPack> packs, string filePath)
    {
        try
        {
            string json =
                JsonSerializer.Serialize(
                    packs,
                    new JsonSerializerOptions
                    {
                        WriteIndented = true
                    });

            await File.WriteAllTextAsync(filePath, json);
        }

        catch (Exception ex)
        {
            MessageBox.Show(
                $"Failed to save packs.\n\n{ex.Message}");
        }
    }

    public async Task<List<QuestionPack>> LoadAsync(string filePath)
    {
        try
        {
            if (!File.Exists(filePath))
            {
                return new List<QuestionPack>();
            }

            string json =
                await File.ReadAllTextAsync(filePath);

            List<QuestionPack>? packs =
                JsonSerializer.Deserialize<List<QuestionPack>>(json);

            return packs ?? new List<QuestionPack>();
        }
        catch (Exception ex)
        {
            MessageBox.Show(
                $"Failed to load packs.\n\n{ex.Message}");

            return new List<QuestionPack>();
        }
    }
}