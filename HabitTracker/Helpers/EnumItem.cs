using System;
using System.Collections.Generic;
using System.Text;

namespace HabitTracker.Helpers
{
    public class EnumItem<T>
    {
        public T Value { get; }
        public string DisplayName { get; }

        public EnumItem(T value, string displayName)
        {
            Value = value;
            DisplayName = displayName;
        }
    }
}
