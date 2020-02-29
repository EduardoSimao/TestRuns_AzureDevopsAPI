using OpenQA.Selenium;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ROBOTESTE_ILibrarySystem.Finalizar
{
    class Encerrar
    {
        public void FecharBrowser(IWebDriver driver)
        {
            Console.WriteLine("Processo realizado!");
            driver.Quit();
        }
    }
}
