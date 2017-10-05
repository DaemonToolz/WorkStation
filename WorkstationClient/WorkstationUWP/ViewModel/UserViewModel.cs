using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WorkstationUWP.ViewModel
{

        public sealed class UserViewModel : INotifyPropertyChanged
        {
            public event PropertyChangedEventHandler PropertyChanged;

            private string _username;
            public string Username{
                get { return _username; }
                set
                {
                    _username = value;
                    if (PropertyChanged != null)
                    {
                        PropertyChanged.Invoke(this, new PropertyChangedEventArgs(nameof(Username)));
                    }
                }
            }
        }
    
}
