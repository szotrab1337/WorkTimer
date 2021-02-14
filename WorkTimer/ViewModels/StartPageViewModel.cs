using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using Microsoft.Toolkit.Uwp.Notifications;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Windows.UI.Notifications;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using WorkTimer.Models;

namespace WorkTimer.ViewModels
{
    public class StartPageViewModel : ViewModelBase
    {
        public StartPageViewModel()
        {
            AddNewProjectCommand = new RelayCommand(AddNewProjectAction);
            DeleteProjectCommand = new RelayCommand(DeleteProjectAction);
            EditProjectCommand = new RelayCommand(EditProjectAction);
            AddNewTaskCommand = new RelayCommand(AddNewTaskAction);
            DeleteTaskCommand = new RelayCommand(DeleteTaskAction);
            EditTaskCommand = new RelayCommand(EditTaskAction);
            StartCommand = new RelayCommand(StartAction);
            PauseCommand = new RelayCommand(PauseAction);
            ResumeCommand = new RelayCommand(ResumeAction);
            EndTimerCommand = new RelayCommand(EndTimerAction);

            InitializeVariables();

            LoadProjects();
            //PopToast();
        }

        #region Commands

        public ICommand AddNewProjectCommand { get; set; }
        public ICommand DeleteProjectCommand { get; set; }
        public ICommand EditProjectCommand { get; set; }
        public ICommand AddNewTaskCommand { get; set; }
        public ICommand DeleteTaskCommand { get; set; }
        public ICommand EditTaskCommand { get; set; }
        public ICommand StartCommand { get; set; }
        public ICommand PauseCommand { get; set; }
        public ICommand ResumeCommand { get; set; }
        public ICommand EndTimerCommand { get; set; }

        #endregion

        #region Attributes

        public DispatcherTimer Timer;
        public short DeltaSeconds = 0;
        public int TimerOldValue;
        public DateTime TriggerTimerTime;
        public int SelectedProjectSecondsOnStart;
        public int SelectedTaskSecondsOnStart;

        public ObservableCollection<Project> Projects
        {
            get { return _Projects; }
            set { _Projects = value; RaisePropertyChanged("Projects"); }
        }
        private ObservableCollection<Project> _Projects;

        public ObservableCollection<PTask> Tasks
        {
            get { return _Tasks; }
            set { _Tasks = value; RaisePropertyChanged("Tasks"); }
        }
        private ObservableCollection<PTask> _Tasks;

        public Project SelectedProject
        {
            get { return _SelectedProject; }
            set
            {
                _SelectedProject = value;
                RaisePropertyChanged("SelectedProject");
                IsProjectSelected = SelectedProject != null ? true : false;
                ProjectDetailsVisibility = SelectedProject != null ? "Visible" : "Collapsed";
                ProjectHolderVisibility = SelectedProject != null ? "Collapsed" : "Visible";

                if (SelectedProject != null)
                    LoadTasks();


                if (Timer != null)
                    EndTimer();
            }
        }
        private Project _SelectedProject;

        public PTask SelectedTask
        {
            get { return _SelectedTask; }
            set
            {
                _SelectedTask = value;
                RaisePropertyChanged("SelectedTask");
                TasksMenuFlyoutIsEnabled = (SelectedTask != null) && UIIsEnabled && !IsTimerActive ? true : false;

                if (Timer != null)
                    EndTimer();
            }
        }
        private PTask _SelectedTask;

        public bool IsProjectSelected
        {
            get { return _IsProjectSelected; }
            set { _IsProjectSelected = value; RaisePropertyChanged("IsProjectSelected"); }
        }
        private bool _IsProjectSelected;

        public int CurrentSessionSeconds
        {
            get { return _CurrentSessionSeconds; }
            set
            {
                _CurrentSessionSeconds = value;
                RaisePropertyChanged("CurrentSessionMiliseconds");
                CurrentSessionString = TimeSpan.FromSeconds(CurrentSessionSeconds).ToString(@"hh\:mm\:ss");
            }
        }
        private int _CurrentSessionSeconds;

        public string CurrentSessionString
        {
            get { return _CurrentSessionString; }
            set { _CurrentSessionString = value; RaisePropertyChanged("CurrentSessionString"); }
        }
        private string _CurrentSessionString;
        
        public string StatusText
        {
            get { return _StatusText; }
            set { _StatusText = value; RaisePropertyChanged("StatusText"); }
        }
        private string _StatusText;

        public bool TasksMenuFlyoutIsEnabled
        {
            get { return _TasksMenuFlyoutIsEnabled; }
            set { _TasksMenuFlyoutIsEnabled = value; RaisePropertyChanged("TasksMenuFlyoutIsEnabled"); }
        }
        private bool _TasksMenuFlyoutIsEnabled;

        public bool UIIsEnabled
        {
            get { return _UIIsEnabled; }
            set { _UIIsEnabled = value; RaisePropertyChanged("UIIsEnabled"); }
        }
        private bool _UIIsEnabled;

        public string ProjectDetailsVisibility
        {
            get { return _ProjectDetailsVisibility; }
            set { _ProjectDetailsVisibility = value; RaisePropertyChanged("ProjectDetailsVisibility"); }
        }
        private string _ProjectDetailsVisibility;

        public string PauseButtonVisibility
        {
            get { return _PauseButtonVisibility; }
            set { _PauseButtonVisibility = value; RaisePropertyChanged("PauseButtonVisibility"); }
        }
        private string _PauseButtonVisibility;

        public string ResumeButtonVisibility
        {
            get { return _ResumeButtonVisibility; }
            set { _ResumeButtonVisibility = value; RaisePropertyChanged("ResumeButtonVisibility"); }
        }
        private string _ResumeButtonVisibility;

        public string ActionButtonsVisibility
        {
            get { return _ActionButtonsVisibility; }
            set { _ActionButtonsVisibility = value; RaisePropertyChanged("ActionButtonsVisibility"); }
        }
        private string _ActionButtonsVisibility;

        public string StartButtonVisibility
        {
            get { return _StartButtonVisibility; }
            set { _StartButtonVisibility = value; RaisePropertyChanged("StartButtonVisibility"); }
        }
        private string _StartButtonVisibility;

        public string ProjectHolderVisibility
        {
            get { return _ProjectHolderVisibility; }
            set { _ProjectHolderVisibility = value; RaisePropertyChanged("ProjectHolderVisibility"); }
        }
        private string _ProjectHolderVisibility;

        public bool IsTimerActive
        {
            get { return _IsTimerActive; }
            set
            {
                _IsTimerActive = value;
                RaisePropertyChanged("IsTimerActive");
                ActionButtonsVisibility = IsTimerActive ? "Visible" : "Collapsed";
                StartButtonVisibility = IsTimerActive ? "Collapsed" : "Visible";
                TasksMenuFlyoutIsEnabled = (SelectedTask != null) && UIIsEnabled && !IsTimerActive ? true : false;
            }
        }
        private bool _IsTimerActive;

        #endregion

        private async void ShowConfirmContentDialog(string Title, string PrimaryButtonText, string Content)
        {
            try
            {
                ContentDialog dialog = new ContentDialog()
                {
                    Title = Title,
                    PrimaryButtonText = PrimaryButtonText,
                    Content = Content,
                    RequestedTheme = Windows.UI.Xaml.ElementTheme.Default
                };

                ContentDialogResult result = await dialog.ShowAsync();
            }
            catch (Exception ex)
            {
                await Logger.LogMessage(ex.ToString(), 3);
                ContentDialog error = new ContentDialog
                {
                    Title = "Błąd",
                    Content = "Wystąpił błąd! Sprawdź logi programu",
                    PrimaryButtonText = "Potwierdź",
                    RequestedTheme = Windows.UI.Xaml.ElementTheme.Default
                };
                await error.ShowAsync();
            }
        }

        private async void InitializeVariables()
        {
            try
            {
                await Logger.LogMessage("Start programu");
                await Logger.LogMessage("Rozpoczęcie przypisania wartości startowych do zmiennych");

                UIIsEnabled = true;
                CurrentSessionSeconds = 0;
                TimerOldValue = 0;
                IsTimerActive = false;
                ProjectDetailsVisibility = "Collapsed";
                ProjectHolderVisibility = "Visible";
                ResumeButtonVisibility = "Collapsed";
                PauseButtonVisibility = "Visible";

                await Logger.LogMessage("Wartości przypisane do zmiennych pomyślnie", 0);
            }
            catch (Exception ex)
            {
                await Logger.LogMessage(ex.ToString(), 3);
                ShowConfirmContentDialog("Błąd", "Potwierdź", "Wystąpił błąd! Sprawdź logi programu.");
            }
        }

        private async Task<string> ShowInputContentDialog(string Placeholder, string Title, string PrimaryButtonText,
            string SecondaryButtonText, string Text)
        {
            try
            {
                await Logger.LogMessage("Wyświetlenie okna do wprowadzania wartości");

                TextBox input = new TextBox()
                {
                    Height = (double)App.Current.Resources["TextControlThemeMinHeight"],
                    PlaceholderText = Placeholder,
                    Text = Text
                };

                ContentDialog dialog = new ContentDialog()
                {
                    Title = Title,
                    PrimaryButtonText = PrimaryButtonText,
                    SecondaryButtonText = SecondaryButtonText,
                    Content = input
                };

                ContentDialogResult result = await dialog.ShowAsync();

                await Logger.LogMessage("Zatwierdzono", 0);

                if (result == ContentDialogResult.Secondary)
                {
                    await Logger.LogMessage("Anulowano operację");
                    return "-1";
                }

                return ((TextBox)dialog.Content).Text;
            }
            catch (Exception ex)
            {
                await Logger.LogMessage(ex.ToString(), 3);
                ShowConfirmContentDialog("Błąd", "Potwierdź", "Wystąpił błąd! Sprawdź logi programu.");
                return string.Empty;
            }
        }

        private async Task<bool> ShowYesNoContentDialog(string Title, string PrimaryButtonText, string SecondaryButtonText, string Content)
        {
            try
            {
                await Logger.LogMessage("Wyświetlenia okna zapytania");

                ContentDialog dialog = new ContentDialog()
                {
                    Title = Title,
                    PrimaryButtonText = PrimaryButtonText,
                    SecondaryButtonText = SecondaryButtonText,
                    Content = Content
                };

                ContentDialogResult result = await dialog.ShowAsync();

                await Logger.LogMessage("Zatwierdzono", 0);

                if (result == ContentDialogResult.Secondary)
                {
                    await Logger.LogMessage("Anulowano", 1);
                    return false;
                }

                return true;
            }
            catch (Exception ex)
            {
                await Logger.LogMessage(ex.ToString(), 3);
                ShowConfirmContentDialog("Błąd", "Potwierdź", "Wystąpił błąd! Sprawdź logi programu.");
                return false;
            }
        }

        private async void AddNewProjectAction()
        {
            try
            {
                await Logger.LogMessage("Dodawanie nowego projeku");

                string ProjectTitle = await ShowInputContentDialog("Wprowadź nazwę projektu", "Dodaj projekt", "Dodaj", "Anuluj", string.Empty);

                if (string.IsNullOrEmpty(ProjectTitle))
                {
                    LocalNotification localNotification = new LocalNotification { Content = "Wprowadzono błędną nazwę projektu", Duration = 2500 };
                    Messenger.Default.Send(new NotificationMessage<LocalNotification>(localNotification, "NewLocalNotification"));
                    await Logger.LogMessage("Wprowadzono błędną nazwę projektu", 3);
                    return;
                }

                if (ProjectTitle == "-1")
                    return;

                Project newProject = new Project()
                {
                    Title = ProjectTitle,
                    CreatedOn = DateTime.Now,
                    Seconds = 0,
                };

                await App.Database.AddNewProject(newProject);
                Projects.Add(newProject);

                await Logger.LogMessage("Pomyślnie dodano nowy projekt", 0);
            }
            catch (Exception ex)
            {
                await Logger.LogMessage(ex.ToString(), 3);
                ShowConfirmContentDialog("Błąd", "Potwierdź", "Wystąpił błąd! Sprawdź logi programu.");
            }
        }

        private async void DeleteProjectAction()
        {
            try
            {
                await Logger.LogMessage("Usuwanie projektu");

                bool Decision = await ShowYesNoContentDialog("Usuwanie", "Usuń", "Anuluj", "Czy na pewno chcesz usunąć projekt?");

                if (!Decision)
                    return;

                await App.Database.RemoveProject(SelectedProject);
                Projects.Remove(Projects.Where(x => x.ProjectId == SelectedProject.ProjectId).FirstOrDefault());

                await Logger.LogMessage("Usunięto projekt", 0);
            }
            catch (Exception ex)
            {
                await Logger.LogMessage(ex.ToString(), 3);
                ShowConfirmContentDialog("Błąd", "Potwierdź", "Wystąpił błąd! Sprawdź logi programu.");
            }
        }

        private async void EditProjectAction()
        {
            try
            {
                await Logger.LogMessage("Rozpoczęcie edycji projektu");

                string ProjectTitle = await ShowInputContentDialog("Wprowadź nazwę projektu", "Edycja projektu", "Zapisz", "Anuluj", SelectedProject.Title);

                if (string.IsNullOrEmpty(ProjectTitle))
                {
                    LocalNotification localNotification = new LocalNotification { Content = "Wprowadzono błędną nazwę projektu", Duration = 2500 };
                    Messenger.Default.Send(new NotificationMessage<LocalNotification>(localNotification, "NewLocalNotification"));
                    await Logger.LogMessage("Wprowadzono błędną nazwę projektu", 3);
                    return;
                }

                if (ProjectTitle == "-1")
                    return;

                Project editedProject = SelectedProject;
                editedProject.Title = ProjectTitle;
                editedProject.ModifiedOn = DateTime.Now;

                await App.Database.UpdateProject(editedProject);
                Projects.Where(x => x.ProjectId == SelectedProject.ProjectId).FirstOrDefault().Title = ProjectTitle;

                await Logger.LogMessage("Edycja projektu zakończona powodzeniem", 0);
            }
            catch (Exception ex)
            {
                await Logger.LogMessage(ex.ToString(), 3);
                ShowConfirmContentDialog("Błąd", "Potwierdź", "Wystąpił błąd! Sprawdź logi programu.");
            }
        }

        private async void EditTaskAction()
        {
            try
            {
                await Logger.LogMessage("Rozpoczęcie edycji zadania");

                string TaskTitle = await ShowInputContentDialog("Wprowadź nazwę zadania", "Edycja zadania", "Zapisz", "Anuluj", SelectedTask.Title);

                if (string.IsNullOrEmpty(TaskTitle))
                {
                    LocalNotification localNotification = new LocalNotification { Content = "Wprowadzono błędną nazwę zadania", Duration = 2500 };
                    Messenger.Default.Send(new NotificationMessage<LocalNotification>(localNotification, "NewLocalNotification"));
                    await Logger.LogMessage("Wprowadzono błędną nazwę zadania", 3);
                    return;
                }

                if (TaskTitle == "-1")
                    return;

                PTask editedTask = SelectedTask;
                editedTask.Title = TaskTitle;
                editedTask.ModifiedOn = DateTime.Now;

                await App.Database.UpdateTask(editedTask);
                Tasks.Where(x => x.TaskId == SelectedTask.TaskId).FirstOrDefault().Title = TaskTitle;
                Tasks.Where(x => x.TaskId == SelectedTask.TaskId).FirstOrDefault().ModifiedOn = editedTask.ModifiedOn;

                await Logger.LogMessage("Edycja zadania zakończona powodzeniem", 0);
            }
            catch (Exception ex)
            {
                await Logger.LogMessage(ex.ToString(), 3);
                ShowConfirmContentDialog("Błąd", "Potwierdź", "Wystąpił błąd! Sprawdź logi programu.");
            }
        }

        private async void DeleteTaskAction()
        {
            try
            {
                await Logger.LogMessage("Usuwanie zadania");

                bool Decision = await ShowYesNoContentDialog("Usuwanie", "Usuń", "Anuluj", "Czy na pewno chcesz usunąć zadanie?");

                if (!Decision)
                    return;

                await App.Database.RemoveTask(SelectedTask);
                Tasks.Remove(Tasks.Where(x => x.TaskId == SelectedTask.TaskId).FirstOrDefault());
                Projects.Where(x => x.ProjectId == SelectedProject.ProjectId).FirstOrDefault().GetTasks();
                RenumberTasks();

                await Logger.LogMessage("Usunięto zadanie", 0);
            }
            catch (Exception ex)
            {
                await Logger.LogMessage(ex.ToString(), 3);
                ShowConfirmContentDialog("Błąd", "Potwierdź", "Wystąpił błąd! Sprawdź logi programu.");
            }
        }

        private async void LoadProjects()
        {
            try
            {
                await Logger.LogMessage("Rozpoczęto wczytywanie listy projektów");
                Projects = new ObservableCollection<Project>(await App.Database.GetAllProjects());
                await Logger.LogMessage("Lista projektów załadowana pomyślnie", 0);
            }
            catch (Exception ex)
            {
                await Logger.LogMessage(ex.ToString(), 3);
                ShowConfirmContentDialog("Błąd", "Potwierdź", "Wystąpił błąd! Sprawdź logi programu.");
            }
        }

        private async void LoadTasks()
        {
            try
            {
                await Logger.LogMessage("Rozpoczęto ładowanie listy zadań");

                Tasks = new ObservableCollection<PTask>(await App.Database.GetAllTasks(SelectedProject.ProjectId));
                RenumberTasks();

                await Logger.LogMessage("Lista zadań załadowana pomyślnie", 0);
            }
            catch (Exception ex)
            {
                await Logger.LogMessage(ex.ToString(), 3);
                ShowConfirmContentDialog("Błąd", "Potwierdź", "Wystąpił błąd! Sprawdź logi programu.");
            }
        }

        private async void AddNewTaskAction()
        {
            try
            {
                await Logger.LogMessage("Dodawanie nowego zadania");

                string TaskTitle = await ShowInputContentDialog("Wprowadź nazwę zadania", "Dodaj zadanie", "Dodaj", "Anuluj", string.Empty);

                if (string.IsNullOrEmpty(TaskTitle))
                {
                    LocalNotification localNotification = new LocalNotification { Content = "Wprowadzono błędną nazwę zadania", Duration = 2500 };
                    Messenger.Default.Send(new NotificationMessage<LocalNotification>(localNotification, "NewLocalNotification"));
                    await Logger.LogMessage("Wprowadzono błędną nazwę zadania", 3);
                    return;
                }

                if (TaskTitle == "-1")
                    return;

                PTask newTask = new PTask()
                {
                    Title = TaskTitle,
                    CreatedOn = DateTime.Now,
                    Seconds = 0,
                    ProjectId = SelectedProject.ProjectId
                };

                await App.Database.AddNewTask(newTask);
                Tasks.Add(newTask);
                Projects.Where(x => x.ProjectId == SelectedProject.ProjectId).FirstOrDefault().GetTasks();
                RenumberTasks();

                await Logger.LogMessage("Pomyślnie dodano nowe zadanie", 3);
            }
            catch (Exception ex)
            {
                await Logger.LogMessage(ex.ToString(), 3);
                ShowConfirmContentDialog("Błąd", "Potwierdź", "Wystąpił błąd! Sprawdź logi programu.");
            }
        }

        private async void RenumberTasks()
        {
            try
            {
                await Logger.LogMessage("Nadawanie numerów do zadań");

                for (int i = 0; i < Tasks.Count; i++)
                {
                    Tasks[i].Number = i + 1;
                }

                await Logger.LogMessage("Numerowanie zakończone powodzeniem", 0);
            }
            catch (Exception ex)
            {
                await Logger.LogMessage(ex.ToString(), 3);
                ShowConfirmContentDialog("Błąd", "Potwierdź", "Wystąpił błąd! Sprawdź logi programu.");
            }
        }

        private async void StartAction()
        {
            try
            {
                await Logger.LogMessage("Uruchomienie stopera");

                IsTimerActive = true;

                Timer = new DispatcherTimer();
                Timer.Interval = TimeSpan.FromSeconds(1);
                Timer.Tick += TimerOnTick;
                SelectedProjectSecondsOnStart = SelectedProject.Seconds;
                SelectedTaskSecondsOnStart = SelectedTask != null ? SelectedTask.Seconds : 0;
                TriggerTimerTime = DateTime.Now;
                UIIsEnabled = false;
                TimerOldValue = 0;
                StatusText = "Odmierzanie czasu";

                Timer.Start();
            }
            catch (Exception ex)
            {
                await Logger.LogMessage(ex.ToString(), 3);
                ShowConfirmContentDialog("Błąd", "Potwierdź", "Wystąpił błąd! Sprawdź logi programu.");
            }
        }

        private async void PauseAction()
        {
            try
            {
                await Logger.LogMessage("Zatrzymanie timera");

                Timer.Stop();
                ResumeButtonVisibility = "Visible";
                PauseButtonVisibility = "Collapsed";
                SaveData();
                StatusText = "Odmierzanie czasu zatrzymane";

                TimerOldValue = CurrentSessionSeconds;
            }
            catch (Exception ex)
            {
                await Logger.LogMessage(ex.ToString(), 3);
                ShowConfirmContentDialog("Błąd", "Potwierdź", "Wystąpił błąd! Sprawdź logi programu.");
            }
        }

        private async void TimerOnTick(object sender, object e)
        {
            try
            {
                CurrentSessionSeconds = TimerOldValue + Convert.ToInt32((DateTime.Now - TriggerTimerTime).TotalSeconds);

                SaveData();
            }
            catch (Exception ex)
            {
                await Logger.LogMessage(ex.ToString(), 3);
                ShowConfirmContentDialog("Błąd", "Potwierdź", "Wystąpił błąd! Sprawdź logi programu.");
            }
        }

        private async void ResumeAction()
        {
            try
            {
                ResumeButtonVisibility = "Collapsed";
                PauseButtonVisibility = "Visible";
                TriggerTimerTime = DateTime.Now;
                Timer.Start();

                await Logger.LogMessage("Wznownienie stopera");
            }
            catch (Exception ex)
            {
                await Logger.LogMessage(ex.ToString(), 3);
                ShowConfirmContentDialog("Błąd", "Potwierdź", "Wystąpił błąd! Sprawdź logi programu.");
            }
        }

        private async void SaveData()
        {
            try
            {
                SelectedProject.Seconds = SelectedProjectSecondsOnStart + CurrentSessionSeconds;
                await App.Database.UpdateProject(SelectedProject);

                if (SelectedTask != null)
                {
                    SelectedTask.Seconds = SelectedTaskSecondsOnStart + CurrentSessionSeconds;
                    await App.Database.UpdateTask(SelectedTask);
                }
            }
            catch (Exception ex)
            {
                await Logger.LogMessage(ex.ToString(), 3);
                ShowConfirmContentDialog("Błąd", "Potwierdź", "Wystąpił błąd! Sprawdź logi programu.");
            }
        }

        private async void EndTimer()
        {
            try
            {
                Timer.Stop();

                UIIsEnabled = true;
                ResumeButtonVisibility = "Collapsed";
                PauseButtonVisibility = "Visible";
                IsTimerActive = false;
                CurrentSessionSeconds = 0;
                StatusText = string.Empty;

                await Logger.LogMessage("Zakończenie mierzenia czasu");
            }
            catch (Exception ex)
            {
                await Logger.LogMessage(ex.ToString(), 3);
                ShowConfirmContentDialog("Błąd", "Potwierdź", "Wystąpił błąd! Sprawdź logi programu.");
            }
        }

        private async void EndTimerAction()
        {
            try
            {
                EndTimer();
            }
            catch (Exception ex)
            {
                await Logger.LogMessage(ex.ToString(), 3);
                ShowConfirmContentDialog("Błąd", "Potwierdź", "Wystąpił błąd! Sprawdź logi programu.");
            }
        }

        private void PopToast()
        {
            // Generate the toast notification content and pop the toast
            ToastContent content = GenerateToastContent();
            ToastNotificationManager.CreateToastNotifier().Show(new ToastNotification(content.GetXml()));
        }

        public static ToastContent GenerateToastContent()
        {
            var builder = new ToastContentBuilder().SetToastScenario(ToastScenario.Reminder)
                .AddToastActivationInfo("action=viewEvent&eventId=1983", ToastActivationType.Foreground)
                .AddText("Adaptive Tiles Meeting")
                .AddText("Conf Room 2001 / Building 135")
                .AddText("10:00 AM - 10:30 AM")
                .AddComboBox("snoozeTime", "15", ("1", "1 minute"),
                                                 ("15", "15 minutes"),
                                                 ("60", "1 hour"),
                                                 ("240", "4 hours"),
                                                 ("1440", "1 day"))
                .AddButton(new ToastButtonSnooze() { SelectionBoxId = "snoozeTime" })
                .AddButton(new ToastButtonDismiss());

            return builder.Content;
        }
    }
}
