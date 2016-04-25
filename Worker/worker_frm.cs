using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Net;
using System.ServiceModel;

namespace Worker
{
    public partial class worker_frm : Form
    {
        public static IntPtr windowHandle;

        private ChatBot cb;

        public worker_frm()
        {
            InitializeComponent();
        }

        private void btn_d_scht_Click(object sender, EventArgs e)
        {
            cb.EndCurrentChat();
        }

        private void btn_q_wzj_Click(object sender, EventArgs e)
        {
            cb.SendMessage("En wat zoek je hier? alleen chat? of meer?");
        }

        private void btn_q_wwj_Click(object sender, EventArgs e)
        {
            cb.SendMessage("In welke stad woon je?");
        }

        private void btn_q_bjm_Click(object sender, EventArgs e)
        {
            cb.SendMessage("Je bent een meisje?");
        }

        private void btn_q_lft_Click(object sender, EventArgs e)
        {
            cb.SendMessage("Wat is je leeftijd?");
        }

        private void btn_a_iz_Click(object sender, EventArgs e)
        {
            cb.SendMessage("Ik zoek niks... Of het onmogelijke, haha, afhankelijk vna hoe je 't bekijkt ;)");
        }

        private void btn_a_ngnfns_Click(object sender, EventArgs e)
        {
            cb.SendMessage("Dat je 't ff weet.. ik doe niet aan 'geile chats', ook niet aan naaktfoto's uitwisselen en ook niet aan sexcammen.");
        }

        private void btn_d_chat_Click(object sender, EventArgs e)
        {
            cb.GetMessages().IgnoreElapsed = true;
        }

        private void btn_d_sstr_Click(object sender, EventArgs e)
        {
            cb.StopStory();
        }

        public bool IsSearching()
        {
            return cb.GetLastKnownState() == State.Searching;
        }

        public bool TrySearching()
        {
            return cb.StartNextChat();
        }

        public bool CanSearch()
        {
            return cb.CanStartSearching();
        }

        private void worker_frm_Load(object sender, EventArgs e)
        {
            windowHandle = this.Handle;

            cb = new ChatBot();

            cb.GetControl().Dock = DockStyle.Fill;
            this.Controls.Add(cb.GetControl());
        }
    }
}
