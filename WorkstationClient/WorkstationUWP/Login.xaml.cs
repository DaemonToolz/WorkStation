using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Security.Claims;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Newtonsoft.Json.Linq;
using WorkstationUWP.Common;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace WorkstationUWP
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class Login : Page{
        public Login(){
            this.InitializeComponent();
        }


        public String Name => LoginUsername.Text;
        public String Pwd => LoginPwd.Password;


        private async Task LoginAsync()
        {
            SessionWrapper newSession = null;
          
            try
            {
                var logModel = new LogInModel() { Username = Name, Password = Pwd, AutoConnect = false };

                var Token = await App.FetchToken(Name, Pwd);

                Token = (JObject.Parse(Token))["message"].ToString();


                if (Token.Split('.').Length != 3)
                    throw new Exception();

                newSession = new SessionWrapper(Name, Pwd, Token, logModel);
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex);
                newSession = null;
            }
            try
            {
                if (newSession != null)
                {
                    var result = await newSession.LogInAsync();
                    if (result)
                    {
                        App.Session = newSession;
                        await Window.Current.Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal,
                            () =>{
                                Frame.Navigate(typeof(WorkstationFrame), newSession.CurrentUser);
                            });
                    }
                }
                
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }

        private void LoginBtn_Click(object sender, RoutedEventArgs e){
            try
            {
                LoginAsync();
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex);
            }
        }
    }
}
