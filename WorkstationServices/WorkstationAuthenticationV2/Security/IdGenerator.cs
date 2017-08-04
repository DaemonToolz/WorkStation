using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Workstation.Authentication.Security{
    public class IdGenerator{
        private readonly IdGenerator _nested;
        private readonly Random _t1, _t2;
        private readonly int _depth;

        // 48 -> 58 / 65 -> 91 / 97 -> 123
        public IdGenerator(int maxDepth){
            
            _t1 = new Random();
            for (int i = 0; i < 5; ++i)
                _t1.Next();

            _t2 = new Random();
            for (int i = 0; i < 5; ++i)
                _t2.Next();

            if (maxDepth > 0){
                _nested = new IdGenerator(maxDepth - 1);
                _depth = maxDepth;
            }
            else
                _depth = 1;
        }

        public int GenerateInt(int min, int max, int passes = 1){
            if (passes <= 0) passes = 1;
            int final = 0;
            for (int i = 0; i < passes; ++i)
            {
                final += (_t2.NextDouble() > 0.5 ? 1 : -1)*((i%2==0?_t1:_t2).Next(min, max));
            }
            return (int)final / passes;
        }

        public float GenerateFloat(int passes)
        {
            double final = 0;
            for (int i = 0; i < passes; ++i)
            {
                final += (_t2.NextDouble() > 0.5 ? 1 : -1) * ((i % 2 == 0 ? _t1 : _t2).NextDouble());
            }
            return (float)final / passes;
        }

        public String GenerateId(int size){
            var builder = new StringBuilder();

            for (int i = 0; i < size / _depth; ++i) {
                if(_t2.NextDouble() < 0.25)
                    builder.Append((char)_t1.Next(48, 58));
                else if(_t2.NextDouble() < 0.75)
                    builder.Append((char)_t2.Next(65, 91));
                else
                    builder.Append(((char)_t1.Next(97, 123)));
            }

            if(_depth > 1)
                builder.Append(_nested.GenerateId((size - (size/_depth)) / (_depth- 1)));

            return builder.ToString();
        }


    }
}
