using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Threading.Tasks;
using OsuLoader;

namespace osumania
{
    
    class NoteManager
    {
        float ClientSizeY = 470f;
        public int ocha = 512;
        int permitStatus = -1;
        double accuracy = 0.0;
        int totalNote = 0;
        public Graphics graphics;
        BeatMap beatMap;
        Image[] noteImages;
        List<NoteIF> activate_Node;

        List<Note> singleNoteList;
        List<LongNote> longNoteList;

        private static NoteManager instance = null;

        public static NoteManager Instance()
        {
            if (instance == null)
                instance = new NoteManager();

            return instance;
        }
        NoteManager()
        {
            noteImages = new Image[2];
            activate_Node = new List<NoteIF>();
        }
        public void SetImage(Image image, int num)
        {
            noteImages[num] = image;
        }
        public void SetBeatMap(BeatMap beatMap)
        {
            this.beatMap = beatMap;
            singleNoteList = this.beatMap.SingleNotes.ToList();
            longNoteList = this.beatMap.LongNotes.ToList();
            objectDebug = singleNoteList.Count + " " + longNoteList.Count;
        }
        public void CreateNote(int coulum, int startTime)
        {
            int noteNum;
            if(coulum == 0 || coulum == 3)
                noteNum = 0;
            else
                noteNum = 1;
            NoteIF noteif = new NoteIF();
            noteif.Set(100 * coulum, 0f, startTime, noteImages[noteNum]);
            activate_Node.Add(noteif);
        }
        public void CreateLongNote(int coulum, int startTime, int endTime)
        {
            int noteNum;
            if (coulum == 0 || coulum == 3)
                noteNum = 0;
            else
                noteNum = 1;
            NoteIF noteif = new NoteIF();

            noteif.Set(100 * coulum, 0f, startTime, noteImages[noteNum]);
            noteif.isLong = true;
            noteif.EndTime = endTime;
            activate_Node.Add(noteif);
        }
        object objectDebug;
        public void NoteDown(float y)
        {
            for(int i = 0; i < activate_Node.Count; i++)
            {
                activate_Node[i].Down(y);
                if(activate_Node[i].isLong)
                {
                    if ((activate_Node[i].GetY() - activate_Node[i].EndY) >= ClientSizeY)
                    {
                        activate_Node[i].isGone = true;
                        permitStatus = 0;
                        totalNote++;
                    }
                }
                else
                {
                    if (activate_Node[i].GetY() >= ClientSizeY)
                    {
                        activate_Node[i].isGone = true;
                        permitStatus = 0;
                        totalNote++;
                    }
                }
            }
            for (int i = 0; i < activate_Node.Count; i++)
            {
                if (activate_Node[i].isGone == true)
                {
                    activate_Node.Remove(activate_Node[i]);
                }
            }
        }
        public void TimerNote(int time)
        {
            for(int i = 0; i < 4; i++)
            {
                if (singleNoteList.Count <= 0)
                    break;
                if(singleNoteList[0].Time <= time - ocha)
                {
                    CreateNote(singleNoteList[0].Column, singleNoteList[0].Time);
                    singleNoteList.RemoveAt(0);
                }
            }
            for (int i = 0; i < 4; i++)
            {
                if (longNoteList.Count <= 0)
                    break;
                if (longNoteList[0].StartTime <= time - ocha)
                {
                    CreateLongNote(longNoteList[0].Column, longNoteList[0].StartTime, longNoteList[0].EndTime);
                    longNoteList.RemoveAt(0);
                }
            }
            
        }
        public void Draw(Graphics graphics, int time)
        {
            if (singleNoteList.Count == 0 && longNoteList.Count == 0 && activate_Node.Count == 0 && totalNote != 0)
            {
                Font font = new Font("Arial", 50);
                graphics.DrawString("Clear!", font, Brushes.BlueViolet, 110, 200);
                permitStatus = -1;
            }
            for (int i = 0; i < activate_Node.Count; i++)
            {
                if(activate_Node[i].isLong)
                {
                    if(activate_Node[i].EndTime >= time - ocha)
                    {
                        graphics.DrawImage(activate_Node[i].GetImage(), activate_Node[i].GetX(), -activate_Node[i].GetImage().Height, activate_Node[i].GetImage().Width, activate_Node[i].GetImage().Height + activate_Node[i].GetY() - activate_Node[i].GetImage().Height);
                        activate_Node[i].EndY = activate_Node[i].GetY();
                    }
                    else
                    {
                        graphics.DrawImage(activate_Node[i].GetImage(), activate_Node[i].GetX(), activate_Node[i].GetY() - activate_Node[i].EndY - activate_Node[i].GetImage().Height, activate_Node[i].GetImage().Width, activate_Node[i].GetImage().Height + activate_Node[i].EndY - activate_Node[i].GetImage().Height);
                    }
                        
                }
                else
                {
                    graphics.DrawImage(activate_Node[i].GetImage(), activate_Node[i].GetX(), activate_Node[i].GetY() - activate_Node[i].GetImage().Height, activate_Node[i].GetImage().Width, activate_Node[i].GetImage().Height);
                }
                //graphics.DrawString("" + singleNoteList[8000], SystemFonts.DefaultFont, Brushes.Black, 0, 0);
            }
        }
        public void Debug(Graphics graphics)
        {
            Font font = new Font("Arial", 10);
            //graphics.DrawString("" + objectDebug, font, Brushes.Red, 500, 350);

            Font font2 = new Font("Arial", 10);
            string a = string.Format("{0:f2}%", (double)(accuracy / totalNote));
            graphics.DrawString(a, font2, Brushes.Black, 500, 60);
            //graphics.DrawString(accuracy + " " + totalNote, font2, Brushes.Black, 500, 80);
        }
        public void DrawPermit(Graphics graphics)
        {
            Font font = new Font("Arial", 20);
            switch (permitStatus)
            {
                case 0:
                    graphics.DrawString("Miss", font, Brushes.Red, 165, 300);
                    break;
                case 3:
                    graphics.DrawString("Perfect", font, Brushes.Gold, 150, 300);
                    break;
            }
        }
        int permitAdvance = 1250;
        int permit = 290;
        public int KeyDown(int coulum, int time)
        {
            bool ok = false;
            for(int i = 0; i < activate_Node.Count; i++)
            {
                if (activate_Node[i].isLong)
                    continue;
                if((activate_Node[i].StartTime >= (time - permitAdvance - permit)
                    && activate_Node[i].StartTime <= (time - permitAdvance + permit))
                    && activate_Node[i].Coulum == coulum)
                {
                    objectDebug = activate_Node[i].StartTime + " " + time;
                    ok = true;
                    activate_Node.Remove(activate_Node[i]);
                    break;
                }
            }
            if (ok)
            {
                permitStatus = 3;
                totalNote++;
                accuracy += 100.0;
                return 100;
            }
            else
                return 0;
        }
        public int KeyDownLong(int coulum, int time)
        {
            bool ok = false;
            for (int i = 0; i < activate_Node.Count; i++)
            {
                if (!activate_Node[i].isLong)
                    continue;
                if ((activate_Node[i].StartTime >= (time - permitAdvance - permit)
                    && activate_Node[i].StartTime <= (time - permitAdvance + permit))
                    && activate_Node[i].Coulum == coulum)
                {
                    objectDebug = activate_Node[i].StartTime + " " + time;
                    activate_Node[i].isKeyDown = true;
                    ok = true;
                    break;
                }
            }
            if (ok)
            {
                permitStatus = 3;
                totalNote++;
                accuracy += 100.0;
                return 100;
            }
            else
                return 0;
        }
        public int KeyUpLong(int coulum, int time)
        {
            bool ok = false;
            for (int i = 0; i < activate_Node.Count; i++)
            {
                if (!activate_Node[i].isLong)
                    continue;
                if(activate_Node[i].isKeyDown == true && activate_Node[i].Coulum == coulum)
                {
                    if ((activate_Node[i].EndTime >= (time - permitAdvance - (permit * 1.2))
                    && activate_Node[i].EndTime <= (time - permitAdvance + (permit * 1.2))))
                    {
                        objectDebug = activate_Node[i].EndTime + " " + time;
                        ok = true;
                        activate_Node.Remove(activate_Node[i]);
                        break;
                    }
                    else
                    {
                        permitStatus = 0;
                        totalNote++;
                        activate_Node.Remove(activate_Node[i]);
                        break;
                    }
                }
                
            }
            if (ok)
            {
                permitStatus = 3;
                totalNote++;
                accuracy += 100.0;
                return 100;
            }
            else
                return 0;
        }
        public void Reset()
        {
            activate_Node.Clear();
            singleNoteList.Clear();
            longNoteList.Clear();
            accuracy = 0.0;
            totalNote = 0;
            permitStatus = -1;
        }
    }

    class NoteIF
    {
        float X;
        float Y;
        Image Image;
        public bool isGone = false;
        public bool isLong = false;
        public bool isKeyDown = false;
        public int StartTime = 0;
        public int EndTime = 0;
        public float EndY = 0;
        public int Coulum = 0;

        public void Set(float x, float y, int startTime, Image image)
        {
            X = x; Y = y; Image = image; StartTime = startTime; Coulum = (int)x/100;
        }
        public void Down(float y)
        {
            Y += y;
        }
        public Image GetImage()
        {
            return Image;
        }
        public float GetX()
        {
            return X;
        }
        public float GetY()
        {
            return Y;
        }
    }
}
