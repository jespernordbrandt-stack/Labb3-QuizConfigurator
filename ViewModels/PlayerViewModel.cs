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
            return _mainWindowViewModel?.ActivePack?.Questions.FirstOrDefault();
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
    }
}

