using Gecko;
using Gecko.Collections;
using Gecko.DOM;

using System;
using System.Diagnostics;
using System.Net;
using System.Windows.Forms;

using System.ServiceModel;
using System.ServiceModel.Channels;

using System.Web;

namespace Worker
{
    public enum State
    {
        Unknown,
        Stopped,
        Searching,
        Chatting
    };

    [ServiceContract]
    public interface IBotOperator
    {
        [OperationContract]
        int GetMyID(IntPtr handle);

        [OperationContract]
        bool ShouldIStartSearching(int id);

        [OperationContract]
        void OnBeginChat(int id);

        [OperationContract]
        void OnBeginSearch(int id);

        [OperationContract]
        void OnEndChat(int id);

        [OperationContract]
        void StateTick(int id, State state);

        [OperationContract]
        void OnNewStrangerMessage(int id);
    }

    public class ChatBot
    {
        private string[] forbidden_index =
        {
            "naakt",
            "ruilen",
            "uitwisselen",
            "ben een jongen",
            "ben jongen",
            "jongen hier",
            "geil",
            "ik ook",
            "man hier",
            "ik zoek ook",
            "j@@",
            "j @@",
            "jong hier",
            "lul",
            "doei",
            "jbijna",
            "j bijna",
            "stijve",
            "fuck",
            "kut",
            "meester",
            "Ga weg",
            "jammer",
            "homo",
            "gay",
            "flikker",
            "j hier",
            "man@@",
            "man @@",
            "jij weer",
            "zoek ok",
            "kanker",
            "rot op",
            "sukkel",
            "telkens",
            "rotop",
            "jongen@@",
            "jongen @@",
            "jongen zoek",
            "jongen die zoek",
            "jong zoek",
            "j zoek",
            "ben je meisj",
            "ben je een meisj",
            "zoek een leuk meisje",
            "zoek een meisje",
            "jonge@@",
            "jonge @@",
            "Zoek een leven",
            "pedo",
            "zus",
            "gozer",
            "kerel",
            "jongen of meisje",
            "ben een j",
            "hier jong",
            "hoer",
        };

        private string[] forbidden_starts_with =
        {
            "haay mandy hier",
            "heey geile meid 24",
            "j ",
            "meisje?",
            "jongen",
            "man",
            "kk",
            "kkr",
            ".",
            "jm"
        };

        public delegate void OnBeginSearchingDelegate(ChatBot chatbot);
        public event OnBeginSearchingDelegate OnBeginSearching;

        public delegate void OnBeginChatDelegate(ChatBot chatbot);
        public event OnBeginChatDelegate OnBeginChat;

        public delegate void OnNewMessageDelegate(ChatBot chatbot, Message message, int messageCount, int messageCountNotMine);
        public event OnNewMessageDelegate OnNewMessage;

        public delegate void OnEndChatDelegate(ChatBot chatbot);
        public event OnEndChatDelegate OnEndChat;

        private GeckoWebBrowser browser;
        private Messages messages;
        private Story story;
        private Stopwatch searchWatch;
        private IBotOperator host;
        private Timer timer;
        private int Identifier { get; set; }
        private State lastKnownState;
        private GeckoElement logwrapper { get { return browser.Document.GetElementById("logwrapper"); } }
        private GeckoElement chatStartStopButton { get { return browser.Document.GetElementById("chatStartStopButton"); } }
        private GeckoElement chatMessageInput { get { return browser.Document.GetElementById("chatMessageInput"); } }
        private GeckoElement endChatButton
        {
            get
            {
                GeckoElement modal_quit = browser.Document.GetElementById("modal-quit");
                if (modal_quit != null)
                {
                    IDomHtmlCollection<GeckoElement> elements = modal_quit.GetElementsByTagName("button");
                    if (elements.Length >= 3)
                    {
                        return elements[2];
                    }
                }
                return null;
            }
        }

        public Messages GetMessages()
        {
            return messages;
        }

        public void StopStory()
        {
            story.Stop();
        }

        public void SetProxy(string host, int port)
        {
            GeckoPreferences.Default["network.proxy.type"] = 1;
            //GeckoPreferences.Default["network.proxy.http"] = "";
            //GeckoPreferences.Default["network.proxy.http_port"] = "";
            //GeckoPreferences.Default["network.proxy.http"] = host;
            //GeckoPreferences.Default["network.proxy.http_port"] = port;
            GeckoPreferences.User["network.proxy.socks"] = host;
            GeckoPreferences.User["network.proxy.socks_port"] = port;
            GeckoPreferences.User["network.proxy.socks_version"] = 5;
            GeckoPreferences.Default["browser.xul.error_pages.enabled"] = true;
        }

        public ChatBot()
        {
            //ChannelFactory<IBotOperator> pipeFactory =
            //  new ChannelFactory<IBotOperator>(
            //    new NetNamedPipeBinding(),
            //    new EndpointAddress(
            //      "net.pipe://localhost/PipeAdvancedChatBotController"));
            //
            //host =
            //  pipeFactory.CreateChannel();

            story = new Story("message/story.txt");

            try
            {
                forbidden_index = System.IO.File.ReadAllLines("message/forbidden_sentences.txt");
            }
            catch { }

            try
            {
                forbidden_starts_with = System.IO.File.ReadAllLines("message/forbidden_start.txt");
            }
            catch { }
            
            ServicePointManager
                .ServerCertificateValidationCallback +=
                    (sender, cert, chain, sslPolicyErrors) => true;

            Gecko.Xpcom.Initialize("..\\..\\xulrunner");

            string[] cmd_params = Environment.GetCommandLineArgs();
            if (cmd_params.Length >= 3)
            {
                int port = 0;
                if (Int32.TryParse(cmd_params[2], out port))
                {
                    SetProxy(
                        cmd_params[1],
                        port
                    );
                }
            }

            browser = new GeckoWebBrowser();
            messages = new Messages();
            searchWatch = new Stopwatch();
        
            /*int temp_id = 0;
            if (cmd_params.Length >= 4)
            {
                Int32.TryParse(cmd_params[3], out temp_id);
            }
            Identifier = temp_id;*/

            lastKnownState = State.Unknown;

            browser.CreateWindow += Browser_CreateWindow2;
            browser.NavigationError += Browser_NavigationError;
            browser.ObserveHttpModifyRequest += Browser_ObserveHttpModifyRequest;
            browser.UseHttpActivityObserver = true;

            browser.Navigate("http://www.praatanoniem.nl/");
            browser.Visible = true;  
        }

        private void Browser_ObserveHttpModifyRequest(object sender, GeckoObserveHttpModifyRequestEventArgs e)
        {
            var query = HttpUtility.ParseQueryString(e.Uri.Query);
            if (query.HasKeys() && query.GetValues("transport") != null && query.GetValues("transport")[0].CompareTo("websocket") == 0)
            {
                string sid = query.GetValues("sid")[0];
                e.Cancel = true;
                browser.Dispose();
                Xpcom.Shutdown();
            }
        }

        private void Browser_NavigationError(object sender, Gecko.Events.GeckoNavigationErrorEventArgs e)
        {
            //Application.Exit();
        }

        private void Browser_DocumentCompleted(object sender, Gecko.Events.GeckoDocumentCompletedEventArgs e)
        {
            //Identifier = host.GetMyID(worker_frm.windowHandle);
            timer.Start();
        }

        private void Timer_Tick(object sender, System.EventArgs e)
        {
            Process();
        }

        private void Browser_CreateWindow2(object sender, GeckoCreateWindowEventArgs e)
        {
            e.Cancel = true;
        }

        private void OnNewMessageHandler(Messages sender, Message message, int messageCount, int messageCountNotMine)
        {
            bool hasReason = false;
            if (messageCountNotMine < 3 && !message.Mine)
            {              
                if (message.Text.Length > 80)
                {
                    hasReason = true;
                }
                else if (message.Text.Length == 1 && (message.Text[0] == 'j' || message.Text[0] == 'J'))
                {
                    hasReason = true;
                }
                else
                {
                    string mtcopy = string.Copy(message.Text);
                    for (char c = '0'; c < '9'; ++c)
                    {
                        mtcopy = mtcopy.Replace(c, '@');
                    }
                    foreach (string plz_no in forbidden_index)
                    {
                        if (mtcopy.IndexOf(plz_no, StringComparison.OrdinalIgnoreCase) != -1)
                        {
                            hasReason = true;
                            break;
                        }
                    }

                    if (!hasReason)
                    {
                        foreach (string plz_no in forbidden_starts_with)
                        {
                            if (mtcopy.StartsWith(plz_no, StringComparison.OrdinalIgnoreCase))
                            {
                                hasReason = true;
                                break;
                            }
                        }
                    }
                }

                if (!hasReason)
                {
                    if (
                        message.Text.Length < 7 &&
                        (message.Text.StartsWith("hoi", StringComparison.OrdinalIgnoreCase) ||
                        message.Text.StartsWith("hey", StringComparison.OrdinalIgnoreCase) ||
                        message.Text.StartsWith("hee", StringComparison.OrdinalIgnoreCase) ||
                        message.Text.StartsWith("heu", StringComparison.OrdinalIgnoreCase) ||
                        message.Text.StartsWith("hi", StringComparison.OrdinalIgnoreCase) ||
                        message.Text.StartsWith("hai", StringComparison.OrdinalIgnoreCase) ||
                        message.Text.StartsWith("hallo", StringComparison.OrdinalIgnoreCase))
                        )
                    {
                        SendMessage("Hoi");
                    }
                }
            }

            if (!hasReason)
            {
                if (!message.Mine)
                {
                    //host.OnNewStrangerMessage(Identifier);
                }
                if(OnNewMessage != null)
                {
                    OnNewMessage(this, message, messageCount, messageCountNotMine);
                }
            }
            else if (hasReason)
            {
                EndCurrentChat();
            }
        }

        public void Process()
        {
            State newState = GetState();
            if(newState != lastKnownState)
            {
                lastKnownState = newState;
                switch (lastKnownState)
                {
                    case State.Searching:
                        searchWatch.Restart();
                        //host.OnBeginSearch(Identifier);
                        if (OnBeginSearching != null)
                        {
                            OnBeginSearching(this);
                        }
                        break;
                    case State.Chatting:
                        messages.Reset();
                        story.Restart();
                        //host.OnBeginChat(Identifier);
                        if (OnBeginChat != null)
                        {
                            OnBeginChat(this);
                        }
                        break;
                    case State.Stopped:
                        //host.OnEndChat(Identifier);
                        if (OnEndChat != null)
                        {
                            OnEndChat(this);
                        }
                        break;
                }
            }

            if (lastKnownState == State.Chatting)
            {
                messages.ProcessNewMessages(logwrapper.GetElementsByTagName("div"));

                if (messages.GetElapsedTimeLastMessage() > 90000.0)
                {
                    EndCurrentChat();
                }

                if (GetLastKnownState() == State.Chatting)
                {
                    SendMessage(story.GetMessage());
                }
            }
            else if(lastKnownState == State.Searching && searchWatch.ElapsedMilliseconds > 20000)
            {
                lastKnownState = State.Unknown;
                //timer.Stop();
                browser.Navigate("http://www.praatanoniem.nl/");
                //host.OnEndChat(Identifier);
            }
            else if(lastKnownState == State.Stopped)
            {
                //if(host.ShouldIStartSearching(Identifier))
                {
                    StartNextChat();
                }
            }

            //host.StateTick(Identifier, lastKnownState);
        }

        public bool StartNextChat()
        {
            GeckoElement intro = browser.Document.GetElementById("intro");
            if (lastKnownState != State.Stopped || intro == null)
            {
                return false;
            }

            if (intro.GetAttribute("style") == null)
            {
                (browser.Document.GetElementById("rosetta-init") as GeckoHtmlElement).Click();
            }
            else
            {
                (chatStartStopButton as GeckoHtmlElement).Click();
            }
            return true;
        }

        public bool EndCurrentChat()
        {
            GeckoHtmlElement endChatHtmlBtn = endChatButton as GeckoHtmlElement;
            if(endChatHtmlBtn != null)
            {
                endChatHtmlBtn.Click();
                return true;
            }
            return false;
        }

        private bool IsSearching()
        {
            if(logwrapper == null)
            {
                return false;
            }
            var collection = logwrapper.GetElementsByTagName("div");
            if(collection.Length == 1)
            {
                return collection[0].TextContent[0] == 'W';
            }
            return false;
        }

        private bool IsChatting()
        {
            if(chatMessageInput == null)
            {
                return false;
            }
            return chatMessageInput.GetAttribute("disabled") == null;
        }

        private bool IsStopped()
        {
            return
                browser.Document == null ||
                browser.Document.GetElementById("intro") == null ||
                browser.Document.GetElementById("intro").GetAttribute("style") == null ||
                browser.Document.GetElementById("rosetta-chat-end-banner") != null;
                
        }

        public void SendMessage(string message)
        {
            if (message != null && message != string.Empty)
            {
                (chatMessageInput as GeckoInputElement).Value = message;
                (browser.Document.GetElementById("chatSendButton") as GeckoHtmlElement).Click();
            }
        }

        private State GetState()
        {
            if (IsStopped())
            {
                return State.Stopped;
            }

            if (IsChatting())
            {
                return State.Chatting;
            }

            if (IsSearching())
            {
                return State.Searching;
            }

            return State.Unknown;
        }

        public State GetLastKnownState()
        {
            return lastKnownState;
        }

        public GeckoWebBrowser GetControl()
        {
            return browser;
        }

        public bool CanStartSearching()
        {
            return 
                lastKnownState == State.Stopped && 
                browser.Document != null && 
                browser.Document.GetElementById("intro") != null;
        }
    }
}
