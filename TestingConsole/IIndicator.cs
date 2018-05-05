using System;
using System.Collections.Generic;
using System.Text;

namespace TestingConsole
{
    public interface IIndicator
    {
        /// <summary>
        /// Расчет значения индикатора
        /// </summary>
        /// <returns></returns>
        void Calculate();
    }
}
