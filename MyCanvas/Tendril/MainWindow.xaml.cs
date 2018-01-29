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
using System.Windows.Threading; 

namespace Tendril
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        DispatcherTimer mUpdate = new DispatcherTimer();//游戏主循环的作用
        float mDeltaTime = 1000/60f;//ms,fps=60

        //先用弹球试试
        bool mb_Pause = true;
        float mBallRadius = 10f;
        Vector2 mSceneSize;
        Vector2 mBallPos;
        Vector2 mBallVel;

        //tendrils
        Tendrils mTendrils;

        public MainWindow()
        {
            InitializeComponent();
            
            //InitScene();//放到Loaded event中

            mUpdate.Interval = TimeSpan.FromMilliseconds((int)mDeltaTime);
            mUpdate.Tick += new EventHandler(HandleTick);
            this.KeyDown += new KeyEventHandler(HandleKeyDown);
            mUpdate.Start();
        }

        void InitScene(object sender, EventArgs e)
        {
            //mSceneSize=new Vector2((float)sceneCanvas.Width,(float)sceneCanvas.Height);//得到的是NAN
            mSceneSize = new Vector2((float)sceneCanvas.ActualWidth, (float)sceneCanvas.ActualHeight);
            //initialize ball
            mBallPos=new Vector2(mBallRadius,mBallRadius);
            mBallVel = new Vector2(0.15f, 0.25f);
            ballEllipse.Width = ballEllipse.Height = 2 * mBallRadius;
            ballEllipse.SetValue(Canvas.LeftProperty, (double)(mBallPos.x - mBallRadius));
            ballEllipse.SetValue(Canvas.TopProperty, (double)(mBallPos.y - mBallRadius));

            mTendrils = new Tendrils(path, PFC);
        }

        void HandleTick(object sender, EventArgs e)
        {
            if (!mb_Pause)
            {
                UpdateBall();
                mTendrils.Update(1/60f*60);
                mTendrils.Draw();
                
            }
        }

        void UpdateBall()
        {
            //have no physics engine,手动碰撞检测 & numerical integration
            mBallPos += mBallVel*mDeltaTime;//暂且假定t=1
            if (mBallPos.x - mBallRadius < 0)
            {
                mBallPos.x = mBallRadius;
                mBallVel.x *= -1;
            }
            else if (mBallPos.x + mBallRadius > mSceneSize.x)
            {
                mBallPos.x = mSceneSize.x - mBallRadius;
                mBallVel.x *= -1;
            }

            if (mBallPos.y - mBallRadius < 0)
            {
                mBallPos.y = mBallRadius;
                mBallVel.y *= -1;
            }
            else if (mBallPos.y + mBallRadius > mSceneSize.y)
            {
                mBallPos.y = mSceneSize.y - mBallRadius;
                mBallVel.y *= -1;
            }

            ballEllipse.SetValue(Canvas.LeftProperty, (double)(mBallPos.x - mBallRadius));
            ballEllipse.SetValue(Canvas.TopProperty, (double)(mBallPos.y - mBallRadius));
        }

        void HandleKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.P)
            {
                mb_Pause = !mb_Pause;
            }
        }

        void HandleMouseMove(object sender, MouseEventArgs e)
        {
            Point pointer=e.GetPosition(sceneCanvas);
            mTendrils.SetTarget(new Vector2((float)pointer.X, (float)pointer.Y));
        }
        


    }
}
