namespace Labb_3.ViewModels;

using Labb_3.Command;
using Labb_3.Models;
using System.Linq;
internal class PlayerViewModel : ViewModelBase
{
    private readonly MainWindowViewModel? _mainWindowViewModel;

    public string ResultText { get; set; } = "";

    public RelayCommand AnswerCommand { get; }

    public PlayerViewModel(MainWindowViewModel? mainWindowViewModel)
    {
        _mainWindowViewModel = mainWindowViewModel;

        AnswerCommand = new RelayCommand(answer => CheckAnswer(answer));
    }

    public Question? CurrentQuestion
    {
        get
        {
            if (_mainWindowViewModel?.ActivePack == null)
            {
                return null;
            }

            if (CurrentQuestionIndex >= _mainWindowViewModel.ActivePack.Questions.Count)
            {
                return null;
            }
            
            return _mainWindowViewModel.ActivePack.Questions[CurrentQuestionIndex];
        }
    }

    public List<string> CurrentQuestionAnswers
    {
        get
        {
            if (CurrentQuestion == null)
            {
                return new();
            }

            List<string> answers = new();

            answers.Add(CurrentQuestion.CorrectAnswer);

            foreach (string answer in CurrentQuestion.IncorrectAnswers)
            {
                answers.Add(answer);
            }

            return answers;
        }
    }

    public List<string> Answers
    {
        get
        {
            if (CurrentQuestion == null)
            {
                return new();
            }
            
            List<string> answers = new();
            
            answers.Add(CurrentQuestion.CorrectAnswer);
            
            foreach (string answer in CurrentQuestion.IncorrectAnswers)
            {
                answers.Add(answer);
            }
            
            return answers;
        }
    }

    private void CheckAnswer(object? answer)
    {
        if (CurrentQuestion == null)
        {
            return;
        }

        if ((string)answer! == CurrentQuestion.CorrectAnswer)
        {
            ResultText = "Correct!";
        }
        else
        {
            ResultText = "Wrong!";
        }

        RaisePropertyChanged(nameof(ResultText));

        NextQuestion();
    }

    private int _currentQuestionIndex = 0;

    public int CurrentQuestionIndex
    {
        get => _currentQuestionIndex;
        set
        {
            if (SetProperty(ref _currentQuestionIndex, value))
            {
                RaisePropertyChanged(nameof(CurrentQuestion));
                RaisePropertyChanged(nameof(Answers));
            }
        }
    }

    private void NextQuestion()
    {
        if (_mainWindowViewModel?.ActivePack == null)
        {
            return;
        }

        if (CurrentQuestionIndex < _mainWindowViewModel.ActivePack.Questions.Count - 1)
        {
            CurrentQuestionIndex++;
        }
    }
}

