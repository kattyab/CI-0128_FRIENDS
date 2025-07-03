using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;
using System;

namespace UIAutomationTest
{
  public class Case02_Test08_ExecutePayrolls
  {
    private IWebDriver _driver;
    private WebDriverWait _wait;

    [SetUp]
    public void Setup()
    {
      _driver = new ChromeDriver();
      _driver.Manage().Window.Maximize();
      _wait = new WebDriverWait(_driver, TimeSpan.FromSeconds(10));
    }

    [Test]
    public void CrearEmpresaPI_DeberiaRegistrarConExito()
    {
      _driver.Navigate().GoToUrl("https://localhost:55281/auth/login");

      _wait.Until(driver => driver.FindElement(By.Id("username")));

      _driver.FindElement(By.Id("username")).SendKeys("maria@sprint3.cr");
      _driver.FindElement(By.Id("password")).SendKeys("PasswordSeguro123");
      _driver.FindElement(By.CssSelector("button[type='submit']")).Click();

      //_wait.Until(d => d.FindElement(By.CssSelector("h1")).Text.Contains("Empresa PI"));
      System.Threading.Thread.Sleep(100);

            _driver.Navigate().GoToUrl("https://localhost:55281/payroll");

      _wait.Until(d => d.FindElement(By.CssSelector("h1")).Text.Contains("Procesar planilla"));

      var inputMonth = _wait.Until(d => d.FindElement(By.CssSelector("input[type='month'].form-control")));
      inputMonth.SendKeys("Mayo\t2025");

      _driver.FindElement(By.CssSelector("form button")).Click();

      _wait.Until(d => d.FindElement(By.CssSelector("table tbody tr:nth-child(1) td:nth-child(3)")).Text.Contains("05-2025"));
    }

    [TearDown]
    public void TearDown()
    {
      _driver.Quit();
      _driver.Dispose();
    }
  }
}