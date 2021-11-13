using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Timers;
using System.Diagnostics;
using OsuLoader;
using Linsft.FmodSharp;
namespace osumania
{
    public partial class Form1 : Form
    {
        string[] DIFFICULT;
        private string songFile;
        private Bitmap buffer;
        private Graphics graphics;
        private NoteManager noteManager;
        private TimeSpan ts;
        private Stopwatch stopWatch;
        private Font font;
        private Image permitImage;
        private Image whiteImage;
        private BeatMap tita;
        private Linsft.FmodSharp.SoundSystem.SoundSystem SoundSystem;
        private Linsft.FmodSharp.Channel.Channel Chan;
        private Linsft.FmodSharp.Channel.Channel Chan2;
        private Linsft.FmodSharp.Sound.Sound SoundFile;
        private Linsft.FmodSharp.Sound.Sound SoundFile2;
        public int selectSong = 0;

        int dummyTime = 0;
        int dummyTime2 = 0;
        int GameTime = 0;
        int score = 0;
        public Form1(int select)
        {
            selectSong = select;
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            DIFFICULT = new string[] { "titania.osu", "titania_HARD.osu", "titania_EXPERT.osu" };
            songFile = DIFFICULT[selectSong]; // 비트맵 지정

            SoundSystem = new Linsft.FmodSharp.SoundSystem.SoundSystem();
            SoundSystem.Init();
            SoundSystem.ReverbProperties = Linsft.FmodSharp.Reverb.Presets.Off;
            SoundFile = SoundSystem.CreateSound("audio.mp3");
            SoundFile2 = SoundSystem.CreateSound("drum.wav");

            graphics = CreateGraphics();
            buffer = new Bitmap(ClientSize.Width, ClientSize.Height);
            noteManager = NoteManager.Instance();
            // 노트 이미지
            noteManager.SetImage(Image.FromFile("note1.png"), 0);
            noteManager.SetImage(Image.FromFile("note2.png"), 1);
            noteManager.graphics = graphics;
            permitImage = Image.FromFile("permit.png");
            whiteImage = Image.FromFile("white.png");

            tita = OsuLoader.OsuLoader.LoadDotOsu(songFile);
            noteManager.SetBeatMap(tita);

            timer1.Interval = 1;
            timer1.Start();
            stopWatch = new Stopwatch();
            stopWatch.Start();
            //noteManager.CreateNote(0, 0);

            font = new Font("Arial", 20);
        }

        private void Form1_Paint(object sender, PaintEventArgs e)
        {
            e.Graphics.DrawImage(buffer, 0, 0); // 더블 버퍼 그리기
            graphics = Graphics.FromImage(buffer);
            
            
        }

        protected override void OnPaintBackground(PaintEventArgs e)
        {
        }
        bool isPlaying = false;
        private void timer1_Tick_1(object sender, EventArgs e)
        {
            //시간
            ts = stopWatch.Elapsed;
            GameTime = (int)ts.TotalMilliseconds;
            noteManager.TimerNote(GameTime);
            //노래
            if(isPlaying == false && GameTime >= 950)
                playSound();
            //그리기
            graphics.Clear(Color.White);
            //float speed = dtime.deltaTime * 212f * 5;
            noteManager.NoteDown(12.5f);
            noteManager.Draw(graphics, GameTime);
            graphics.DrawImage(permitImage, 0, 400, permitImage.Width, permitImage.Height);
            graphics.DrawImage(whiteImage, 0, 420, whiteImage.Width, (int)(whiteImage.Height * 1.2));
            graphics.DrawLine(Pens.Black, -1, 0, -1, 500);
            graphics.DrawLine(Pens.Black, 100, 0, 100, 500);
            graphics.DrawLine(Pens.Black, 200, 0, 200, 500);
            graphics.DrawLine(Pens.Black, 300, 0, 300, 500);
            graphics.DrawLine(Pens.Black, 400, 0, 400, 500);
            noteManager.DrawPermit(graphics);
            
            if(dummyTime2 == 0)
                dummyTime++;
            if (dummyTime >= 33)
            {
                dummyTime = 0;
                dummyTime2 = (int)ts.TotalMilliseconds;
                //noteManager.ocha = dummyTime2;
            }
            noteManager.Debug(graphics);
            //graphics.DrawString("" + dummyTime2, SystemFonts.DefaultFont, Brushes.Black, 500, 0);
            graphics.DrawString("" + score, font, Brushes.Black, 500, 30);
            if (pressD)
                graphics.DrawString("D", font, Brushes.Black, 35, 430);
            if (pressF)
                graphics.DrawString("F", font, Brushes.Black, 138, 430);
            if (pressJ)
                graphics.DrawString("J", font, Brushes.Black, 238, 430);
            if (pressK)
                graphics.DrawString("K", font, Brushes.Black, 338, 430);


            Invalidate();
        }
        bool isStop = false;
        private void Form1_KeyDown(object sender, KeyEventArgs e)
        {
            if(e.KeyCode == Keys.D)
            {
                score += noteManager.KeyDown(0, GameTime);
            }
            if (e.KeyCode == Keys.F)
            {
                score += noteManager.KeyDown(1, GameTime);
            }
            if (e.KeyCode == Keys.J)
            {
                score += noteManager.KeyDown(2, GameTime);
            }
            if (e.KeyCode == Keys.K)
            {
                score += noteManager.KeyDown(3, GameTime);
            }
            if (e.KeyCode == Keys.P)
            {
                if (!isStop)
                {
                    stopWatch.Stop();
                    timer1.Stop();
                    if (Chan != null && Chan.IsPlaying)
                        Chan.Paused = true;
                    isPlaying = false;
                    isStop = true;
                }
                else
                {
                    stopWatch.Start();
                    timer1.Start();
                    if(Chan != null && Chan.IsPlaying)
                        Chan.Paused = false;
                    isStop = false;
                    isPlaying = true;
                } 
            }
            if (e.KeyCode == Keys.R)
            {
                if(timer1.Enabled)
                {
                    timer1.Stop();
                    timer1.Dispose();
                    stopWatch.Stop();
                    stopSound();
                    score = 0;
                    GameTime = 0;
                    dummyTime2 = 0;
                    noteManager.Reset();
                    noteManager.SetBeatMap(tita);
                    stopWatch.Reset();
                    timer1.Start();
                    stopWatch.Start();
                }
            }
        }
        bool pressD = false;
        bool pressF = false;
        bool pressJ = false;
        bool pressK = false;
        private void Form1_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar.Equals('d'))
            {
                score += noteManager.KeyDownLong(0, GameTime);
                if(!pressD)
                {
                    pressD = true;
                    playHitSound();
                }
            }
            if (e.KeyChar.Equals('f'))
            {
                score += noteManager.KeyDownLong(1, GameTime);
                if (!pressF)
                {
                    pressF = true;
                    playHitSound();
                }
            }
            if (e.KeyChar.Equals('j'))
            {
                score += noteManager.KeyDownLong(2, GameTime);
                if (!pressJ)
                {
                    pressJ = true;
                    playHitSound();
                }
            }
            if (e.KeyChar.Equals('k'))
            {
                score += noteManager.KeyDownLong(3, GameTime);
                if (!pressK)
                {
                    pressK = true;
                    playHitSound();
                }
            }
        }

        private void Form1_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.D)
            {
                score += noteManager.KeyUpLong(0, GameTime);
                if (pressD)
                    pressD = false;
            }
            if (e.KeyCode == Keys.F)
            {
                score += noteManager.KeyUpLong(1, GameTime);
                if (pressF)
                    pressF = false;
            }
            if (e.KeyCode == Keys.J)
            {
                score += noteManager.KeyUpLong(2, GameTime);
                if (pressJ)
                    pressJ = false;
            }
            if (e.KeyCode == Keys.K)
            {
                score += noteManager.KeyUpLong(3, GameTime);
                if (pressK)
                    pressK = false;
            }
        }
        private void playSound()
        {
            Chan = SoundSystem.PlaySound(SoundFile);
            Chan.Volume = 0.2f;
            isPlaying = true;
        }
        private void playHitSound()
        {
            Chan2 = SoundSystem.PlaySound(SoundFile2);
            Chan2.Volume = 0.15f;
        }
        private void stopSound()
        {
            if(Chan != null && Chan.IsPlaying)
            {
                Chan.Stop();
            }
            if (Chan2 != null && Chan2.IsPlaying)
            {
                Chan2.Stop();
            }
            isPlaying = false;
        }
        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
            timer1.Stop();
            timer1.Dispose();
            stopWatch.Stop();
            stopSound();
            score = 0;
            GameTime = 0;
            dummyTime2 = 0;
            noteManager.Reset();
            noteManager.SetBeatMap(tita);
            stopWatch.Reset();

            SoundFile.Dispose();
            SoundFile2.Dispose();
            if(Chan != null)
                Chan.Dispose();
            if (Chan2 != null)
                Chan2.Dispose();
            SoundSystem.CloseSystem();
            SoundSystem.Dispose();
        }
    }
}
