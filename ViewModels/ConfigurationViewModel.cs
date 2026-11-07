using Labb_3.Command;
using Labb_3.Models;
using System.Collections.ObjectModel;

namespace Labb_3.ViewModels;

internal class ConfigurationViewModel : ViewModelBase
{
    private readonly MainWindowViewModel? _mainWindowViewModel;

    public RelayCommand ChangePackNameCommand { get; }

    public RelayCommand AddPackCommand { get; } //Command som lägger till ett nytt QuestionPack

    public RelayCommand RemovePackCommand { get; } //Command som tar bort ett QuestionPack

    public RelayCommand AddQuestionCommand { get; } //Command som lägger till en ny fråga i det aktiva QuestionPack

    public RelayCommand RemoveQuestionCommand { get; } //Command som tar bort en fråga från det aktiva QuestionPack

    public RelayCommand SavePacksCommand { get; }

    public RelayCommand LoadPacksCommand { get; }

    private Question? _selectedQuestion;

    public Question? SelectedQuestion
    {
        get => _selectedQuestion;
        set => SetProperty(ref _selectedQuestion, value);
    }

    private string _demoText = string.Empty;
    public string DemoText {
        get => _demoText;
        set
        {
            if (SetProperty(ref _demoText, value))
            {
                ChangePackNameCommand.RaiseCanExecuteChanged();
            }
        }
    }

    public ObservableCollection<QuestionPackViewModel> Packs
    {
        get => _mainWindowViewModel!.Packs;
    }

    public QuestionPackViewModel? ActivePack 
    { 
        get => _mainWindowViewModel?.ActivePack; 
        set
        {
            if (_mainWindowViewModel != null) 
            {
                _mainWindowViewModel.ActivePack = value;
            }
        }
    }

    public void RaiseActivePackChanged() => RaisePropertyChanged(nameof(ActivePack));

    public ConfigurationViewModel(MainWindowViewModel? mainWindowViewModel)
    {
        _mainWindowViewModel = mainWindowViewModel;

        ChangePackNameCommand = new RelayCommand(_ => ChangePackName(), _ => DemoText != string.Empty);

        AddPackCommand = new RelayCommand(_ => AddPack()); //kopplar AddPackCommand till AddPack metoden

        RemovePackCommand = new RelayCommand(_=> RemovePack(), _=> ActivePack != null); //kopplar RemovePackCommand till RemovePack metoden och gör att den bara kan köras om ActivePack inte är null

        AddQuestionCommand = new RelayCommand(_ => AddQuestion(), _ => ActivePack != null); //kopplar AddQuestionCommand till AddQuestion metoden och gör att den bara kan köras om ActivePack inte är null

        RemoveQuestionCommand = new RelayCommand(_ => RemoveQuestion(), _ => ActivePack != null); //kopplar RemoveQuestionCommand till RemoveQuestion metoden och gör att den bara kan köras om ActivePack inte är null och det finns frågor i ActivePack

        SavePacksCommand = new RelayCommand(async _ => await SaveAsync());

        LoadPacksCommand = new RelayCommand(async _ => await LoadAsync());
    }

    private bool CanChangePackName(object? arg)
    {
        return DemoText != string.Empty;
    }

    private void ChangePackName()
    {
        ActivePack?.Name = DemoText;
    }

    private void AddPack() //Skapar ett nytt QuestionPack och lägger till det i listan med QuestionPacks i MainWindowViewModel
    {
        QuestionPack questionPack = new QuestionPack("Nytt pack", Difficulty.Easy); //skapar ett nytt paket
        QuestionPackViewModel packViewModel = new QuestionPackViewModel(questionPack); //skapar en viewmodel för paketet
        _mainWindowViewModel?.Packs.Add(packViewModel); //lägger till paketet i listan med QuestionPacks i MainWindowViewModel
        _mainWindowViewModel?.ActivePack = packViewModel; //gör det nya paketet till det aktiva paketet
    }

    private void RemovePack()
    {
        if (ActivePack == null)
        {
            return;
        }
        _mainWindowViewModel?.Packs.Remove(ActivePack); //Tar bort det aktiva paketet från listan med QuestionPacks i MainWindowViewModel
        _mainWindowViewModel!.ActivePack = _mainWindowViewModel.Packs.FirstOrDefault();
    }

    private void AddQuestion()
    {
        if (ActivePack == null)
        {
            return;
        }
        Question question = new Question("Ny fråga", "Rätt svar", "Fel svar 1", "Fel svar 2", "Fel svar 3"); //Skapar en ny fråga

        ActivePack.AddQuestion(question); //Lägger till frågan i det aktiva paketet
    }

    private void RemoveQuestion()
    {
        if (ActivePack == null)
        {
            return;
        }

        if (SelectedQuestion == null)
        {
            return;
        }

        ActivePack.RemoveQuestion(SelectedQuestion);

        SelectedQuestion = null;
    }

    public async Task SaveAsync()
    {
        await _mainWindowViewModel!.SavePacksAsync();
    }

    public async Task LoadAsync()
    {
        await _mainWindowViewModel!.LoadPacksAsync();
    }
}
