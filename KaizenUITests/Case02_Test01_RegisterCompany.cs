using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;
using System;

namespace UIAutomationTest
{
    public class Case02_Test01_RegisterCompany
    {
        private IWebDriver _driver;

        [SetUp]
        public void Setup()
        {
            _driver = new ChromeDriver();
            _driver.Manage().Window.Maximize();
        }

        [Test]
        public void CrearEmpresaSprint3_ConDosBeneficios_Mensual()
        {
            _driver.Navigate().GoToUrl("https://localhost:55281/auth/register-company");

            var wait = new WebDriverWait(_driver, TimeSpan.FromSeconds(10));
            wait.Until(driver => driver.FindElement(By.Id("brandName")));

            // Datos de empresa
            _driver.FindElement(By.Id("brandName")).SendKeys("Sprint 3");
            _driver.FindElement(By.Id("cedulaJuridica")).SendKeys("3-102-242459");
            _driver.FindElement(By.Id("nombreEmpresa")).SendKeys("Sprint 3 S.A.");

            var cicloPagoSelect = wait.Until(driver => driver.FindElements(By.CssSelector("select.form-select"))[0]);
            var selectElement = new SelectElement(cicloPagoSelect);
            selectElement.SelectByValue("M");

            _driver.FindElement(By.Id("province")).SendKeys("San José");
            _driver.FindElement(By.Id("canton")).SendKeys("Montes de Oca");
            _driver.FindElement(By.Id("district")).SendKeys("San Pedro");
            _driver.FindElement(By.Id("additionalSigns")).SendKeys("Frente a la UCR");
            _driver.FindElement(By.Id("telefonoEmpresa")).SendKeys("8888-5678");
            _driver.FindElement(By.Id("razonSocial")).SendKeys("Empresa de tecnología Sprint 3.");

            // Datos del dueño
            _driver.FindElement(By.Id("ownerId")).SendKeys("01-0222-0222");
            _driver.FindElement(By.Id("ownerName")).SendKeys("María");
            _driver.FindElement(By.Id("ownerLastName")).SendKeys("Gómez Rodríguez");

            // Seleccionar sexo
            var labelSexo = wait.Until(driver =>
            {
                try
                {
                    var el = driver.FindElement(By.CssSelector("label[for='ownerSex-Mujer']"));
                    return (el.Displayed && el.Enabled) ? el : null;
                }
                catch { return null; }
            });
            ((IJavaScriptExecutor)_driver).ExecuteScript("arguments[0].scrollIntoView(true);", labelSexo);
            ((IJavaScriptExecutor)_driver).ExecuteScript("arguments[0].click();", labelSexo);

            _driver.FindElement(By.Id("ownerBirthDate")).SendKeys("10-10-1985");
            _driver.FindElement(By.Id("ownerEmail")).SendKeys("maria@sprint3.cr");
            _driver.FindElement(By.Id("ownerPassword")).SendKeys("PasswordSeguro123");

            // Submit
            var submitBtn = _driver.FindElement(By.CssSelector("form button[type='submit']"));
            ((IJavaScriptExecutor)_driver).ExecuteScript("arguments[0].scrollIntoView(true);", submitBtn);
            ((IJavaScriptExecutor)_driver).ExecuteScript("arguments[0].click();", submitBtn);

            // Esperar mensaje de éxito
            var mensaje = wait.Until(driver =>
            {
                try
                {
                    var p = driver.FindElement(By.CssSelector("p.text-success"));
                    return p.Displayed ? p.Text : null;
                }
                catch { return null; }
            });

            StringAssert.Contains("Empresa y dueño registrados correctamente.", mensaje);


            _driver.Navigate().GoToUrl("https://localhost:55281/auth/login");

            wait = new WebDriverWait(_driver, TimeSpan.FromSeconds(10));
            wait.Until(driver => driver.FindElement(By.Id("username")));

            _driver.FindElement(By.Id("username")).SendKeys("maria@sprint3.cr");
            _driver.FindElement(By.Id("password")).SendKeys("PasswordSeguro123");
            var loginBtn = _driver.FindElement(By.CssSelector("form button[type='submit']"));
            ((IJavaScriptExecutor)_driver).ExecuteScript("arguments[0].scrollIntoView(true);", loginBtn);
            ((IJavaScriptExecutor)_driver).ExecuteScript("arguments[0].click();", loginBtn);

            wait = new WebDriverWait(_driver, TimeSpan.FromSeconds(10));
            wait.Until(driver => driver.Url.Contains("/landing-page") || driver.FindElements(By.CssSelector("a.btn.btn-primary[href='/company/edit']")).Count > 0);

            if (!_driver.Url.Contains("/landing-page"))
                _driver.Navigate().GoToUrl("https://localhost:55281/landing-page");

            var editarBtn = wait.Until(driver =>
            {
                try
                {
                    var el = driver.FindElement(By.CssSelector("a.btn.btn-primary[href='/company/edit']"));
                    return (el.Displayed && el.Enabled) ? el : null;
                }
                catch (NoSuchElementException) { return null; }
            });
            ((IJavaScriptExecutor)_driver).ExecuteScript("arguments[0].scrollIntoView(true);", editarBtn);
            ((IJavaScriptExecutor)_driver).ExecuteScript("arguments[0].click();", editarBtn);

            var inputBeneficios = wait.Until(driver =>
            {
                try
                {
                    var el = driver.FindElement(By.Id("max_benefits"));
                    return el.Displayed ? el : null;
                }
                catch (NoSuchElementException) { return null; }
            });

            ((IJavaScriptExecutor)_driver).ExecuteScript("arguments[0].scrollIntoView(true);", inputBeneficios);
            inputBeneficios.Clear();
            inputBeneficios.SendKeys("2");
        }

        [TearDown]
        public void TearDown()
        {
            _driver.Quit();
            _driver.Dispose();
        }
    }
}
