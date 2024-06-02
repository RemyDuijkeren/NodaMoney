using BenchmarkDotNet.Attributes;

namespace Benchmark;

[MemoryDiagnoser]
public class CurrencyUnitBenchmarks
{
        [Benchmark(Baseline = true)]
        public ushort CreateCurrencyUnit()
        {
            string code = "EUR";
            bool flag = false;
            
            if (code == null) throw new ArgumentNullException(nameof(code));
        
            var chars = code.ToCharArray();
            if (chars.Length != 3) throw new ArgumentException("Currency code must be three characters long", nameof(code));
            if (chars.Any(c => c is < 'A' or > 'Z')) throw new ArgumentException("Currency code should only exist out of capital letters", nameof(code));

            byte[] bytes = chars.Select(c => (byte)(c - 'A' + 1)).ToArray(); // A-Z (65-90 in ASCII) => 1-26 (fits in 5 bits)
            var _code = (ushort)(bytes[0] << 10 | bytes[1] << 5 | bytes[2]); // store in ushort (2 bytes) by shifting bits in steps of 5
        
            // ushort = 2bytes, only 15bits needed for code, 1bit left => use last bit to indicate flag for ...? IsIso4217?
            if (flag) _code |= 1 << 15; // set last bit to 1

            if (_code == 25368) _code = 0; // 25368 = 'XXX' (No Currency) => set to 0 (default)

            return _code;
        }

        [Benchmark]
        public ushort CreateCurrencyUnitNoLinq()
        {
            string code = "EUR";
            bool flag = false;
            
            if (code == null) throw new ArgumentNullException(nameof(code));
        
            var chars = code.ToCharArray();
            if (chars.Length != 3) throw new ArgumentException("Currency code must be three characters long", nameof(code));
            byte[] bytes = new byte[3];
            for (var i = 0; i < chars.Length; i++)
            {
                var c = chars[i];
                if (c is < 'A' or > 'Z') throw new ArgumentException("Currency code should only exist out of capital letters", nameof(code));
                bytes[i] = (byte)(c - 'A' + 1);
            }

            //byte[] bytes = chars.Select(c => (byte)(c - 'A' + 1)).ToArray(); // A-Z (65-90 in ASCII) => 1-26 (fits in 5 bits)

            var _code = (ushort)(bytes[0] << 10 | bytes[1] << 5 | bytes[2]); // store in ushort (2 bytes) by shifting bits in steps of 5
        
            // ushort = 2bytes, only 15bits needed for code, 1bit left => use last bit to indicate flag for ...? IsIso4217?
            if (flag) _code |= 1 << 15; // set last bit to 1

            if (_code == 25368) _code = 0; // 25368 = 'XXX' (No Currency) => set to 0 (default)

            return _code;
        }
        
        [Benchmark]
        public ushort CreateCurrencyUnitNoLinqAndPattern()
        {
            string code = "EUR";
            bool flag = false;
            
            if (code == null) throw new ArgumentNullException(nameof(code));
            if (code.Length != 3) throw new ArgumentException("Currency code must be three characters long", nameof(code));
        
            var chars = code.ToCharArray();
            // if (chars.Length != 3) throw new ArgumentException("Currency code must be three characters long", nameof(code));
            byte[] bytes = new byte[3];
            for (var i = 0; i < chars.Length; i++)
            {
                var c = chars[i];
                if (c is < 'A' or > 'Z') throw new ArgumentException("Currency code should only exist out of capital letters", nameof(code));
                bytes[i] = (byte)(c - 'A' + 1);
            }

            //byte[] bytes = chars.Select(c => (byte)(c - 'A' + 1)).ToArray(); // A-Z (65-90 in ASCII) => 1-26 (fits in 5 bits)

            var _code = (ushort)(bytes[0] << 10 | bytes[1] << 5 | bytes[2]); // store in ushort (2 bytes) by shifting bits in steps of 5
        
            // ushort = 2bytes, only 15bits needed for code, 1bit left => use last bit to indicate flag for ...? IsIso4217?
            if (flag) _code |= 1 << 15; // set last bit to 1

            if (_code == 25368) _code = 0; // 25368 = 'XXX' (No Currency) => set to 0 (default)

            return _code;
        }
}