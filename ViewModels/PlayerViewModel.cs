namespace Labb_3.ViewModels;

using Labb_3.Command;
using Labb_3.Models;
using System.Linq;
internal class PlayerViewModel : ViewModelBase
{
    private readonly MainWindowViewModel? _mainWindowViewModel;

    public string ResultText { get; set; } = "";

    public RelayCommand AnswerCommand { get; }

    private int _score;

    public int Score
    {
        get => _score;
        set => SetProperty(ref _score, value);
    }

    public PlayerViewModel(MainWindowViewModel? mainWindowViewModel)
    {
        _mainWindowViewModel = mainWindowViewModel;

        AnswerCommand = new RelayCommand(answer => CheckAnswer(answer));
    }

    public Question? CurrentQuestion
    {
        get
        {
            if (IsQuizFinished)
            {
                return null;
            }

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
            if (IsQuizFinished || CurrentQuestion == null)
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

            Random random = new();

            answers = answers.OrderBy(_ => random.Next()).ToList();

            return answers;
        }
    }

    private bool _isQuizFinished;

    public bool IsQuizFinished
    {
        get => _isQuizFinished;
        set => SetProperty(ref _isQuizFinished, value);
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
            Score ++;

            RaisePropertyChanged(nameof(ResultScreenText));
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
                RaisePropertyChanged(nameof(ResultScreenText));
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

        else
        {
            IsQuizFinished = true;

            RaisePropertyChanged(nameof(ResultScreenText));
        }
    }

    public string ResultScreenText
    {
        get
        {
            if (!IsQuizFinished)
            {
                return "";
            }

            if (_mainWindowViewModel?.ActivePack == null)
            {
                return "";
            }
            
            return $"Quiz Completed! You scored {Score} out of {_mainWindowViewModel.ActivePack.Questions.Count}!";
        }
    }
}

