using System;
using System.Collections.Generic;

namespace SharedObjects
{
    public class MessageId
    {
        #region Private Properties
        private static short _nextSeqNumber;                        // Initialize to 0, which means it will start with message #1
        private static readonly object MyLock = new object();
        #endregion

        #region Public Properties
        public short Pid { get; set; }
        public short Seq { get; set; }

        #endregion

        #region Constructors and Factories
        /// <summary>
        /// Factory method creates and new, unique message number.
        /// </summary>
        /// <returns>A new message number</returns>
        public static MessageId Create()
        {
            MessageId result = new MessageId()
            {
                Pid = LocalProcessInfo.Instance.ProcessId,
                Seq = GetNextSeqNumber()
            };
            return result;
        }

        public MessageId Clone()
        {
            return MemberwiseClone() as MessageId;
        }
        #endregion

        #region Overridden public methods of Object
        public override string ToString()
        {
            return Pid.ToString() + "." + Seq.ToString();
        }
        #endregion

        #region Private Methods
        private static short GetNextSeqNumber()
        {
            lock (MyLock)
            {
                if (_nextSeqNumber == short.MaxValue)
                    _nextSeqNumber = 0;
                ++_nextSeqNumber;
            }
            return _nextSeqNumber;
        }
        #endregion

        #region Comparison Methods and Operators
        public override int GetHashCode()
        {
            return (Convert.ToInt32(Pid) << 16) | (Convert.ToInt32(Seq) & 0xFFFF);
        }

        public override bool Equals(object obj)
        {
            return Compare(this, obj as MessageId) == 0;
        }

        public static int Compare(MessageId a, MessageId b)
        {
            int result = 0;

            if (!ReferenceEquals(a, b))
            {
                if (((object)a == null) && ((object)b != null))
                    result = -1;
                else if (((object)a != null) && ((object)b == null))
                    result = 1;
                else
                {
                    if (a.Pid < b.Pid)
                        result = -1;
                    else if (a.Pid > b.Pid)
                        result = 1;
                    else if (a.Seq < b.Seq)
                        result = -1;
                    else if (a.Seq > b.Seq)
                        result = 1;
                }
            }
            return result;
        }

        public static bool operator ==(MessageId a, MessageId b)
        {
            return (Compare(a, b) == 0);
        }

        public static bool operator !=(MessageId a, MessageId b)
        {
            return (Compare(a, b) != 0);
        }

        public static bool operator <(MessageId a, MessageId b)
        {
            return (Compare(a, b) < 0);
        }

        public static bool operator >(MessageId a, MessageId b)
        {
            return (Compare(a, b) > 0);
        }

        public static bool operator <=(MessageId a, MessageId b)
        {
            return (Compare(a, b) <= 0);
        }

        public static bool operator >=(MessageId a, MessageId b)
        {
            return (Compare(a, b) >= 0);
        }

        #endregion

        /// <summary>
        /// This is a class provide a comparer so MessageNumber can used as a dictionary key
        /// </summary>
        public class MessageIdComparer : IEqualityComparer<MessageId>
        {
            public bool Equals(MessageId id1, MessageId id2)
            {
                return id1 == id2;
            }

            public int GetHashCode(MessageId id)
            {
                return id.GetHashCode();
            }
        }
    }


}
