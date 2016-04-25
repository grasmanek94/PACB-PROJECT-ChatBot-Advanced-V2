using System.Collections.Generic;
using System.Windows.Forms;
using Gecko;
using Gecko.Collections;
using System;
using System.Globalization;
using System.Text.RegularExpressions;

namespace Worker
{
    public class Message
    {
        public bool Mine { get; set; }
        public string Text { get; set; }
        public DateTime Time { get; private set; }

        public Message(bool mine, string text)
        {
            Mine = mine;
            Text = text;
            Time = DateTime.Now;
        }

        public double Elapsed()
        {
            return (DateTime.Now - Time).Duration().TotalMilliseconds;
        }
    }

    public class Messages
    {
        public delegate void OnNewMessageDelegate(Messages sender, Message message, int messageCount, int messageCountNotMine);
        public event OnNewMessageDelegate OnNewMessage;

        private List<Message> messages;
        private int currentMessage;
        public int NotMineMessages { get; private set; }
        public bool IgnoreElapsed { get; set; }

        public Messages()
        {
            messages = new List<Message>();
            Reset();
        }

        public int ProcessNewMessages(IDomHtmlCollection<GeckoElement> divs)
        {
            int messagesAdded = 0;
            int oldSize = messages.Count;
            foreach (GeckoElement div in divs)
            {
                if(div.GetAttribute("class")[0] == 'm')
                {
                    if(++messagesAdded > currentMessage)
                    {
                        currentMessage = messagesAdded;

                        var spans = div.GetElementsByTagName("span");
                        
                        Message message = new Message(
                                spans[0].TextContent[0] == 'J',
                                spans[2].TextContent
                            );

                        if(!message.Mine)
                        {
                            ++NotMineMessages;
                        }

                        messages.Add(message);
                        if (OnNewMessage != null)
                        {
                            OnNewMessage(this, message, messages.Count, NotMineMessages);
                        }
                    }
                }
            }
            return messages.Count - oldSize;
        }

        public void Reset()
        {
            messages.Clear();
            currentMessage = 0;
            NotMineMessages = 0;
            IgnoreElapsed = false;
        }

        public List<Message> GetMessages()
        {
            return messages;
        }

        public double GetElapsedTimeLastMessage()
        {
            if(IgnoreElapsed || messages.Count < 1)
            {
                return 0.0;
            }
            return messages[messages.Count - 1].Elapsed();
        }
    }
}
