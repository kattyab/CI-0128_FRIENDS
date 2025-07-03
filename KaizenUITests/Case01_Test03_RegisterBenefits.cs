using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;
using System;

namespace UIAutomationTest
{
  public class Case01_Test03_RegisterBenefits
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
    public void CrearVariosBeneficios_DeberiaRegistrarCorrectamente()
    {
      _driver.Navigate().GoToUrl("https://localhost:55281/auth/login");

      _wait.Until(driver => driver.FindElement(By.Id("username")));

      _driver.FindElement(By.Id("username")).SendKeys("carlos@empresapi.cr");
      _driver.FindElement(By.Id("password")).SendKeys("PasswordSeguro123");
      _driver.FindElement(By.CssSelector("button[type='submit']")).Click();

      _wait.Until(driver => driver.Url.Contains("/dashboard-owner"));

      var beneficios = new[]
      {
        new {
          Nombre = "Gimnasio",
          Tipo = "fixedAmount",
          Monto = "35000",
          Porcentaje = "",
          MinMes = "1"
        },
        new {
          Nombre = "Educación",
          Tipo = "percentage",
          Monto = "",
          Porcentaje = "3",
          MinMes = "1"
        },
      };

      foreach (var beneficio in beneficios)
      {
        _driver.Navigate().GoToUrl("https://localhost:55281/benefits/create");
        _wait.Until(driver => driver.FindElement(By.Id("benefitName")));

        _driver.FindElement(By.Id("benefitName")).Clear();
        _driver.FindElement(By.Id("benefitName")).SendKeys(beneficio.Nombre);
        _driver.FindElement(By.Id("minimumTime")).Clear();
        _driver.FindElement(By.Id("minimumTime")).SendKeys(beneficio.MinMes);

        var fullTimeCheckbox = _driver.FindElement(By.Id("fullTime"));
        if (!fullTimeCheckbox.Selected)
          fullTimeCheckbox.Click();

        if (beneficio.Tipo == "fixedAmount")
        {
          var tipoSelect = new SelectElement(_driver.FindElement(By.Id("benefitType")));
          tipoSelect.SelectByValue("fixedAmount");
          _driver.FindElement(By.Id("fixedAmount")).Clear();
          _driver.FindElement(By.Id("fixedAmount")).SendKeys(beneficio.Monto);
        }
        else if (beneficio.Tipo == "percentage")
        {
          var tipoSelect = new SelectElement(_driver.FindElement(By.Id("benefitType")));
          tipoSelect.SelectByValue("percentage");
          _driver.FindElement(By.Id("percentage")).Clear();
          _driver.FindElement(By.Id("percentage")).SendKeys(beneficio.Porcentaje);
        }

        var submitBtn = _driver.FindElement(By.CssSelector("form button[type='submit']"));
        ((IJavaScriptExecutor)_driver).ExecuteScript("arguments[0].scrollIntoView(true);", submitBtn);
        ((IJavaScriptExecutor)_driver).ExecuteScript("arguments[0].click();", submitBtn);

        var mensaje = _wait.Until(driver =>
        {
          try
          {
            var div = driver.FindElement(By.CssSelector(".success-message"));
            return div.Displayed ? div.Text : null;
          }
          catch (NoSuchElementException) { return null; }
        });

        StringAssert.Contains("Beneficio registrado exitosamente", mensaje);
      }
    }

    [TearDown]
    public void TearDown()
    {
      _driver.Quit();
      _driver.Dispose();
    }
  }
}