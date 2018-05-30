using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Timers;
using System.Windows.Threading;
using System.Threading;

namespace TankArena
{
    class PowerUp
    {
        public Image Img { get; set; }
        public int CoordX { get; set; }
        public int CoordY { get; set; }

        public PowerUp(int x,int y,Image img)
        {
            Img = img;
            CoordX = x;
            CoordY = y;
        }
    }


    class Bullet
    {
        private const int defaultSpeed = 50;
        private const int maxSpeed = 100;
        
        public Tank Tnk { get; set; }
        public Image Img { get; set; }
        public int CoordX { get; set; }
        public int CoordY { get; set; }
        public int Orient { get; set; }
        public int speed = 4;
        public string WhoShot { get; set; }

        public Bullet(int coordX,int coordY, int orient, int x,string whoShot,Tank tnk,Image img)
        {
            if (x == 1)
                this.speed = defaultSpeed;
            else this.speed = maxSpeed;

            this.Tnk = tnk;
            this.WhoShot = whoShot;
            this.CoordX = coordX;
            this.CoordY = coordY;
            this.Orient = orient;   // 1 = <- // 2 = up // 3 = -> // 4 = down
            this.Img = img;
        }
    }

    class Tank
    {
        public int Hp { get; set; }
        public int Armor { get; set; }
        public int CoordX { get; set; }
        public int CoordY { get; set; }
        public const int maxArmor = 4;
        public string Name { get; set; }
        public int Orient { get; set; }
        public Image Img { get; set; }
        public int Money { get; set; }

        public Tank(int CoordX,int CoordY,int Orient, string Name, Image Img)
        {
            this.CoordX = CoordX;
            this.CoordY = CoordY;
            this.Img = Img;
            this.Orient = Orient;
            this.Armor = 0;
            this.Hp = 100;
            this.Name = Name;
            this.Money = 0;
        }

        public void ArmorPickUP()
        {
            if(this.Armor<4)
                this.Armor ++;
        }

        public void MoneyPickUP()
        {
            this.Money += 50;
        }

        public void HealthPickUP()
        {
            this.Hp += 30;
            if (this.Hp > 100)
                this.Hp = 100;
        }

        public void IsShot(int rasp = 0,int bonus = 0)
        {
            this.Hp -= 50;
            this.Hp += 5 * Armor;

            if (rasp == 1)
                this.Hp += 15;
            if (bonus == 1)
                this.Hp -= 15;

            if (Hp < 0)
                Hp = 0;
        }

        public Bullet Shoot(Grid Field,int speed)
        {
            string uriString = @"Img/BulletFast" + Orient.ToString() + ".png";

            Image img = new Image
            {
                Source = new BitmapImage(new Uri(uriString, UriKind.Relative)),
                Stretch = Stretch.Fill,
                Opacity=0.7
            };

            int x=CoordX, y=CoordY; 
            switch(Orient)
            {
                case 1:
                    {
                        y-=1;
                        break;
                    }
                case 2:
                    {
                        x-=1;
                        break;
                    }
                case 3:
                    {
                        y+=1;
                        break;
                    }
                case 4:
                    {
                        x+=1;
                        break;
                    }
            }
            
            return new Bullet(x, y, Orient, speed,Name,this,img);
        }
    }

    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private enum Turn { Left=-1, Right = 1};
        private enum Move { Back = -1, Front = 1 };


        DispatcherTimer timer = new DispatcherTimer();
        DispatcherTimer AItimer = new DispatcherTimer();
        DispatcherTimer timer1 = new DispatcherTimer();
        DispatcherTimer timer2 = new DispatcherTimer();
        DispatcherTimer bulletTimer = new DispatcherTimer();
        DispatcherTimer fastBulletTimer = new DispatcherTimer();

        DispatcherTimer HealthArmor = new DispatcherTimer();


        int[,] Matrix = new int[7, 12];

        List<Tank> tanks = new List<Tank>();
        List<Bullet> bullets = new List<Bullet>();
        List<Bullet> fastBullets = new List<Bullet>();
        List<PowerUp> PowerUps = new List<PowerUp>();

        int[,] Stops = new int[2,7];

        int popTime,contorPop=0;
        const int sMoney = 500;
        const int noHealthPickUps = 3;
        const int noMoneyPickUps = 2;
        const int noArmorPickUps = 1;
        const int noTanks = 4;
        const int noStops = 7;
        int playersAlive = 4;

        public MainWindow()
        {
            InitializeComponent();
            InitializeMatrix();
            InitializeOthers();
            InitializeTimers();

            // tanks[0].Name = "Smth";
        }

        private void InitializeOthers()
        {
            Player1Name.Text = tanks[0].Name;
            Player2Name.Text = tanks[1].Name;
            Player3Name.Text = tanks[2].Name;
            Player4Name.Text = tanks[3].Name;
        }

        private void InitializeGame()
        {
            StartingIn.Opacity = 1;
            StartingIn.Text = "Game Starting in 3...";
            Thread.Sleep(1000);
            StartingIn.Text = "2...";
            Thread.Sleep(1000);
            StartingIn.Text = "1...";
            Thread.Sleep(1000);
            StartingIn.Text = "Go!";
            StartingIn.Opacity = 0;
        }

        private void InitializeTimers()
        {
            timer.Interval = new TimeSpan(0,0,1); 
            timer.Tick += DispatcherTimer_Tick;

            timer1.Interval = new TimeSpan(300000);
            timer1.Tick += DispatcherTimer_Tick1;

            timer2.Interval = new TimeSpan(300000);
            timer2.Tick += DispatcherTimer_Tick2;

            bulletTimer.Interval = new TimeSpan(0,0,0,0,500);
            bulletTimer.Tick += BulletTick;

            HealthArmor.Interval = new TimeSpan(0, 0, 1);
            HealthArmor.Tick += HealthArmorTick;

            fastBulletTimer.Interval = new TimeSpan(0,0,0,0,300);
            fastBulletTimer.Tick += FastBulletTick;

            AItimer.Interval = new TimeSpan(0, 0, 1);
            AItimer.Tick += AItimer_Tick;

            AItimer.Start();
            fastBulletTimer.Start();
            timer.Start();
            bulletTimer.Start();
            HealthArmor.Start();
        }

        private void AItimer_Tick(object sender, EventArgs e)
        {
            foreach (Tank tank in tanks)
            {
                if (tank.Name == tanks[0].Name || tank.Hp==0)
                    continue;

                if (Rand(53, 0) > 26)
                {
                    if (Rand(53, 0) > 26)
                        MoveTank(tank, 1);
                    else MoveTank(tank, -1);
                }
                else
                {
                    if (Rand(53, 0) > 26)
                        TurnTank(tank, 1);
                    else TurnTank(tank, -1);
                }

                if (Rand(100, 0) < 50)
                {
                    Bullet bull = tank.Shoot(Field, 1);

                    if (bull.CoordY >= 0 && bull.CoordX >= 0 && bull.CoordX < 7 && bull.CoordY < 12 && Matrix[bull.CoordX, bull.CoordY] != 5 && Matrix[bull.CoordX, bull.CoordY] != 1)
                    {
                        Field.Children.Add(bull.Img);
                        Grid.SetColumn(bull.Img, bull.CoordY);
                        Grid.SetRow(bull.Img, bull.CoordX);
                        bullets.Add(bull);
                    }

                }
            }
        }

        private void HealthArmorTick(object sender, EventArgs e)
        {
            Player1Health.Value = tanks[0].Hp;
            Player1Armor.Value = tanks[0].Armor * 25;
            if(Player1Health.Value == 0)
            {
                Image1.Visibility = Visibility.Visible;
                Button1.IsEnabled = false;
                Button2.IsEnabled = false;
                Button3.IsEnabled = false;
                Button4.IsEnabled = false;
                MessageBox.Show("You lost! Try again next time with new decks!");
                this.Close();
            }

            Player2Health.Value = tanks[1].Hp;
            Player2Armor.Value = tanks[1].Armor * 25;
            if (Player2Health.Value == 0)
            {
                Image2.Visibility = Visibility.Visible;
            }

            Player3Armor.Value = tanks[2].Armor * 25;
            Player3Health.Value = tanks[2].Hp;
            if (Player3Health.Value == 0)
            {
                Image3.Visibility = Visibility.Visible;
            }

            Player4Health.Value = tanks[3].Hp;
            Player4Armor.Value = tanks[3].Armor * 25;
            if (Player4Health.Value == 0)
            {
                Image4.Visibility = Visibility.Visible;
            }

            playersAlive = 4;
            foreach (Tank tank in tanks.ToList())
            {
                if(tank.Hp == 0)
                {
                    playersAlive-=1;
                    Field.Children.Remove(tank.Img);
                    Matrix[tank.CoordX, tank.CoordY] = 0;
                }
                if (playersAlive == 1)
                {

                    StopTimers();
                    foreach (Tank tan in tanks.ToList())
                    {
                        if (tan.Hp != 0)
                        {
                            MessageBox.Show("We have a winner! Congratulations to " + tan.Name.ToString() + " for winning this game!");
                            if (tan.Name==tanks[0].Name)
                                MessageBox.Show("You won! Congratulations!");
                             else   MessageBox.Show("You didn't win this time but come for more with stronger scripts! Farewell and Good Luck!");
                             MessageBox.Show("Thanks for playing!");
                            
                            this.Close();

                            break;
                        }
                    }
                }
            }
            

        }

        private void RemoveBullet(Bullet bullet)
        {
            Field.Children.Remove(bullet.Img);
            bullets.Remove(bullet);
        }

        private Tank GetTank(int x,int y)
        {
            foreach (Tank t in tanks)
            {
                if (t.CoordX == x && t.CoordY == y)
                    return t;
            }
            return null;
        }

        private bool Impact(Bullet bul)
        {
            if (bul.CoordX < 0 || bul.CoordY < 0 || bul.CoordX >= 7 || bul.CoordY >= 12)
                return false;

            if (Matrix[bul.CoordX, bul.CoordY] == 5)
            {
                RemoveBullet(bul);
                return false;
            }

            Tank t;
            if (Matrix[bul.CoordX, bul.CoordY] == 1)
            {

                t = GetTank(bul.CoordX, bul.CoordY);
                //  MessageBox.Show(t.Name.ToString() + " is hit! ");

                if (t.Name == tanks[0].Name || bul.Tnk.Name==tanks[0].Name)
                {
                    StopTimers();

                    Thread.Sleep(1000);

                    Window1 win = new Window1();
                    win.Show();
                    if (win.Valid() == true)
                    {
                        if(t.Name==tanks[0].Name)
                        {
                            t.IsShot(1, 0);
                        }
                        else
                        {
                            t.IsShot(0, 1);
                        }
                    }
                    else t.IsShot();

                    Thread.Sleep(3000);

                    StartTimers();
                }
                else t.IsShot();
                RemoveBullet(bul);
                return false;
            }

            return true;
        }

        private bool Impact(Tank tank)
        {
            if (tank.CoordX < 0 || tank.CoordY < 0 || tank.CoordX >= 7 || tank.CoordY >= 12)
                return false;

            if (Matrix[tank.CoordX, tank.CoordY] == 5 || Matrix[tank.CoordX, tank.CoordY] == 1)
                return false;
            
            return true;
        }
        
        private void InitializeMatrix()
        {
            for (int i = 0; i < noTanks; i++)
            {
                int orient = Rand(5,1);
                string uriString = @"Img/Tank" + orient.ToString() + ".png";

                Image img = new Image
                {
                    Source = new BitmapImage(new Uri(uriString, UriKind.Relative)),
                    Stretch = Stretch.Fill,
                    Opacity = 1,
                    Visibility= Visibility.Visible
                };
                int xM, yM, xm, ym;

                switch(i)
                {
                    case 0:
                        {
                            xM = 3;
                            xm = 0;
                            yM = 4;
                            ym = 0;
                            break;
                        }
                    case 1:
                        {
                            xM = 7;
                            xm = 4;
                            yM = 4;
                            ym = 0;
                            break;
                        }
                    case 2:
                        {
                            xM = 3;
                            xm = 0;
                            yM = 12;
                            ym = 7;
                            break;
                        }
                    default:
                        {
                            xM = 7;
                            xm = 4;
                            yM = 12;
                            ym = 7;
                            break;
                        }

                }  // in cele 4 cadrane

                Tank tank = new Tank(Rand(xM,xm), Rand(yM,ym),orient, "Tank" + (1+i).ToString(), img);
                tanks.Add(tank);

                Field.Children.Add(tank.Img);
                Grid.SetRow(tank.Img, tank.CoordX);
                Grid.SetColumn(tank.Img, tank.CoordY);

                Matrix[tank.CoordX,tank.CoordY] = 1;
            }  // Init Tanks

            for(int i=0;i<noHealthPickUps;i++)
            {
                Image img = new Image
                {
                    Source = new BitmapImage(new Uri(@"Img/Cross.png", UriKind.Relative)),
                    Stretch = Stretch.Fill,
                    Opacity = 1,
                    Visibility = Visibility.Visible
                };

                int x = Rand(7, 0);
                int y = Rand(12, 0);
                while (Matrix[x, y] != 0)
                {
                    x = Rand(7, 0);
                    y = Rand(12, 0);
                }

                PowerUp power = new PowerUp(x, y, img);
                PowerUps.Add(power);

                Field.Children.Add(img);
                Grid.SetRow(img, x);
                Grid.SetColumn(img, y);

                Matrix[x,y] = 2;

            } // Init Health

            for (int i = 0; i < noMoneyPickUps; i++)
            {
                Image img = new Image
                {
                    Source = new BitmapImage(new Uri(@"Img/Cash.png", UriKind.Relative)),
                    Stretch = Stretch.Fill,
                    Opacity = 1,
                    Visibility = Visibility.Visible
                };

                int x = Rand(7, 0);
                int y = Rand(12, 0);
                while (Matrix[x, y] != 0)
                {
                    x = Rand(7, 0);
                    y = Rand(12, 0);
                }

                PowerUp power = new PowerUp(x, y, img);
                PowerUps.Add(power);

                Field.Children.Add(img);
                Grid.SetRow(img, x);
                Grid.SetColumn(img, y);

                Matrix[x, y] = 3;

            } // Init Money

            for (int i = 0; i < noArmorPickUps; i++)
            {
                Image img = new Image
                {
                    Source = new BitmapImage(new Uri(@"Img/ArmU.png", UriKind.Relative)),
                    Stretch = Stretch.Fill,
                    Opacity = 1,
                    Visibility = Visibility.Visible
                };

                int x = Rand(7, 0);
                int y = Rand(12, 0);
                while (Matrix[x, y] != 0)
                {
                    x = Rand(7, 0);
                    y = Rand(12, 0);
                }

                PowerUp power = new PowerUp(x, y, img);
                PowerUps.Add(power);

                Field.Children.Add(img);
                Grid.SetRow(img, x);
                Grid.SetColumn(img, y);

                Matrix[x, y] = 4;

            } // Init Armor
                
            for(int i=0;i<noStops;i++)
            {
                Image img = new Image
                {
                    Source = new BitmapImage(new Uri(@"Img/Tree.png", UriKind.Relative)),
                    Stretch = Stretch.Fill,
                    Opacity = 1,
                    Visibility = Visibility.Visible
                };

                Field.Children.Add(img);

                int x = Rand(7, 0);
                int y = Rand(12, 0);
                while (Matrix[x, y] != 0)
                {
                    x = Rand(7, 0);
                    y = Rand(12, 0);
                }

                Grid.SetRow(img, x);
                Grid.SetColumn(img, y);

                Matrix[x, y] = 5;

            } // Init Stops

            {
                int c = 0;
                for (int i = 0; i < 7; i++)
                {
                    for (int j = 0; j < 12; j++)
                    {
                        if (Matrix[i, j] == 5)
                        {
                            Stops[0, c] = i;
                            Stops[1, c] = j;
                            c++;
                        }
                    }
                }
            } // Init Stops Array

            // while (Matrix[x, y] != 0 && Matrix[x - 1, y] != 0 && Matrix[x - 1, y - 1] != 0 && Matrix[x - 1, y + 1] != 0 && Matrix[x, y - 1] != 0 && Matrix[x, y + 1] != 0 && Matrix[x + 1, y - 1] != 0 && Matrix[x + 1, y + 1] != 0 && Matrix[x + 1, y] != 0)
            // nothing around
        }

        private int Rand(int max,int min=0)
        {
            Thread.Sleep(10);
            Random random = new Random();
            return random.Next(min, max);
        }

        private void MoveTank(Tank tank, int x)
        {
            switch (tank.Orient)
            {
                case 1:
                    {
                        Matrix[tank.CoordX, tank.CoordY] = 0;
                        tank.CoordY -= x;
                        if (Impact(tank) == false)
                        {
                            tank.CoordY += x;
                            break;
                        }
                        if(Matrix[tank.CoordX, tank.CoordY] != 0)
                        {
                            switch (Matrix[tank.CoordX, tank.CoordY])
                            {

                                case 2:
                                    {
                                        tank.HealthPickUP();

                                        foreach (PowerUp pow in PowerUps)
                                        {
                                            if(pow.CoordX == tank.CoordX && pow.CoordY==tank.CoordY)
                                            {
                                                Field.Children.Remove(pow.Img);
                                                PowerUps.Remove(pow);
                                                break;
                                            }
                                        }
                                        break;
                                    }
                                case 3:
                                    {
                                        tank.MoneyPickUP();

                                        foreach (PowerUp pow in PowerUps)
                                        {
                                            if (pow.CoordX == tank.CoordX && pow.CoordY == tank.CoordY)
                                            {
                                                Field.Children.Remove(pow.Img);
                                                PowerUps.Remove(pow);
                                                break;
                                            }
                                        }
                                        break;
                                    }
                                case 4:
                                    {
                                        tank.ArmorPickUP();

                                        foreach (PowerUp pow in PowerUps)
                                        {
                                            if (pow.CoordX == tank.CoordX && pow.CoordY == tank.CoordY)
                                            {
                                                Field.Children.Remove(pow.Img);
                                                PowerUps.Remove(pow);
                                                break;
                                            }
                                        }
                                        break;
                                    }

                            }

                        }

                        Matrix[tank.CoordX, tank.CoordY] = 1;
                        

                        Grid.SetColumn(tank.Img, tank.CoordY);
                        break;
                    }
                case 2:
                    {
                        Matrix[tank.CoordX, tank.CoordY] = 0;
                        tank.CoordX -= x;
                        if (Impact(tank) == false)
                        {
                            tank.CoordX += x;
                            break;
                        }

                        if (Matrix[tank.CoordX, tank.CoordY] != 0)
                        {
                            switch (Matrix[tank.CoordX, tank.CoordY])
                            {
                                case 2:
                                    {
                                        tank.HealthPickUP();
                                        foreach (PowerUp pow in PowerUps)
                                        {
                                            if (pow.CoordX == tank.CoordX && pow.CoordY == tank.CoordY)
                                            {
                                                Field.Children.Remove(pow.Img);
                                                PowerUps.Remove(pow);
                                                break;
                                            }
                                        }
                                        break;
                                    }
                                case 3:
                                    {
                                        tank.MoneyPickUP();
                                        foreach (PowerUp pow in PowerUps)
                                        {
                                            if (pow.CoordX == tank.CoordX && pow.CoordY == tank.CoordY)
                                            {
                                                Field.Children.Remove(pow.Img);
                                                PowerUps.Remove(pow);
                                                break;
                                            }
                                        }
                                        break;
                                    }
                                case 4:
                                    {
                                        tank.ArmorPickUP();
                                        foreach (PowerUp pow in PowerUps)
                                        {
                                            if (pow.CoordX == tank.CoordX && pow.CoordY == tank.CoordY)
                                            {
                                                Field.Children.Remove(pow.Img);
                                                PowerUps.Remove(pow);
                                                break;
                                            }
                                        }
                                        break;
                                    }
                            }
                        }

                        Matrix[tank.CoordX, tank.CoordY] = 1;
                        Grid.SetRow(tank.Img, tank.CoordX);
                        break;
                    }
                case 3:
                    {
                        Matrix[tank.CoordX, tank.CoordY] = 0;
                        tank.CoordY += x;
                        if (Impact(tank) == false)
                        {
                            tank.CoordY -= x;
                            break;
                        }

                        if (Matrix[tank.CoordX, tank.CoordY] != 0)
                        {
                            switch (Matrix[tank.CoordX, tank.CoordY])
                            {
                                case 2:
                                    {
                                        tank.HealthPickUP();
                                        foreach (PowerUp pow in PowerUps)
                                        {
                                            if (pow.CoordX == tank.CoordX && pow.CoordY == tank.CoordY)
                                            {
                                                Field.Children.Remove(pow.Img);
                                                PowerUps.Remove(pow);
                                                break;
                                            }
                                        }
                                        break;
                                    }
                                case 3:
                                    {
                                        tank.MoneyPickUP();
                                        foreach (PowerUp pow in PowerUps)
                                        {
                                            if (pow.CoordX == tank.CoordX && pow.CoordY == tank.CoordY)
                                            {
                                                Field.Children.Remove(pow.Img);
                                                PowerUps.Remove(pow);
                                                break;
                                            }
                                        }
                                        break;
                                    }
                                case 4:
                                    {
                                        tank.ArmorPickUP();
                                        foreach (PowerUp pow in PowerUps)
                                        {
                                            if (pow.CoordX == tank.CoordX && pow.CoordY == tank.CoordY)
                                            {
                                                Field.Children.Remove(pow.Img);
                                                PowerUps.Remove(pow);
                                                break;
                                            }
                                        }
                                        break;
                                    }
                            }
                        }

                        Matrix[tank.CoordX, tank.CoordY] = 1;
                        Grid.SetColumn(tank.Img, tank.CoordY);
                        break;
                    }
                case 4:
                    {
                        Matrix[tank.CoordX, tank.CoordY] = 0;
                        tank.CoordX += x;
                        if (Impact(tank) == false)
                        {
                            tank.CoordX -= x;
                            break;
                        }

                        if (Matrix[tank.CoordX, tank.CoordY] != 0)
                        {
                            switch (Matrix[tank.CoordX, tank.CoordY])
                            {
                                case 2:
                                    {
                                        tank.HealthPickUP();
                                        foreach (PowerUp pow in PowerUps)
                                        {
                                            if (pow.CoordX == tank.CoordX && pow.CoordY == tank.CoordY)
                                            {
                                                Field.Children.Remove(pow.Img);
                                                PowerUps.Remove(pow);
                                                break;
                                            }
                                        }
                                        break;
                                    }
                                case 3:
                                    {
                                        tank.MoneyPickUP();
                                        foreach (PowerUp pow in PowerUps)
                                        {
                                            if (pow.CoordX == tank.CoordX && pow.CoordY == tank.CoordY)
                                            {
                                                Field.Children.Remove(pow.Img);
                                                PowerUps.Remove(pow);
                                                break;
                                            }
                                        }
                                        break;
                                    }
                                case 4:
                                    {
                                        tank.ArmorPickUP();
                                        foreach (PowerUp pow in PowerUps)
                                        {
                                            if (pow.CoordX == tank.CoordX && pow.CoordY == tank.CoordY)
                                            {
                                                Field.Children.Remove(pow.Img);
                                                PowerUps.Remove(pow);
                                                break;
                                            }
                                        }
                                        break;
                                    }
                            }
                        }

                        Matrix[tank.CoordX, tank.CoordY] = 1;
                        Grid.SetRow(tank.Img, tank.CoordX);
                        break;
                    }
            }
        }

        private void TurnTank(Tank tank,int x)
        {
            if (tank.Orient == 1 && x == -1)
            {
                tank.Orient = 4;
            }
            else if (tank.Orient == 4 && x == 1)
            {
                tank.Orient = 1;
            }
            else tank.Orient += x;

            string stringUri = @"Img/Tank" + tank.Orient.ToString() + ".png";
            tank.Img.Source = new BitmapImage(new Uri(stringUri, UriKind.Relative));

            //Field.Children.Remove(tank.Img);
            //Field.Children.Add(tank.Img);
            //Grid.SetColumn(tank.Img, tank.CoordY);
            //Grid.SetRow(tank.Img, tank.CoordX);
        }
        

        private void BulletTick(object sender, EventArgs e)
        {
            foreach (Bullet bul in bullets.ToList())
            switch (bul.Orient)
                {
                     case 1:
                        {
                            bul.CoordY--;
                            if (Impact(bul) == false)
                            {
                                RemoveBullet(bul);
                                break;
                            }
                            Grid.SetColumn(bul.Img, bul.CoordY);
                            break;
                        }
                    case 2:
                        {
                            bul.CoordX--;

                            if (Impact(bul) == false)
                            {
                                RemoveBullet(bul);
                                break;
                            }
                            Grid.SetRow(bul.Img, bul.CoordX);
                            break;
                        }
                    case 3:
                        {
                            bul.CoordY++;
                            if (Impact(bul) == false)
                            {
                                RemoveBullet(bul);
                                break;
                            }
                            Grid.SetColumn(bul.Img, bul.CoordY);
                            break;
                        }
                    case 4:
                        {
                            bul.CoordX++;

                            if (Impact(bul) == false)
                            {
                                RemoveBullet(bul);
                                break;
                            }
                            Grid.SetRow(bul.Img, bul.CoordX);
                            break;
                        }
                }
            //here code
        }

        private void FastBulletTick(object sender, EventArgs e)
        {
            foreach (Bullet bul in fastBullets.ToList())
                switch (bul.Orient)
                {
                    case 1:
                        {
                            bul.CoordY--;
                            if (Impact(bul) == false)
                            {
                                RemoveBullet(bul);
                                break;
                            }
                            Grid.SetColumn(bul.Img, bul.CoordY);
                            break;
                        }
                    case 2:
                        {
                            bul.CoordX--;

                            if (Impact(bul) == false)
                            {
                                RemoveBullet(bul);
                                break;
                            }
                            Grid.SetRow(bul.Img, bul.CoordX);
                            break;
                        }
                    case 3:
                        {
                            bul.CoordY++;
                            if (Impact(bul) == false)
                            {
                                RemoveBullet(bul);
                                break;
                            }
                            Grid.SetColumn(bul.Img, bul.CoordY);
                            break;
                        }
                    case 4:
                        {
                            bul.CoordX++;

                            if (Impact(bul) == false)
                            {
                                RemoveBullet(bul);
                                break;
                            }
                            Grid.SetRow(bul.Img, bul.CoordX);
                            break;
                        }
                }
            //here code
        }

        private void Image_MouseLeftButtonDown_1(object sender, MouseButtonEventArgs e)
        {
            foreach(Tank tank in tanks)
            {
                MessageBox.Show("" + tank.Name + " la coord X:" + tank.CoordX + " si coord Y: " + tank.CoordY);
            }
            //MessageBox.Show("There are " + i.ToString() + " pictures in imgs");

        }

        private void Image_MouseLeftButtonDown_2(object sender, MouseButtonEventArgs e)
        {
            Bullet bull = tanks[Rand(4,0)].Shoot(Field, 1);

            if (bull.CoordY >= 0 && bull.CoordX >= 0 && bull.CoordX < 7 && bull.CoordY < 12 && Matrix[bull.CoordX,bull.CoordY] != 5 && Matrix[bull.CoordX, bull.CoordY] != 1)
            {
                Field.Children.Add(bull.Img);
                Grid.SetColumn(bull.Img, bull.CoordY);
                Grid.SetRow(bull.Img, bull.CoordX);
                bullets.Add(bull);
            }
        }

        private void StopTimers()
        {
            timer.Stop();
            timer1.Stop();
            AItimer.Stop();
            fastBulletTimer.Stop();
            timer2.Stop();
            bulletTimer.Stop();
            HealthArmor.Stop();
        }

        private void StartTimers()
        {
            timer.Start();
            //timer1.Start();
            AItimer.Start();
            fastBulletTimer.Start();
            //timer2.Start();
            bulletTimer.Start();
            HealthArmor.Start();
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            try
            {
                StopTimers();

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void Image_MouseLeftButtonDown_3(object sender, RoutedEventArgs e)
        {
            MoveTank(tanks[0],1);
        }

        private void Image_MouseLeftButtonDown_4(object sender, RoutedEventArgs e)
        {
            MoveTank(tanks[0],-1);

        }

        private void Image_MouseLeftButtonDown_5(object sender, RoutedEventArgs e)
        {

            TurnTank(tanks[0],1);
        }

        private void Image_MouseLeftButtonDown_6(object sender, RoutedEventArgs e)
        {
            TurnTank(tanks[0],-1);

        }

        private void Image_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (tanks[0].Armor < 4 && tanks[0].Money >= 40)
            {
                tanks[0].Armor ++;
                tanks[0].Money -= 40;

                popTime = 60;
                if (contorPop != 0)
                {
                    PopUpArmor.Opacity = 1.6;
                    contorPop = 0;
                }
                else
                {
                    New_TimerArmorPopUp();
                }
            }
            else  if(Player1Armor.Value == 100)
            { 
                PopUpArmor.Opacity = 0;
                popTime = 60;
                if (contorPop != 0)
                {
                    PopUpMaxArmor.Opacity = 1.6;
                    contorPop = 0;
                }
                else
                {
                    New_TimerMaxArmorPopUp();
                }
            }
        }

        private void DispatcherTimer_Tick(object sender, EventArgs e)
        {
            foreach (Tank tank in tanks)
            {
                tank.Money += 1;
            }
            MoneyBox.Text = "Money: " + tanks[0].Money.ToString();
            {
                /*
                if(Player1Armor.Value==100)
                {
                    Player1Armor.Value = Player2Armor.Value = Player3Armor.Value = Player4Armor.Value = 0;
                }
                else
                {
                    Player1Armor.Value = Player2Armor.Value = Player3Armor.Value = Player4Armor.Value += 25;
                }

                if (Player1Health.Value == 100)
                {
                    Player1Health.Value = Player2Health.Value = Player3Health.Value = Player4Health.Value = 0;
                } 
                else
                {
                    Player1Health.Value = Player2Health.Value = Player3Health.Value = Player4Health.Value += 1;
                }*/
            }
        }

        private void New_TimerArmorPopUp()
        {
            timer1.Start();

            PopUpArmor.Opacity = 1.6;
        }

        private void New_TimerMaxArmorPopUp()
        {
            PopUpMaxArmor.Opacity = 1.6;
            contorPop = 0;
            timer2.Start();
        }

        private void DispatcherTimer_Tick1(object sender, EventArgs e)
        {
            contorPop += 1;
            PopUpArmor.Opacity -= 0.02;
            if (contorPop == popTime)
            {
                timer1.Stop();
                contorPop = 0;
                PopUpArmor.Opacity = 0;
            }
        }


        private void FastBullet(object sender, MouseButtonEventArgs e)
        {
            if (tanks[0].Money >= 20)
            {
                tanks[0].Money -= 20;
                Bullet bull = tanks[0].Shoot(Field,2);

                if (bull.CoordY >= 0 && bull.CoordX >= 0 && bull.CoordX < 7 && bull.CoordY < 12 && Matrix[bull.CoordX, bull.CoordY] != 5 && Matrix[bull.CoordX, bull.CoordY] != 1)
                {
                    Field.Children.Add(bull.Img);
                    Grid.SetColumn(bull.Img, bull.CoordY);
                    Grid.SetRow(bull.Img, bull.CoordX);
                    fastBullets.Add(bull);
                }
            }
        }

        private void Sacrifice(object sender, MouseButtonEventArgs e)
        {
            if(tanks[0].Money >= 20 && tanks[0].Hp>=30 && tanks[0].Armor<4)
            {
                tanks[0].Money -= 20;
                tanks[0].Hp -= 30;
                tanks[0].Armor++;
            }
        }

        private void HealthPack(object sender, MouseButtonEventArgs e)
        {
            Bullet bull = tanks[0].Shoot(Field, 1);

            if (bull.CoordY >= 0 && bull.CoordX >= 0 && bull.CoordX < 7 && bull.CoordY < 12 && Matrix[bull.CoordX, bull.CoordY] != 5 && Matrix[bull.CoordX, bull.CoordY] != 1)
            {
                Field.Children.Add(bull.Img);
                Grid.SetColumn(bull.Img, bull.CoordY);
                Grid.SetRow(bull.Img, bull.CoordX);
                bullets.Add(bull);
            }
        }

        private void DispatcherTimer_Tick2(object sender, EventArgs e)
        {
            contorPop += 1;
            PopUpMaxArmor.Opacity -= 0.02;
            if (contorPop == popTime)
            {
                timer2.Stop();
                contorPop = 0;
                PopUpMaxArmor.Opacity = 0;
            }
        }
    }

}
