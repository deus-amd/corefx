// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using System.Globalization;

namespace System.Linq.Expressions.Interpreter
{
    internal sealed class LoadObjectInstruction : Instruction
    {
        private readonly object _value;

        internal LoadObjectInstruction(object value)
        {
            _value = value;
        }

        public override int ProducedStack => 1;
        public override string InstructionName => "LoadObject";

        public override int Run(InterpretedFrame frame)
        {
            frame.Data[frame.StackIndex++] = _value;
            return +1;
        }

        public override string ToString() => "LoadObject(" + (_value ?? "null") + ")";
    }

    internal sealed class LoadCachedObjectInstruction : Instruction
    {
        private readonly uint _index;

        internal LoadCachedObjectInstruction(uint index)
        {
            _index = index;
        }

        public override int ProducedStack => 1;
        public override string InstructionName => "LoadCachedObject";

        public override int Run(InterpretedFrame frame)
        {
            frame.Data[frame.StackIndex++] = frame.Interpreter._objects[_index];
            return +1;
        }

        public override string ToDebugString(int instructionIndex, object cookie, Func<int, int> labelIndexer, IList<object> objects)
        {
            return String.Format(CultureInfo.InvariantCulture, "LoadCached({0}: {1})", _index, objects[(int)_index]);
        }

        public override string ToString() => "LoadCached(" + _index + ")";
    }

    internal sealed class PopInstruction : Instruction
    {
        internal static readonly PopInstruction Instance = new PopInstruction();

        private PopInstruction() { }

        public override int ConsumedStack => 1;
        public override string InstructionName =>"Pop";
        
        public override int Run(InterpretedFrame frame)
        {
            frame.Pop();
            return +1;
        }

        public override string ToString() => "Pop()";
    }

    internal sealed class DupInstruction : Instruction
    {
        internal readonly static DupInstruction Instance = new DupInstruction();

        private DupInstruction() { }

        public override int ConsumedStack => 0;
        public override int ProducedStack => 1;
        public override string InstructionName => "Dup";
        
        public override int Run(InterpretedFrame frame)
        {
            object value = frame.Peek();
            frame.Data[frame.StackIndex++] = value;
            return +1;
        }

        public override string ToString() => "Dup()";
    }
}
