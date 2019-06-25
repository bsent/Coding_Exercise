

namespace JobLib.Util
{

    using System;
    using System.Collections.Generic;
    using System.Linq;

    public class JobSorter<ValueType>
    {
        // inner class: one to many relationships on dependencies
        private class Relations
        {
            public int Dependencies = 0;
            public HashSet<ValueType> Dependents = new HashSet<ValueType>();
        }

        private Dictionary<ValueType, Relations> map = new Dictionary<ValueType, Relations>();

        public void Add(ValueType obj)
        {    
            if (!map.ContainsKey(obj))
                map.Add(obj, new Relations());
        }

      
        public void Add(ValueType obj, ValueType dependency)
        {                 
            if (!map.ContainsKey(dependency)) map.Add(dependency, new Relations());

            var dependents = map[dependency].Dependents;

            if (!dependents.Contains(obj))
            {
                dependents.Add(obj);

                if (!map.ContainsKey(obj)) map.Add(obj, new Relations());

                ++map[obj].Dependencies;
            }
        }

        public void Add(ValueType obj, IEnumerable<ValueType> dependencies)
        {
            foreach (var dependency in dependencies)
                Add(obj, dependency);
        }

        // Validation sequence patern doesn't let it be dependent on itself
        private void Validate(ValueType obj, params ValueType[] dependencies)
        {          
            for (int i = 0; i < dependencies.Length; i++)
            {
                if( dependencies[i].Equals(obj))
                {                 
                    throw new Exception("Jobs can't depend on itself  !");
                }
            }
        }

        public void Add(ValueType obj, params ValueType[] dependencies)
        {
            Validate(obj, dependencies);

            Add(obj, dependencies as IEnumerable<ValueType>);
        }

       // topologic sorting :
        public Tuple<IEnumerable<ValueType>, IEnumerable<ValueType>> Sort()
        {
            List<ValueType> sorted = new List<ValueType>(), cycled = new List<ValueType>();

            var mapping = map.ToDictionary(kvp => kvp.Key, kvp => kvp.Value);
   
            sorted.AddRange(mapping.Where(kvp => kvp.Value.Dependencies == 0).Select(kvp => kvp.Key));

            for (int idx = 0; idx < sorted.Count; ++idx)
                sorted.AddRange(mapping[sorted[idx]].Dependents.Where(k => --mapping[k].Dependencies == 0));

            cycled.AddRange(mapping.Where(kvp => kvp.Value.Dependencies != 0).Select(kvp => kvp.Key));

            return new Tuple<IEnumerable<ValueType>, IEnumerable<ValueType>>(sorted, cycled);
        }

        public void Clear()
        {
            map.Clear();
        }
    }
}
