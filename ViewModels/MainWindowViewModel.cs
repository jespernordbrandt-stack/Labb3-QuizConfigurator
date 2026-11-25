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
    public RelayCommand ExitCommand { get; }

    private readonly JsonService _jsonService = new();
    private readonly OpenTriviaService _openTriviaService = new();

    private bool _isplayMode;
    private int _importQuestionAmount = 5;
    private Difficulty _selectedImportDifficulty = Difficulty.Easy;
    private QuestionPackViewModel? _activePack;
    private TriviaCategory? _selectedImportCategory;

    public PlayerViewModel? PlayerViewModel { get; }
    public ConfigurationViewModel? ConfigurationViewModel { get; }

    public List<Difficulty> ImportDifficulties { get; } =
[
    Difficulty.Easy,
    Difficulty.Medium,
    Difficulty.Hard
];

    public Difficulty SelectedImportDifficulty
    {
        get => _selectedImportDifficulty;
        set => SetProperty(ref _selectedImportDifficulty, value);
    }

    public int ImportQuestionAmount
    {
        get => _importQuestionAmount;
        set => SetProperty(ref _importQuestionAmount, value);
    }

    public TriviaCategory? SelectedImportCategory
    {
        get => _selectedImportCategory;
        set => SetProperty(ref _selectedImportCategory, value);
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
        ExitCommand = new RelayCommand(_ => Application.Current.Shutdown());

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
        MessageBoxResult result = MessageBox.Show(
            "Do you want to save all current packs?",
            "Save Packs",
            MessageBoxButton.YesNo,
            MessageBoxImage.Question);

        if (result == MessageBoxResult.No)
        {
            return;
        }

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

        SelectedImportCategory = Categories.FirstOrDefault();
    }

    public async Task LoadPacksAsync()
    {
        MessageBoxResult result = MessageBox.Show(
            "Loading packs will replace your current unsaved changes.\n\nDo you want to continue?",
            "Load Packs",
            MessageBoxButton.YesNo,
            MessageBoxImage.Warning);

        if (result == MessageBoxResult.No)
        {
            return;
        }

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

        ActivePack = Packs.FirstOrDefault();
    }

    private async Task ImportQuestionsAsync()
    {
        

        if (SelectedImportCategory == null)
        {
            MessageBox.Show("No category selected.");

            return;
        }

        List<TriviaQuestion> questions =
            await _openTriviaService.GetQuestionsAsync(
                ImportQuestionAmount,
                SelectedImportCategory.Id,
                SelectedImportDifficulty.ToString().ToLower());

        if (questions.Count == 0)
        {
            return;
        }

        QuestionPack importedPack =
            new QuestionPack(
            "Imported Pack",
            SelectedImportDifficulty);

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
