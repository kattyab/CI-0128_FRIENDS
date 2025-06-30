using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;
using System;

namespace UIAutomationTest
{
    public class Test01_RegisterCompany
    {
        private IWebDriver _driver;

        [SetUp]
        public void Setup()
        {
            _driver = new ChromeDriver();
            _driver.Manage().Window.Maximize();
        }

        [Test]
        public void CrearEmpresaPI_DeberiaRegistrarConExito()
        {
            _driver.Navigate().GoToUrl("https://localhost:55281/auth/register-company");

            // Completar datos de empresa
            _driver.FindElement(By.Id("brandName")).SendKeys("Empresa PI");
            _driver.FindElement(By.Id("cedulaJuridica")).SendKeys("3-102-242458");
            _driver.FindElement(By.Id("nombreEmpresa")).SendKeys("Empresa PI S.A.");
            _driver.FindElement(By.Id("province")).SendKeys("San José");
            _driver.FindElement(By.Id("canton")).SendKeys("Montes de Oca");
            _driver.FindElement(By.Id("district")).SendKeys("San Pedro");
            _driver.FindElement(By.Id("additionalSigns")).SendKeys("Frente a la UCR");
            _driver.FindElement(By.Id("telefonoEmpresa")).SendKeys("8888-1234");
            _driver.FindElement(By.Id("razonSocial")).SendKeys("Empresa enfocada en desarrollo de software.");

            _driver.FindElement(By.Id("ownerId")).SendKeys("01-0111-0111");
            _driver.FindElement(By.Id("ownerName")).SendKeys("Carlos");
            _driver.FindElement(By.Id("ownerLastName")).SendKeys("Pérez Sánchez");

            // Esperar y seleccionar sexo
            var wait = new WebDriverWait(_driver, TimeSpan.FromSeconds(10));
            var labelSexo = wait.Until(driver =>
            {
                try
                {
                    var el = driver.FindElement(By.CssSelector("label[for='ownerSex-Hombre']"));
                    return (el.Displayed && el.Enabled) ? el : null;
                }
                catch (StaleElementReferenceException) { return null; }
                catch (NoSuchElementException) { return null; }
            });

            // Scroll y click por JS para evitar interceptación
            ((IJavaScriptExecutor)_driver).ExecuteScript("arguments[0].scrollIntoView(true);", labelSexo);
            ((IJavaScriptExecutor)_driver).ExecuteScript("arguments[0].click();", labelSexo);

            _driver.FindElement(By.Id("ownerBirthDate")).SendKeys("15-05-1980");
            _driver.FindElement(By.Id("ownerEmail")).SendKeys("carlos@empresapi.cr");
            _driver.FindElement(By.Id("ownerPassword")).SendKeys("PasswordSeguro123");

            var submitBtn = _driver.FindElement(By.CssSelector("form button[type='submit']"));
            ((IJavaScriptExecutor)_driver).ExecuteScript("arguments[0].scrollIntoView(true);", submitBtn);
            ((IJavaScriptExecutor)_driver).ExecuteScript("arguments[0].click();", submitBtn);

            var mensaje = wait.Until(driver =>
            {
                try
                {
                    var p = driver.FindElement(By.CssSelector("p.text-success"));
                    return p.Displayed ? p.Text : null;
                }
                catch (NoSuchElementException) { return null; }
            });

            StringAssert.Contains("Empresa y dueño registrados correctamente.", mensaje);
        }

        [TearDown]
        public void TearDown()
        {
            _driver.Quit();
            _driver.Dispose();
        }
    }
}