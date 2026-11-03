using Labb_3.Command;
using Labb_3.Models;
using System.Collections.ObjectModel;

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

    public PlayerViewModel? PlayerViewModel { get; }
    public ConfigurationViewModel? ConfigurationViewModel { get; }


    public MainWindowViewModel()
    {
		PlayerViewModel = new PlayerViewModel(this);
        ConfigurationViewModel = new ConfigurationViewModel(this);

        

        var questionPack = new QuestionPack("Mina frågor!", Difficulty.Easy); //Skapar ett testpaket
        
        questionPack.Questions.Add(new Question("Vad heter sveriges huvudstad?", "Stockholm", "Oslo", "Göteborg", "London"));
        questionPack.Questions.Add(new Question("Vad är 2 + 3?", "5", "10", "8", "1")); //Lägger till frågor i testpaketet

        var packViewModel = new QuestionPackViewModel(questionPack);//Skapar ett ViewModel objekt av paketet
        
        Packs.Add(packViewModel); // Lägger till paketet i listan med QuestionPacks

        ActivePack = packViewModel; //Väljer paketet
    }
}
