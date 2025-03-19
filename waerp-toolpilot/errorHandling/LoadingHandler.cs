using System;
using System.Threading;
using System.Windows;

namespace waerp_toolpilot.errorHandling
{
    public class LoadingHandler
    {
        private Thread StatusThread = null;

        private BufferLoader_Error Popup = null;

        public void Start()
        {
            //create the thread with its ThreadStart method
            StatusThread = new Thread(() =>
            {
                try
                {
                    Popup = new BufferLoader_Error();
                    Popup.Show();
                    Popup.Closed += (lsender, le) =>
                    {
                        //when the window closes, close the thread invoking the shutdown of the dispatcher
                        Popup.Dispatcher.InvokeShutdown();
                        Popup = null;
                        StatusThread = null;
                    };

                    //this call is needed so the thread remains open until the dispatcher is closed
                    System.Windows.Threading.Dispatcher.Run();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message + "\r\n" + ex.StackTrace);
                }
            });

            //run the thread in STA mode to make it work correctly
            StatusThread.SetApartmentState(ApartmentState.STA);
            StatusThread.Priority = ThreadPriority.Normal;
            StatusThread.Start();
        }

        public void Stop()
        {
            if (Popup != null)
            {
                //need to use the dispatcher to call the Close method, because the window is created in another thread, and this method is called by the main thread
                Popup.Dispatcher.BeginInvoke(new Action(() =>
                {
                    Popup.Close();
                }));
            }
        }
    }
}
