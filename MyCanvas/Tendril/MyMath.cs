using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tendril
{
    //typedef
    //using real = System.Double;


    /// <summary>
    /// float2,double版本的有现成的：System.Windows.Point
    /// </summary>
    public struct Vector2
    {
        public float x, y;
        public Vector2(float x = 0f, float y = 0f)
        {
            this.x = x;
            this.y = y;
        }

        public static Vector2 operator +(Vector2 lhs, Vector2 rhs)//c++ friend
        {
            return new Vector2(lhs.x + rhs.x, lhs.y + rhs.y);
        }
        public static Vector2 operator -(Vector2 lhs, Vector2 rhs)
        {
            return new Vector2(lhs.x - rhs.x, lhs.y - rhs.y);
        }
        public static Vector2 operator *(Vector2 v, float k)
        {
            return new Vector2(v.x * k, v.y * k);
        }
        public static Vector2 operator *(float k, Vector2 v)
        {
            return new Vector2(v.x * k, v.y * k);
        }
    }





}
