namespace gppg
{
    using System;

    public class ParserStack<T>
    {
        public T[] array;
        public int top;

        public ParserStack()
        {
            this.array = new T[1];
            this.top = 0;
        }

        public bool IsEmpty()
        {
            return (this.top == 0);
        }

        public T Pop()
        {
            return this.array[--this.top];
        }

        public void Push(T value)
        {
            if (this.top >= this.array.Length)
            {
                T[] destinationArray = new T[this.array.Length * 2];
                Array.Copy(this.array, destinationArray, this.top);
                this.array = destinationArray;
            }
            this.array[this.top++] = value;
        }

        public T First()
        {
            return this.array[this.top - 1];
        }
    }
}

