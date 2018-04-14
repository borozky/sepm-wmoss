using System;
using System.Collections.Generic;
using System.Text;

namespace WMoSS.Tests.TestUtils
{
    public interface IEntityFactory
    {
        /// <summary>
        /// Defines how entities will be generated. Requires a callback.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="factory"></param>
        void Define<T>(Func<T> factory) where T : class;

        /// <summary>
        /// Generates a fake entity. Entity's property values will depend on
        /// how it is defined in Define() method
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        T Make<T>() where T : class;

        /// <summary>
        /// Generates multiple entities. Entity's property values will depend on
        /// how iti is defined in Define() method
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="numOfEntities"></param>
        /// <returns></returns>
        IEnumerable<T> Make<T>(int numOfEntities) where T : class;
    }
}
