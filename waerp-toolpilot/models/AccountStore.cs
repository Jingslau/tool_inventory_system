using System.ComponentModel;

public class AccountStore : INotifyPropertyChanged
{
    string _username;
    // string _password;


    private string username
    {
        get { return _username; }
        set
        {
            _username = value;

            OnPropertyChanged("username");
        }
    }

    #region INotifyPropertyChanged Members

    public event PropertyChangedEventHandler PropertyChanged;

    public void OnPropertyChanged(string name)
    {
        if (PropertyChanged != null) PropertyChanged(this, new PropertyChangedEventArgs(name));
    }

    #endregion
}