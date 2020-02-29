using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using ROBOTESTE_ILibrarySystem.Model;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;

namespace ROBOTESTE_ILibrarySystem.Processar
{
    class Communs
    {
        HttpClient client = new HttpClient();

        XmlDocument document = new XmlDocument();

        public bool LerXMl(Usuario user)
        {
            try
            {
                document.Load("C:\\Users\\Globalti\\Pictures\\XML\\Dados.xml");

                XmlNodeList xnList = document.GetElementsByTagName("Usuario");

                user.Usename = xnList[0]["Login"].InnerText;
                user.Password = xnList[0]["Senha"].InnerText;

                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
        public bool ClicarBotaoJS(IWebDriver driver, IWebElement component)
        {
            try
            {
                IWebElement botao = component;
                IJavaScriptExecutor clickBtn = (IJavaScriptExecutor)driver;
                clickBtn.ExecuteScript("arguments[0].click();", botao);

                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
        public static WebDriverWait NewMethod(IWebDriver driver, int time)
        {
            return new WebDriverWait(driver, TimeSpan.FromSeconds(time));
        }
        public bool PreencherCampo(IWebDriver driver, string idCampo, string valorCampo)
        {
            try
            {
                WebDriverWait waitCampo = NewMethod(driver, 15);
                waitCampo.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.ElementToBeClickable
                    (By.Id(idCampo))).Clear();
                Thread.Sleep(1000);
                waitCampo.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.ElementToBeClickable
                    (By.Id(idCampo))).SendKeys(valorCampo);

                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
        public void PastaResultado()
        {
            //Cria uma pasta com a data e a hora do teste, onde será salvo o printScreen
            string path = System.AppDomain.CurrentDomain.BaseDirectory.ToString();
            string folderResult = path + "Resultado\\" + "Testes" + "_" + DateTime.Now.ToString().Replace("/", "_").Replace(":", "_").Trim() +"\\Login";

            if (!Directory.Exists(folderResult))
            {
                Directory.CreateDirectory(folderResult);
            }
        }
        public string UltimaPastaResultado()
        {
            //Varre a pasta Resultado, afim de pegar a pasta com o ultimo teste executado
            string path = System.AppDomain.CurrentDomain.BaseDirectory.ToString();

            DirectoryInfo Dir = new DirectoryInfo(path + "Resultado");
            DirectoryInfo[] DirTest = Dir.GetDirectories();

            Array.Sort(DirTest, delegate (DirectoryInfo a, DirectoryInfo b) { return DateTime.Compare(a.CreationTime, b.CreationTime); });
            var folderPath = DirTest[DirTest.Length - 1].FullName;

            return folderPath;
        }
        public void Screenshot(IWebDriver driver)
        {
            //Tira o printScreen da tela e salva dentro da pasta criada para esse teste que está sendo executado
            string caminhoPrint = UltimaPastaResultado() + "\\Login\\Login.png";

            ITakesScreenshot tela = driver as ITakesScreenshot;
            Screenshot printScreen = tela.GetScreenshot();
            printScreen.SaveAsFile(caminhoPrint, ScreenshotImageFormat.Png);
        }
        public async Task AddAttachment()
        {
            try
            {
                Construtor ct = new Construtor();

                //PEga a ultima pasta criada e todas as pastas disponiveis nela
                string caminho = UltimaPastaResultado();
                DirectoryInfo Dir = new DirectoryInfo(caminho);
                DirectoryInfo[] DirResults = Dir.GetDirectories();

                int count = 0;

                foreach (DirectoryInfo results in DirResults)
                {
                    //Varre pasta por pasta, pegando o printScreen dentro dela
                    ct.ResulID = "";

                    if (count == 0)
                    {
                        // chama a função para pegar primeiro o ID do ultimo Teste Run execuado
                        Task.Run(() => ConsultaAzureAPI("Runs", ct)).Wait();
                        count++;
                    }
                    // chama a função para pegar id do resultado do teste
                    Task.Run(() => ConsultaAzureAPI(results.Name, ct)).Wait(); 

                    if (ct.ResulID != "")
                    {
                        DirectoryInfo oDirectoryInfo = new DirectoryInfo(caminho + "\\" + results.Name);
                        FileInfo[] oListFileInfo = oDirectoryInfo.GetFiles("*.png");

                        //Converte a imagem para BAse64 e cria um objeto do tipo Attachments com as informações do anexo e converte pata JSON
                        Byte[] bytes = File.ReadAllBytes(caminho + "\\" + results.Name + "\\" + oListFileInfo[0].Name);
                        String file = Convert.ToBase64String(bytes);

                        Attachments attachment = new Attachments();
                        attachment.Stream = file;
                        attachment.FileName = oListFileInfo[0].Name;
                        attachment.Comment = "attachment upload";
                        attachment.AttachmentType = "GeneralAttachment";

                        var data = JsonConvert.SerializeObject(attachment);

                        //Monta a URL e envia o JSOn com o anexo
                        string url = "https://dev.azure.com/{organization}/{project}/_apis/test/runs/" + ct.RunsID + "/results/" + ct.ResulID + "/attachments?api-version=5.1-preview.1";
                        var uri = new Uri(string.Format(url));

                        var content = new StringContent(data, Encoding.UTF8, "application/json");
                        HttpResponseMessage response = null;

                        var byteArray = Encoding.ASCII.GetBytes("{E-mail}:{Senha}");
                        client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers
                            .AuthenticationHeaderValue("Basic", Convert.ToBase64String(byteArray));

                        response = await client.PostAsync(uri, content);

                        if (!response.IsSuccessStatusCode)
                        {
                            Console.WriteLine("Erro ao incluir Anexo no Azure Devops");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public void ConsultaAzureAPI(string tipo, Construtor ct)
        {
            try
            {
                Uri uri;

                if (tipo == "Runs")
                {
                    uri = new Uri(string.Format("https://dev.azure.com/{organization}/{project}/_apis/test/runs/"));//pega o ID do ultimo Teste Run execuado
                }
                else
                {
                    //pega o todos os Resultados daquele teste
                    uri = new Uri(string.Format("https://dev.azure.com/{organization}/{project}/_apis/test/runs/" + ct.RunsID + "/results")); 
                }

                HttpResponseMessage response = null;
                //Faz a requisição para pegar os Test Runs ou para pegar os Test Results do Test Run
                var byteArray = Encoding.ASCII.GetBytes("{E-mail}:{Senha}");
                client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Basic", Convert.ToBase64String(byteArray));
                response = client.GetAsync(uri).Result;
                var Testes = response.Content.ReadAsStringAsync().Result;

                if (tipo == "Runs")
                {
                    var list = JObject.Parse(Testes)["value"].Select(el => new { id = (string)el["id"] }).ToList();
                    ct.RunsID = list[list.Count - 1].id; //Salva o ID do ultimo Test Run
                }
                else
                {
                    //Varre a lista de resultados para poder pegar o ID do resultado especifico passado pelo parametro TIPO.
                    var listcase = JObject.Parse(Testes)["value"].Select(el => new { id = (string)el["id"], TestCaseTitle = (string)el["testCaseTitle"] }).ToList();
                    foreach (var testCase in listcase)
                    {
                        if (testCase.TestCaseTitle.Contains(tipo))
                        {
                            ct.ResulID = testCase.id; 
                            break;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
