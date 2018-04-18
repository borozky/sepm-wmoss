using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WMoSS.Tests.TestUtils
{
    public class ExcelEntityFactory : IEntityFactory
    {
        /// <summary>
        /// Holds all definition for how entities will be generated
        /// </summary>
        public IDictionary<Type, Func<object>> Factory = new Dictionary<Type, Func<object>>();

        public void Define<T>(Func<T> factory) where T : class
        {
            Factory[typeof(T)] = factory;
        }

        public T Make<T>() where T : class
        {
            return Make<T>(1).FirstOrDefault();
        }

        public IEnumerable<T> Make<T>(int numOfEntities) where T : class
        {
            var entities = new List<T>();
            for (var i = 0; i < numOfEntities; i++)
            {
                var factory = Factory[typeof(T)] as Func<T>;
                if (factory != null)
                {
                    var entity = factory.Invoke();
                    entities.Add(entity);
                }
            }
            return entities;
        }
    }
}
