using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using ROBOTESTE_ILibrarySystem.Finalizar;
using ROBOTESTE_ILibrarySystem.Model;
using System;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;
using TechTalk.SpecFlow;

namespace ROBOTESTE_ILibrarySystem.Processar.StepFiles
{
    [Binding]
    public class RealizarLoginSteps
    {
        IWebDriver chromeDriver;
        Usuario user = new Usuario();
        Communs communs = new Communs();
        Encerrar encerrar = new Encerrar();

        [BeforeScenario]
        public void Iniciar()
        {
            Task.Run(() => communs.AddAttachment()).Wait();

            communs.PastaResultado();

        }

        [Given(@"que o usuario tenha acessado a pagina inicial")]
        public void DadoQueOUsuarioTenhaAcessadoAPaginaInicial()
        {
            Assert.IsTrue(communs.LerXMl(user));

            try
            {
                chromeDriver = new ChromeDriver();
                chromeDriver.Navigate().GoToUrl("https://ilibrarysystem.herokuapp.com/");
                chromeDriver.Manage().Window.Maximize();
            }
            catch (Exception)
            {
                Console.WriteLine("Falha ao iniciar Driver");
            }
        }

        [Given(@"tenha clicado no botão Entrar")]
        public void DadoClicadoNoBotaoEntrar()
        {
            Assert.IsTrue(communs.ClicarBotaoJS(chromeDriver, chromeDriver.FindElement(By.XPath("//div/nav/ul/a"))));
        }

        [When(@"o mesmo inserir os dados necessarios corretamente")]
        public void QuandoOMesmoInserirOsDadosNecessariosCorretamente()
        {
            Assert.IsTrue(communs.PreencherCampo(chromeDriver, "username", user.Usename));

            Assert.IsTrue(communs.PreencherCampo(chromeDriver, "password", user.Password));
        }

        [When(@"clicar no botão Entrar")]
        public void QuandoClicarNoBotaoEntrar()
        {
            Assert.IsTrue(communs.ClicarBotaoJS(chromeDriver, chromeDriver.FindElement(By.XPath("//div/div[1]/form/div[3]/button"))));
        }

        [Then(@"o login terá sido realizado com sucesso")]
        public void EntaoOLoginTeraSidoRealizadoComSucesso()
        {
            communs.Screenshot(chromeDriver);

            Thread.Sleep(3000);

            //Task.Run(() => communs.AddAttachment()).Wait();

            encerrar.FecharBrowser(chromeDriver);
        }
    }
}
