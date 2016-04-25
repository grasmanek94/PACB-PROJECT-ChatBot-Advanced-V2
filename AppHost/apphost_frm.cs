using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Diagnostics;
using System.Runtime.InteropServices;
//using System.Threading;
using System.ServiceModel;

namespace AppHost
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

    [ServiceBehavior(InstanceContextMode = InstanceContextMode.Single)]
    public partial class apphost_frm : Form, IBotOperator
    {
        [DllImport("user32.dll")]
        static extern IntPtr SetParent(IntPtr hWndChild, IntPtr hWndNewParent);

        [DllImport("user32.dll")]
        static extern int SetWindowLong(IntPtr hWnd, int nIndex, int dwNewLong);

        [DllImport("user32.dll")]
        static extern bool MoveWindow(IntPtr Handle, int x, int y, int w, int h, bool repaint);

        [DllImport("user32.dll")]
        static extern IntPtr SendMessage(IntPtr Handle, int Msg, int wParam, int lParam);

        [DllImport("user32.dll")]
        static extern bool RedrawWindow(IntPtr hWnd, [In] ref RECT lprcUpdate, IntPtr hrgnUpdate, RedrawWindowFlags flags);

        [DllImport("user32.dll")]
        static extern bool RedrawWindow(IntPtr hWnd, IntPtr lprcUpdate, IntPtr hrgnUpdate, RedrawWindowFlags flags);

        private SimpleChatBot currentlySearching;
        private SimpleChatBot[] chatBots;
        private Dictionary<IntPtr, SimpleChatBot> ptr_2_bot;
        private Timer timer;

        private int usedProxy;
        readonly private string[] listProxy =
        {
            "13.95.159.212 1080",
            "13.95.157.246 1080",
            "13.95.152.123 1080",
            "13.95.156.38 1080",
            "13.95.159.251 1080",
            "13.95.156.230 1080",
            "13.95.152.104 1080",
            "13.95.154.150 1080",
            "40.68.227.218 1080",
            "13.95.158.44 1080"
        };

        public apphost_frm()
        {
            InitializeComponent();

            chatBots = new SimpleChatBot[512];
            ptr_2_bot = new Dictionary<IntPtr, SimpleChatBot>();
            currentlySearching = null;

            usedProxy = 0;

            timer = new Timer();
            timer.Interval = 75;
            timer.Tick += Timer_Tick;
            timer.Start();

        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            Graphics g = worker_tabs.CreateGraphics();
            for(int i = 0; i < worker_tabs.TabPages.Count; ++i)
            {
                Rectangle rect = new Rectangle(i * worker_tabs.ItemSize.Width + 2, 2, worker_tabs.ItemSize.Width - 2, worker_tabs.ItemSize.Height - 2);
                g.FillRectangle(new SolidBrush(worker_tabs.TabPages[i].BackColor), rect);
                g.DrawString(worker_tabs.TabPages[i].Text, new Font(worker_tabs.TabPages[i].Font, FontStyle.Bold), Brushes.White, rect);
            }
        }

        const int   GWL_STYLE = -16;
        const long  WS_VISIBLE = 0x10000000,
                    WS_MAXIMIZE = 0x01000000,
                    WS_BORDER = 0x00800000,
                    WS_CHILD = 0x40000000;

        private void worker_tabs_SelectedIndexChanged(object sender, EventArgs e)
        {
            if(worker_tabs.SelectedIndex >= 0 && worker_tabs.SelectedIndex < worker_tabs.TabPages.Count)
            {
                worker_tabs.SelectedTab.Refresh();
                //RedrawWindow(chatBots[Int32.Parse(worker_tabs.SelectedTab.Text)].Handle, IntPtr.Zero, IntPtr.Zero, RedrawWindowFlags.Invalidate);
            }
        }

        IntPtr EmbedProcess(Control Panel, string Path, string Arguments)
        {
            try
            {
                Process Task = Process.Start(Path, Arguments);
                Task.WaitForInputIdle();
                IntPtr Handle = new IntPtr();
                while(Handle == IntPtr.Zero) { Handle = Task.MainWindowHandle; System.Threading.Thread.Sleep(100); }
                SetParent(Handle, Panel.Handle);
                SetWindowLong(Handle, GWL_STYLE, (int)(WS_VISIBLE + (WS_MAXIMIZE | WS_BORDER)));

                MoveWindow(Handle, 0, 0, Panel.Width, Panel.Height, true);

                Panel.Resize += new EventHandler(delegate (object sender, EventArgs e) { MoveWindow(Handle, 0, 0, Panel.Width, Panel.Height, true); });

                this.FormClosed += new FormClosedEventHandler(delegate (object sender, FormClosedEventArgs e)
                {
                    SendMessage(Handle, 83, 0, 0);
                    System.Threading.Thread.Sleep(100);
                    Handle = IntPtr.Zero;
                });

                return Handle;
            }
            catch (Exception e) { MessageBox.Show(this, e.Message, "Error"); }
            return new IntPtr();
        }

        void AddBot(string proxy)
        {
            TabPage page = new TabPage(worker_tabs.TabPages.Count.ToString());
            IntPtr ptr = EmbedProcess(page, "Worker.exe", proxy);
            if(ptr != IntPtr.Zero)
            {
                worker_tabs.TabPages.Add(page);
                for(int i = 0; i < chatBots.Length; ++i)
                {
                    if(chatBots[i] == null)
                    {
                        chatBots[i] = new SimpleChatBot(ptr, page, i);
                        ptr_2_bot.Add(ptr, chatBots[i]);

                        page.Text = i.ToString();
                        break;
                    }
                }
            }
        }

        private void btn_add_Click(object sender, EventArgs e)
        {
            if (usedProxy < listProxy.Length)
            {
                AddBot(listProxy[usedProxy++]);
            }
        }

        public int GetMyID(IntPtr handle)
        {
            return ptr_2_bot[handle].Id;
        }

        public bool ShouldIStartSearching(int id)
        {
            if(currentlySearching == null)
            {
                currentlySearching = chatBots[id];
                return true;
            }
            return false;
        }

        public void OnBeginChat(int id)
        {

        }

        private void worker_tabs_DrawItem(object sender, DrawItemEventArgs e)
        {
            e.Graphics.FillRectangle(new SolidBrush(worker_tabs.TabPages[e.Index].BackColor), e.Bounds);
            // Then draw the current tab button text 
            Rectangle paddedBounds = e.Bounds;
            paddedBounds.Inflate(-2, -2);
            e.Graphics.DrawString(worker_tabs.TabPages[e.Index].Text, this.Font, SystemBrushes.HighlightText, paddedBounds);
        }

        public void OnBeginSearch(int id)
        {

        }

        public void OnEndChat(int id)
        {

        }

        public void StateTick(int id, State state)
        {
            chatBots[id].CurrentState = state;
            if (currentlySearching == chatBots[id] && state != State.Searching)
            {
                currentlySearching = null;
            }

            switch (chatBots[id].CurrentState)
            {
                case State.Unknown:
                    chatBots[id].Page.BackColor = Color.White;
                    break;
                case State.Stopped:
                    chatBots[id].Page.BackColor = Color.DarkGray;
                    break;
                case State.Searching:
                    chatBots[id].Page.BackColor = Color.Blue;
                    break;
                case State.Chatting:
                    if (chatBots[id].Page.BackColor != Color.Red || chatBots[id].Page == worker_tabs.SelectedTab)
                    {
                        chatBots[id].Page.BackColor = Color.Black;
                    }
                    break;
            }
        }

        public void OnNewStrangerMessage(int id)
        {
            if (worker_tabs.SelectedTab != chatBots[id].Page)
            {
                chatBots[id].Page.BackColor = Color.Red;
            }
        }
    }

    public class SimpleChatBot
    {
        public IntPtr Handle { get; set; }
        public TabPage Page { get; set; }
        public int Id { get; set; }
        public bool ShouldStartSearching { get; set; }
        public State CurrentState { get; set; }
        public SimpleChatBot(IntPtr handle, TabPage page, int id)
        {
            Handle = handle;
            Page = page;
            Id = id;
            ShouldStartSearching = false;
            CurrentState = State.Unknown;
        }
    }

}
