using SQLite;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WorkTimer.Models
{
    public class Project : INotifyPropertyChanged
    {
        [PrimaryKey, AutoIncrement]
        public int ProjectId { get; set; }

        public string Title
        {
            get { return _Title; }
            set { _Title = value; OnPropertyChanged("Title"); GetTasks(); }
        }
        private string _Title;

        public int Seconds
        {
            get { return _Seconds; }
            set { _Seconds = value; OnPropertyChanged("Seconds"); OnPropertyChanged("FormattedTotalTime"); }
        }
        private int _Seconds;

        public DateTime CreatedOn { get; set; }
        public DateTime? ModifiedOn { get; set; }

        [Ignore]
        public List<PTask> Tasks
        {
            get { return _Tasks; }
            set { _Tasks = value; OnPropertyChanged("Tasks"); OnPropertyChanged("FormattedTaskQuantity"); }
        }
        private List<PTask> _Tasks;

        [Ignore]
        public string FormattedTotalTime
        {
            get
            {
                return TimeSpan.FromSeconds(Seconds).ToString(@"hh\:mm\:ss");               
            }
        }

        [Ignore]
        public TimeSpan TotalTimeSpan
        {
            get
            {
                if (Seconds != 0)
                    return TimeSpan.FromSeconds(Seconds);
                else
                    return TimeSpan.FromSeconds(0);
            }
        }

        [Ignore]
        public string FormattedTaskQuantity
        {
            get
            {
                GetTasks();
                string Ending = "";

                if(Tasks == null)
                    return "0 zadań";

                if (Tasks.Count == 0 || Tasks.Count >= 5)
                    Ending = " zadań";
                else if (Tasks.Count == 1)
                    Ending = " zadanie";
                else
                    Ending = " zadania";

                return Tasks.Count + Ending;
            }
        }

        public async void GetTasks()
        {
            Tasks = await App.Database.GetAllTasks(ProjectId);
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        protected void OnPropertyChanged(PropertyChangedEventArgs e)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null)
                handler(this, e);
        }
    }
}
