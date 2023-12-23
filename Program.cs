using SFML.Graphics;
using SFML.Window;
using SFML.System;
using System;
using System.Diagnostics;

namespace Game
{
    public class TileSet : Sprite
    {
        public int Layer;
        public TileSet(Texture texture, int layer_)
        {
            Texture = texture;
            Layer = layer_;
        }
    }
    public class Enemy : Sprite
    {
        public int Direction;
        public float gravity;
        public Vector2f velocity;
        public bool isCollide;
        FloatRect rect;
        int Width;
        int Height;
        int[] posArray;
        public Enemy()
        {   
            posArray = new int[2] { 0, 1200};
            Texture = new Texture("Enemy.png");
            Position = new Vector2f(posArray[new Random().Next(0, 2)], 0);
            if (Position.X == 0) Direction = 1;
            else if (Position.X == 1200) Direction = -1;

            Width = (int)GetGlobalBounds().Width;
            Height = (int)GetGlobalBounds().Height;
        }
        public void Move(float xVel, float yVel, float delta)
        {
            gravity += 5 * delta;
            velocity.Y = gravity;
            if (Direction == -1) TextureRect = new IntRect(Width, 0,  - Width, Height);
            if (Direction == 1) TextureRect = new IntRect(0, 0,  Width, Height); 
            Position += new Vector2f(xVel * 250 * delta, yVel * 250 * delta);
        }
        public void Collide(float xVel ,float yVel, TileSet[] tiles)
        {
            for(int i=0; i<tiles.Length; i++)
            {
                if (tiles[i] != null)
                {
                    if (GetGlobalBounds().Intersects(tiles[i].GetGlobalBounds()))
                    {
                        isCollide = true;
                        rect = tiles[i].GetGlobalBounds();
                        break;
                    }
                    else isCollide = false;
                }
            }
            if (isCollide)
            {
                if(xVel > 0)
                {
                    Position = new Vector2f(rect.Left - Width, Position.Y);
                }
                if (xVel < 0)
                {
                    Position = new Vector2f(rect.Left + rect.Width, Position.Y);
                    //gravity = 0;
                }
                if(yVel > 0)
                {
                    Position = new Vector2f(Position.X, rect.Top - Height);
                    gravity = 0;
                }
                if(yVel < 0)
                {
                    Position = new Vector2f(Position.X, rect.Top + rect.Height);
                }
            }
        }
        public void Update(float delta, TileSet[] tiles_, RenderWindow window, List<Bullet> bullets, List<Enemy> enemys)
        {
            velocity = new Vector2f(0, 0);
            for(int i=0; i<bullets.Count; i++)
            {
                if (GetGlobalBounds().Intersects(bullets[i].GetGlobalBounds()))
                {
                    bullets.RemoveAt(i);
                    enemys.Remove(this);
                    GameProcess.Socre++;
                }
            }
            
            Move(Direction, 0, delta);
            Collide(Direction, 0, tiles_);
            Move(0, velocity.Y, delta);
            Collide(0, velocity.Y, tiles_);

            //Console.WriteLine(gravity);
            window.Draw(this);
        }
    }
    public class Bullet : RectangleShape
    {
        int Direction;
        public Bullet(int direction, Vector2f pos)
        {
            FillColor = Color.Yellow;
            Size = new Vector2f(10, 5);
            Direction = direction;
            Position = pos;
        }
        public void Update(RenderWindow window, float delta)
        {
            Position += new Vector2f(Direction * 800 * delta, 0);

            window.Draw(this);
        }
    }
    
    public class Player : Sprite
    {
        public bool isCollide=true;
        public Vector2f velocity;
        FloatRect rect = new FloatRect();
        public float gravity;
        public bool isGround;
        public bool mouseEntered;
        public int Width;
        public int Height;
        public int Direction;
        public float shootDelay;
        public float shootTimer;
        public bool canShoot;
        public Player()
        {
            Position = new Vector2f(450, 10000);
            Texture = new Texture("Player.png");
            Width = (int)GetGlobalBounds().Width;
            Height = (int)GetGlobalBounds().Height;
            
            Direction = 1;
            shootDelay = 0.2f;

        }
        public void Move(float xVel, float yVel, float delta)
        {
            
            if (Keyboard.IsKeyPressed(Keyboard.Key.Space) && isGround) 
            {
                gravity = -5.5f;
            }
            gravity += 10 * delta;
            velocity.Y = gravity;

            if (xVel > 0) TextureRect = new IntRect(0, 0, Width, Height);
            if (xVel < 0) TextureRect = new IntRect(Width, 0, -Width, Height);

            if (xVel > 0) Direction = 1;
            if (xVel  < 0) Direction = -1;
            Position += new Vector2f(xVel * 300 * delta, yVel * 150 * delta);
        }
        public void Shoot(float delta)
        {
            shootTimer += delta;
            if(shootTimer >= shootDelay)
            {
                canShoot = true;
                shootTimer = 0;
            }
            if (Mouse.IsButtonPressed(0) && canShoot)
            {
                GameProcess.bullets.Add(new Bullet(Direction, new Vector2f(Position.X, Position.Y+25)));
                canShoot = false;
            }
        }
        public void Collide(float xVel, float yVel, TileSet[] tiles)
        {
            for(int i=0; i<tiles.Length; i++)
            {
                if (tiles[i] != null)
                {
                    if (GetGlobalBounds().Intersects(tiles[i].GetGlobalBounds()) && !mouseEntered)
                    {
                        isCollide = true;
                        rect = tiles[i].GetGlobalBounds();
                        break;
                    }
                    else isCollide = false;
                }
                
            }
            if (isCollide)
            {
                if(yVel > 0)
                {
                    Position = new Vector2f(Position.X, rect.Top - Height);
                    gravity = 0;
                    isGround = true;
                }
                if(yVel < 0)
                {
                    Position = new Vector2f(Position.X, rect.Top + rect.Height);
                    gravity = 0;
                }
                if (xVel > 0)
                {
                    Position = new Vector2f(rect.Left - Width, Position.Y);
                }
                if(xVel < 0)
                {
                    Position = new Vector2f(rect.Left + rect.Width, Position.Y);
                }
            }
        }
        public void Update(TileSet[] tiles, float delta_)
        {
            velocity.X = 0;
            if (Position.Y >= 5000) Position = new Vector2f(Position.X, 250);

            if (Keyboard.IsKeyPressed(Keyboard.Key.A)) velocity.X += -1;
            if (Keyboard.IsKeyPressed(Keyboard.Key.D)) velocity.X += 1;

            velocity.X = Math.Clamp(velocity.X, -1,1 );

            Move(velocity.X, 0, delta_);
            Collide(velocity.X, 0, tiles);
            Move(0, velocity.Y, delta_);
            Collide(0, velocity.Y, tiles);

            Shoot(delta_);
            if (!isCollide && (velocity.Y <= 0 || velocity.Y >= 0)) isGround = false;
        }
    }
    public class GameProcess
    {   
        //Creating Window
        public static RenderWindow window = new RenderWindow(new VideoMode(1250, 700), "TileMap");

        //Variable of delta time
        static float dt;

        
        //Map and tiles
        TileSet[] Maps = new TileSet[350];
        TileSet[] tileArr = new TileSet[10];

        //BulletList
        public static List<Bullet> bullets = new List<Bullet>();

        //Clock
        Clock clock = new Clock();

        //Player
        Player player;

        RectangleShape mouseRect = new RectangleShape(new Vector2f(50, 50));

        //Font
        Font fontArial = new Font("arial.ttf");

        Text textScore = new Text();
        public static int Socre;

        //FPS
        float FPS;
        Text fpsText = new Text();


        //Enemys
        List<Enemy> enemys;
        float spawnerTimer;
        float spawnerDelay;

        //GameOver Boolean
        public bool isGameOver;

        //Method for handle KeyEvent
        void OnKeyPressed(object sender, SFML.Window.KeyEventArgs e)
        {
            if(e.Code == Keyboard.Key.Enter && isGameOver)
            {
                Restart();
                GamePlay();
            }
        }

        //Method for restart game data
        public void Restart()
        {
            isGameOver = false;
            GameProcess.Socre = 0;
            enemys = new List<Enemy>();
        }

        //Main gameplay system
        public void GamePlay()
        {   
            //Spawner
            spawnerTimer += dt;
            if (spawnerTimer > spawnerDelay)
            {
                spawnerTimer = 0;
                spawnerDelay = new Random().Next(0, 5);
                enemys.Add(new Enemy());
            }

            //Drawing Entire Map
            for (int i = 0; i < 350; i++)
            {
                if (Maps[i] != null)
                {
                    window.Draw(Maps[i]);
                }
            }

            //Bullet Update
            for (int i = 0; i < bullets.Count; i++)
            {
                bullets[i].Update(window, dt);
                if (bullets[i].Position.X < 0 || bullets[i].Position.X > 1250) bullets.RemoveAt(i);
            }

            //Enemy Update
            for (int i = 0; i < enemys.Count; i++)
            {
                if (enemys[i].GetGlobalBounds().Intersects(player.GetGlobalBounds()))
                {
                    isGameOver = true;
                }
                enemys[i].Update(dt, Maps, window, bullets, enemys);
            }

            //Player Update
            player.Update(Maps, dt);
            window.Draw(player);

            //Drawing Text Score
            textScore.DisplayedString = "Score : " + Socre.ToString();
            window.Draw(textScore);
        }

        //Called when game GameOver
        public void GameOver()
        {
            Text header = new Text();
            header.Font = fontArial;
            header.CharacterSize = 100;
            header.DisplayedString = "Game Over!!!!";
            header.Position = new Vector2f((1250 - header.GetGlobalBounds().Width) / 2, 20);

            Text instruction = new Text();
            instruction.Font = fontArial;
            instruction.CharacterSize = 30;
            instruction.DisplayedString = "Pencet 'ENTER' untuk main lagi";
            instruction.Position = new Vector2f((1250 - instruction.GetGlobalBounds().Width) /2, 300);

            window.KeyPressed += OnKeyPressed;

            window.Draw(instruction);
            window.Draw(header);
        }

        //MainLoop Method
        public void Process()
        {   
            //Delta Time
            dt = clock.Restart().AsSeconds();

            //Window
            window.Closed += (sender, args) => window.Close();

            //Array of Tiles
            tileArr[1] = new TileSet(new Texture("Grass.png"), 1);
            tileArr[2] = new TileSet(new Texture("Dirt.png"), 2);
            tileArr[3] = new TileSet(new Texture("Box.png"), 3);

            spawnerDelay = new Random().Next(0, 5);

            int[] mapData =
            {
                0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
                0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
                0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
                0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
                0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
                0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
                0, 0, 0, 0, 0, 0, 0, 0, 3, 3, 3, 0, 0, 3, 3, 3, 0, 0, 0, 0, 0, 0, 0, 0, 0,
                0, 0, 0, 0, 0, 3, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 3, 0, 0, 0, 0, 0, 0,
                0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
                0, 0, 3, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 3, 0, 0, 0,
                0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
                1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1,
                2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2,
                2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2,
            };

            int width = 25;
            int height = 14;

            //Generating Maps
            for(int i=0; i < width; i++)
            {
                for(int j=0; j < height; j++)
                {
                    int tileNumber = i + j * width;
                    if (mapData[tileNumber] != 0)
                    {
                        int mapValue = mapData[tileNumber];
                        TileSet tile = new TileSet(tileArr[mapValue].Texture, 0);
                        tile.Position = new Vector2f(i * 50, j * 50);
                        Maps[tileNumber] = tile;
                    }
                }
            }

            //Instantiate Player;
            player = new Player();
            player.Position = new Vector2f(450, 10000);

            //Score Text
            textScore.CharacterSize = 30;
            textScore.Font = fontArial;
            textScore.DisplayedString = "Score : " + Socre.ToString();

            //FPS Text
            fpsText.CharacterSize = 30;
            fpsText.DisplayedString = FPS.ToString();
            fpsText.Font = fontArial;
            fpsText.Position = new Vector2f(1200, 0);

            //MainLoop
            while (window.IsOpen)
            {
                dt = clock.Restart().AsSeconds();
                if (!isGameOver)
                {
                    GamePlay();
                }
                else GameOver();

                FPS = 1f / dt;
                fpsText.DisplayedString = FPS.ToString();

                window.Draw(fpsText);
                window.DispatchEvents();
                window.Display();
                window.Clear(new Color(50,130,246));
            }
        }
    }
    public class Program
    {
        public static void Main()
        {
            GameProcess gameProcess = new GameProcess();
            gameProcess.Restart();
            gameProcess.Process();
        }
    }
}