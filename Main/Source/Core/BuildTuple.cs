using System;
using System.Collections.Generic;

namespace Sandcastle
{
    public static class BuildTuple
    {
        public static BuildTuple<T1, T2> Create<T1, T2>(T1 t1, T2 t2)
        {
            return new BuildTuple<T1, T2>(t1, t2);
        }

        public static BuildTuple<T1, T2, T3> Create<T1, T2, T3>(T1 t1, T2 t2, T3 t3)
        {
            return new BuildTuple<T1, T2, T3>(t1, t2, t3);
        }
    }

    /// <summary>
    /// Represents a container with two objects
    /// </summary>
    /// <typeparam name="T1"></typeparam>
    /// <typeparam name="T2"></typeparam>
    public class BuildTuple<T1, T2> : IEquatable<BuildTuple<T1, T2>>
    {
        /// <summary>
        /// First item
        /// </summary>
        public T1 First { get; private set; }

        /// <summary>
        /// Second item
        /// </summary>
        public T2 Second { get; private set; }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="first"></param>
        /// <param name="second"></param>
        public BuildTuple(T1 first, T2 second)
        {
            First = first;
            Second = second;
        }

        /// <summary>
        /// Checks for equality
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public bool Equals(BuildTuple<T1, T2> other)
        {
            return EqualityComparer<T1>.Default.Equals(this.First, other.First)
                && EqualityComparer<T2>.Default.Equals(this.Second, other.Second);
        }

        /// <summary>
        /// Checks for equality
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public override bool Equals(object obj)
        {
            return obj is BuildTuple<T1, T2> && this.Equals((BuildTuple<T1, T2>)obj);
        }

        /// <summary>
        /// Returns the hash code of a object
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public override int GetHashCode()
        {
            return First.GetHashCode() ^ Second.GetHashCode();
        }

        /// <summary>
        /// Overrides the == operator
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        public static bool operator ==(BuildTuple<T1, T2> left, BuildTuple<T1, T2> right)
        {
            if (((object)left) == null && ((object)right) == null)
            {
                return true;
            }

            return left.Equals(right);
        }

        /// <summary>
        /// Overrides the != operator
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        public static bool operator !=(BuildTuple<T1, T2> left, BuildTuple<T1, T2> right)
        {
            if (((object)left) == null && ((object)right) == null)
            {
                return false;
            }

            return !left.Equals(right);
        }
    }

    /// <summary>
    /// Represents a container which contains three object
    /// </summary>
    /// <typeparam name="T1"></typeparam>
    /// <typeparam name="T2"></typeparam>
    /// <typeparam name="T3"></typeparam>
    public class BuildTuple<T1, T2, T3> : BuildTuple<T1, T2>, IEquatable<BuildTuple<T1, T2, T3>>
    {
        /// <summary>
        /// Third item
        /// </summary>
        public T3 Third { get; private set; }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="first"></param>
        /// <param name="second"></param>
        /// <param name="third"></param>
        public BuildTuple(T1 first, T2 second, T3 third)
            : base(first, second)
        {
            Third = third;
        }

        /// <summary>
        /// Checks for equality
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public bool Equals(BuildTuple<T1, T2, T3> other)
        {
            return EqualityComparer<T1>.Default.Equals(this.First, other.First) &&
                EqualityComparer<T2>.Default.Equals(this.Second, other.Second) &&
                EqualityComparer<T3>.Default.Equals(this.Third, other.Third);
        }

        /// <summary>
        /// Checks for equality of a specific object
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public override bool Equals(object obj)
        {
            return obj is BuildTuple<T1, T2, T3> && 
                this.Equals((BuildTuple<T1, T2, T3>)obj);
        }

        /// <summary>
        /// Returns the hash code of a specific object
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public override int GetHashCode()
        {
            return First.GetHashCode() ^ Second.GetHashCode() ^ 
                Third.GetHashCode();
        }

        /// <summary>
        /// Overrides the == operator
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        public static bool operator ==(BuildTuple<T1, T2, T3> left,
                                       BuildTuple<T1, T2, T3> right)
        {
            if (((object)left) == null && ((object)right) == null)
            {
                return true;
            }

            return left.Equals(right);
        }

        /// <summary>
        /// Overrides the != operator
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        public static bool operator !=(BuildTuple<T1, T2, T3> left,
                                       BuildTuple<T1, T2, T3> right)
        {
            if (((object)left) == null && ((object)right) == null)
            {
                return false;
            }

            return !left.Equals(right);
        }
    }
}
