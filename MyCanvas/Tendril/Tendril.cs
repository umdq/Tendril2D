using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
//WPF相关
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace Tendril
{
    class CTendril
    {
        class Node
        {
            public Vector2 pos;
            public Vector2 vel;
            //public Point pos;
            //public Point vel;
        }

        float mSpring;//base劲度系数
        float mDampening = 0.25f;
        float mTension = 0.98f;
        float mFriction;
        int mNodeNum = 50;
        Node[] mNodes;

        Vector2 mTarget;
        public Vector2 Target
        {
            set
            {
                mTarget = value;
            }
        }

        //WPf相关
        PathFigureCollection mPFC;//外面传过来
        PathFigure mPF;//start point
        //PointCollection mPoints;
        PolyQuadraticBezierSegment mSegment;

        void InitControls()
        {
            mPF = new PathFigure();
            mPFC.Add(mPF);

            /*
            int polyNum = 2 * mNodeNum - 4;//n+(n-2-1)-1polyStart
            mPoints = new PointCollection(polyNum);
            //for (int i = 0; i < polyNum; i++)
            //{
                //mPoints.Add(new Point());
            //}//mPoints[index]返回的是copy值而不是引用，不宜修改，尤其元素是值类型时
            PolyQuadraticBezierSegment segment = new PolyQuadraticBezierSegment();
            segment.Points = mPoints;
            mPF.Segments.Add(segment);
             */
            mSegment = new PolyQuadraticBezierSegment();
            mPF.Segments.Add(mSegment);
        }

        public CTendril(float spring, float friction,PathFigureCollection pfc)
        {
            mSpring = spring;
            mFriction=friction;
            mNodes = new Node[mNodeNum];
            for (int i = 0; i < mNodeNum; i++)
            {
                mNodes[i] = new Node();
            }

            mPFC = pfc;
            InitControls();
        }

        public void Update(float deltaTime)
        {
            float spring = mSpring;
            Node node = mNodes[0];
            //head
            //node.vel += (mTarget - node.pos) * spring * deltaTime * mFriction;//暂且假定m=1吧
            //debug了老半天，发现就头部没法收敛！原来问题出在这！
            //mFriction作为sum的modify factor而不只是修饰添加部分，并不参与numerical integration，两个式子不等价
            node.vel += (mTarget - node.pos) * spring * deltaTime;
            node.vel *= mFriction;
            node.pos+=node.vel*deltaTime;
            //debug
            /*if (mNodes[mNodeNum-1].pos.x > 0.1)
            {
                int hehe = 1 + 1;//断点
            }*/
            //others
            spring*=mTension;
            for (int i = 1; i < mNodeNum; i++)
            {
                node = mNodes[i];
                Node prev = mNodes[i - 1];
                node.vel += (prev.pos - node.pos) * spring * deltaTime + prev.vel * mDampening;
                node.vel*=mFriction ;
                node.pos += node.vel * deltaTime;
                spring *= mTension;
            }
        }

        //wpf相关啊，不能分离开很丑陋
        //——》
        //不想写了。对于通过控制点画二次Bezier Curve，
        //wpf的<PathFigure StartPoint="">与<PolyQuadraticBezierSegment Points=""/>嵌套这么多层简直就是一坨屎，不适宜programaticallly draw；
        //unity没提供通过control point draw Bezier Curve的API，懒得自己用LineRender造轮子了；
        //反而是html5的canvas舒服，直接ctx.quadraticCurveTo()，虽然讨厌JS；
        //——》
        //哈哈，弃置几天整理完要事后，打算还是写完它吧
        public void Draw()
        {
            mPF.StartPoint = new Point(mNodes[0].pos.x,mNodes[0].pos.y);
            mSegment.Points.Clear();
            int endIndex=mNodeNum-2;
            for (int i = 1; i < endIndex; i++)
            {
                //control
                Point control = new Point(mNodes[i].pos.x, mNodes[i].pos.y);
                //end
                Vector2 mid = (mNodes[i].pos + mNodes[i + 1].pos) * 0.5f;
                Point end = new Point(mid.x, mid.y);
                mSegment.Points.Add(control);
                mSegment.Points.Add(end);
            }
            Point lastControl = new Point(mNodes[endIndex].pos.x, mNodes[endIndex].pos.y);
            Point lastEnd = new Point(mNodes[mNodeNum - 1].pos.x, mNodes[mNodeNum - 1].pos.y);
            mSegment.Points.Add(lastControl);
            mSegment.Points.Add(lastEnd);
        }
        

    }

    /// <summary>
    ///  变色
    /// </summary>
    class Oscillator
    {
        float mPhase;
        float mOffset=285f;
        float mFrequency = 0.0015f;
        float mAmplitude=85f;
        float mResult;
        public float valueY
        {
            get
            {
                return mResult;
            }
        }

        public Oscillator(float phase)
        {
            mPhase = (float)Math.PI * 2 * phase;
        }

        public void Update(float deltaTime)
        {
            mPhase += mFrequency * deltaTime;
            mResult = mAmplitude * (float)Math.Sin(mPhase) + mOffset;
        }
    }

    class Tendrils
    {
        float mBaseFriction=0.5f;
        int mTendrilNum=20;
        CTendril[] mTendrils;

        //wpf相关,本来只要传一个canvas然后全部procedurally生成，懒得写了就固定吧
        Path mPath;
        PathFigureCollection mPFC;

        Oscillator mOscillator;

        public Tendrils(Path path,PathFigureCollection pfc)
        {
            mPath = path;
            mPFC = pfc;

            mTendrils = new CTendril[mTendrilNum];
            Random random=new Random();
            for (int i = 0; i < mTendrilNum; i++)
            {
                float spring = 0.45f + 0.025f * (i / mTendrilNum);
                spring+=(float)(random.NextDouble() * 0.1 - 0.05);
                float friction = mBaseFriction + (float)(random.NextDouble() * 0.01 - 0.005);
                mTendrils[i] = new CTendril(spring, friction,mPFC);
            }

            mOscillator = new Oscillator((float)random.NextDouble());
        }

        public void Update(float deltaTime)
        {
            for (int i = 0; i < mTendrilNum; i++)
            {
                mTendrils[i].Update(deltaTime);
            }
            mOscillator.Update(deltaTime);
        }

        float hue2rgb(float p, float q, float t)
        {
            if (t < 0) t += 1;
            if (t > 1) t -= 1;
            if (t < 1 / 6.0f) return p + (q - p) * 6 * t;
            if (t < 1 / 2.0f) return q;
            if (t < 2 / 3.0f) return p + (q - p) * (2 / 3.0f - t) * 6;
            return p;
        }
        Color hslToRgb(float h, float s, float l,float a)
        {
            //h clamp to [0,1]!
            h /= 360f;
            float r, g, b;
            if(s <float.Epsilon) 
            {
                r = g = b = l; // achromatic
            } 
            else 
            {
                float q = l < 0.5f ? l * (1 + s) : l + s - l * s;
                float p = 2 * l - q;
                r = hue2rgb(p, q, h + 1/3.0f);
                g = hue2rgb(p, q, h);
                b = hue2rgb(p, q, h - 1/3.0f);
            }
            return Color.FromScRgb(a, r, g, b);
        }

        public void Draw()
        {
            //mPath.Fill = new SolidColorBrush(Color.FromScRgb(0.4f, 8f, 5f, 16f));
            //mPath.Width = 1;
            mPath.StrokeThickness = 1;
            mPath.Stroke = new SolidColorBrush(hslToRgb(mOscillator.valueY, 0.9f, 0.5f, 1f));
            for (int i = 0; i < mTendrilNum; i++)
            {
                mTendrils[i].Draw();
            }
        }

        public void SetTarget(Vector2 target)
        {
            for (int i = 0; i < mTendrilNum; i++)
            {
                mTendrils[i].Target = target;
            }
        }

    }
}
