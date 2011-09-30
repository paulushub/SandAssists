using System;
using System.Diagnostics;
using System.Collections.Generic;

using Microsoft.Internal;

namespace Sandcastle.Composition.Hosting
{
    /// <summary>
    /// Represents a single composition operation for transactional composition.
    /// </summary>
    /// <remarks>
    /// <para>
    /// AtomicComposition provides lightweight atomic compositional semantics 
    /// to enable temporary state to be managed for a series of nested atomic compositions. 
    /// </para>
    /// <para>
    /// Each atomic composition maintains queryable state along with a 
    /// sequence of actions necessary to complete the state when the atomic
    /// composition is no longer in danger of being rolled back.  State is 
    /// completed or rolled back when the atomic composition is disposed, 
    /// depending on the state of the <c>CompleteOnDipose</c> property which 
    /// defaults to false.  The using(...) pattern in <c>C#</c> is a
    /// convenient mechanism for defining atomic composition scopes.
    /// </para>
    /// <para>
    /// The least obvious aspects of AtomicComposition deal with nesting.
    /// </para>
    /// <para>
    /// Firstly, no complete actions are actually performed until the outermost atomic composition is
    /// completed.  Completing or rolling back nested atomicCompositions serves only to change which
    /// actions would be completed the outer atomic composition.
    /// </para>
    /// <para>
    /// Secondly, state is added in the form of queries associated with an object key.  The
    /// key represents a unique object the state is being held on behalf of.  The queries are
    /// accessed through the Query methods which provide automatic chaining to execute queries
    /// across the target atomic composition and its inner atomic composition as appropriate.
    /// </para>
    /// <para>
    /// Lastly, when a nested atomic composition is created for a given outer the outer atomic composition is locked.
    /// It remains locked until the inner atomic composition is disposed or completed preventing the addition of
    /// state, actions or other inner atomicCompositions.
    /// </para>
    /// </remarks>
    public class AtomicComposition : IDisposable
    {
        private bool _isDisposed;
        private bool _isCompleted;
        private bool _containsInnerAtomicComposition;

        private KeyValuePair<object, object>[] _values;
        private int _valueCount;
        private List<Action> _completeActionList;
        private List<Action> _revertActionList;

        private AtomicComposition _outerAtomicComposition;

        public AtomicComposition()
            : this(null)
        {
        }

        public AtomicComposition(AtomicComposition outerAtomicComposition)
        {
            this._outerAtomicComposition = outerAtomicComposition;

            // Lock the inner atomic composition so that we can assume nothing changes except on
            // the innermost scope, and thereby optimize the query path
            if (outerAtomicComposition != null)
            {
                this._outerAtomicComposition.ContainsInnerAtomicComposition = true;
            }
        }

        public void SetValue(object key, object value)
        {
            ThrowIfDisposed();
            ThrowIfCompleteed();
            ThrowIfContainsInnerAtomicComposition();

            Requires.NotNull(key, "key");

            SetValueInternal(key, value);
        }

        public bool TryGetValue<T>(object key, out T value) 
        {
            return TryGetValue(key, out value, false);
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1021:AvoidOutParameters")]
        public bool TryGetValue<T>(object key, out T value, bool localAtomicCompositionOnly) 
        {
            ThrowIfDisposed();
            ThrowIfCompleteed();

            Requires.NotNull(key, "key");

            return TryGetValueInternal(key, out value, localAtomicCompositionOnly);
        }

        public void AddCompleteAction(Action completeAction)
        {
            ThrowIfDisposed();
            ThrowIfCompleteed();
            ThrowIfContainsInnerAtomicComposition();

            Requires.NotNull(completeAction, "completeAction");

            if (this._completeActionList == null)
            {
                this._completeActionList = new List<Action>();
            }
            this._completeActionList.Add(completeAction);
        }

        public void AddRevertAction(Action revertAction)
        {
            ThrowIfDisposed();
            ThrowIfCompleteed();
            ThrowIfContainsInnerAtomicComposition();

            Requires.NotNull(revertAction, "revertAction");

            if (this._revertActionList == null)
            {
                this._revertActionList = new List<Action>();
            }
            this._revertActionList.Add(revertAction);
        }

        public void Complete()
        {
            ThrowIfDisposed();
            ThrowIfCompleteed();

            if (this._outerAtomicComposition == null)
            {   // Execute all the complete actions
                FinalComplete();
            }
            else
            {   // Copy the actions and state to the outer atomic composition
                CopyComplete();
            }

            this._isCompleted = true;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            ThrowIfDisposed();
            this._isDisposed = true;

            if (this._outerAtomicComposition != null)
            {
                this._outerAtomicComposition.ContainsInnerAtomicComposition = false;
            }

            // Revert is always immediate and involves forgetting information and
            // executing any appropriate revert actions
            if (!this._isCompleted)
            {
                if (this._revertActionList != null)
                {
                    // Execute the revert actions in reverse order to ensure
                    // everything incrementally rollback its state.
                    for (int i = this._revertActionList.Count - 1; i >= 0; i--)
                    {
                        Action action = this._revertActionList[i];
                        action();
                    }
                    this._revertActionList = null;
                }
            }
        }

        private void FinalComplete()
        {
            // Completing the outer most scope is easy, just execute all the actions
            if (this._completeActionList != null)
            {
                foreach (Action action in this._completeActionList)
                {
                    action();
                }
                this._completeActionList = null;
            }
        }

        private void CopyComplete()
        {
            Assumes.NotNull(this._outerAtomicComposition);

            this._outerAtomicComposition.ContainsInnerAtomicComposition = false;

            // Inner scopes are much odder, because completing them means coalescing them into the
            // outer scope - the complete or revert actions are deferred until the outermost scope completes
            // or any intermediate rolls back
            if (this._completeActionList != null)
            {
                foreach (Action action in this._completeActionList)
                {
                    this._outerAtomicComposition.AddCompleteAction(action);
                }
            }

            if (this._revertActionList != null)
            {
                foreach (Action action in this._revertActionList)
                {
                    this._outerAtomicComposition.AddRevertAction(action);
                }
            }

            // We can copy over existing atomic composition entries because they're either already chained or
            // overwrite by design and can now be completed or rolled back together
            for (var index = 0; index < this._valueCount; index++)
            {
                this._outerAtomicComposition.SetValueInternal(
                    this._values[index].Key, this._values[index].Value);
            }
        }

        private bool ContainsInnerAtomicComposition
        {
            set
            {
                if (value == true && this._containsInnerAtomicComposition == true)
                {
                    throw new InvalidOperationException(Strings.AtomicComposition_AlreadyNested);
                }
                this._containsInnerAtomicComposition = value;
            }
        }

        private bool TryGetValueInternal<T>(object key, out T value, bool localAtomicCompositionOnly) 
        {
            for (var index = 0; index < this._valueCount; index++)
            {
                if (this._values[index].Key == key)
                {
                    value = (T)this._values[index].Value;
                    return true;
                }
            }

            // If there's no atomic composition available then recurse until we hit the outermost
            // scope, where upon we go ahead and return null
            if (!localAtomicCompositionOnly && this._outerAtomicComposition != null)
            {
                return this._outerAtomicComposition.TryGetValueInternal<T>(key, out value, localAtomicCompositionOnly);
            }

            value = default(T);
            return false;
        }

        private void SetValueInternal(object key, object value)
        {
            // Handle overwrites quickly
            for (var index = 0; index < this._valueCount; index++)
            {
                if (this._values[index].Key == key)
                {
                    this._values[index] = new KeyValuePair<object,object>(key, value);
                    return;
                }
            }

            // Expand storage when needed
            if (this._values == null || this._valueCount == this._values.Length)
            {
                var newQueries = new KeyValuePair<object, object>[this._valueCount == 0 ? 5 : this._valueCount * 2];
                if (this._values != null)
                {
                    Array.Copy(this._values, newQueries, this._valueCount);
                }
                this._values = newQueries;
            }

            // Store a new entry
            this._values[_valueCount] = new KeyValuePair<object, object>(key, value);
            this._valueCount++;
            return;
        }

        [DebuggerStepThrough]
        private void ThrowIfContainsInnerAtomicComposition()
        {
            if (this._containsInnerAtomicComposition)
            {
                throw new InvalidOperationException(Strings.AtomicComposition_PartOfAnotherAtomicComposition);
            }
        }

        [DebuggerStepThrough]
        private void ThrowIfCompleteed()
        {
            if (this._isCompleted)
            {
                throw new InvalidOperationException(Strings.AtomicComposition_AlreadyCompleted);
            }
        }

        [DebuggerStepThrough]
        private void ThrowIfDisposed()
        {
            if (this._isDisposed)
            {
                throw ExceptionBuilder.CreateObjectDisposed(this);
            }
        }
    }
}
