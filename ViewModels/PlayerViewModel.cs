
using Labb_3.Command;
using Labb_3.Models;
using System.Linq;
using System.Windows.Threading;

namespace Labb_3.ViewModels;

internal class PlayerViewModel : ViewModelBase
{
    private readonly MainWindowViewModel? _mainWindowViewModel;
    private readonly DispatcherTimer _timer;
    private readonly DispatcherTimer _answerDelayTimer;

    private List<Question> _shuffledQuestions = new();

    public string ResultText { get; set; } = "";

    public RelayCommand AnswerCommand { get; }
    public RelayCommand RestartQuizCommand { get; }

    private int _score;
    private int _timeRemaining;
    

    public int Score
    {
        get => _score;
        set => SetProperty(ref _score, value);
    }

    public int TimeRemaining
    {
        get => _timeRemaining;
        set => SetProperty(ref _timeRemaining, value);
    }

    public PlayerViewModel(MainWindowViewModel? mainWindowViewModel)
    {
        _mainWindowViewModel = mainWindowViewModel;

        AnswerCommand = new RelayCommand(answer => CheckAnswer(answer));

        RestartQuizCommand = new RelayCommand(_ => RestartQuiz());

        _timer = new DispatcherTimer();
        _answerDelayTimer = new DispatcherTimer();

        _answerDelayTimer.Interval = TimeSpan.FromSeconds(2);

        _answerDelayTimer.Tick += AnswerDelayTimer_Tick;

        _timer.Interval = TimeSpan.FromSeconds(1);

        _timer.Tick += Timer_Tick;

        StartTimer();


    }

    

    private void RestartQuiz()
    {
        Score = 0;
        CurrentQuestionIndex = 0;
        IsQuizFinished = false;

        ResultText = "";

        RaisePropertyChanged(nameof(CurrentQuestion));
        RaisePropertyChanged(nameof(Answers));
        RaisePropertyChanged(nameof(ResultScreenText));
        RaisePropertyChanged(nameof(Score));
        RaisePropertyChanged(nameof(ResultText));

        ShuffleQuestions();

        StartTimer();
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

            if (CurrentQuestionIndex >= _shuffledQuestions.Count)
            {
                return null;
            }

            return _shuffledQuestions[CurrentQuestionIndex];
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

        _timer.Stop();

        NextQuestion();
    }

    private void Timer_Tick(object? sender, EventArgs e)
    {
        TimeRemaining--;

        if (TimeRemaining <= 0)
        {
            _timer.Stop();

            ResultText = $"Time's up! Correct answer: {CurrentQuestion?.CorrectAnswer}";

            RaisePropertyChanged(nameof(ResultText));

            _answerDelayTimer.Start();
        }
    }

    private void AnswerDelayTimer_Tick(object? sender, EventArgs e)
    {
        _answerDelayTimer.Stop();
        NextQuestion();
    }

    public void StartQuiz()
    {
        CurrentQuestionIndex = 0;
        IsQuizFinished = false;

        ShuffleQuestions();

        RaisePropertyChanged(nameof(CurrentQuestion));
        RaisePropertyChanged(nameof(Answers));

        StartTimer();
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

    private void StartTimer()
    {
        if (_mainWindowViewModel?.ActivePack == null)
        {
            return;
        }

        _timer.Stop();

        TimeRemaining =
            _mainWindowViewModel.ActivePack.TimeLimitInSeconds;

        _timer.Start();
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

            StartTimer();

            
        }
        

        else
        {
            _timer.Stop();

            IsQuizFinished = true;

            RaisePropertyChanged(nameof(ResultScreenText));
        }
    }

    private void ShuffleQuestions()
    {
        if (_mainWindowViewModel?.ActivePack == null)
        {
            return;
        }
        Random random = new();
        _shuffledQuestions = _mainWindowViewModel.ActivePack.Questions
            .OrderBy(_ => random.Next())
            .ToList();
        RaisePropertyChanged(nameof(CurrentQuestion));
        RaisePropertyChanged(nameof(Answers));
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

