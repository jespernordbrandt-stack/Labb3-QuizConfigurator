using Labb_3.Command;
using Labb_3.Models;
using Labb_3.Services;
using System.Collections.ObjectModel;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Data;

namespace Labb_3.ViewModels;

internal class MainWindowViewModel : ViewModelBase
{
    public ObservableCollection<QuestionPackViewModel> Packs { get; } 
        = new ObservableCollection<QuestionPackViewModel>(); //Skapar en tom lista med QuestionPacks

    private QuestionPackViewModel? _activePack;
    public QuestionPackViewModel? ActivePack
	{
		get => _activePack;
        set
        {
            if (SetProperty(ref _activePack, value))
            {
                ConfigurationViewModel?.RaiseActivePackChanged();
            }
        }
	}

    public RelayCommand ShowPlayModeCommand { get; }
    public RelayCommand ShowEditModeCommand { get; }

    private bool _isplayMode;

    public bool IsPlayMode
    {
        get => _isplayMode;
        set => SetProperty(ref _isplayMode, value);
    }

    public PlayerViewModel? PlayerViewModel { get; }
    public ConfigurationViewModel? ConfigurationViewModel { get; }


    public MainWindowViewModel()
    {
		PlayerViewModel = new PlayerViewModel(this);
        ConfigurationViewModel = new ConfigurationViewModel(this);

        ShowPlayModeCommand = new RelayCommand(_ => { IsPlayMode = true; PlayerViewModel?.StartQuiz(); });

        ShowEditModeCommand = new RelayCommand(_ => IsPlayMode = false);

        var questionPack = new QuestionPack("Mina frågor!", Difficulty.Easy); //Skapar ett testpaket
        
        questionPack.Questions.Add(new Question("Vad heter sveriges huvudstad?", "Stockholm", "Oslo", "Göteborg", "London"));
        questionPack.Questions.Add(new Question("Vad är 2 + 3?", "5", "10", "8", "1")); //Lägger till frågor i testpaketet

        var packViewModel = new QuestionPackViewModel(questionPack);//Skapar ett ViewModel objekt av paketet
        
        Packs.Add(packViewModel); // Lägger till paketet i listan med QuestionPacks

        ActivePack = packViewModel; //Väljer paketet

        TestApi();
    }

    private readonly JsonService _jsonService = new ();

    public async Task SavePacksAsync()
    {
        string appDataPath = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);

        string folderPath = Path.Combine(appDataPath, "Labb_3");

        Directory.CreateDirectory(folderPath);

        string filePath = Path.Combine(folderPath, "packs.json");

        List<QuestionPack> packs = Packs.Select(pack => pack.Model).ToList();
        
        await _jsonService.SaveAsync(packs, filePath);
    }

    public async Task LoadPacksAsync()
    {
        string appDataPath =
            Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);

        string folderPath = Path.Combine(appDataPath, "Labb_3");

        string filePath = Path.Combine(folderPath, "packs.json");

        List<QuestionPack> packs = await _jsonService.LoadAsync(filePath);

        Packs.Clear();

        foreach (QuestionPack pack in packs)
        {
            Packs.Add(new QuestionPackViewModel(pack));
        }
        MessageBox.Show(packs[0].Questions.Count.ToString());

        ActivePack = Packs.FirstOrDefault();
    }

    private async void TestApi()
    {
        OpenTriviaService service = new();

        List<TriviaQuestion> questions =
            await service.GetQuestionsAsync(5, 9);

        MessageBox.Show(
            $"Count: {questions.Count}\n" +
            $"Question: {questions[0].Question}\n" +
            $"Correct: {questions[0].CorrectAnswer}");
    }
}
