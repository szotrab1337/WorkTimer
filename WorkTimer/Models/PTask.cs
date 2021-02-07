using SQLite;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WorkTimer.Models
{
    public class PTask : INotifyPropertyChanged
    {
        [PrimaryKey, AutoIncrement]
        public int TaskId { get; set; }
        public int ProjectId { get; set; }

        public string Title
        {
            get { return _Title; }
            set { _Title = value; OnPropertyChanged("Title"); }
        }
        private string _Title;

        public int Seconds
        {
            get { return _Seconds; }
            set { _Seconds = value; OnPropertyChanged("Seconds"); OnPropertyChanged("FormatedTaskTime"); }
        }
        private int _Seconds;

        public DateTime CreatedOn { get; set; }

        public DateTime? ModifiedOn
        {
            get { return _ModifiedOn; }
            set { _ModifiedOn = value; OnPropertyChanged("ModifiedOn"); OnPropertyChanged("FormatedModifiedOn"); }
        }
        private DateTime? _ModifiedOn;

        [Ignore]
        public int Number
        {
            get
            {
                return _Number;
            }
            set
            {
                _Number = value;
                OnPropertyChanged("Number");
            }
        }
        private int _Number;

        [Ignore]
        public string FormatedTaskTime
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
        public string FormatedModifiedOn
        {
            get
            {
                if (ModifiedOn == null)
                    return "----";
                else
                    return ModifiedOn.Value.ToString(@"dd-MM-yyyy HH:mm:ss");
            }
        }
        
        [Ignore]
        public string FormatedCreatedOn
        {
            get
            {
                return CreatedOn.ToString(@"dd-MM-yyyy HH:mm:ss");
            }
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
