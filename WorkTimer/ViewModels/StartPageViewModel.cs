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

            UIIsEnabled = true;
            CurrentSessionSeconds = 0;
            IsTimerActive = false;
            IsTaskSelectedVisibility = "Collapsed";
            ProjectDetailsVisibility = "Collapsed";
            ProjectHolderVisibility = "Visible";
            ResumeButtonVisibility = "Collapsed";
            PauseButtonVisibility = "Visible";

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
                {
                    ProjectTimeString = SelectedProject.TotalTimeSpan.ToString(@"hh\:mm\:ss");
                    LoadTasks();
                }

                if (Timer != null)
                {
                    EndTimer();
                }

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
                IsTaskSelected = (SelectedTask != null) && UIIsEnabled ? true : false;
                IsTaskSelectedVisibility = SelectedTask != null ? "Visible" : "Collapsed";

                if (SelectedTask != null)
                    TaskTimeString = SelectedTask.TotalTimeSpan.ToString(@"hh\:mm\:ss");

                if (Timer != null)
                {
                    EndTimer();
                }
            }
        }
        private PTask _SelectedTask;
        
        public bool IsProjectSelected
        {
            get { return _IsProjectSelected; }
            set { _IsProjectSelected = value; RaisePropertyChanged("IsProjectSelected"); }
        }
        private bool _IsProjectSelected;

        public string ProjectTimeString
        {
            get { return _ProjectTimeString; }
            set { _ProjectTimeString = value; RaisePropertyChanged("ProjectTimeString"); }
        }
        private string _ProjectTimeString;

        public string TaskTimeString
        {
            get { return _TaskTimeString; }
            set { _TaskTimeString = value; RaisePropertyChanged("TaskTimeString"); }
        }
        private string _TaskTimeString;

        public ulong CurrentSessionSeconds
        {
            get { return _CurrentSessionSeconds; }
            set
            {
                _CurrentSessionSeconds = value; 
                RaisePropertyChanged("CurrentSessionMiliseconds");
                CurrentSessionString = TimeSpan.FromSeconds(CurrentSessionSeconds).ToString(@"hh\:mm\:ss");

                if (SelectedProject != null)
                    ProjectTimeString = SelectedProject.TotalTimeSpan.ToString(@"hh\:mm\:ss");

                if (SelectedTask != null)
                    TaskTimeString = SelectedTask.TotalTimeSpan.ToString(@"hh\:mm\:ss");
            }
        }
        private ulong _CurrentSessionSeconds;
        
        public string CurrentSessionString
        {
            get { return _CurrentSessionString; }
            set { _CurrentSessionString = value; RaisePropertyChanged("CurrentSessionString"); }
        }
        private string _CurrentSessionString;
        
        public string IsTaskSelectedVisibility
        {
            get { return _IsTaskSelectedVisibility; }
            set { _IsTaskSelectedVisibility = value; RaisePropertyChanged("IsTaskSelectedVisibility"); }
        }
        private string _IsTaskSelectedVisibility;
        
        public bool IsTaskSelected
        {
            get { return _IsTaskSelected; }
            set { _IsTaskSelected = value; RaisePropertyChanged("IsTaskSelected"); }
        }
        private bool _IsTaskSelected;
        
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
            }
        }
        private bool _IsTimerActive;

        #endregion

        private async Task<string> ShowInputContentDialog(string Placeholder, string Title, string PrimaryButtonText,
            string SecondaryButtonText, string Text)
        {
            try
            {
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

                if (result == ContentDialogResult.Secondary)
                    return "-1";

                return ((TextBox)dialog.Content).Text;
            }
            catch (Exception ex)
            {
                ContentDialog error = new ContentDialog
                {
                    Title = "Błąd",
                    Content = ex.ToString(),
                    PrimaryButtonText = "Potwierdź",
                    RequestedTheme = Windows.UI.Xaml.ElementTheme.Default
                };
                await error.ShowAsync();
                return string.Empty;
            }
        }

        private async Task<bool> ShowYesNoContentDialog(string Title, string PrimaryButtonText, string SecondaryButtonText, string Content)
        {
            try
            {
                ContentDialog dialog = new ContentDialog()
                {
                    Title = Title,
                    PrimaryButtonText = PrimaryButtonText,
                    SecondaryButtonText = SecondaryButtonText,
                    Content = Content
                };

                ContentDialogResult result = await dialog.ShowAsync();

                if (result == ContentDialogResult.Secondary)
                    return false;

                return true;
            }
            catch (Exception ex)
            {
                ContentDialog error = new ContentDialog
                {
                    Title = "Błąd",
                    Content = ex.ToString(),
                    PrimaryButtonText = "Potwierdź",
                    RequestedTheme = Windows.UI.Xaml.ElementTheme.Default
                };
                await error.ShowAsync();
                return false;
            }
        }

        private async void AddNewProjectAction()
        {
            try
            {
                string ProjectTitle = await ShowInputContentDialog("Wprowadź nazwę projektu", "Dodaj projekt", "Dodaj", "Anuluj", string.Empty);

                if (string.IsNullOrEmpty(ProjectTitle))
                {
                    LocalNotification localNotification = new LocalNotification { Content = "Wprowadzono błędną nazwę projektu", Duration = 2500 };
                    Messenger.Default.Send(new NotificationMessage<LocalNotification>(localNotification, "NewLocalNotification"));
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
            }
            catch (Exception ex)
            {
                ContentDialog error = new ContentDialog
                {
                    Title = "Błąd",
                    Content = ex.ToString(),
                    PrimaryButtonText = "Potwierdź",
                    RequestedTheme = Windows.UI.Xaml.ElementTheme.Default
                };
                await error.ShowAsync();
            }
        }

        private async void DeleteProjectAction()
        {
            try
            {
                bool Decision = await ShowYesNoContentDialog("Usuwanie", "Usuń", "Anuluj", "Czy na pewno chcesz usunąć projekt?");

                if (!Decision)
                    return;

                await App.Database.RemoveProject(SelectedProject);
                Projects.Remove(Projects.Where(x => x.ProjectId == SelectedProject.ProjectId).FirstOrDefault());
            }
            catch (Exception ex)
            {
                ContentDialog error = new ContentDialog
                {
                    Title = "Błąd",
                    Content = ex.ToString(),
                    PrimaryButtonText = "Potwierdź",
                    RequestedTheme = Windows.UI.Xaml.ElementTheme.Default
                };
                await error.ShowAsync();
            }
        }

        private async void EditProjectAction()
        {
            try
            {
                string ProjectTitle = await ShowInputContentDialog("Wprowadź nazwę projektu", "Edycja projektu", "Zapisz", "Anuluj", SelectedProject.Title);

                if (string.IsNullOrEmpty(ProjectTitle))
                {
                    LocalNotification localNotification = new LocalNotification { Content = "Wprowadzono błędną nazwę projektu", Duration = 2500 };
                    Messenger.Default.Send(new NotificationMessage<LocalNotification>(localNotification, "NewLocalNotification"));
                    return;
                }

                if (ProjectTitle == "-1")
                    return;

                Project editedProject = SelectedProject;
                editedProject.Title = ProjectTitle;
                editedProject.ModifiedOn = DateTime.Now;

                await App.Database.UpdateProject(editedProject);
                Projects.Where(x => x.ProjectId == SelectedProject.ProjectId).FirstOrDefault().Title = ProjectTitle;
            }
            catch (Exception ex)
            {
                ContentDialog error = new ContentDialog
                {
                    Title = "Błąd",
                    Content = ex.ToString(),
                    PrimaryButtonText = "Potwierdź",
                    RequestedTheme = Windows.UI.Xaml.ElementTheme.Default
                };
                await error.ShowAsync();
            }
        }

        private async void EditTaskAction()
        {
            try
            {
                string TaskTitle = await ShowInputContentDialog("Wprowadź nazwę zadania", "Edycja zadania", "Zapisz", "Anuluj", SelectedTask.Title);

                if (string.IsNullOrEmpty(TaskTitle))
                {
                    LocalNotification localNotification = new LocalNotification { Content = "Wprowadzono błędną nazwę zadania", Duration = 2500 };
                    Messenger.Default.Send(new NotificationMessage<LocalNotification>(localNotification, "NewLocalNotification"));
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
            }
            catch (Exception ex)
            {
                ContentDialog error = new ContentDialog
                {
                    Title = "Błąd",
                    Content = ex.ToString(),
                    PrimaryButtonText = "Potwierdź",
                    RequestedTheme = Windows.UI.Xaml.ElementTheme.Default
                };
                await error.ShowAsync();
            }
        }

        private async void DeleteTaskAction()
        {
            try
            {
                bool Decision = await ShowYesNoContentDialog("Usuwanie", "Usuń", "Anuluj", "Czy na pewno chcesz usunąć zadanie?");

                if (!Decision)
                    return;

                await App.Database.RemoveTask(SelectedTask);
                Tasks.Remove(Tasks.Where(x => x.TaskId == SelectedTask.TaskId).FirstOrDefault());
                Projects.Where(x => x.ProjectId == SelectedProject.ProjectId).FirstOrDefault().GetTasks();
                RenumberTasks();
            }
            catch (Exception ex)
            {
                ContentDialog error = new ContentDialog
                {
                    Title = "Błąd",
                    Content = ex.ToString(),
                    PrimaryButtonText = "Potwierdź",
                    RequestedTheme = Windows.UI.Xaml.ElementTheme.Default
                };
                await error.ShowAsync();
            }
        }

        private async void LoadProjects()
        {
            try
            {
                Projects = new ObservableCollection<Project>(await App.Database.GetAllProjects());
            }
            catch (Exception ex)
            {
                ContentDialog error = new ContentDialog
                {
                    Title = "Błąd",
                    Content = ex.ToString(),
                    PrimaryButtonText = "Potwierdź",
                    RequestedTheme = Windows.UI.Xaml.ElementTheme.Default
                };
                await error.ShowAsync();
            }
        }

        private async void LoadTasks()
        {
            try
            {
                Tasks = new ObservableCollection<PTask>(await App.Database.GetAllTasks(SelectedProject.ProjectId));
                RenumberTasks();
            }
            catch (Exception ex)
            {
                ContentDialog error = new ContentDialog
                {
                    Title = "Błąd",
                    Content = ex.ToString(),
                    PrimaryButtonText = "Potwierdź",
                    RequestedTheme = Windows.UI.Xaml.ElementTheme.Default
                };
                await error.ShowAsync();
            }
        }

        private async void AddNewTaskAction()
        {
            try
            {
                string TaskTitle = await ShowInputContentDialog("Wprowadź nazwę zadania", "Dodaj zadanie", "Dodaj", "Anuluj", string.Empty);

                if (string.IsNullOrEmpty(TaskTitle))
                {
                    LocalNotification localNotification = new LocalNotification { Content = "Wprowadzono błędną nazwę zadania", Duration = 2500 };
                    Messenger.Default.Send(new NotificationMessage<LocalNotification>(localNotification, "NewLocalNotification"));
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
            }
            catch (Exception ex)
            {
                ContentDialog error = new ContentDialog
                {
                    Title = "Błąd",
                    Content = ex.ToString(),
                    PrimaryButtonText = "Potwierdź",
                    RequestedTheme = Windows.UI.Xaml.ElementTheme.Default
                };
                await error.ShowAsync();
            }
        }

        private void RenumberTasks()
        {
            for (int i = 0; i< Tasks.Count;i++)
            {
                Tasks[i].Number = i + 1;
            }
        }

        private void StartAction()
        {
            IsTimerActive = true;

            Timer = new DispatcherTimer();
            Timer.Interval = TimeSpan.FromSeconds(1);
            Timer.Tick += TimerOnTick;
            Timer.Start();
            UIIsEnabled = false;
        }

        private async void PauseAction()
        {
            Timer.Stop();
            ResumeButtonVisibility = "Visible";
            PauseButtonVisibility = "Collapsed";
            SaveData();
        }

        private void TimerOnTick(object sender, object e)
        {
            CurrentSessionSeconds++;
            SelectedProject.Seconds++;

            if (SelectedTask != null)
                SelectedTask.Seconds++;

            DeltaSeconds++;

            if(DeltaSeconds == 15)
                SaveData();
        }

        private void PopToast()
        {
            // Generate the toast notification content and pop the toast
            ToastContent content = GenerateToastContent();
            ToastNotificationManager.CreateToastNotifier().Show(new ToastNotification(content.GetXml()));
        }

        private void ResumeAction()
        {
            ResumeButtonVisibility = "Collapsed";
            PauseButtonVisibility = "Visible";
            Timer.Start();
        }

        private async void SaveData()
        {
            try
            {
                Project project = SelectedProject;
                PTask task = SelectedTask != null ? SelectedTask : null;

                await App.Database.UpdateProject(project);
                if (task != null)
                    await App.Database.UpdateTask(task);

                DeltaSeconds = 0;
            }
            catch (Exception ex)
            {
                ContentDialog error = new ContentDialog
                {
                    Title = "Błąd",
                    Content = ex.ToString(),
                    PrimaryButtonText = "Potwierdź",
                    RequestedTheme = Windows.UI.Xaml.ElementTheme.Default
                };
                await error.ShowAsync();
            }
        }

        private void EndTimer()
        {
            Timer.Stop();
            SaveData();
            UIIsEnabled = true;
            ResumeButtonVisibility = "Collapsed";
            PauseButtonVisibility = "Visible";
            IsTimerActive = false;
            CurrentSessionSeconds = 0;
        }

        private void EndTimerAction()
        {
            EndTimer();
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
