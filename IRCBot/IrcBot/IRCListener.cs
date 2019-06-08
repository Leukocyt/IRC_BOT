using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
//using ChatSharp;
using Meebey.SmartIrc4net;
using System.IO;
using System.Threading;
using Quartz;
using IRCBot.Scheduling;
using IRCBot.DB;


namespace IRCBot
{
    public class IRCListener
    {
        IrcClient ircClient;
        List<string> channels = new List<string>();
        string nick;
        string name;
        string server;
        Random rnd1 = new Random();
        string dateformat = "dd.MM.yyyy HH:mm:ss";
        IRCMessageScheduler scheduler = null;
        
        public enum botCommands
        {
            unknown = -1,
            random = 0,
            random_old = 1,
            OMXHPI = 2,
            stats = 3,
            words = 4,
            chars = 5
        }

        public IRCListener(
            string nick,
            string name,
            string server            
            )
        {
            this.nick = nick;
            this.name = name;
            this.server = server;
            ircClient = new IrcClient();
            ircClient.Encoding = System.Text.Encoding.UTF8;
            ircClient.SendDelay = 200;
            ircClient.ActiveChannelSyncing = true;
            ircClient.AutoRejoin = true;
            ircClient.AutoRelogin = true;
            ircClient.AutoReconnect = true;
            ircClient.AutoRejoinOnKick = true;
            ircClient.AutoJoinOnInvite = true;
            ircClient.OnChannelMessage += IrcClient_OnQueryMessage;
            ircClient.OnQueryMessage += IrcClient_onPrivmessage;
            ircClient.OnConnecting += IrcClient_OnConnecting;
            ircClient.OnConnectionError += IrcClient_OnConnectionError;
            ircClient.OnConnected += IrcClient_OnConnected;
            ircClient.OnErrorMessage += IrcClient_OnErrorMessage;
            //Skedulointi
            scheduler = new IRCMessageScheduler(this);
        }
        #region tapahtumakäsittelijät
        private void IrcClient_onPrivmessage(object sender, IrcEventArgs e)
        {
            try
            {
                handleMessage(e);
            } catch(Exception ee)
            {
                helperClass.writeLog("Virhe privaattiviestin käsittelyssä! Viesti: " + ee.ToString(), 3);
            }
        }

        private void IrcClient_OnErrorMessage(object sender, IrcEventArgs e)
        {
            helperClass.writeLog("Virhe! Viesti: " + e.Data.Message, 3);
        }

        private void IrcClient_OnConnected(object sender, EventArgs e)
        {
            helperClass.writeLog("Botti yhdistetty!", 0);
            scheduler.start_scheduling();
        }

        private void IrcClient_OnConnectionError(object sender, EventArgs e)
        {
            helperClass.writeLog("Virhe yhdistettäessä! Viesti:" + e.ToString() , 3);
        }

        private void IrcClient_OnConnecting(object sender, EventArgs e)
        {
            helperClass.writeLog("Botti yrittää yhdistää", 0);
        }
        #endregion

        #region Viestien käsittely
        private void IrcClient_OnQueryMessage(object sender, IrcEventArgs e)
        {
            logMessage(e);
            handleMessage(e);                                 
        }
        private void logMessage(IrcEventArgs e)
        {
            try
            {
                using (var DB = new internetEntities())
                {
                    irkki testRow = new irkki
                    {
                        viesti = e.Data.Message,
                        nick = e.Data.Nick,
                        kanava = e.Data.Channel,
                        aika = DateTime.Now,
                        maara = 1
                    };
                    DB.irkki.Add(testRow);
                    DB.SaveChanges();
                }
            }
            catch (Exception eip)
            { }
        }

        private void handleMessage(IrcEventArgs e)
        {
            try {
                string like = "";
                botCommands com = botCommands.unknown;
                string channel = "";
                string messageTarget = "";            
                if( e.Data.Channel != null ) //Kanavalla tullut viesti.
                {
                    channel = e.Data.Channel;
                    messageTarget = channel;
                    com = parseCommand(e, out like, false);
                } else //Privaatti viesti.
                {
                    channel = e.Data.MessageArray[1];
                    messageTarget = e.Data.Nick;
                    com = parseCommand(e, out like, true);
                }
             
                if (com == botCommands.random)
                {
                    ircClient.SendMessage(SendType.Message, messageTarget, random(channel, like, false, messageTarget));
                } else if( com == botCommands.random_old)
                {
                    ircClient.SendMessage(SendType.Message, messageTarget, random(channel, like, true, messageTarget));
                } else if( com == botCommands.stats || com == botCommands.words || com == botCommands.chars)
                {
                    int choise = 0;
                    switch(com)
                    {
                        case botCommands.stats:
                            choise = 0;
                            break;
                        case botCommands.chars:
                            choise = 1;
                            break;
                        case botCommands.words:
                            choise = 2;
                            break;
                    }
                    int range = 0;
                    if (like.Length > 0)
                    {
                        try
                        {
                            range = Int32.Parse(like);
                        }
                        catch (Exception exu)
                        {
                            range = 0;
                        }
                    }
                    ircClient.SendMessage(SendType.Message, messageTarget, get_stats(channel, range, choise));
                }
            } catch(Exception ej)
            {
            }
        }


        private void handlePrivateMessage(IrcEventArgs e)
        {
            try
            {


            } catch(Exception ex)
            {

            }
        }
        //Käsittelee ja parsii kanavalle tulleen viestin.
        private botCommands parseCommand(IrcEventArgs e, out string like, bool privMessage = false)
        {
            like = "";
            try {
                like = "";
                string message = e.Data.Message; 
                string[] splitted = e.Data.MessageArray;
                if (splitted.Length > 0)
                {
                    botCommands comType = get_command_type(splitted[0]);
                    if( comType == botCommands.random || comType == botCommands.random_old)
                    {
                        if (!privMessage)
                            like = get_like(message, splitted[0]);
                        else
                            like = get_like(message, splitted[1]);
                    } else
                    {
                        if( !privMessage && splitted.Length >= 2)
                        {
                            like = splitted[1];
                        } else if( privMessage && splitted.Length >= 3)
                        {
                            like = splitted[2];
                        }
                    }            
                    return comType;
                }
                return botCommands.unknown;
            } catch(Exception eee)
            {
                return botCommands.unknown;
            }
        }
        private botCommands get_command_type(string s)
        {
            try
            {
                s = s.Trim();
                switch (s)
                {
                    case "!random":
                        return botCommands.random;
                    case "!satunnainen":
                        return botCommands.random;
                    case "!randomold":
                        return botCommands.random_old;
                    case "!stats":
                        return botCommands.stats;
                    case "!rivit":
                        return botCommands.stats;
                    case "!sanat":
                        return botCommands.words;
                    case "!merkit":
                        return botCommands.chars;
                    default:
                        return botCommands.unknown;
                }
            } catch(Exception e)
            {
                return botCommands.unknown;
            }
        }


        private string get_like(string message, string word_to_search)
        {
            try
            {
                int startI = message.IndexOf(word_to_search);
                return message.Substring(startI + word_to_search.Length).Trim();
            } catch(Exception e)
            {
                helperClass.writeLog("Virhe viestiä käsiteltäessä! Viesti: " + e.ToString(), 3);
                return "";
            }
        }

        #endregion


        #region random-kyselyt

        private irkki_old getOldRand(string channel, string like, string messageTarget = "")
        {
            try
            {
                using (var DB = new internetEntities())
                {
                    List<string> sent_rands = DB.rand_messages.Where(x => (messageTarget == "" || x.initiator == messageTarget) && x.kanava == channel).Select(x => x.message).ToList();
                    List<irkki_old> mList = null;
                    mList = DB.irkki_old.Where(x => x.viesti != "x" && !x.viesti.StartsWith("!random") && (like == "" || x.viesti.Contains(like)) && x.kanava.ToLower() == channel.ToLower() && (!sent_rands.Contains(x.viesti))).OrderBy(x => Guid.NewGuid()).Take(1).ToList();
                    irkki_old mes = null;
                    if( mList.Any())
                    {
                        mes = mList.First();
                        storeRandMessageToBuffer(null, mes, messageTarget);
                        return mes;
                    } else
                    {
                        return null;
                    }
                }
            } catch (Exception e)
            {
                //return new List<irkki_old>();
                return null;
            }
        }

        private irkki getRand(string channel, string like, string messageTarget = "")
        {
            try {
                using (var DB = new internetEntities())
                {
                    List<string> sent_rands = DB.rand_messages.Where(x => (messageTarget == "" || x.initiator == messageTarget) && x.kanava == channel).Select(x => x.message).ToList();
                    List<irkki> tList = null;
                    tList = DB.irkki.Where(x => x.viesti != "x" && !x.viesti.StartsWith("!random") && (like == "" || x.viesti.Contains(like)) && x.kanava.ToLower() == channel.ToLower() && (!sent_rands.Contains(x.viesti))).OrderBy(x => Guid.NewGuid()).Take(1).ToList();
                    irkki mes = null;
                    if( tList.Any())
                    {
                        mes = tList.First();
                        storeRandMessageToBuffer(mes,null,messageTarget);
                        return mes;
                    } else
                    {
                        return null;
                    }
                }
            } catch(Exception e)
            {
                return null;
            } 
        }

        private string random(string channel, string like = "", bool onlyOld = false, string messageTarget = "")
        {
            string rand = "";
            irkki mes = null;
            irkki_old oldMes = null;
            try
            {
                using (var DB = new internetEntities())
                {
                    double countOld = DB.irkki_old.Count();
                    double countNew = DB.irkki.Where(x => x.viesti != "x").Count();
                    double total = countOld + countNew;
                    double limit = rnd1.NextDouble();
                    if( onlyOld ||  (limit < countOld / total)) //Old
                    {
                        oldMes = getOldRand(channel, like, messageTarget);
                        if(oldMes != null)
                        {
                            rand = formRandomMessage(oldMes.aika.Value, oldMes.nick, oldMes.viesti);
                        } else
                        {
                            mes = getRand(channel, like, messageTarget);
                            if(mes != null)
                            {
                                rand = formRandomMessage(mes.aika.Value, mes.nick, mes.viesti);
                            } 
                         }                        
                    } else // new
                    {
                        mes = getRand(channel, like, messageTarget);
                        if (mes != null)
                        {
                            rand = formRandomMessage(mes.aika.Value, mes.nick, mes.viesti);
                        } else
                        {
                            oldMes = getOldRand(channel, like, messageTarget);
                            if (oldMes != null)
                            {
                                rand = formRandomMessage(oldMes.aika.Value, oldMes.nick, oldMes.viesti);
                            }
                        }                            
                    }              
                }
            } catch(Exception e)
            {
                helperClass.writeLog("Virhe random()-metodissa. Virheviesti: " + e.ToString(), 4);
            }
            return rand;
        }
        private string formRandomMessage(DateTime ts, string nick, string message)
        {
            string rand = "";
            rand = '<' + ts.ToString(dateformat) + "> " + remove_hilite(nick) + ": " + message;
            return rand;
        }


        private void storeRandMessageToBuffer(irkki message = null, irkki_old mesOld = null, string initiator = "")
        {
            try
            {
                using (var db = new DB.internetEntities())
                {
                    rand_messages rm = null;
                    if (message != null)
                    {
                        rm = new rand_messages { rowid_irc = message.id, message = message.viesti, kanava = message.kanava, initiator = initiator };
                    }
                    else if (mesOld != null)
                    {
                        rm = new rand_messages { rowid_irc = mesOld.id, message = mesOld.viesti, kanava = mesOld.kanava, initiator = initiator };
                    }
                    if (rm != null)
                    {
                        db.rand_messages.Add(rm);
                        db.SaveChanges();
                    }
                }
            } catch(Exception e)
            {
                helperClass.writeLog("storeRandMessageToBuffer() Message: " + e.ToString(), 3);
            }
        }


        #endregion

        #region STATS

        private string get_stats(string channel, int ?range, int choise = 0)
        {
            try
            {
                string insertNick = "";
                List<F_Get_Stats_Result> stats = new List<F_Get_Stats_Result>();
                DateTime end = DateTime.Now;

                DateTime start = DateTime.Now;
                if( range != null && range > 0)
                {
                    start = end.AddDays(-1 * range.Value);
                } else
                {
                    start = new DateTime(end.Year, end.Month, end.Day);
                }
                using (var db = new internetEntities())
                {
                    stats = db.F_Get_Stats(start, channel, choise).OrderByDescending(x => x.count).Take(10).ToList();
                }
                string retVal = "";
                foreach (var s in stats)
                {
                    if (retVal.Length > 0)
                        retVal += ", ";
                    //Lisätään kontrollimerkki, jotta rivit eivät hilittaa.
                    retVal += remove_hilite(s.nick) + ": " + s.count.ToString();
                }
                return retVal;
            } catch(Exception e)
            {
                helperClass.writeLog("Virhe haettaessa tilastoja! Viesti: " + e.ToString(), 4);
                return "";
            }
        }
        /*
         int unicode = 65;
        char character = (char) unicode;
        string text = character.ToString(); 
         * */

        #endregion

        #region apufunkkarit

        private string remove_hilite(string nick)
        {
            try
            {
                int unicode = 2;
                char specialChar = (char)unicode;
                if (nick.Length >= 2)
                {
                    nick = nick.Insert(1, specialChar.ToString());
                    nick = nick.Insert(1, specialChar.ToString());
                }
                return nick;
            } catch(Exception e)
            {
                return nick;
            }
        }
        #endregion

        #region Logitus ja kirjautuminen kirjaston tapahtumiin
        private void IrcClient_NetworkError(object sender, ChatSharp.Events.SocketErrorEventArgs e)
        {
            helperClass.writeLog("Virhe! networkError", 1);
            this.Connect();
        }
        private void stringSaveMessage(ChatSharp.Events.PrivateMessageEventArgs e)
        {
            try
            {
                using (var DB = new internetEntities())
                {
                    irkki testRow = new irkki
                    {
                        viesti = e.PrivateMessage.Message,
                        nick = e.PrivateMessage.User.Nick,
                        kanava = e.PrivateMessage.Source,
                        aika = DateTime.Now,
                        maara = 1
                    };
                    DB.irkki.Add(testRow);
                    DB.SaveChanges();
                }
            }
            catch (Exception eip)
            {
                try {
                    using ( var DB = new internetEntities())
                    {
                        DB.logs.Add(new logs { message = e.ToString(), severity = 1 });
                        DB.SaveChanges();
                    }
                } catch(Exception se)
                {

                }
                
            }
        }
        public void addChannel(string channel)
        {
            this.channels.Add(channel);
        }
        private void IrcClient_ConnectionComplete(object sender, EventArgs e)
        {
            int i, c = 0;
            c = this.channels.Count();
            for (i = 0; i < c; i++)
            {
                try
                {
                    //ircClient.JoinChannel(channels[i]);
                }
                catch (Exception ex)
                {
                    System.Console.WriteLine(ex.ToString());
                }
            }
        }
        public void onConnect()
        {
            
        }

        public void disconnect()
        {
            ircClient.Disconnect();
        }
        public void Connect()
        {
            try
            {
                ircClient.Connect(this.server, 6667);
            } catch( Exception e)
            {
                helperClass.writeLog(e.ToString(), 4);
            }
            try
            {
                ircClient.Login(this.nick, this.name);
            } catch( Exception e)
            {
                helperClass.writeLog(e.ToString(), 4);
            }
            try {
                //Join channels.
                foreach (var c in channels)
                {
                    ircClient.RfcJoin(c);
                }
            } catch( Exception e)
            {

            }
            ircClient.Listen();
        }

        #endregion

        #region public functions 

        public void sendMessage(string channel, string message )
        {
            ircClient.SendMessage(SendType.Message, channel, message);
        }

        #endregion

    }
}
