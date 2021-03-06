namespace UnityEngine
{
    using System;
    //using System.Reflection;
    //using System.Runtime.InteropServices;
    //using UnityEngine.Internal;
    //using UnityEngine.Scripting;

    //[StructLayout(LayoutKind.Sequential), UsedByNativeCode]
    public struct Vector2
    {
        public const float kEpsilon = 1E-05f;
        public float x;
        public float y;
        public Vector2(float x, float y)
        {
            this.x = x;
            this.y = y;
        }

        public float this[int index]
        {
            get
            {
                switch (index)
                {
                    case 0:
                        return this.x;

                    case 1:
                        return this.y;
                }
                throw new IndexOutOfRangeException("Invalid Vector2 index!");
            }
            set
            {
                switch (index)
                {
                    case 0:
                        this.x = value;
                        break;

                    case 1:
                        this.y = value;
                        break;

                    default:
                        throw new IndexOutOfRangeException("Invalid Vector2 index!");
                }
            }
        }
        public void Set(float new_x, float new_y)
        {
            this.x = new_x;
            this.y = new_y;
        }

        public static Vector2 Lerp(Vector2 a, Vector2 b, float t)
        {
            t = Mathf.Clamp01(t);
            return new Vector2(a.x + ((b.x - a.x) * t), a.y + ((b.y - a.y) * t));
        }

        public static Vector2 LerpUnclamped(Vector2 a, Vector2 b, float t)
        {
            return new Vector2(a.x + ((b.x - a.x) * t), a.y + ((b.y - a.y) * t));
        }

        public static Vector2 MoveTowards(Vector2 current, Vector2 target, float maxDistanceDelta)
        {
            Vector2 vector = target - current;
            float magnitude = vector.magnitude;
            if ((magnitude > maxDistanceDelta) && (magnitude != 0f))
            {
                return (current + ((Vector2) ((vector / magnitude) * maxDistanceDelta)));
            }
            return target;
        }

        public static Vector2 Scale(Vector2 a, Vector2 b)
        {
            return new Vector2(a.x * b.x, a.y * b.y);
        }

        public void Scale(Vector2 scale)
        {
            this.x *= scale.x;
            this.y *= scale.y;
        }

        public void Normalize()
        {
            float magnitude = this.magnitude;
            if (magnitude > 1E-05f)
            {
                this = (Vector2) (this / magnitude);
            }
            else
            {
                this = zero;
            }
        }

        public Vector2 normalized
        {
            get
            {
                Vector2 vector = new Vector2(this.x, this.y);
                vector.Normalize();
                return vector;
            }
        }
        public override string ToString()
        {
            object[] args = new object[] { this.x, this.y };
            return string.Format("({0:F1}, {1:F1})", args);
        }

        public string ToString(string format)
        {
            object[] args = new object[] { this.x.ToString(format), this.y.ToString(format) };
            return string.Format("({0}, {1})", args);
        }

        public override int GetHashCode()
        {
            return (this.x.GetHashCode() ^ (this.y.GetHashCode() << 2));
        }

        public override bool Equals(object other)
        {
            if (!(other is Vector2))
            {
                return false;
            }
            Vector2 vector = (Vector2) other;
            return (this.x.Equals(vector.x) && this.y.Equals(vector.y));
        }

        public static Vector2 Reflect(Vector2 inDirection, Vector2 inNormal)
        {
            return (((Vector2) ((-2f * Dot(inNormal, inDirection)) * inNormal)) + inDirection);
        }

        public static float Dot(Vector2 lhs, Vector2 rhs)
        {
            return ((lhs.x * rhs.x) + (lhs.y * rhs.y));
        }

        public float magnitude
        {
            get
            {
                return Mathf.Sqrt((this.x * this.x) + (this.y * this.y));
            }
        }
        public float sqrMagnitude
        {
            get
            {
                return ((this.x * this.x) + (this.y * this.y));
            }
        }
        public static float Angle(Vector2 from, Vector2 to)
        {
            return (Mathf.Acos(Mathf.Clamp(Dot(from.normalized, to.normalized), -1f, 1f)) * 57.29578f);
        }

        public static float Distance(Vector2 a, Vector2 b)
        {
            Vector2 vector = a - b;
            return vector.magnitude;
        }

        public static Vector2 ClampMagnitude(Vector2 vector, float maxLength)
        {
            if (vector.sqrMagnitude > (maxLength * maxLength))
            {
                return (Vector2) (vector.normalized * maxLength);
            }
            return vector;
        }

        public static float SqrMagnitude(Vector2 a)
        {
            return ((a.x * a.x) + (a.y * a.y));
        }

        public float SqrMagnitude()
        {
            return ((this.x * this.x) + (this.y * this.y));
        }

        public static Vector2 Min(Vector2 lhs, Vector2 rhs)
        {
            return new Vector2(Mathf.Min(lhs.x, rhs.x), Mathf.Min(lhs.y, rhs.y));
        }

        public static Vector2 Max(Vector2 lhs, Vector2 rhs)
        {
            return new Vector2(Mathf.Max(lhs.x, rhs.x), Mathf.Max(lhs.y, rhs.y));
        }

        /*[ExcludeFromDocs]
        public static Vector2 SmoothDamp(Vector2 current, Vector2 target, ref Vector2 currentVelocity, float smoothTime, float maxSpeed)
        {
            float deltaTime = Time.deltaTime;
            return SmoothDamp(current, target, ref currentVelocity, smoothTime, maxSpeed, deltaTime);
        }

        [ExcludeFromDocs]
        public static Vector2 SmoothDamp(Vector2 current, Vector2 target, ref Vector2 currentVelocity, float smoothTime)
        {
            float deltaTime = Time.deltaTime;
            float positiveInfinity = float.PositiveInfinity;
            return SmoothDamp(current, target, ref currentVelocity, smoothTime, positiveInfinity, deltaTime);
        }

        public static Vector2 SmoothDamp(Vector2 current, Vector2 target, ref Vector2 currentVelocity, float smoothTime, [DefaultValue("Mathf.Infinity")] float maxSpeed, [DefaultValue("Time.deltaTime")] float deltaTime)
        {
            smoothTime = Mathf.Max(0.0001f, smoothTime);
            float num = 2f / smoothTime;
            float num2 = num * deltaTime;
            float num3 = 1f / (((1f + num2) + ((0.48f * num2) * num2)) + (((0.235f * num2) * num2) * num2));
            Vector2 vector = current - target;
            Vector2 vector2 = target;
            float maxLength = maxSpeed * smoothTime;
            vector = ClampMagnitude(vector, maxLength);
            target = current - vector;
            Vector2 vector3 = (Vector2) ((currentVelocity + (num * vector)) * deltaTime);
            currentVelocity = (Vector2) ((currentVelocity - (num * vector3)) * num3);
            Vector2 vector4 = target + ((Vector2) ((vector + vector3) * num3));
            if (Dot(vector2 - current, vector4 - vector2) > 0f)
            {
                vector4 = vector2;
                currentVelocity = (Vector2) ((vector4 - vector2) / deltaTime);
            }
            return vector4;
        }*/

        public static Vector2 zero
        {
            get
            {
                return new Vector2(0f, 0f);
            }
        }
        public static Vector2 one
        {
            get
            {
                return new Vector2(1f, 1f);
            }
        }
        public static Vector2 up
        {
            get
            {
                return new Vector2(0f, 1f);
            }
        }
        public static Vector2 down
        {
            get
            {
                return new Vector2(0f, -1f);
            }
        }
        public static Vector2 left
        {
            get
            {
                return new Vector2(-1f, 0f);
            }
        }
        public static Vector2 right
        {
            get
            {
                return new Vector2(1f, 0f);
            }
        }
        public static Vector2 operator +(Vector2 a, Vector2 b)
        {
            return new Vector2(a.x + b.x, a.y + b.y);
        }

        public static Vector2 operator -(Vector2 a, Vector2 b)
        {
            return new Vector2(a.x - b.x, a.y - b.y);
        }

        public static Vector2 operator -(Vector2 a)
        {
            return new Vector2(-a.x, -a.y);
        }

        public static Vector2 operator *(Vector2 a, float d)
        {
            return new Vector2(a.x * d, a.y * d);
        }

        public static Vector2 operator *(float d, Vector2 a)
        {
            return new Vector2(a.x * d, a.y * d);
        }

        public static Vector2 operator /(Vector2 a, float d)
        {
            return new Vector2(a.x / d, a.y / d);
        }

        public static bool operator ==(Vector2 lhs, Vector2 rhs)
        {
            return (SqrMagnitude(lhs - rhs) < 9.999999E-11f);
        }

        public static bool operator !=(Vector2 lhs, Vector2 rhs)
        {
            return (SqrMagnitude(lhs - rhs) >= 9.999999E-11f);
        }

        public static implicit operator Vector2(Vector3 v)
        {
            return new Vector2(v.x, v.y);
        }

        public static implicit operator Vector3(Vector2 v)
        {
            return new Vector3(v.x, v.y, 0f);
        }
    }
}

