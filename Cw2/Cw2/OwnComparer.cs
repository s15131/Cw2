using System;
using System.Collections.Generic;
using System.Text;

namespace Cw2
{
    public class OwnComparer : IEqualityComparer<Student>
    {

        public bool Equals(Student x, Student y)
        {

            return StringComparer
                .InvariantCultureIgnoreCase
                .Equals($"{x.imie} {x.nazwisko} {x.numerindexu}",
                $"{y.imie} {y.nazwisko} {y.numerindexu}");
        }

        public int GetHashCode(Student obj)
        {
            return StringComparer
                .CurrentCultureIgnoreCase
                .GetHashCode($"{obj.imie} {obj.nazwisko} {obj.numerindexu}");
        }
    }
}
