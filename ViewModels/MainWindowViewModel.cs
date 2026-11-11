using Labb_3.Command;
using Labb_3.Dialogs;
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
    public ObservableCollection<TriviaCategory> Categories { get; }
    = new();

    public RelayCommand ShowPlayModeCommand { get; }
    public RelayCommand ShowEditModeCommand { get; }
    public RelayCommand ImportQuestionsCommand { get; }
    public RelayCommand ShowImportQuestionsDialogCommand { get; }

    private readonly JsonService _jsonService = new();
    private readonly OpenTriviaService _openTriviaService = new();

    private bool _isplayMode;
    private int _questionAmount = 5;
    private Difficulty _selectedDifficulty = Difficulty.Easy;
    private QuestionPackViewModel? _activePack;
    private TriviaCategory? _selectedCategory;

    public PlayerViewModel? PlayerViewModel { get; }
    public ConfigurationViewModel? ConfigurationViewModel { get; }

    public Difficulty SelectedDifficulty
    {
        get => _selectedDifficulty;
        set => SetProperty(ref _selectedDifficulty, value);
    }

    public int QuestionAmount
    {
        get => _questionAmount;
        set => SetProperty(ref _questionAmount, value);
    }

    public TriviaCategory? SelectedCategory
    {
        get => _selectedCategory;
        set => SetProperty(ref _selectedCategory, value);
    }

    public bool IsPlayMode
    {
        get => _isplayMode;
        set => SetProperty(ref _isplayMode, value);
    }

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

    private void ShowImportQuestionsDialog()
    {
        ImportQuestionsDialog dialog = new();

        dialog.DataContext = this;

        dialog.ShowDialog();
    }

    public MainWindowViewModel()
    {
		PlayerViewModel = new PlayerViewModel(this);
        ConfigurationViewModel = new ConfigurationViewModel(this);

        ShowPlayModeCommand = new RelayCommand(_ => { IsPlayMode = true; PlayerViewModel?.StartQuiz(); });
        ShowEditModeCommand = new RelayCommand(_ => IsPlayMode = false);
        ShowImportQuestionsDialogCommand = new RelayCommand(_ => ShowImportQuestionsDialog());
        ImportQuestionsCommand = new RelayCommand(async _ => await ImportQuestionsAsync());

        var questionPack = new QuestionPack("Mina frågor!", Difficulty.Easy); //Skapar ett testpaket
        
        questionPack.Questions.Add(new Question("Vad heter sveriges huvudstad?", "Stockholm", "Oslo", "Göteborg", "London"));
        questionPack.Questions.Add(new Question("Vad är 2 + 3?", "5", "10", "8", "1")); //Lägger till frågor i testpaketet

        var packViewModel = new QuestionPackViewModel(questionPack);//Skapar ett ViewModel objekt av paketet
        
        Packs.Add(packViewModel); // Lägger till paketet i listan med QuestionPacks

        ActivePack = packViewModel; //Väljer paketet

        _ = LoadCategoriesAsync();
    }

    public async Task SavePacksAsync()
    {
        string appDataPath = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);

        string folderPath = Path.Combine(appDataPath, "Labb_3");

        Directory.CreateDirectory(folderPath);

        string filePath = Path.Combine(folderPath, "packs.json");

        List<QuestionPack> packs = Packs.Select(pack => pack.Model).ToList();
        
        await _jsonService.SaveAsync(packs, filePath);
    }

    private async Task LoadCategoriesAsync()
    {
        List<TriviaCategory> categories =
            await _openTriviaService.GetCategoriesAsync();

        Categories.Clear();

        foreach (TriviaCategory category in categories)
        {
            Categories.Add(category);
        }

        SelectedCategory = Categories.FirstOrDefault();
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

    private async Task ImportQuestionsAsync()
    {
        OpenTriviaService service = new();

        if (SelectedCategory == null)
        {
            MessageBox.Show("No category selected.");

            return;
        }

        List<TriviaQuestion> questions =
            await service.GetQuestionsAsync(
                QuestionAmount,
                SelectedCategory.Id);

        QuestionPack importedPack =
            new QuestionPack(
            "Imported Pack",
            SelectedDifficulty);

        foreach (TriviaQuestion triviaQuestion in questions)
        {
            Question question = new Question(
                triviaQuestion.Question,
                triviaQuestion.CorrectAnswer,
                triviaQuestion.IncorrectAnswers[0],
                triviaQuestion.IncorrectAnswers[1],
                triviaQuestion.IncorrectAnswers[2]);

            importedPack.Questions.Add(question);
        }

        QuestionPackViewModel packViewModel =
            new QuestionPackViewModel(importedPack);

        Packs.Add(packViewModel);

        ActivePack = packViewModel;

        MessageBox.Show(
            $"Imported {questions.Count} questions");
    }
}
